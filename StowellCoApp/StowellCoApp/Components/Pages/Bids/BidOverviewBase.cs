using Microsoft.AspNetCore.Components;

namespace StowellCoApp.Components.Pages.Bids
{
    public partial class BidOverviewBase : ComponentBase
    {
        [Parameter] public required string BidId { get; set; }
        [Parameter] public required string Type { get; set; }
    }
}
