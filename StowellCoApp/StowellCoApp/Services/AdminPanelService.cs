using Microsoft.AspNetCore.Components;
using StowellCoApp.DTO;
using StowellCoApp.Models;
using StowellCoApp.Common;
using System.Net.Http;

namespace StowellCoApp.Services
{
    public class AdminPanelService:IAdminPanelService
    {

        private readonly HttpClient _httpClient;
        private readonly ILogger<JobService> _logger;
        private readonly NavigationManager _navigation;


        public AdminPanelService(HttpClient httpClient, ILogger<JobService> logger, NavigationManager navigation)
        {
            _httpClient = httpClient;
            _logger = logger;
            _navigation = navigation;
        }
        
        public async Task<IEnumerable<CostCodeRecord>> GetProjectIds()
        {
            try
            {
                var apiUrl = $"{GlobalEndpoints.GetProjectIdsUrl}";
                var jobs = await _httpClient.GetFromJsonAsync<CostCodeRecord[]>(GlobalEndpoints.GetProjectIdsUrl);
                return jobs ?? Array.Empty<CostCodeRecord>();

            }
            catch (Exception ex)
            {
                return Array.Empty<CostCodeRecord>();
            }
        }
		 public async Task<IEnumerable<ProjectRoles>> GetUserRoles()
        {
            try
            {
                // var apiUrl = $"{_navigation.BaseUri}{GlobalEndpoints.GetProjectIdsUrl}";
                var apiname = "api/AdminPanel/GetUserRoles";
                var apiUrl = $"{apiname}";
                var jobs = await _httpClient.GetFromJsonAsync<ProjectRoles[]>(apiUrl);
                return jobs ?? Array.Empty<ProjectRoles>();

            }
            catch (Exception ex)
            {
                return Array.Empty<ProjectRoles>();
            }
        }
        public async Task<IEnumerable<StowellUser>> GetAdminPanelUsers()
        {
            try
            {
               // var apiUrl = "https://localhost:7176/api/AdminPanel/GetAdminPanelUsers";
                var apiname = "api/AdminPanel/GetAdminPanelUsers";
                var apiUrl = $"{_navigation.BaseUri}{apiname}";
                var users = await _httpClient.GetFromJsonAsync<StowellUser[]>(apiname);

                return users ?? Array.Empty<StowellUser>();
            }
            catch (Exception ex)
            {
                // Optional: log exception
                return Array.Empty<StowellUser>();
            }
        }
        public async Task<IEnumerable<ProjectManagementUser>> GetProjectEmails()
        {
            try
            {
                //var apiUrl = "https://localhost:7176/api/AdminPanel/GetAdminPanelUsers";
                var apiname = "api/AdminPanel/GetProjectEmails";
                var apiUrl = $"{_navigation.BaseUri}{apiname}";
                var users = await _httpClient.GetFromJsonAsync<ProjectManagementUser[]>(apiname);

                return users ?? Array.Empty<ProjectManagementUser>();
            }
            catch (Exception ex)
            {
                // Optional: log exception
                return Array.Empty<ProjectManagementUser>();
            }
        }
        public async Task<bool> InsertProjectManagementUser(ProjectManagementUser projectManagementUser)
        {
            try
            {
                var apiname = "api/AdminPanel/InsertProjectManagementUser";
                var apiUrl = $"{_navigation.BaseUri}{apiname}";
              //  var apiUrl = "https://localhost:7176/api/AdminPanel/InsertProjectManagementUser";
                //var apiname = "api/Estimation/InsertPhase";
              //  var apiUrl = $"{_navigation.BaseUri}{apiname}";
                var response = await _httpClient.PostAsJsonAsync(apiname, projectManagementUser);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // log if needed
                return false;
            }
        }
  public async Task<bool> DeleteProjectManagementUser(int jobNumber, string emailId)
        {
            try
            {
                var apiName = "api/AdminPanel/DeleteProjectManagementUser";
                var apiUrl =
                    $"{apiName}?jobNumber={jobNumber}&emailId={Uri.EscapeDataString(emailId)}";

                var response = await _httpClient.DeleteAsync(apiUrl);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                // optional logging
                return false;
            }
        }
    }
}
