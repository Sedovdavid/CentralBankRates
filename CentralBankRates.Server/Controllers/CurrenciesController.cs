using CentralBankRates.Server.db;
using Microsoft.AspNetCore.Mvc;
using CentralBankRates.Server.Models;
using CentralBankRates.Server.Models.xml;
using CentralBankRates.Server.Utils;

namespace CentralBankRates.Server.Controllers
{
    public record DateRange(DateOnly DateFrom, DateOnly DateTo);

    [ApiController]
    [Route("api/[controller]")]
    public class CurrenciesController : ControllerBase
    {
        private static readonly CentralBankRatesContext DbContext = new();

        [HttpGet("GetCurrencies")]
        public async Task<IActionResult> GetCurrencies(string dateFrom, string dateTo)
        {
            DateOnly localDateFrom, localDateTo;
            try
            {
                localDateFrom = DateOnly.FromDateTime(DateTime.Parse(dateFrom));
                localDateTo = DateOnly.FromDateTime(DateTime.Parse(dateTo));
            }
            catch (Exception)
            {
                (localDateFrom, localDateTo) = DefaultDates();
            }

            var answer = await GetCurrenciesDynamic(localDateFrom, localDateTo);
            return Ok(answer);
        }

        [HttpGet("GetDefaultDates")]
        public IActionResult GetDefaultDates()
        {
            var defaultDates = DefaultDates();
            return Ok(new
                {
                    dateFrom = defaultDates.DateFrom,
                    dateTo = defaultDates.DateTo
                }
            );
        }

        private static DateRange DefaultDates()
        {
            return new DateRange(
                DateOnly.FromDateTime(DateTime.Today.AddDays(-14)),
                DateOnly.FromDateTime(DateTime.Today)
            );
        }

        private static async Task<TableAnswer> GetCurrenciesDynamic(DateOnly dateFrom, DateOnly dateTo)
        {
            var answer = new TableAnswer();

            //первый столбец - дата
            answer.Header.Add("Дата");

            //получение актуального каталога валют
            var currencyCatalog = await GetCurrenciesCatalog(Constants.CurrenciesUsedIso);

            var rateOnDate = new List<RateOnDate>();

            foreach (var currency in currencyCatalog)
            {
                //новый элемент в хэдере - название валюты на русском
                answer.Header.Add(currency.Name);

                //выборка выбранной валюты на даты
                rateOnDate.AddRange(
                    await GetRateOnDate(currency.Id, dateFrom, dateTo)
                );
            }

            //сортировка по дате с сохранением относительного порядка записей
            rateOnDate = rateOnDate.OrderByDescending(x => x.Date).ToList();

            //формирование динамической таблицы для фронта
            foreach (var group in rateOnDate.GroupBy(x => x.Date))
            {
                var itemsWithRate = group.Where(x => x.Rate != null).ToList();

                //если нет ни одного элемента с заполненным rate -
                //это лишняя строка без данных, ее не надо заполнять
                if (itemsWithRate.Count == 0) continue;

                List<string> row = [group.First().Date.ToString()]; //первый столбец - дата
                foreach (var item in itemsWithRate)
                    row.Add(item.Rate.ToString()!);

                answer.Values.Add(row);
            }

            return answer;
        }

        private static async Task<List<CurrenciesCatalog>> GetCurrenciesCatalog(List<string> currenciesIso)
        {
            //условие "только используемые валюты"
            Func<CurrenciesCatalog, bool> onlyUsedCurrencies =
                x => currenciesIso.Contains(x.IsoCharCode);

            //выбираются буферизированные данные
            var currenciesCatalog =
                DbContext.CurrenciesCatalogs.Local.Where(
                    onlyUsedCurrencies
                ).ToList();

            if (currenciesCatalog.Count != 0) return currenciesCatalog;

            //если еще не были выбраны, из бд выбираются записи (id, описание) только для нужных валют
            currenciesCatalog = DbContext.CurrenciesCatalogs.Where(
                onlyUsedCurrencies
            ).ToList();

            if (currenciesCatalog.Count != 0) return currenciesCatalog;

            //если в бд нет данных - взять из api, обновить бд
            var currenciesCatalogXml = await ApiHelper.GetApi<CurrenciesCatalogXml>(ApiRoutes.GetCurrenciesCatalog());

            var currenciesCatalogList = currenciesCatalogXml.XmlToEntity();

            DbContext.CurrenciesCatalogs.AddRange(currenciesCatalogList);
            await DbContext.SaveChangesAsync();

            return currenciesCatalog;
        }

        private static async Task<List<RateOnDate>> GetRateOnDate(string currencyId, DateOnly dateFrom, DateOnly dateTo)
        {
            //условие для выбора записей
            // Func<RateOnDate, bool> rateOnDateWhere =
            //     x => x.CurrencyId == currencyId && x.Date >= dateFrom && x.Date <= dateTo;

            //выбираем из буфера
            var ratesOnDate = DbContext.RateOnDates.Local.Where(
                x => x.CurrencyId == currencyId && x.Date >= dateFrom && x.Date <= dateTo
            ).ToList();

            //выбираем недостающие диапазоны дат на основе исходного диапазона и всех дат,
            //которые удалось выбрать из буфера
            var missingDateRanges = GetMissingDateRanges(new DateRange(dateFrom, dateTo),
                ratesOnDate.Select(x => x.Date).ToList());
            var dbChanged = false;

            foreach (var missingDateRange in missingDateRanges)
            {
                //то, что не найдено в буфере, выбираем из бд
                var localRatesOnDate = DbContext.RateOnDates.Where(
                    x => x.CurrencyId == currencyId
                         && x.Date >= missingDateRange.DateFrom
                         && x.Date <= missingDateRange.DateTo
                ).ToList();
                //если в бд найдены данные по диапазону, то не нужно их снова искать в api
                if (localRatesOnDate.Count != 0)
                {
                    ratesOnDate.AddRange(localRatesOnDate);
                    continue;
                }

                //иначе вызываем api и складываем в бд
                var routeDynamic = ApiRoutes.GetCurrenciesDynamic(
                    missingDateRange.DateFrom,
                    missingDateRange.DateTo,
                    currencyId);

                var currencyDynamicXml = await ApiHelper.GetApi<CurrencyDynamicXml>(routeDynamic);

                foreach (var item in currencyDynamicXml.Items)
                    localRatesOnDate.Add(new RateOnDate
                    {
                        CurrencyId = currencyId,
                        Date = item.Date,
                        Rate = item.VunitRate
                    });

                //восполняем недостающие из api данные по датам и заполняем их с rate = null,
                //то есть данные были выбраны, но отсутствуют, и их не нужно выбирать заново
                for (var date = missingDateRange.DateFrom; date < missingDateRange.DateTo; date = date.AddDays(1))
                {
                    if (localRatesOnDate.Any(x => x.Date == date)) continue;

                    localRatesOnDate.Add(new RateOnDate
                    {
                        CurrencyId = currencyId,
                        Date = date,
                        Rate = null
                    });
                }

                if (localRatesOnDate.Count == 0) continue;
                
                ratesOnDate.AddRange(localRatesOnDate);
                DbContext.RateOnDates.AddRange(localRatesOnDate);
                dbChanged = true;
            }

            if (dbChanged)
                await DbContext.SaveChangesAsync();

            return ratesOnDate;
        }

        //сформировать недостающие диапазоны дат для выбора из api
        private static List<DateRange> GetMissingDateRanges(DateRange initRange, List<DateOnly> fetchedDates)
        {
            var ranges = new List<DateRange>();

            if (fetchedDates.Count==0)
                return [initRange];
 
            DateOnly? dateFrom = null;
            DateOnly? dateTo = null;
            
            for (var date = initRange.DateFrom; date < initRange.DateTo; date = date.AddDays(1))
            {
                //если дата есть в списке выбранных дат -
                //значит интервал уже закончился (или еще не начинался)
                if (fetchedDates.Contains(date))
                {
                    if (dateFrom == null || dateTo == null) continue;
                    ranges.Add(new DateRange(dateFrom.Value, dateTo.Value));
                    dateFrom = dateTo = null;
                }
                //иначе интервал продолжается (или начался)
                else
                {
                    dateFrom ??= date;
                    dateTo = date;
                }
            }
            
            //если последняя дата входит в интервал, ее тоже нужно добавить
            if (dateFrom != null && dateTo != null)
                ranges.Add(new DateRange(dateFrom.Value, dateTo.Value));
             
            return ranges;
        }
    }
}