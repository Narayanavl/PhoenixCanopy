using Microsoft.AspNetCore.Mvc;
using StowellCoApp.DTO;

namespace StowellCoApp.Services
{
    public interface IBidService
    {
        Task<IEnumerable<Bids>> GetCurrentBids();
        Task<BidQueueDTO> GetBidQueueData();
        Task<bool> CreateNewBid(CreateBidRequest request);
        Task<BidRecords> GetBidById(string displayBidId);
        Task<IEnumerable<BidAmounts>> GetBidAmounts(string displayBidId);
        Task<IEnumerable<BidAccessSummary>> GetBidAccessSummary(string displayBidId);
        Task<(string Message, int NewId)> AddBidAmount(AddBidAmountRequest request);
        Task<string> UpdateBidAmount(UpdateBidAmountRequest request);
        Task<string> DeleteBidAmount(DeleteBidAmountRequest request);
        Task<(string Message, int NewId)> AddBidAccess(AddBidAccessRequest request);
        Task<string> RemoveBidAccess(RemoveBidAccessRequest request);
        Task<(string Message, string NewProjectId)> ConvertBidToProject(ConvertBidToProjectRequest request);
        Task<string> CloseBidAsLost(CloseBidAsLostRequest request);
        Task<bool> UpdateBidDetails(UpdateBidDetailsRequest request);
        Task<string?> GetNextDisplayBidID();
        Task<ContractorClient?> GetContractorClientByName(string clientName);
        Task<List<Employee>> GetBidAccessEmployees();
        Task<IEnumerable<Phase>> GetAllBidPhases();
        Task<IEnumerable<BidStatus>> GetAllBidStatus();
    }
}
