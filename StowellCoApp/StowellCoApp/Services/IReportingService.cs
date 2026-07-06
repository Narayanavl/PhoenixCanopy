using StowellCoApp.Models;

namespace StowellCoApp.Services
{
    public interface IReportingService
    {
        Task<CashFlowForecastResponse> GetCashFlowForecastReportData();
    }
}
