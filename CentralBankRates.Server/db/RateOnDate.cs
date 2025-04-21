using System;
using System.Collections.Generic;

namespace CentralBankRates.Server.db;

public partial class RateOnDate
{
    public DateOnly Date { get; set; }

    public string CurrencyId { get; set; } = null!;

    public double? Rate { get; set; }
}
