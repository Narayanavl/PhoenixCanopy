using Microsoft.AspNetCore.Components;
using StowellCoApp.DTO;
using StowellCoApp.Common;

namespace StowellCoApp.Services
{
    public class ProjectOverviewService : IProjectOverviewService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProjectOverviewService> _logger;
        private readonly NavigationManager _navigation;


        public ProjectOverviewService(HttpClient httpClient, ILogger<ProjectOverviewService> logger, NavigationManager navigation)
        {
            _httpClient = httpClient;
            _logger = logger;
            _navigation = navigation;
        }
        public async Task<CurrentCostSummaryViewModel> GetCurrentCostSummaryViewModel(string recnum)
        {
            try
            {
                
                var apiUrl = $"{GlobalEndpoints.GetCurrentCostSummaryViewModelUrl}/{recnum}";
                var currentCostSummaryViewModel =
                    await _httpClient.GetFromJsonAsync<CurrentCostSummaryViewModel>(apiUrl);

                return currentCostSummaryViewModel ?? new CurrentCostSummaryViewModel();
            }
            catch (Exception ex)
            {
                // log ex here if needed
                return new CurrentCostSummaryViewModel();
            }
        }
        public async Task<Job> GetJobCodeDetails(string recnum)
        {
            try
            {

                var apiUrl = $"{GlobalEndpoints.GetJobCodeDetails}/{recnum}";
                var jobcodedetails =
                    await _httpClient.GetFromJsonAsync<Job>(apiUrl);

                return jobcodedetails ?? new Job();
            }
            catch (Exception ex)
            {
                // log ex here if needed
                return new Job();
            }
        }
    }
}
