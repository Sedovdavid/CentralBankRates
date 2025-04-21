using System;
using System.Collections.Generic;

namespace CentralBankRates.Server.db;

public partial class CurrenciesCatalog
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string EngName { get; set; } = null!;

    public int Denomination { get; set; }

    public string ParentCode { get; set; } = null!;

    public int IsoNumCode { get; set; }

    public string IsoCharCode { get; set; } = null!;
}
