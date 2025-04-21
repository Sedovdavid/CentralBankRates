namespace CentralBankRates.Server.Models;

public class TableAnswer
{
    public List<string> Header { get; set; } = [];
    public List<List<string>> Values { get; set; } = [];
}