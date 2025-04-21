using System.Xml.Serialization;

namespace CentralBankRates.Server.Models.xml;

[XmlRoot("Valuta")]
public class CurrenciesCatalogXml
{
    [XmlAttribute("name")] public string Name { get; set; }

    [XmlElement("Item")] public List<CurrenciesCatalogItem> Items { get; set; }
}

public class CurrenciesCatalogItem
{
    [XmlAttribute("ID")] public string Id { get; set; }

    [XmlElement("Name")] public string Name { get; set; }

    [XmlElement("EngName")] public string EngName { get; set; }

    [XmlElement("Nominal")] public int Denomination { get; set; }

    [XmlElement("ParentCode")] public string ParentCode { get; set; }

    [XmlElement("ISO_Num_Code")] public string RawIsoNumCode { get; set; }
    [XmlIgnore] public int IsoNumCode => int.TryParse(RawIsoNumCode, out var isoNum) ? isoNum : 0;

    [XmlElement("ISO_Char_Code")] public string IsoCharCode { get; set; }
}