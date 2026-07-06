using Microsoft.AspNetCore.Components;
using StowellCoApp.DTO;
using StowellCoApp.Services;

namespace StowellCoApp.Components.Pages.ProjectManagement
{
    public class FinancialsTabBase :ComponentBase
    {
        [Parameter] public int ProjectId { get; set; }

        [Inject]
        public IProjectOverviewService _overviewservice { get; set; }

        //[Inject]
        //public IEstimationService _estimateservice { get; set; }

        public CurrentCostSummaryViewModel _currencycostModel { get; set; }

        [Inject]
        public HttpClient Http { get; set; }

        //[Parameter] public BidInfoDto _bidModel { get; set; }

        // public string JobId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            _currencycostModel = await _overviewservice.GetCurrentCostSummaryViewModel(Convert.ToString(ProjectId));
            //_bidModel = await _estimateservice.GetBidDetails(ProjectId);
        }
    }
}
