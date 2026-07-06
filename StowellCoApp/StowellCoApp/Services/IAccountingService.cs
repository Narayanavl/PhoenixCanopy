using StowellCoApp.DTO;
using StowellCoApp.Models;

namespace StowellCoApp.Services
{
    public interface IAccountingService
    {
        Task<IEnumerable<CostCodeList>> GetCostCodes(string filter);
        Task<IEnumerable<AccountingItem>> GetRecords();
        Task<IEnumerable<BudgetRecord>> GetApproveBudgetData(Phase phase);
        Task<IEnumerable<BudgetRecord>> GetModifyBudgetData(Phase phase);
        Task<ApiResponse> BudgetApproveRequestAsync(Phase phase);
        Task<ApiResponse> BudgetRejectRequest(Phase phase);
        Task<SubmitToSageResponse> SubmitToSage(BidInfoDto bid);
    }
}
