using CentralBankRates.Server.db;
using CentralBankRates.Server.Models.xml;

namespace CentralBankRates.Server.Utils;

public static class MethodsExtensions
{
    public static List<CurrenciesCatalog> XmlToEntity(this CurrenciesCatalogXml catalog)
    {
        var result = new List<CurrenciesCatalog>();

        foreach (var item in catalog.Items)
        {
            result.Add(new CurrenciesCatalog
            {
                Id = item.Id,
                Name = item.Name,
                EngName = item.EngName,
                Denomination = item.Denomination,
                ParentCode = item.ParentCode,
                IsoNumCode = item.IsoNumCode,
                IsoCharCode = item.IsoCharCode
            });
        }

        return result;
    }
}