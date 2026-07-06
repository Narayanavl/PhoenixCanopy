using Microsoft.AspNetCore.Components;
using StowellCoApp.Common;
using StowellCoApp.DTO;
using System.Security.Claims;

namespace StowellCoApp.Services
{
    public class ProjectBudget : IProjectBudget
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProjectBudget> _logger;
        private readonly NavigationManager _navigation;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProjectBudget(HttpClient httpClient, ILogger<ProjectBudget> logger, NavigationManager navigation,IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _navigation = navigation;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IEnumerable<ProjectBudgetQueueItem>> GetProjectBudgetRecords()
        {
            try
            {
                var email =await GetUserEmail();
               // var apiUrl = "https://localhost:7176/api/ProjectBudgetConfiguration/GetProjectBudgetRecords";
               var apiUrl = new Uri(new Uri(_navigation.BaseUri), "api/ProjectBudgetConfiguration/GetProjectBudgetRecords").ToString();
                var jobs = await _httpClient.GetFromJsonAsync<ProjectBudgetQueueItem[]>($"api/ProjectBudgetConfiguration/GetProjectBudgetRecords/{email}");
                return jobs ?? Array.Empty<ProjectBudgetQueueItem>();
            }
            catch (Exception ex)
            {
                return Array.Empty<ProjectBudgetQueueItem>();
            }
        }

        public async Task<IEnumerable<BudgetRecord>> BudgetData(Phase phase)
        {
            try
            {
               // var apiUrl = "https://localhost:7176/api/ProjectBudgetConfiguration/GetProjectBudgetRecords";
                var apiUrl = new Uri(new Uri(_navigation.BaseUri), "api/ProjectBudgetConfiguration/GetProjectBudgetRecords").ToString();
                // POST the Phase object to the API
                var response = await _httpClient.PostAsJsonAsync("api/ProjectBudgetConfiguration/GetProjectBudgetRecords", phase);

                if (response.IsSuccessStatusCode)
                {
                    var jobs = await response.Content.ReadFromJsonAsync<IEnumerable<BudgetRecord>>();
                    return jobs ?? Array.Empty<BudgetRecord>();
                }

                return Array.Empty<BudgetRecord>();
            }
            catch (Exception ex)
            {
                // log if needed
                return Array.Empty<BudgetRecord>();
            }

        }

        public async Task<IEnumerable<BudgetRecord>> SaveCreateBatchCostCodes(List<BudgetRecord> input)
        {
            try
            {
                string email =await GetUserEmail();
                //if (string.IsNullOrEmpty(email) || input == null)
                //    return;

                // Update each record
                foreach (var record in input)
                {
                    record.Email = email;
                }
                // var apiUrl = "https://localhost:7176/api/ProjectBudgetConfiguration/SaveCreateBatchCostCodes";
                var apiUrl = new Uri(new Uri(_navigation.BaseUri), "api/ProjectBudgetConfiguration/SaveCreateBatchCostCodes").ToString();
                // POST the Phase object to the API
                var response = await _httpClient.PostAsJsonAsync("api/ProjectBudgetConfiguration/SaveCreateBatchCostCodes", input);

                if (response.IsSuccessStatusCode)
                {
                    var jobs = await response.Content.ReadFromJsonAsync<IEnumerable<BudgetRecord>>();
                    return jobs ?? Array.Empty<BudgetRecord>();
                }

                return Array.Empty<BudgetRecord>();
            }
            catch (Exception ex)
            {
                // log if needed
                return Array.Empty<BudgetRecord>();
            }

        }
        public async Task<IEnumerable<BudgetRecord>> SubmitBatchCostCodes(List<BudgetRecord> input)
        {
            try
            {
                string email =await GetUserEmail();
                //if (string.IsNullOrEmpty(email) || input == null)
                //    return;

                // Update each record
                foreach (var record in input)
                {
                    record.Email = email;
                }
                // var apiUrl = string.Format("https://localhost:7176/api/ProjectBudgetConfiguration/SubmitBatchCostCodes");
                var apiUrl = new Uri(new Uri(_navigation.BaseUri), "api/ProjectBudgetConfiguration/SubmitBatchCostCodes").ToString();

                // POST the Phase object to the API
                var response = await _httpClient.PostAsJsonAsync("api/ProjectBudgetConfiguration/SubmitBatchCostCodes", input);

                if (response.IsSuccessStatusCode)
                {
                    var jobs = await response.Content.ReadFromJsonAsync<IEnumerable<BudgetRecord>>();
                    return jobs ?? Array.Empty<BudgetRecord>();
                }

                return Array.Empty<BudgetRecord>();
            }
            catch (Exception ex)
            {
                // log if needed
                return Array.Empty<BudgetRecord>();
            }

        }

        public async Task<IEnumerable<string>> GetCostCodesAsync(string filter)
        {
            try
            {
                //var apiUrl = "https://localhost:7176/api/ProjectBudgetConfiguration/GetCostCodesAsync/{filter}";
                // var apiUrl = $"https://localhost:7176/api/ProjectBudgetConfiguration/GetCostCodesAsync/{filter}";
                var apiname = "api/ProjectBudgetConfiguration/GetCostCodesAsync";
                var apiUrl = $"{apiname}/{filter}";
                // POST the Phase object to the API
                //var response = await _httpClient.GetFromJsonAsync(apiUrl);
                var jobs = await _httpClient.GetFromJsonAsync<string[]>(apiUrl);
                return jobs ?? Array.Empty<string>();
            }
            catch (Exception ex)
            {
                // log if needed
                return Array.Empty<string>();
            }

        }
        public async Task<HttpResponseMessage> UploadAddExcelAsync(MultipartFormDataContent content)
        {

            try
            {
                var apiUrl = $"api/ProjectBudgetConfiguration/UploadAddExcel";

                return await _httpClient.PostAsync(apiUrl, content);
            }
            catch (Exception ex)
            {
                // Return a proper HTTP response containing the error message
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
                {
                    Content = new StringContent(ex.Message)
                };
                return response;
            }
        }
        public async Task<HttpResponseMessage> UploadAddExcelAsync(List<BudgetRecord> budgetRecord)
        {

            try
            {
                string email =await GetUserEmail();
                //if (string.IsNullOrEmpty(email) || input == null)
                //    return;

                // Update each record
                foreach (var record in budgetRecord)
                {
                    record.Email = email;
                }
                //var apiUrl = $"https://localhost:7176/api/ProjectBudgetConfiguration/UploadAddExcel";
                var apiUrl = $"api/ProjectBudgetConfiguration/UploadAddExcel";
                var responseMessage = await _httpClient.PostAsJsonAsync(apiUrl, budgetRecord);

                return responseMessage;

            }

            catch (Exception ex)

            {

                return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)

                {

                    Content = new StringContent(ex.Message)

                };

            }

        }
        public async Task<string> GetUserEmail()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity.IsAuthenticated)
                return null;

            // Try email claim first, then preferred_username
            return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
                   ?? user.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
        }
    }
}
