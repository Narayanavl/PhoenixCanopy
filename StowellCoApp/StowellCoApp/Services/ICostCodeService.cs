using Microsoft.AspNetCore.Mvc;
using StowellCoApp.DTO;

namespace StowellCoApp.Services
{
    public interface ICostCodeService
    {
        Task<IEnumerable<CostCodeRecord>> GetCostCodeRecords();
        Task<IEnumerable<BidItem>> GetBids();
		Task<IEnumerable<Phase>> GetJobPhasesData(int jobId);
        Task<IEnumerable<StatusModel>> GetAllStatusesData();
        Task<IEnumerable<CostCodeRecord>> GetAllUserJobsByStatus(int status);
        Task<IEnumerable<CashFlowRecord>> CashFlowAllDataByPeriod(CashFlowQueryInput input);
        Task<IEnumerable<CostCodeRecord>> GetAllUserJobs();
        Task<CurrentCostSummaryViewModel> GetCurrentCostSummaryViewModel(string recnum, List<string> costCode = null, string phase = "", string startDate = "", string endDate = "");
        Task<IEnumerable<ContractSummary>> GetContractSummaryData(string jobNumber);
        Task<IEnumerable<PrimeChangeList>> GetPrimeChangeList(string jobNumber);
        Task<IEnumerable<InvoiceReceivable>> GetInvoiceReceivable(string jobNumber);
        Task<IEnumerable<InvoicePayment>> GetARInvoice(string jobNumber);
        Task<IEnumerable<Phase>> GetAllPhases();
        Task<IEnumerable<string>> GetAllPeriodsData();
        Task<IEnumerable<CashFlowRecord>> CashFlowAllDataByMulitpleJobsAndPeriod(CashFlowQueryInput input);
        Task<IEnumerable<CostCode>> GetCostCodesForReport(string jobID);
        Task<List<StowellCoApp.DTO.Job>> GetJobCodesForReport();
        Task<List<Phase>> GetPhasesForReport(string jobID);
    }
}
