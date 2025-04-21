using System.Xml.Serialization;

namespace CentralBankRates.Server.Models.xml;

[XmlRoot("ValCurs")]
public class CurrencyDynamicXml
{
    [XmlElement("Record")] public List<CurrencyDynamicItem> Items { get; set; }

    [XmlAttribute("ID")] public string ID { get; set; }

    [XmlAttribute("DateRange1")] public string DateRange1 { get; set; }

    [XmlAttribute("DateRange2")] public string DateRange2 { get; set; }

    [XmlAttribute("name")] public string Name { get; set; }

    // [XmlText] public double Text { get; set; }
}

public class CurrencyDynamicItem 
{
    [XmlElement("Nominal")] public int Nominal { get; set; }

    [XmlElement("Value")] public string Value { get; set; }

    [XmlElement("VunitRate")] public string RawVunitRate { get; set; }
    [XmlIgnore] public double VunitRate => double.Parse(RawVunitRate);

    [XmlAttribute("Date")] public string RawDate { get; set; }
    [XmlIgnore] public DateOnly Date => DateOnly.Parse(RawDate);

    [XmlAttribute("Id")] public string CurrencyId { get; set; }

    // [XmlText] public double Text { get; set; }
}
