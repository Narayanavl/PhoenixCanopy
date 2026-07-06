namespace StowellCoApp.Common
{
    public static class GlobalEndpoints
    {
        public const string GetCurrentJobsUrl = "api/PmHome/GetCurrentJobs";
        public const string GetChartDatasetsUrl = "api/PmHome/GetChartDatasets";
        public const string GetCashCollectedDatasetsUrl = "api/PmHome/GetCashCollectedDatasets";
        public const string GetSampleChartDatasetsUrl = "api/PmHome/GetSampleChartDatasets";
        public const string GetNetCashChartUrl = "api/PmHome/GetNetCashChart";
        public const string GetCalendarEventsUrl = "api/PmHome/GetCalendarEvents";
        public const string GetCurrentCostSummaryViewModelUrl = "api/ProjectOverview/GetCurrentCostSummaryViewModel";
        public const string GetJobCostSummaryRecordsUrl = "api/JobSummary/GetJobCostSummaryRecords";
        public const string GetJobCostSummaryRecordsWithRecnumUrl = "api/JobSummary/GetJobCostSummaryRecordsWithRecnum";
        public const string GetBidDetails = "api/Estimation/GetBidDetails";
        public const string GetEmployees = "api/Estimation/GetEmployees";
        public const string GetAllStatusesDataUrl = "api/CostCode/GetAllStatusesData";
        public const string GetAllUserJobsByStatusUrl = "api/CostCode/GetAllUserJobsByStatus";
        public const string GetCashFlowAllDataByPeriodUrl = "api/CashFlow/CashFlowAllDataByPeriod";
        public const string GetProcessDisconnectUsersUrl = "api/StowellAdmin/ProcessDisconnectUsers";
        public const string GetAllUserJobsUrl = "api/CostCode/GetAllUserJobs";
        public const string CashFlowAllDataByMulitpleJobsAndPeriodUrl = "api/CashFlow/CashFlowAllDataByMulitpleJobsAndPeriod";
        public const string GetProjectIdsUrl = "api/AdminPanel/GetProjectIds";
        public const string GetJobCodeDetails = "api/ProjectOverview/GetJobDetails";

        // Bid Details
        public const string GetCurrentBidsUrl = "api/Bids/GetCurrentBids";
        public const string GetBidQueueDataUrl = "api/Bids/GetBidQueueData";
        public const string GetBidByIdUrl = "api/Bids/GetBidById";
        public const string UpdateBidDetailsUrl = "api/Bids/UpdateBidDetails";
        public const string ConvertBidToProjectUrl = "api/Bids/ConvertBidToProject";
        public const string CloseBidAsLostUrl = "api/Bids/CloseBidAsLost";

        // Bid Amounts
        public const string GetBidAmountsUrl = "api/Bids/GetBidAmounts";
        public const string AddBidAmountUrl = "api/Bids/AddBidAmount";
        public const string UpdateBidAmountUrl = "api/Bids/UpdateBidAmount";
        public const string DeleteBidAmountUrl = "api/Bids/DeleteBidAmount";

        // Bid Access
        public const string GetBidAccessSummaryUrl = "api/Bids/GetBidAccessSummary";
        public const string AddBidAccessUrl = "api/Bids/AddBidAccess";
        public const string RemoveBidAccessUrl = "api/Bids/RemoveBidAccess";

        // create new bid
        public const string GetNextDisplayBidIDUrl = "api/Bids/GetNextDisplayBidID";
        public const string CreateNewBidUrl = "api/Bids/CreateNewBid";
        public const string GetEmployeesUrl = "api/Bids/GetBidAccessEmployees";
        public const string GetAllBidPhasesUrl= "api/Bids/GetAllBidPhases";
        public const string GetAllBidStatusUrl = "api/Bids/GetAllBidStatus";
    }
}
