using Microsoft.AspNetCore.Components;
using StowellCoApp.DTO;
using System.Net.Http;

namespace StowellCoApp.Services
{
    public class ContactService : IContactService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ContactService> _logger;
        private readonly NavigationManager _navigation;

        public ContactService(HttpClient httpClient, ILogger<ContactService> logger, NavigationManager navigation)
        {
            _httpClient = httpClient;
            _logger = logger;
            _navigation = navigation;
        }
        public async Task<IEnumerable<ContactGroups>> GetAllGroups()
        {
            try
            {
                var contactGroups = await _httpClient.GetFromJsonAsync<ContactGroups[]>($"api/SecurityGroup/GetAllGroups");
                return contactGroups ?? Array.Empty<ContactGroups>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                // log if needed
                return Array.Empty<ContactGroups>();
            }

        }
        public async Task<IEnumerable<ContactGroupMembers>> GetGroupUsers(string groupId)
        {
            try
            {
                var contactGroups = await _httpClient.GetFromJsonAsync<ContactGroupMembers[]>($"api/SecurityGroup/GetGroupUsers/{groupId}");
                return contactGroups ?? Array.Empty<ContactGroupMembers>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                // log if needed
                return Array.Empty<ContactGroupMembers>();
            }

        }
    }
}
