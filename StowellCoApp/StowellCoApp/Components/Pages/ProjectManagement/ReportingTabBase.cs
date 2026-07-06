using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using StowellCoApp.DTO;
using StowellCoApp.Services;
using static StowellCoApp.Components.LineChart1;

namespace StowellCoApp.Components.Pages.ProjectManagement
{
    public class ReportingTabBase :ComponentBase
    {

        [Parameter] public int ProjectId { get; set; }

        protected double value = 0;
        public  DataItem[] revenue = Array.Empty<DataItem>();

        [Inject]
        public IProjectOverviewService _overviewservice { get; set; }

        //[Inject]
        //public IEstimationService _estimateservice { get; set; }

        public CurrentCostSummaryViewModel _currencycostModel { get; set; }

        [Inject]
        public HttpClient Http { get; set; }

        IEnumerable<ColorScheme> colorSchemes = Enum.GetValues(typeof(ColorScheme)).Cast<ColorScheme>();
        ColorScheme colorScheme = ColorScheme.Palette;
        bool showDataLabels = true;

        //[Parameter] public BidInfoDto _bidModel { get; set; }

        // public string JobId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _currencycostModel = await _overviewservice.GetCurrentCostSummaryViewModel(Convert.ToString(ProjectId));
            // Safely read the percentage
            value = _currencycostModel?.CostCodeSummaryModel?.Percentage ?? 0;

            var cash = _currencycostModel?.CashFlowRecord;
            revenue = new DataItem[]
            {
            new DataItem
            {
                Quarter = "Cash Collected",
                Revenue = cash?.CashCollected ?? 0
            },
            new DataItem
            {
                Quarter = "Cash Paid",
                Revenue = cash?.CashPaid ?? 0
            },
            new DataItem
            {
                Quarter = "Net Cash",
                Revenue = cash?.NetCash ?? 0
            },
            };
            //_bidModel = await _estimateservice.GetBidDetails(ProjectId);
        }
        void OnSeriesClick(SeriesClickEventArgs args)
        {
            // console.Log(args);
        }
    }

    public class DataItem
    {
        public string Quarter { get; set; }
        public decimal Revenue { get; set; }
    }
}
