using StowellCoApp.DTO;

namespace StowellCoApp.Services
{
    public interface IProjectBudget
    {
        Task<IEnumerable<ProjectBudgetQueueItem>> GetProjectBudgetRecords();
        Task<IEnumerable<BudgetRecord>> BudgetData(Phase input);
        Task<IEnumerable<BudgetRecord>> SaveCreateBatchCostCodes(List<BudgetRecord> input);
        Task<IEnumerable<BudgetRecord>> SubmitBatchCostCodes(List<BudgetRecord> input);
        Task<IEnumerable<string>> GetCostCodesAsync(string filter);
        Task<HttpResponseMessage> UploadAddExcelAsync(MultipartFormDataContent content);
        Task<HttpResponseMessage> UploadAddExcelAsync(List<BudgetRecord> budgetRecord);

    }
}
