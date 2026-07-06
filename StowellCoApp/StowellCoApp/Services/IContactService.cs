using StowellCoApp.DTO;

namespace StowellCoApp.Services
{
    public interface IContactService
    {
        Task<IEnumerable<ContactGroups>> GetAllGroups();
        Task<IEnumerable<ContactGroupMembers>> GetGroupUsers(string groupId);
    }
}
