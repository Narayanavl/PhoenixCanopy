using StowellCoApp.DTO;

namespace StowellCoApp.Services
{
    public interface IProjectOverviewService
    {
        Task<CurrentCostSummaryViewModel> GetCurrentCostSummaryViewModel(string recnum);
        Task<Job> GetJobCodeDetails(string recnum);
    }
}
