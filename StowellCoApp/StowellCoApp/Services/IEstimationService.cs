using Microsoft.AspNetCore.Mvc;
using StowellCoApp.DTO;
using StowellCoApp.Models;

namespace StowellCoApp.Services
{
    public interface IEstimationService
    {
        Task<IEnumerable<BidItem>> GetOpenBids();
        Task<IEnumerable<BidItem>> GetClosedBids();
        Task<IEnumerable<BidItem>> GetPendingBids();
        Task<BidInfoDto> GetBidDetails(int jobId);
        Task<IEnumerable<EmployeeDetails>> GetEmployees(int jobId);
        Task<IEnumerable<BidEmployee>> GetBidEmployees(string jobId);
        Task<IEnumerable<ContractorClient>> GetAllContractorClients();
        Task<ContractorClient> GetContractorClientById(string clientId);
        Task<SubmitToSageResponse> InsertBid(BidAmount bidAmount);
        Task<bool> InsertPhase(BidPhase bidPhase);
        Task<bool> InsertEmployee(BidEmployee bidEmployee);
        Task<string> GetUserName();
        Task<string> GetNewBidNumber();
        Task<IEnumerable<ContractorClient>> GetAllEmployees();
        Task<IEnumerable<BidAmount>> GetBidsAmountAsync(string jobId);
        Task<IEnumerable<BidPhase>> GetPhases(string jobId);
        Task<SubmitToSageResponse> SubmitBid([FromBody] BidInfoDto bid);
        Task<SubmitToSageResponse> UpdateBid([FromBody] BidInfoDto bid);
        Task<bool> DeleteBidAmount(string id);
        Task<bool> DeletePhase(string id);
        Task<bool> DeleteEmployee(string id);
        Task<string> GetLoggedUserEmail();
    }

}
