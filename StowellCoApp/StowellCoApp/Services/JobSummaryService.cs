using Microsoft.AspNetCore.Components;
using StowellCoApp.DTO;
using StowellCoApp.Common;

namespace StowellCoApp.Services
{
    public class JobSummaryService : IJobSummaryService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<JobSummaryService> _logger;
        private readonly NavigationManager _navigation;
        public JobSummaryService(HttpClient httpClient, ILogger<JobSummaryService> logger, NavigationManager navigation)
        {
            _httpClient = httpClient;
            _logger = logger;
            _navigation = navigation;
        }


        public async Task<IEnumerable<CostCodeSummaryRecord>> GetJobCostSummaryRecords(string recnum="")
        {
            try
            {
                if (!string.IsNullOrEmpty(recnum))
                {

                    var apiUrl = $"{GlobalEndpoints.GetJobCostSummaryRecordsWithRecnumUrl}/{recnum}";
                    var currentCostSummaryViewModel =
                        await _httpClient.GetFromJsonAsync<List<CostCodeSummaryRecord>>(apiUrl);

                    return currentCostSummaryViewModel ?? new List<CostCodeSummaryRecord>();
                }
                else
                {
                    var apiUrl = $"{GlobalEndpoints.GetJobCostSummaryRecordsUrl}";
                    var currentCostSummaryViewModel =
                        await _httpClient.GetFromJsonAsync<List<CostCodeSummaryRecord>>(apiUrl);

                    return currentCostSummaryViewModel ?? new List<CostCodeSummaryRecord>();
                }
            }
            catch (Exception ex)
            {
                // log ex here if needed
                return new List<CostCodeSummaryRecord>();
            }
        }
    }

}