using Azure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using StowellCoApp.Common;
using StowellCoApp.DTO;
using StowellCoApp.Models;
using System.Security.Claims;

namespace StowellCoApp.Services
{
    public class CostCodeService : ICostCodeService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CostCodeService> _logger;
        private readonly NavigationManager _navigation;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CostCodeService(HttpClient httpClient, ILogger<CostCodeService> logger, NavigationManager navigation, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _navigation = navigation;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IEnumerable<CostCodeRecord>> GetCostCodeRecords()
        {
            try
            {
                //  var apiUrl = "https://localhost:7176/api/CostCode/GetCostCodeRecords";
                var apiUrl = new Uri(new Uri(_navigation.BaseUri), "api/CostCode/GetCostCodeRecords").ToString();
                var jobs = await _httpClient.GetFromJsonAsync<CostCodeRecord[]>("api/CostCode/GetCostCodeRecords");
                return jobs ?? Array.Empty<CostCodeRecord>();

            }
            catch (Exception ex)
            {
                return Array.Empty<CostCodeRecord>();
            }
        }
        public async Task<IEnumerable<BidItem>> GetBids()
        {
            try
            {
                //var apiUrl = "https://localhost:7176/api/CostCode/GetBids";
                var apiUrl = new Uri(new Uri(_navigation.BaseUri), "api/CostCode/GetBids").ToString();
                var jobs = await _httpClient.GetFromJsonAsync<BidItem[]>("api/CostCode/GetBids");
                return jobs ?? Array.Empty<BidItem>();

            }
            catch (Exception ex)
            {
                return Array.Empty<BidItem>();
            }
        }
        public async Task<IEnumerable<Phase>> GetJobPhasesData(int jobId)
        {
            try
            {
                //var apiUrl = string.Format("https://localhost:7176/api/CostCode/GetJobPhasesData/{0}", jobId);
                var apiname = "api/CostCode/GetJobPhasesData";
                var apiUrl = $"{_navigation.BaseUri}{apiname}/{jobId}";
                // var apiUrl = "https://localhost:7176/api/CostCode/GetBids/{jobId}";
                //var apiUrl = $"{_navigation.BaseUri}{GlobalEndpoints.GetJobPhasesData}/{jobId}";
                var jobs = await _httpClient.GetFromJsonAsync<Phase[]>($"api/CostCode/GetJobPhasesData/{jobId}");

                //var apiUrl = "https://localhost:7176/api/CostCode/GetBids";
                //var apiUrl = new Uri(new Uri(_navigation.BaseUri), "api/CostCode/GetJobPhasesData").ToString();
                //var jobs = await _httpClient.GetFromJsonAsync<Phase[]>(apiUrl);
                return jobs ?? Array.Empty<Phase>();

            }
            catch (Exception ex)
            {
                return Array.Empty<Phase>();
            }
        }
        public async Task<IEnumerable<StatusModel>> GetAllStatusesData()
        {
            try
            {
                var apiUrl = new Uri(new Uri(_navigation.BaseUri), GlobalEndpoints.GetAllStatusesDataUrl).ToString();
                var jobs = await _httpClient.GetFromJsonAsync<StatusModel[]>(GlobalEndpoints.GetAllStatusesDataUrl);
                return jobs ?? Array.Empty<StatusModel>();

            }
            catch (Exception ex)
            {
                return Array.Empty<StatusModel>();
            }
        }
        public async Task<IEnumerable<CostCodeRecord>> GetAllUserJobsByStatus(int status)
        {
            try
            {
                var apiUrl = $"{_navigation.BaseUri}{GlobalEndpoints.GetAllUserJobsByStatusUrl}/{status}";
                // var apiUrl = $"https://localhost:7297/api/CostCode/GetAllUserJobsByStatus/{status}";
                var jobs = await _httpClient.GetFromJsonAsync<CostCodeRecord[]>($"{GlobalEndpoints.GetAllUserJobsByStatusUrl}/{status}");
                return jobs ?? Array.Empty<CostCodeRecord>();

            }
            catch (Exception ex)
            {
                return Array.Empty<CostCodeRecord>();
            }
        }
        public async Task<IEnumerable<CashFlowRecord>> CashFlowAllDataByPeriod(CashFlowQueryInput input)
        {
            try
            {
                //var apiUrl = "https://localhost:7297/api/CashFlow/CashFlowAllDataByPeriod";
                var apiUrl = $"{GlobalEndpoints.GetCashFlowAllDataByPeriodUrl}";
                var response = await _httpClient.PostAsJsonAsync(apiUrl, input);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CashFlowRecord[]>();
                }

                return default;

            }
            catch (Exception ex)
            {
                return Array.Empty<CashFlowRecord>();
            }
        }
        public async Task<IEnumerable<CostCodeRecord>> GetAllUserJobs()
        {
            try
            {
                string email = GetUserEmail();
               // var apiUrl = "https://localhost:7176/api/CostCode/GetAllUserJobs";
                var apiUrl = $"{_navigation.BaseUri}{GlobalEndpoints.GetAllUserJobsUrl}";
                // var apiUrl = new Uri(new Uri(_navigation.BaseUri), "api/CostCode/GetAllUserJobs").ToString();
                var jobs = await _httpClient.GetFromJsonAsync<CostCodeRecord[]>($"{GlobalEndpoints.GetAllUserJobsUrl}/{email}");
                return jobs ?? Array.Empty<CostCodeRecord>();

            }
            catch (Exception ex)
            {
                return Array.Empty<CostCodeRecord>();
            }
        }
        public async Task<CurrentCostSummaryViewModel> GetCurrentCostSummaryViewModel(
        string recnum="",
        List<string> costCode = null,
        string phase = "",
        string startDate = "",
        string endDate = "")
        {
            try
            {
                // Build the query string with parameters
                var queryString = $"?recnum={recnum}";

                if (costCode != null && costCode.Any())
                {
                    queryString += $"&costCode={string.Join(",", costCode)}"; // Join list as comma-separated
                }

                if (!string.IsNullOrEmpty(phase))
                {
                    queryString += $"&phase={phase}";
                }

                if (!string.IsNullOrEmpty(startDate))
                {
                    queryString += $"&startDate={startDate}";
                }

                if (!string.IsNullOrEmpty(endDate))
                {
                    queryString += $"&endDate={endDate}";
                }

                // Combine the base API URL with the query string
                //var apiUrl = "https://localhost:7176/api/CostCodeSummary/GetCurrentCostSummaryViewModel1" + queryString;
                var apiname = "api/CostCodeSummary/GetCurrentCostSummaryViewModel1";
                var apiUrl = $"{apiname}" + queryString;
                // Fetch data from the API
                var _currentCostSummaryViewModel = await _httpClient.GetFromJsonAsync<CurrentCostSummaryViewModel>(apiUrl);

                // Transform the fetched data into a CurrentCostSummaryViewModel
                if (_currentCostSummaryViewModel != null)
                {
                    return new CurrentCostSummaryViewModel
                    {
                        CostCodeSummaryModel = _currentCostSummaryViewModel.CostCodeSummaryModel,
                        SelectedJobId = recnum,
                        CostCodes = _currentCostSummaryViewModel.CostCodes,
                        Jobs = _currentCostSummaryViewModel.Jobs,
                        Phases = _currentCostSummaryViewModel.Phases,
                        SelectedJobName = _currentCostSummaryViewModel.SelectedJobName,
                        SelectedPhaseId=_currentCostSummaryViewModel.SelectedPhaseId,
                        SelectedCostCodeIds=_currentCostSummaryViewModel.SelectedCostCodeIds
                    };
                }
                else
                {
                    return new CurrentCostSummaryViewModel
                    {
                        SelectedJobId = recnum,
                        CostCodes = new List<StowellCoApp.DTO.CostCode>(),
                        Jobs = new List<StowellCoApp.DTO.Job>(),
                        Phases = new List<Phase>(),
                    };
                }
            }
            catch (Exception ex)
            {
                // Optionally log the exception
                return new CurrentCostSummaryViewModel
                {
                    SelectedJobId = recnum,
                    CostCodes = new List<StowellCoApp.DTO.CostCode>(),
                    Jobs = new List<StowellCoApp.DTO.Job>(),
                    Phases = new List<Phase>(),
                };
            }
        }
        public async Task<IEnumerable<ContractSummary>> GetContractSummaryData(string jobNumber)
        {
            try
            {
                var apiUrl = $"{_navigation.BaseUri}{"api/CashFlow/GetContractSummaryData"}/{jobNumber}";
                //var jobs = await _httpClient.GetFromJsonAsync<ContractSummary[]>($"{"api/CashFlow/GetContractSummaryData"}/{jobNumber}");
                var jobs = await _httpClient.GetFromJsonAsync<ContractSummary[]>($"api/CashFlow/GetContractSummaryData/{jobNumber}")?? Array.Empty<ContractSummary>();

                return jobs ?? Array.Empty<ContractSummary>();

            }
            catch (Exception ex)
            {
                return Array.Empty<ContractSummary>();
            }
        }
        public async Task<IEnumerable<PrimeChangeList>> GetPrimeChangeList(string jobNumber)
        {
            try
            {
                var apiUrl = $"{"api/CashFlow/GetPrimeChangeList"}/{jobNumber}";
                var jobs = await _httpClient.GetFromJsonAsync<PrimeChangeList[]>(apiUrl);
                return jobs ?? Array.Empty<PrimeChangeList>();

            }
            catch (Exception ex)
            {
                return Array.Empty<PrimeChangeList>();
            }
        }
        public async Task<IEnumerable<InvoiceReceivable>> GetInvoiceReceivable(string jobNumber)
        {
            try
            {
                var apiUrl = $"{"api/CashFlow/GetInvoiceReceivable"}/{jobNumber}";
                var jobs = await _httpClient.GetFromJsonAsync<InvoiceReceivable[]>(apiUrl);
                return jobs ?? Array.Empty<InvoiceReceivable>();

            }
            catch (Exception ex)
            {
                return Array.Empty<InvoiceReceivable>();
            }
        }
        public async Task<IEnumerable<InvoicePayment>> GetARInvoice(string jobNumber)
        {
            try
            {
                var apiUrl = $"{"api/CashFlow/GetARInvoice"}/{jobNumber}";
                var jobs = await _httpClient.GetFromJsonAsync<InvoicePayment[]>(apiUrl);
                return jobs ?? Array.Empty<InvoicePayment>();

            }
            catch (Exception ex)
            {
                return Array.Empty<InvoicePayment>();
            }
        }
        public async Task<IEnumerable<CostCode>> GetCostCodesForReport(string jobID)
        {
            try
            {
                var apiUrl = $"{"api/CostCodeSummary/GetCostCodesForReport"}/{jobID}";
                var jobs = await _httpClient.GetFromJsonAsync<CostCode[]>(apiUrl);
                return jobs ?? Array.Empty<CostCode>();

            }
            catch (Exception ex)
            {
                return Array.Empty<CostCode>();
            }
        }
        public async Task<List<StowellCoApp.DTO.Job>> GetJobCodesForReport()
        {
            try
            {
                var apiUrl = $"{"api/CostCodeSummary/GetJobCodesForReport"}";
                var jobs = await _httpClient.GetFromJsonAsync<List<StowellCoApp.DTO.Job>>(apiUrl);
                return jobs ?? new List<StowellCoApp.DTO.Job>();

            }
            catch (Exception ex)
            {
                return new List<StowellCoApp.DTO.Job>();
            }
        }
        public async Task<List<Phase>> GetPhasesForReport(string jobID)
        {
            try
            {
                var apiUrl = $"{"api/CostCodeSummary/GetPhasesForReport"}/{jobID}";
                var jobs = await _httpClient.GetFromJsonAsync<List<Phase>>(apiUrl);
                return jobs ?? new List<Phase>();

            }
            catch (Exception ex)
            {
                return new List<Phase>();
            }
        }
        public async Task<IEnumerable<Phase>> GetAllPhases()
        {
            try
            {
                var apiUrl = $"{_navigation.BaseUri}{"api/CostCode/GetAllPhases"}";
                //var apiUrl = "https://localhost:7176/api/CostCode/GetAllPhases";

                var response =
                    await _httpClient.GetFromJsonAsync<ApiListResponse<Phase>>("api/CostCode/GetAllPhases");

                return response?.Data ?? Enumerable.Empty<Phase>();
            }
            catch
            {
                return Enumerable.Empty<Phase>();
            }
        }
      public async  Task<IEnumerable<string>> GetAllPeriodsData()
        {
            try
            {
                var apiUrl = $"{_navigation.BaseUri}{"api/CostCode/GetAllPeriodsData"}";
                //var apiUrl = "https://localhost:7176/api/CostCode/GetAllPhases";

                var response =
                    await _httpClient.GetFromJsonAsync<IEnumerable<string>>("api/CostCode/GetAllPeriodsData");

                return response ?? Enumerable.Empty<string>();
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }
        public async Task<IEnumerable<CashFlowRecord>> CashFlowAllDataByMulitpleJobsAndPeriod(CashFlowQueryInput input)
        {
            try
            {
                //var apiUrl = "https://localhost:7297/api/CashFlow/CashFlowAllDataByPeriod";
                var apiUrl = $"{GlobalEndpoints.CashFlowAllDataByMulitpleJobsAndPeriodUrl}";
                var response = await _httpClient.PostAsJsonAsync(apiUrl, input);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<CashFlowRecord[]>();
                }

                return default;

            }
            catch (Exception ex)
            {
                return Array.Empty<CashFlowRecord>();
            }
        }
        public string GetUserEmail()
        {
            _logger.LogInformation("entered into GetUserEmail");
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity.IsAuthenticated)
                return null;
            foreach (var c in user.Claims)
            {
                _logger.LogInformation($"ClaimType : {c.Type} = Claim Value: {c.Value}");
            }
            // Try email claim first, then preferred_username
            return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
                   ?? user.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
        }
    }
}
