using StowellCoApp.DTO;

namespace StowellCoApp.Services
{
    public interface IJobSummaryService
    {
        Task<IEnumerable<CostCodeSummaryRecord>> GetJobCostSummaryRecords(string recnum = "");
    }
}
