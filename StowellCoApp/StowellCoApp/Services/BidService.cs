using Microsoft.AspNetCore.Components;
using Org.BouncyCastle.Asn1.Ocsp;
using StowellCoApp.Common;
using StowellCoApp.Components.Pages.Bids;
using StowellCoApp.DTO;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;

namespace StowellCoApp.Services
{
    public class BidService : IBidService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<BidService> _logger;
        private readonly NavigationManager _navigation;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public BidService(HttpClient httpClient, ILogger<BidService> logger, NavigationManager navigation, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _navigation = navigation;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<IEnumerable<Bids>> GetCurrentBids()
        {
            // Log the API URL you are calling via HttpClient
            _logger.LogInformation("Calling API: {Url}", GlobalEndpoints.GetCurrentBidsUrl);

            try
            {
                _logger.LogInformation($"httpclient:" + _httpClient.BaseAddress);
                var bids = await _httpClient.GetFromJsonAsync<Bids[]>($"{GlobalEndpoints.GetCurrentBidsUrl}");
                _logger.LogInformation($"http client end: ");
                _logger.LogInformation($"bids: {bids.Count()}");
                return bids ?? Array.Empty<Bids>();

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"exception:{ex}");
                _logger.LogInformation($"after getting result: {ex.Message}");
                //Console.WriteLine($"Error loading jobs: {ex.Message}");
                return Array.Empty<Bids>();
            }
        }
        public async Task<BidQueueDTO> GetBidQueueData()
        {
            try
            {
                var apiUrl = $"{_navigation.BaseUri}{GlobalEndpoints.GetBidQueueDataUrl}";
                // var apiUrl = new Uri(new Uri(_navigation.BaseUri), "api/CostCode/GetAllUserJobs").ToString();
                var jobs = await _httpClient.GetFromJsonAsync<BidQueueDTO>($"{GlobalEndpoints.GetBidQueueDataUrl}");
                return jobs ?? new BidQueueDTO();

            }
            catch (Exception ex)
            {
                return new BidQueueDTO();
            }
        }
        public async Task<bool> CreateNewBid(CreateBidRequest request)
        {
            try
            {
                request.CreatedBy = GetUserEmail();
                var response = await _httpClient.PostAsJsonAsync(
                    GlobalEndpoints.CreateNewBidUrl,
                    request);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                // Optional: Read error response
                var errorMessage = await response.Content.ReadAsStringAsync();

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<BidRecords> GetBidById(string displayBidId)
        {
            _logger.LogInformation("Calling API: GetBidById for {DisplayBidId}", displayBidId);

            try
            {
                string url = $"{GlobalEndpoints.GetBidByIdUrl}/{displayBidId}";

                _logger.LogInformation("HttpClient BaseAddress: {BaseAddress}", _httpClient.BaseAddress);
                _logger.LogInformation("Final URL: {Url}", url);

                var bid = await _httpClient.GetFromJsonAsync<BidRecords>(url);

                _logger.LogInformation("API call completed");

                return bid ?? null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching bid by id {DisplayBidId}", displayBidId);

                return null;
            }
        }
        public async Task<IEnumerable<BidAmounts>> GetBidAmounts(string displayBidId)
        {
            _logger.LogInformation("Calling API: GetBidAmounts for {DisplayBidId}", displayBidId);

            try
            {
                string url = $"{GlobalEndpoints.GetBidAmountsUrl}/{displayBidId}";

                _logger.LogInformation("HttpClient BaseAddress: {BaseAddress}", _httpClient.BaseAddress);
                _logger.LogInformation("Final URL: {Url}", url);

                var bidAmounts = await _httpClient.GetFromJsonAsync<BidAmounts[]>(url);

                _logger.LogInformation("Bid amounts retrieved: {Count}", bidAmounts?.Length ?? 0);

                return bidAmounts ?? Array.Empty<BidAmounts>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching bid amounts for {DisplayBidId}", displayBidId);
                return Array.Empty<BidAmounts>();
            }
        }
        public async Task<IEnumerable<BidAccessSummary>> GetBidAccessSummary(string displayBidId)
        {
            _logger.LogInformation("Calling API: GetBidAccessSummary for {DisplayBidId}", displayBidId);

            try
            {
                string url = $"{GlobalEndpoints.GetBidAccessSummaryUrl}/{displayBidId}";

                var data = await _httpClient.GetFromJsonAsync<BidAccessSummary[]>(url);

                return data ?? Array.Empty<BidAccessSummary>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching bid access summary");
                return Array.Empty<BidAccessSummary>();
            }
        }
        public async Task<(string Message, int NewId)> AddBidAmount(AddBidAmountRequest request)
        {
            try
            {
                request.ModifiedBy = GetUserEmail();
                var response = await _httpClient.PostAsJsonAsync(
                    GlobalEndpoints.AddBidAmountUrl,
                    request);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError(
                        "API Error: {StatusCode}, Response: {Response}",
                        response.StatusCode,
                        responseContent);

                    return ($"API Error: {responseContent}", 0);
                }

                var result = JsonSerializer.Deserialize<AddBidAmountResponse>(
                    responseContent,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                return (result?.Message ?? "", result?.NewBidAmountID ?? 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding bid amount");
                return ("Error", 0);
            }
        }
        public async Task<string> UpdateBidAmount(UpdateBidAmountRequest request)
        {
            try
            {
                request.ModifiedBy = GetUserEmail();
                var response = await _httpClient.PutAsJsonAsync(
                    GlobalEndpoints.UpdateBidAmountUrl, request);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<SimpleResponse>();

                return result?.Message ?? "Success";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating bid amount");
                return "Error";
            }
        }
        public async Task<string> DeleteBidAmount(DeleteBidAmountRequest request)
        {
            try
            {
                request.ModifiedBy = GetUserEmail();
                var httpRequest = new HttpRequestMessage(HttpMethod.Delete, GlobalEndpoints.DeleteBidAmountUrl)
                {
                    Content = JsonContent.Create(request)
                };

                var response = await _httpClient.SendAsync(httpRequest);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<SimpleResponse>();

                return result?.Message ?? "Success";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting bid amount");
                return "Error";
            }
        }
        public async Task<(string Message, int NewId)> AddBidAccess(AddBidAccessRequest request)
        {
            try
            {
                request.ModifiedBy = GetUserEmail();
                var response = await _httpClient.PostAsJsonAsync(
                    GlobalEndpoints.AddBidAccessUrl, request);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<AddBidAccessResponse>();

                return (result?.Message ?? "", result?.NewBidAccessID ?? 0);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding bid access");
                return ("Error", 0);
            }
        }
        public async Task<string> RemoveBidAccess(RemoveBidAccessRequest request)
        {
            try
            {
                request.ModifiedBy = GetUserEmail();
                var httpRequest = new HttpRequestMessage(HttpMethod.Delete, GlobalEndpoints.RemoveBidAccessUrl)
                {
                    Content = JsonContent.Create(request)
                };

                var response = await _httpClient.SendAsync(httpRequest);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<SimpleResponse>();

                return result?.Message ?? "Success";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing bid access");
                return "Error";
            }
        }
        public async Task<(string Message, string NewProjectId)> ConvertBidToProject(ConvertBidToProjectRequest request)
        {
            try
            {
                request.ModifiedBy = GetUserEmail();
                var response = await _httpClient.PostAsJsonAsync(
                    GlobalEndpoints.ConvertBidToProjectUrl, request);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ConvertBidResponse>();

                return (result?.Message ?? "", result?.NewProjectID ?? "");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error converting bid");
                return ("Error", "");
            }
        }
        public async Task<string> CloseBidAsLost(CloseBidAsLostRequest request)
        {
            try
            {
                request.ModifiedBy= GetUserEmail();
                var response = await _httpClient.PostAsJsonAsync(
                    GlobalEndpoints.CloseBidAsLostUrl, request);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<SimpleResponse>();

                return result?.Message ?? "Success";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error closing bid as lost");
                return "Error";
            }
        }
        public async Task<bool> UpdateBidDetails(UpdateBidDetailsRequest request)
        {
            try
            {
                request.ModifiedBy = GetUserEmail();
                var response = await _httpClient.PostAsJsonAsync(
                    GlobalEndpoints.UpdateBidDetailsUrl,
                    request
                );

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateBidDetails failed");
                return false;
            }
        }
		public async Task<string?> GetNextDisplayBidID()
        {
            try
            {
                string createdBy = GetUserEmail();
                var response = await _httpClient.GetAsync(
                    $"{GlobalEndpoints.GetNextDisplayBidIDUrl}/{createdBy}");

                if (response.IsSuccessStatusCode)
                {
                    // var result = await _httpClient.GetFromJsonAsync<string>($"{GlobalEndpoints.GetNextDisplayBidID}");
                    var result = await response.Content.ReadFromJsonAsync<NextDisplayBidIDResponse>();

                    return result?.DisplayBidID;
                }

                // Optional: Read error message
                var errorMessage = await response.Content.ReadAsStringAsync();

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<List<Employee>> GetBidAccessEmployees()
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    GlobalEndpoints.GetEmployeesUrl);

                if (response.IsSuccessStatusCode)
                {
                    var result =
                        await response.Content.ReadFromJsonAsync<List<Employee>>();

                    return result ?? new List<Employee>();
                }

                var errorMessage = await response.Content.ReadAsStringAsync();

                _logger.LogError(
                    "Error getting employees: {StatusCode}, {Message}",
                    response.StatusCode,
                    errorMessage);

                return new List<Employee>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employees");

                return new List<Employee>();
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
            catch (Exception ex)
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
        public async Task<ContractorClient?> GetContractorClientByName(string clientName)
        {
            try
            {
                var apiname = "api/Bids/GetContractorClientByName";
                var apiUrl = $"{apiname}/{clientName}";
                var client = await _httpClient
                    .GetFromJsonAsync<ContractorClient>(apiUrl);

                return client;
            }
            catch
            {
                return null;
            }
        }
        public async Task<IEnumerable<Phase>> GetAllBidPhases()
        {
            try
            {
                var apiUrl = $"{_navigation.BaseUri}{"api/Bids/GetAllBidPhases"}";
              

                var response =
                    await _httpClient.GetFromJsonAsync<ApiListResponse<Phase>>("api/Bids/GetAllBidPhases");

                return response?.Data ?? Enumerable.Empty<Phase>();
            }
            catch
            {
                return Enumerable.Empty<Phase>();
            }
        }
        public async Task<IEnumerable<BidStatus>> GetAllBidStatus()
        {
            try
            {
                var apiUrl = $"{_navigation.BaseUri}{"api/Bids/GetAllBidStatus"}";


                var response =
                    await _httpClient.GetFromJsonAsync<ApiListResponse<BidStatus>>("api/Bids/GetAllBidStatus");

                return response?.Data ?? Enumerable.Empty<BidStatus>();
            }
            catch
            {
                return Enumerable.Empty<BidStatus>();
            }
        }
    }
}
