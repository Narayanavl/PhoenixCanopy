using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using StowellCoApp.DTO;
using StowellCoApp.Models;
using StowellCoApp.Services;
using System.Globalization;
using static System.Net.WebRequestMethods;

namespace StowellCoApp.Components.Pages.ProjectManagement
{
    public class OverviewTabBase :  ComponentBase
    {
        protected RadzenDataGrid<StowellCoApp.Models.BidPhase> _bidPhaseGrid;
        public IEnumerable<StowellCoApp.Models.BidPhase> _bidphaseRecords { get; set; } = new List<StowellCoApp.Models.BidPhase>();
        protected string? selectedDivision = null;
        [Parameter] public int ProjectId { get; set; }

        [Inject]
        public IProjectOverviewService _overviewservice { get; set; }

        [Inject]
        public IEstimationService _estimateservice { get; set; }

        public CurrentCostSummaryViewModel _currencycostModel { get; set; }

        [Inject]
        public HttpClient Http { get; set; }
        [Inject]
        public IConfiguration _configuration { get; set; }

        [Parameter] public BidInfoDto _bidModel { get; set; }
        protected string? selectedBidStatus = null;
        // public string JobId { get; set; }
        protected string? projectfolderlibraryUrl = string.Empty;
        protected override async Task OnInitializedAsync()
        {
            projectfolderlibraryUrl = _configuration["ProjectFolderLibrary"];
            _currencycostModel = await _overviewservice.GetCurrentCostSummaryViewModel(Convert.ToString(ProjectId));
            _bidModel = await _estimateservice.GetBidDetails(ProjectId);
            _bidphaseRecords = await _estimateservice.GetPhases(ProjectId.ToString());
            //var httpTask = Http.GetFromJsonAsync<BidInfoDto>($"/Estimation/GetBidDetails?jobId={JobId}");
            selectedDivision = _bidModel.Division;
            selectedBidStatus = _bidModel.BidStatus;
            //await Task.WhenAll(serviceTask, httpTask);

            // Assign results

        }
        public readonly CultureInfo UsCulture = new("en-US");

    }
}
