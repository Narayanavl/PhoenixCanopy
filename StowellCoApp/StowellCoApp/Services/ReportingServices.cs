using Microsoft.AspNetCore.Components;
using StowellCoApp.Models;
using System.Net.Http;

namespace StowellCoApp.Services
{
    public class ReportingServices : IReportingService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ReportingServices> _logger;
        private readonly NavigationManager _navigation;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReportingServices(HttpClient httpClient, ILogger<ReportingServices> logger, NavigationManager navigation, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _navigation = navigation;
            _httpContextAccessor = httpContextAccessor;
        }
    
        public async Task<CashFlowForecastResponse> GetCashFlowForecastReportData()
        {
            try
            {
                var apiname = "api/Reports/GetCashFlowForecastReportData";
                var apiUrl = $"{apiname}";
                // POST the Phase object to the API
                var forecastResponse = await _httpClient.GetFromJsonAsync<CashFlowForecastResponse>(apiUrl);
                return forecastResponse ?? new CashFlowForecastResponse();
            }
            catch (Exception ex)
            {
                // log if needed
                return new CashFlowForecastResponse();
            }

        }
    }
}
