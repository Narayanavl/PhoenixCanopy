using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using StowellCoApp.Common;
using StowellCoApp.DTO;
using System.Data;
using System.Net.Http;
using System.Security.Claims;

namespace StowellCoApp.Services
{
    public class JobService : IJobService
    {

        private readonly HttpClient _httpClient;
        private readonly ILogger<JobService> _logger;
        private readonly NavigationManager _navigation;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JobService(HttpClient httpClient, ILogger<JobService> logger,NavigationManager navigation,IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _logger = logger;
            _navigation = navigation;
            _httpContextAccessor = httpContextAccessor;
        }


        //public async Task<IEnumerable<CurrentJob>> GetCurrentJobs()
        //{
            

        //    return await _httpClient.GetFromJsonAsync<CurrentJob[]>(GlobalEndpoints.GetCurrentJobsUrl);
        //}
        public async Task<IEnumerable<CurrentJob>> GetCurrentJobs()
        {
            // Log the API URL you are calling via HttpClient
            _logger.LogInformation("Calling API: {Url}", GlobalEndpoints.GetCurrentJobsUrl);

            //var jobs = await _httpClient.GetFromJsonAsync<CurrentJob[]>(GlobalEndpoints.GetCurrentJobsUrl);
          //  return jobs;
            try
            {
                // Build the API URL dynamically based on current host
                //var apiUrl = new Uri(new Uri(_navigation.BaseUri), GlobalEndpoints.GetCurrentJobsUrl).ToString();
                //_logger.LogInformation($"Calling apiUrl: {apiUrl}");
                //Console.WriteLine($"Calling API: {apiUrl}");
                // Call the API
                string email = await GetUserEmail();
                _logger.LogInformation($"httpclient:"+ _httpClient.BaseAddress);
                var jobs = await _httpClient.GetFromJsonAsync<CurrentJob[]>($"{GlobalEndpoints.GetCurrentJobsUrl}/{email}");
                _logger.LogInformation($"http client end: ");
                _logger.LogInformation($"jobs: {jobs.Count()}");
                return jobs ?? Array.Empty<CurrentJob>();
                
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"exception:{ex}");
                _logger.LogInformation($"after getting result: {ex.Message}");
                //Console.WriteLine($"Error loading jobs: {ex.Message}");
                return Array.Empty<CurrentJob>();
            }
        }

        public async Task<IEnumerable<ChartData>> GetChartDatasets()
        {
            return await _httpClient.GetFromJsonAsync<ChartData[]>(GlobalEndpoints.GetChartDatasetsUrl);
        }
        public async Task<IEnumerable<ChartData>> GetCashCollectedDatasets()
        {
            _logger.LogInformation("Calling API: {Url}", GlobalEndpoints.GetCashCollectedDatasetsUrl);

            //var jobs = await _httpClient.GetFromJsonAsync<CurrentJob[]>(GlobalEndpoints.GetCurrentJobsUrl);
            //  return jobs;
            try
            {
                // Build the API URL dynamically based on current host
                //var apiUrl = new Uri(new Uri(_navigation.BaseUri), GlobalEndpoints.GetCashCollectedDatasetsUrl).ToString();
                //_logger.LogInformation($"Calling apiUrl: {apiUrl}");
                //Console.WriteLine($"Calling API: {apiUrl}");

                // Call the API
                var chartDatas = await _httpClient.GetFromJsonAsync<ChartData[]>(GlobalEndpoints.GetCashCollectedDatasetsUrl);
                _logger.LogInformation($"http client end: ");
                _logger.LogInformation($"chartDatas: {chartDatas.Count()}");
                return chartDatas ?? Array.Empty<ChartData>();

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"exception:{ex}");
                _logger.LogInformation($"after getting result: {ex.Message}");
                //Console.WriteLine($"Error loading jobs: {ex.Message}");
                return Array.Empty<ChartData>();
            }
        }
        public async Task<IEnumerable<ChartData>> GetSampleChartDatasets()
        {
            //   return await _httpClient.GetFromJsonAsync<ChartData[]>(GlobalEndpoints.GetSampleChartDatasetsUrl);
            // Log the API URL you are calling via HttpClient
            _logger.LogInformation("Calling API: {Url}", GlobalEndpoints.GetSampleChartDatasetsUrl);

            //var jobs = await _httpClient.GetFromJsonAsync<CurrentJob[]>(GlobalEndpoints.GetCurrentJobsUrl);
            //  return jobs;
            try
            {
                // Build the API URL dynamically based on current host
                //var apiUrl = new Uri(new Uri(_navigation.BaseUri), GlobalEndpoints.GetSampleChartDatasetsUrl).ToString();
                //_logger.LogInformation($"Calling apiUrl: {apiUrl}");
                //Console.WriteLine($"Calling API: {apiUrl}");
                string email = await GetUserEmail();
                // Call the API
                var chartDatas = await _httpClient.GetFromJsonAsync<ChartData[]>($"{GlobalEndpoints.GetSampleChartDatasetsUrl}/{email}");
                _logger.LogInformation($"http client end: ");
                _logger.LogInformation($"chartDatas: {chartDatas.Count()}");
                return chartDatas ?? Array.Empty<ChartData>();

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"exception:{ex}");
                _logger.LogInformation($"after getting result: {ex.Message}");
                //Console.WriteLine($"Error loading jobs: {ex.Message}");
                return Array.Empty<ChartData>();
            }
        }
        public async Task<IEnumerable<ChartData>> GetNetCashChart()
        {
            // Log the API URL you are calling via HttpClient
            _logger.LogInformation("Calling API: {Url}", GlobalEndpoints.GetNetCashChartUrl);

            //var jobs = await _httpClient.GetFromJsonAsync<CurrentJob[]>(GlobalEndpoints.GetCurrentJobsUrl);
            //  return jobs;
            try
            {
                // Build the API URL dynamically based on current host
                //var apiUrl = new Uri(new Uri(_navigation.BaseUri), GlobalEndpoints.GetNetCashChartUrl).ToString();
                //_logger.LogInformation($"Calling apiUrl: {apiUrl}");
                //Console.WriteLine($"Calling API: {apiUrl}");

                // Call the API
                var jobs = await _httpClient.GetFromJsonAsync<ChartData[]>(GlobalEndpoints.GetNetCashChartUrl);
                _logger.LogInformation($"http client end: ");
                _logger.LogInformation($"jobs: {jobs.Count()}");
                return jobs ?? Array.Empty<ChartData>();

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"exception:{ex}");
                _logger.LogInformation($"after getting result: {ex.Message}");
                //Console.WriteLine($"Error loading jobs: {ex.Message}");
                return Array.Empty<ChartData>();
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
