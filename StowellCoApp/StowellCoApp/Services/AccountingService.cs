using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using StowellCoApp.Common;
using StowellCoApp.DTO;
using StowellCoApp.Models;
using System;
using System.Net.Http;
using System.Security.Claims;

namespace StowellCoApp.Services
{
    public class AccountingService : IAccountingService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AccountingService> _logger;
        private readonly NavigationManager _navigation;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountingService(HttpClient httpClient, ILogger<AccountingService> logger, NavigationManager navigation, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _navigation = navigation;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IEnumerable<CostCodeList>> GetCostCodes(string filter)
        {
            try
            {
                //var apiUrl = "https://localhost:7176/api/ProjectBudgetConfiguration/GetCostCodes/{filter}";
                // var apiUrl = $"https://localhost:7176/api/Accounting/GetCostCodes";
                var apiname = "api/Accounting/GetCostCodes";
                var apiUrl = $"{apiname}/{filter}";
                // POST the Phase object to the API
                //var response = await _httpClient.GetFromJsonAsync(apiUrl);
                var accounts = await _httpClient.GetFromJsonAsync<CostCodeList[]>(apiUrl);
                return accounts ?? Array.Empty<CostCodeList>();
            }
            catch (Exception ex)
            {
                // log if needed
                return Array.Empty<CostCodeList>();
            }

        }
        public async Task<IEnumerable<AccountingItem>> GetRecords()
        {
            try
            {
                _logger.LogInformation($"GetUserEmail before calling: ");
                string email = GetUserEmail();
                _logger.LogInformation($"GetUserEmail: " + email);
                var apiname = $"api/Accounting/GetRecords/{email}";
                var apiUrl = $"{_navigation.BaseUri}{apiname}";
                // POST the Phase object to the API
                //var response = await _httpClient.GetFromJsonAsync(apiUrl);
                _logger.LogInformation($"apiname: " + $"api/Accounting/GetRecords/{email}");
                var accounts = await _httpClient.GetFromJsonAsync<AccountingItem[]>($"api/Accounting/GetRecords/{email}");
                return accounts ?? Array.Empty<AccountingItem>();
            }
            catch (Exception ex)
            {
                // log if needed
                return Array.Empty<AccountingItem>();
            }

        }
        public async Task<IEnumerable<BudgetRecord>> GetApproveBudgetData(Phase phase)
        {
            try
            {
                //  var apiUrl = "https://localhost:7176/api/Accounting/GetApproveBudgetData";
                var apiUrl = new Uri(new Uri(_navigation.BaseUri), "api/Accounting/GetApproveBudgetData").ToString();
                // POST the Phase object to the API
                //var response = await _httpClient.PostAsJsonAsync(apiUrl, phase);
                var response = await _httpClient.PostAsJsonAsync("api/Accounting/GetApproveBudgetData", phase);

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
        public async Task<IEnumerable<BudgetRecord>> GetModifyBudgetData(Phase phase)
        {
            try
            {
                // var apiUrl = "https://localhost:7176/api/Accounting/GetModifyBudgetData";
                var apiUrl = new Uri(new Uri(_navigation.BaseUri), "api/Accounting/GetModifyBudgetData").ToString();
                // POST the Phase object to the API
                //var response = await _httpClient.PostAsJsonAsync(apiUrl, phase);
                var response = await _httpClient.PostAsJsonAsync("api/Accounting/GetModifyBudgetData", phase);

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
        
        public async Task<ApiResponse> BudgetApproveRequestAsync(Phase phase)
        {
            try
            {
                string email = GetUserEmail();
                phase.Email = email;
                var apiUrl = new Uri(new Uri(_navigation.BaseUri), "api/Accounting/BudgetApproveRequest").ToString();
                var response = await _httpClient.PostAsJsonAsync("api/Accounting/BudgetApproveRequest", phase);

                var result = await response.Content
                    .ReadFromJsonAsync<ApiResponse>();

                return result ?? new ApiResponse
                {
                    Success = false,
                    Message = "An error occurred while approving the budget."
                };
            }
            catch
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Unable to reach server."
                };
            }
        }
        public async Task<ApiResponse> BudgetRejectRequest(Phase phase)
        {
            try
            {
                string email = GetUserEmail();
                phase.Email = email;
                var apiUrl = new Uri(new Uri(_navigation.BaseUri), "api/Accounting/BudgetRejectRequest").ToString();
                var response = await _httpClient.PostAsJsonAsync("api/Accounting/BudgetRejectRequest", phase);

                var result = await response.Content
                    .ReadFromJsonAsync<ApiResponse>();

                return result ?? new ApiResponse
                {
                    Success = false,
                    Message = "An error occurred while rejecting the budget."
                };
            }
            catch
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Unable to reach server."
                };
            }
        }
        public async Task<SubmitToSageResponse> SubmitToSage(BidInfoDto bid)
        {
            try
            {
                var apiname = "api/Accounting/SubmitToSage"; // Adjust if needed
                var apiUrl = new Uri(new Uri(_navigation.BaseUri), apiname).ToString();

                var response = await _httpClient.PostAsJsonAsync(apiname, bid);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<SubmitToSageResponse>();
                    return result ?? new SubmitToSageResponse { Success = false, Message = "Empty response",IsExist=false };
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return new SubmitToSageResponse
                    {
                        Success = false,
                        Message = $"API call failed: {error}",
                        IsExist=false
                    };
                }
            }
            catch (Exception ex)
            {
                // log if needed
                return new SubmitToSageResponse
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}"
                };
            }
        }
        public string GetUserEmail()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity.IsAuthenticated)
                return null;

            // Try email claim first, then preferred_username
            return user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
                   ?? user.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
        }
    }
    public class SubmitToSageResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public bool IsExist { get; set; }
    }
}
