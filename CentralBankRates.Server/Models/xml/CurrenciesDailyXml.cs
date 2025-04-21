using System.Globalization;

namespace CentralBankRates.Server.Models.xml;

using System.Xml.Serialization;

[XmlRoot("ValCurs")]
public class CurrenciesDailyXml
{
    [XmlElement("Valute")] public List<Currency> Currencies { get; set; }
    
    [XmlAttribute("Date")] public string Date { get; set; }

    [XmlAttribute("name")] public string Name { get; set; }

    [XmlText] public string Text { get; set; }
}

public class Currency
{
    [XmlElement("NumCode")] public int NumCode { get; set; }

    [XmlElement("CharCode")] public string CharCode { get; set; }

    [XmlElement("Nominal")] public int Denomination { get; set; }

    [XmlElement("Name")] public string Name { get; set; }

    [XmlElement("Value")] public string RawValue { get; set; }
    [XmlIgnore] public double Value => double.Parse(RawValue);

    [XmlElement("VunitRate")] public string RawVunitRate { get; set; }
    [XmlIgnore] public double VunitRate => double.Parse(RawVunitRate);

    [XmlAttribute("ID")] public string ID { get; set; }

    [XmlText] public string Text { get; set; }
}