using System;
using System.Collections.Generic;

namespace StowellCoApp.Models;

public partial class BudgetPayment
{
    public int Id { get; set; }

    public string? JobId { get; set; }

    public decimal? AmountPaid { get; set; }

    public string? CreatedBy { get; set; }

    public DateOnly? CreatedDate { get; set; }

    public string? ModifiedBy { get; set; }

    public DateOnly? ModifiedDate { get; set; }
}
