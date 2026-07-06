using Microsoft.AspNetCore.Components;
using StowellCoApp.DTO;
using StowellCoApp.Common;
using System.Net.Http.Json;
using static StowellCoApp.Components.Pages.CalendarPage;
namespace StowellCoApp.Services
{
    public class CalendarApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CalendarApiClient> _logger;
        private readonly NavigationManager _navigation;
        //private readonly IHttpClientFactory _httpClientFactory;


        //public CalendarApiClient(IHttpClientFactory httpClientFactory)
        //{
        //    _httpClient = httpClientFactory.CreateClient("StowellCoAPI");
        //}
        //public CalendarApiClient(IHttpClientFactory httpClientFactory,HttpClient httpClient, ILogger<CalendarApiClient> logger,NavigationManager navigation)
        //{
        //    _httpClient = httpClient;
        //    _logger = logger;
        //    _navigation = navigation;
        //    _httpClientFactory = httpClientFactory;
        //}
        //public CalendarApiClient(IHttpClientFactory httpClientFactory, NavigationManager navigation, ILogger<CalendarApiClient> logger)
        //{
        //    _httpClient = httpClientFactory.CreateClient("StowellCoAPI");
        //    _navigation = navigation;
        //    _logger = logger;
        //}
        public CalendarApiClient(HttpClient httpClient, ILogger<CalendarApiClient> logger, NavigationManager navigation)
        {
            _httpClient = httpClient;
            _logger = logger;
            _logger.LogInformation("CalendarApiClient Constructor Start");
            _navigation = navigation;
            _logger.LogInformation("CalendarApiClient Constructor end");
        }
        public async Task<IEnumerable<MyEventViewModel>?> GetUserCalendarAsync(string email)
        { 
        //{
        //    _logger.LogInformation("Calling API: {Url}", GlobalEndpoints.GetCurrentJobsUrl);
        //    var apiUrl = new Uri(new Uri(_navigation.BaseUri), GlobalEndpoints.GetCurrentJobsUrl).ToString();
        //    _logger.LogInformation($"Calling apiUrl: {apiUrl}");
        //    //Console.WriteLine($"Calling API: {apiUrl}");

        //    // Call the API
        //    var jobs = await _httpClient.GetFromJsonAsync<CurrentJob[]>(apiUrl);
        //    //return await _httpClient.GetFromJsonAsync<IEnumerable<Event>>($"api/calendar/{email}");
        //    return await _httpClient.GetFromJsonAsync<IEnumerable<Event>>($"api/PmHome/GetCalendarEvents");
            // Log the API URL you are calling via HttpClient
            _logger.LogInformation("Calling API: {Url}", GlobalEndpoints.GetCalendarEventsUrl);

            //var jobs = await _httpClient.GetFromJsonAsync<CurrentJob[]>(GlobalEndpoints.GetCurrentJobsUrl);
            //  return jobs;
            try
            {
                // Build the API URL dynamically based on current host
                var apiUrl = new Uri(new Uri(_navigation.BaseUri), GlobalEndpoints.GetCalendarEventsUrl).ToString();
                _logger.LogInformation($"Calling apiUrl: {apiUrl}");
                //Console.WriteLine($"Calling API: {apiUrl}");
                var jobs = await _httpClient.GetFromJsonAsync<MyEventViewModel[]>(GlobalEndpoints.GetCalendarEventsUrl);

                // Call the API

               // var jobs = await _httpClient.GetFromJsonAsync<MyEventViewModel[]>(apiUrl);
                _logger.LogInformation($"http client end: ");
                _logger.LogInformation($"jobs: {jobs.Count()}");
                return jobs ?? Array.Empty<MyEventViewModel>();

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"exception:{ex}");
                _logger.LogInformation($"after getting result: {ex.Message}");
                //Console.WriteLine($"Error loading jobs: {ex.Message}");
                return Array.Empty<MyEventViewModel>();
            }
        }
    }

}