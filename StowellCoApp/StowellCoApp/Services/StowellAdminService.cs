using Microsoft.AspNetCore.Components;
using StowellCoApp.DTO;
using StowellCoApp.Common;

namespace StowellCoApp.Services
{
    public class StowellAdminService : IStowellAdminService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<StowellAdminService> _logger;
        private readonly NavigationManager _navigation;

        public StowellAdminService(HttpClient httpClient, ILogger<StowellAdminService> logger, NavigationManager navigation)
        {
            _httpClient = httpClient;
            _logger = logger;
            _navigation = navigation;
        }
        public async Task ProcessDisconnectUsers()
        {
            try
            {
                var apiUrl = new Uri(new Uri(_navigation.BaseUri), GlobalEndpoints.GetProcessDisconnectUsersUrl).ToString();

                await _httpClient.PostAsync(GlobalEndpoints.GetProcessDisconnectUsersUrl, null);
               // response.EnsureSuccessStatusCode();

                _logger.LogInformation("ProcessDisconnectUsers executed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ProcessDisconnectUsers");
            }
        }

    }
}
