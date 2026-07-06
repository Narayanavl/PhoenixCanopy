using Azure;
using Microsoft.AspNetCore.Components;
using StowellCoApp.Common;
using StowellCoApp.DTO;
using StowellCoApp.Models;
using System;
using System.Security.Claims;

namespace StowellCoApp.Services
{
    public class EstimationService :IEstimationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<JobService> _logger;
        private readonly NavigationManager _navigation;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EstimationService(HttpClient httpClient, ILogger<JobService> logger, NavigationManager navigation, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _navigation = navigation;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<BidItem>> GetClosedBids()
        {
            try
            {
                //  var apiUrl = "https://localhost:7176/api/CostCode/GetCostCodeRecords";
                var apiUrl = new Uri(new Uri(_navigation.BaseUri), "api/Estimation/GetClosedBids").ToString();
                var jobs = await _httpClient.GetFromJsonAsync<BidItem[]>("api/Estimation/GetClosedBids");
                return jobs ?? Array.Empty<BidItem>();

            }
            catch (Exception ex)
            {
                return Array.Empty<BidItem>();
            }
        }

        public async Task<IEnumerable<BidItem>> GetOpenBids()
        {
            try
            {
                //  var apiUrl = "https://localhost:7176/api/CostCode/GetCostCodeRecords";
                var apiUrl = new Uri(new Uri(_navigation.BaseUri), "api/Estimation/GetOpenBids").ToString();
                var jobs = await _httpClient.GetFromJsonAsync<BidItem[]>("api/Estimation/GetOpenBids");
                return jobs ?? Array.Empty<BidItem>();

            }
            catch (Exception ex)
            {
                return Array.Empty<BidItem>();
            }
        }

        public async Task<IEnumerable<BidItem>> GetPendingBids()
        {
            try
            {
                //  var apiUrl = "https://localhost:7176/api/CostCode/GetCostCodeRecords";
                var apiUrl = new Uri(new Uri(_navigation.BaseUri), "api/Estimation/GetPendingBids").ToString();
                var jobs = await _httpClient.GetFromJsonAsync<BidItem[]>("api/Estimation/GetPendingBids");
                return jobs ?? Array.Empty<BidItem>();

            }
            catch (Exception ex)
            {
                return Array.Empty<BidItem>();
            }
        }
        public async Task<BidInfoDto> GetBidDetails(int jobId)
        {
            try
            {
                //var apiUrl = "https://localhost:7176/api/Estimation/GetBidDetails/{jobId}";
                var apiUrl = $"{GlobalEndpoints.GetBidDetails}/{jobId}";
                var jobs = await _httpClient.GetFromJsonAsync<BidInfoDto>(apiUrl);
                // var apiUrl = new Uri(new Uri(_navigation.BaseUri), "api/Estimation/GetBidDetails{jobId}").ToString();
                //var jobs = await _httpClient.GetFromJsonAsync<BidInfoDto>(apiUrl);
                return jobs ?? new BidInfoDto();
              

            }
            catch (Exception ex)
            {
                return new BidInfoDto();
            }
        }

        public async Task<IEnumerable<EmployeeDetails>> GetEmployees(int jobId)
        {
            try
            {
                //var apiUrl = "https://localhost:7176/api/Estimation/GetBidDetails/{jobId}";
                var apiUrl = $"{GlobalEndpoints.GetEmployees}/{jobId}";
                var employees = await _httpClient.GetFromJsonAsync<EmployeeDetails[]>(apiUrl);
                // var apiUrl = new Uri(new Uri(_navigation.BaseUri), "api/Estimation/GetBidDetails{jobId}").ToString();
                //var jobs = await _httpClient.GetFromJsonAsync<BidInfoDto>(apiUrl);
                return employees ?? Array.Empty<EmployeeDetails>();
               // return employees ?? new EmployeeDetails();


            }
            catch (Exception ex)
            {
                return Array.Empty<EmployeeDetails>();
            }
        }

        public async Task<IEnumerable<ContractorClient>> GetAllEmployees()
        {
            try
            {
                //var apiUrl = "https://localhost:7176/api/Estimation/GetAllEmployees";
                var apiname = "api/Estimation/GetAllEmployees";
                var apiUrl = $"{_navigation.BaseUri}{apiname}";
                //var apiUrl = $"{_navigation.BaseUri}{GlobalEndpoints.GetBidDetails}/{jobId}";
                var employees = await _httpClient.GetFromJsonAsync<ContractorClient[]>(apiname);
                // var apiUrl = new Uri(new Uri(_navigation.BaseUri), "api/Estimation/GetBidDetails{jobId}").ToString();
                //var jobs = await _httpClient.GetFromJsonAsync<BidInfoDto>(apiUrl);
                return employees ?? Array.Empty<ContractorClient>();
            }
            catch (Exception ex)
            {
                return Array.Empty<ContractorClient>();
            }
        }

        public async Task<IEnumerable<ContractorClient>> GetAllContractorClients()
        {
            try
            {
                //var apiUrl = "https://localhost:7176/api/Estimation/GetAllContractorClients";
                var apiname = "api/Estimation/GetAllContractorClients";
                var apiUrl = $"{_navigation.BaseUri}{apiname}";
                var response = await _httpClient
                    .GetFromJsonAsync<ApiListResponse<ContractorClient>>(apiname);

                return response?.Data ?? Enumerable.Empty<ContractorClient>();
            }
            catch
            {
                return Enumerable.Empty<ContractorClient>();
            }
        }

        public async Task<ContractorClient?> GetContractorClientById(string clientId)
        {
            try
            {
                //var apiUrl = $"https://localhost:7176/api/Estimation/GetContractorClientById/{clientId}";
                var apiname = "api/Estimation/GetContractorClientById";
                var apiUrl = $"{apiname}/{clientId}";
                var client = await _httpClient
                    .GetFromJsonAsync<ContractorClient>(apiUrl);

                return client;
            }
            catch
            {
                return null;
            }
        }


        //public async Task<IEnumerable<EmployeeDetails>> GetEmployees(int jobId)
        //{
        //    try
        //    {
        //        //var apiUrl = "https://localhost:7176/api/Estimation/GetBidDetails/{jobId}";
        //        var apiUrl = $"{_navigation.BaseUri}{GlobalEndpoints.GetEmployees}/{jobId}";
        //        var employees = await _httpClient.GetFromJsonAsync<EmployeeDetails[]>(apiUrl);
        //        // var apiUrl = new Uri(new Uri(_navigation.BaseUri), "api/Estimation/GetBidDetails{jobId}").ToString();
        //        //var jobs = await _httpClient.GetFromJsonAsync<BidInfoDto>(apiUrl);
        //        return employees ?? Array.Empty<EmployeeDetails>();
        //        // return employees ?? new EmployeeDetails();


        //    }
        //    catch (Exception ex)
        //    {
        //        return Array.Empty<EmployeeDetails>();
        //    }
        //}
        public async Task<IEnumerable<BidEmployee>> GetBidEmployees(string jobId)
        {
            try
            {
               // var apiUrl = $"https://localhost:7176/api/Estimation/GetBidEmployees/{jobId}";
                var apiname = "api/Estimation/GetBidEmployees";
                var apiUrl = $"{apiname}/{jobId}";
                //  var apiUrl = $"{_navigation.BaseUri}{GlobalEndpoints.GetEmployees}/{jobId}";
                var employees = await _httpClient.GetFromJsonAsync<BidEmployee[]>(apiUrl);
                // var apiUrl = new Uri(new Uri(_navigation.BaseUri), "api/Estimation/GetBidDetails{jobId}").ToString();
                //var jobs = await _httpClient.GetFromJsonAsync<BidInfoDto>(apiUrl);
                return employees ?? Array.Empty<BidEmployee>();
                // return employees ?? new EmployeeDetails();


            }
            catch (Exception ex)
            {
                return Array.Empty<BidEmployee>();
            }
        }

        public async Task<SubmitToSageResponse> InsertBid(BidAmount bidAmount)
        {
            try
            {
                //var apiUrl = "https://localhost:7176/api/Estimation/InsertBid";
                var apiname = "api/Estimation/InsertBid";
                var apiUrl = $"{_navigation.BaseUri}{apiname}";
                var response = await _httpClient.PostAsJsonAsync(apiname, bidAmount);

                //return response.IsSuccessStatusCode;
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<SubmitToSageResponse>();
                    return result ?? new SubmitToSageResponse { Success = false, Message = "Empty response", IsExist = false };
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return new SubmitToSageResponse
                    {
                        Success = false,
                        Message = $"API call failed: {error}",
                        IsExist = false
                    };
                }
            }
            catch (Exception ex)
            {
                // log if needed
                // return false;
                return new SubmitToSageResponse
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}"
                };
            }
        }

        public async Task<string> GetUserName()
        {
            try
            {
                string email= GetUserEmail();
                if (string.IsNullOrWhiteSpace(email))
                    return string.Empty;

                var encodedEmail = Uri.EscapeDataString(email);
                //var apiUrl = $"https://localhost:7176/api/Estimation/GetUserName/{email}";
                var apiname = "api/Estimation/GetUserName";
                var apiUrl = $"{apiname}/{email}";
                var response = await _httpClient.GetAsync(apiUrl);

                if (!response.IsSuccessStatusCode)
                    return string.Empty;

                return await response.Content.ReadAsStringAsync();
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task<string> GetNewBidNumber()
        {
            try
            {
               // var apiUrl =
                //    $"https://localhost:7176/api/Estimation/GetNewBidNumber";
                var apiname = "api/Estimation/GetNewBidNumber";
                var apiUrl = $"{apiname}";
                var response = await _httpClient.GetAsync(apiname);

                if (!response.IsSuccessStatusCode)
                    return string.Empty;

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                // log if needed
                return string.Empty;
            }
        }

        public async Task<IEnumerable<BidAmount>> GetBidsAmountAsync(string jobId)
        {
            try
            {
                //var apiUrl = $"https://localhost:7176/api/Estimation/GetBidsAmountAsync/{jobId}";
                var apiname = "api/Estimation/GetBidsAmountAsync";
                var apiUrl = $"{apiname}/{jobId}";
                var bidAmounts = await _httpClient.GetFromJsonAsync<List<BidAmount>>(apiUrl);

                return bidAmounts ?? Enumerable.Empty<BidAmount>();
            }
            catch (Exception ex)
            {
                // optional logging
                return Enumerable.Empty<BidAmount>();
            }
        }

        public async Task<IEnumerable<BidPhase>> GetPhases(string jobId)
        {
            try
            {
                //var apiUrl = $"https://localhost:7176/api/Estimation/GetPhases/{jobId}";
                var apiname = "api/Estimation/GetPhases";
                var apiUrl = $"{apiname}/{jobId}";
                var bidAmounts = await _httpClient.GetFromJsonAsync<List<BidPhase>>(apiUrl);

                return bidAmounts ?? Enumerable.Empty<BidPhase>();
            }
            catch (Exception ex)
            {
                // optional logging
                return Enumerable.Empty<BidPhase>();
            }
        }

        public async Task<bool> InsertPhase(BidPhase bidPhase)
        {
            try
            {
                //var apiUrl = "https://localhost:7176/api/Estimation/InsertPhase";
                var apiname = "api/Estimation/InsertPhase";
                var apiUrl = $"{_navigation.BaseUri}{apiname}";
                var response = await _httpClient.PostAsJsonAsync(apiname, bidPhase);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // log if needed
                return false;
            }
        }
        public async Task<bool> InsertEmployee(BidEmployee bidEmployee)
        {
            try
            {
                //var apiUrl = "https://localhost:7176/api/Estimation/InsertEmployee";
                var apiname = "api/Estimation/InsertEmployee";
                var apiUrl = $"{_navigation.BaseUri}{apiname}";
                var response = await _httpClient.PostAsJsonAsync(apiname, bidEmployee);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // log if needed
                return false;
            }
        }

        public async Task<SubmitToSageResponse> SubmitBid(BidInfoDto bid)
        {
            try
            {
                //var apiUrl = "https://localhost:7176/api/Estimation/SubmitBid";
                var apiname = "api/Estimation/SubmitBid";
                var apiUrl = new Uri(new Uri(_navigation.BaseUri), apiname).ToString();
                var response = await _httpClient.PostAsJsonAsync(apiname, bid);

                //return response.IsSuccessStatusCode;
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<SubmitToSageResponse>();
                    return new SubmitToSageResponse { Success = result.Success, Message = result.Message, IsExist = false };
                }
                else
                {
                    var error = await response.Content.ReadFromJsonAsync<SubmitToSageResponse>();
                    return new SubmitToSageResponse
                    {
                        Success = false,
                        Message =error.Message,
                        IsExist = false
                    };
                }
            }
            catch (Exception ex)
            {
                // log if needed
                //return false;
                return new SubmitToSageResponse
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}"
                };
            }
        }
        public async Task<SubmitToSageResponse> UpdateBid(BidInfoDto bid)
        {
            try
            {
                //var apiUrl = "https://localhost:7176/api/Estimation/SubmitBid";
                var apiname = "api/Estimation/UpdateBid";
                var apiUrl = new Uri(new Uri(_navigation.BaseUri), apiname).ToString();
                var response = await _httpClient.PostAsJsonAsync(apiname, bid);

                //return response.IsSuccessStatusCode;
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<SubmitToSageResponse>();
                    return result ?? new SubmitToSageResponse { Success = false, Message = "Empty response", IsExist = false };
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return new SubmitToSageResponse
                    {
                        Success = false,
                        Message = $"API call failed: {error}",
                        IsExist = false
                    };
                }
            }
            catch (Exception ex)
            {
                return new SubmitToSageResponse
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}"
                };
            }
        }
        public async Task<string> GetLoggedUserEmail()
        {
            try
            {
                string email = GetUserEmail();
                if (string.IsNullOrWhiteSpace(email))
                    return string.Empty;
                return email;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return string.Empty;
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
        public async Task<bool> DeleteBidAmount(string Id)
        {
            try
            {
                var apiname = "api/Estimation/DeleteBidAmount";
                var response = await _httpClient.PostAsJsonAsync(apiname, Id);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // log if needed
                return false;
            }
        }
        public async Task<bool> DeletePhase(string Id)
        {
            try
            {
                var apiname = "api/Estimation/DeletePhase";
                var response = await _httpClient.PostAsJsonAsync(apiname, Id);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // log if needed
                return false;
            }
        }
        public async Task<bool> DeleteEmployee(string Id)
        {
            try
            {
                var apiname = "api/Estimation/DeleteEmployee";
                var response = await _httpClient.PostAsJsonAsync(apiname, Id);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // log if needed
                return false;
            }
        }
    }
}
