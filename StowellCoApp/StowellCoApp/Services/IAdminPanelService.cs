using StowellCoApp.DTO;
using StowellCoApp.Models;

namespace StowellCoApp.Services
{
    public interface IAdminPanelService
    {
        Task<IEnumerable<CostCodeRecord>> GetProjectIds();
        Task<IEnumerable<StowellUser>> GetAdminPanelUsers();
        Task<IEnumerable<ProjectManagementUser>> GetProjectEmails();
        Task<bool> InsertProjectManagementUser(ProjectManagementUser projectManagementUser);
        Task<IEnumerable<ProjectRoles>> GetUserRoles();
        Task<bool> DeleteProjectManagementUser(int jobNumber, string emailId);
    }
}
