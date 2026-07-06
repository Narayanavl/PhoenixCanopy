using System;
using System.Collections.Generic;

namespace StowellCoApp.Models;

public partial class CostCodeList
{
    public int Id { get; set; }

    public string CostCodeDescription { get; set; } = null!;

    public string? ShortDescription { get; set; }

    public int? PreferredCode { get; set; }

    public DateTime CreatedDate { get; set; }

    public string CreatedBy { get; set; } = null!;

    public DateTime? ModifiedDate { get; set; }

    public string? ModifiedBy { get; set; }
    public string Description { get; set; }
}
public class CashFlowForecastResponse
{
    public List<CashFlowForecastRow> BudgetForecast { get; set; } = new();
    public List<CashFlowForecastRow> ContractForecast { get; set; } = new();
}

public class CashFlowForecastRow
{
    public string JobNumber { get; set; }
    public string JobName { get; set; }

    public decimal TotalAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public decimal Remaining { get; set; }

    public Dictionary<string, decimal> ForecastMonths { get; set; }
        = new Dictionary<string, decimal>();
}
