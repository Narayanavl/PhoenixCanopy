using StowellCoApp.DTO;

namespace StowellCoApp.Services
{
    public interface IJobService
    {
       Task<IEnumerable<CurrentJob>> GetCurrentJobs();
        Task<IEnumerable<ChartData>> GetChartDatasets();
        Task<IEnumerable<ChartData>> GetCashCollectedDatasets();
        Task<IEnumerable<ChartData>> GetSampleChartDatasets();
        Task<IEnumerable<ChartData>> GetNetCashChart();
    }
}
