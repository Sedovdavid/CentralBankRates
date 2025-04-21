using CentralBankRates.Server.Models;

namespace CentralBankRates.Server.Utils;

public abstract class ApiRoutes
{
    private const string CurrenciesCatalog = "https://cbr.ru/scripts/XML_valFull.asp";
    private const string CurrenciesDaily = "https://cbr.ru/scripts/XML_daily.asp";
    private const string CurrenciesDynamic = "https://cbr.ru/scripts/XML_dynamic.asp?date_req1={0}&date_req2={1}&VAL_NM_RQ={2}";

    public static string GetCurrenciesCatalog() => CurrenciesCatalog;
    public static string GetCurrenciesDaily() => CurrenciesDaily;

    public static string GetCurrenciesDynamic(DateOnly dateFrom, DateOnly dateTo, string valNmRq)
        => string.Format(CurrenciesDynamic, dateFrom, dateTo, valNmRq);
}