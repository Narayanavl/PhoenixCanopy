namespace StowellCoApp.DTO
{
    public class BidInfoDto
    {
        public string JobID { get; set; }
        public string? Bidder { get; set; }
        public string? Phase { get; set; }
        public string? BidStatus { get; set; }
        public string? Division { get; set; }
        public string? Department { get; set; }
        public string? JobType { get; set; }
        public string? JobName { get; set; }
        public string? ShortName { get; set; }
        public string? ContractNumber { get; set; }
        public string? ContractDate { get; set; }
        public decimal? ContractAmount { get; set; } = 0.0m;
        public string? EstStartDate { get; set; }
        public string? EstCompletionDate { get; set; }

        public ProjectLocationDto?  ProjectLocation { get; set; }
        public ContractorInfoDto? ContractorInfo { get; set; }
        public ProjectInfoDto? ProjectInfo { get; set; }
    }

    public class ProjectLocationDto
    {
        public string? JobID { get; set; }
        public string? Jobsite { get; set; }
        public string? Address { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? SalesTaxDistrict { get; set; }
    }

    public class ContractorInfoDto
    {
        public string? JobID { get; set; }
        public string? Client { get; set; }
        public string? Address { get; set; }
        public string? Address2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string?  Zip { get; set; }
    }

    public class ProjectInfoDto
    {
        public string? JobID { get; set; }
        public int?  InsuranceType { get; set; }
        public int? Bonded { get; set; }
        public int? TaxExcempt { get; set; }
        public int? InvoiceSubmittal { get; set; }
        public int? CertifiedPayroll { get; set; }
        public string? NetTermsDate { get; set; }
        public decimal? RetainagePerc { get; set; } = 0.0m;
    }
    public class BidItem
    {
        public int ID { get; set; }
        public string JobID { get; set; }
        public DateTime BidDate { get; set; }
        public string Address { get; set; }
        public string Submitter { get; set; }
        public string BidStatus { get; set; }
    }

    public class BidQueueViewModel
    {
        public List<BidItem> OpenBids { get; set; }
        public List<BidItem> ClosedBids { get; set; }
        public List<BidItem> PendingBids { get; set; }
    }

    public class AccountingItem
    {
        public int ID { get; set; }
        public string JobID { get; set; }
        public string JobName { get; set; }
        public string JobType { get; set; }
        public DateTime BidDate { get; set; }
        public string Address { get; set; }
        public string Submitter { get; set; }
        public string BidStatus { get; set; }
        public string StatusID { get; set; }
    }

    public class ProjectBudgetQueueItem
    {
        public int ID { get; set; }
        public string JobID { get; set; }
        public string JobName { get; set; }
        public string JobType { get; set; }
        public string Address { get; set; }
        public string Phase { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
    }
    public class EmployeeDetails
    {
        public string Id { get; set; }
        public string Role { get; set; }
        public string Employee { get; set; }
    }
    public class StatusModel
    {
        public int Id { get; set; }
        public string Status { get; set; }
    }
    public class Bids
    {
        public string BidId { get; set; }
        public string BidName { get; set; }
        public string BidManager { get; set; }
        public string Bidder { get; set; }
        public string Status { get; set; }
    }
    public class BidQueueDTO
    {
        public List<Bids> CurrentBids { get; set; }
        public List<Bids> ClosedBids { get; set; }
    }
    public class CreateBidRequest
    {
        public string BidId { get; set;}
        public string Department { get; set; }
        public string Division { get; set; }
        public string BidName { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string CreatedBy { get; set; }
    }
    public class BidRecords
    {
        public int InternalBidID { get; set; }
        public string DisplayBidID { get; set; }
        public bool IsProject { get; set; }
        public string Department { get; set; }
        public string Division { get; set; }
        public string BidName { get; set; }
        public string BidPhase { get; set; }
        public string BidStatus { get; set; }
        public string Jobsite { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string SalesTaxDistrict { get; set; }
        public string ClientName { get; set; }

        public string GC_Address1 { get; set; }
        public string GC_Address2 { get; set; }
        public string GC_City { get; set; }
        public string GC_State { get; set; }
        public string GC_ZipCode { get; set; }
        public string GC_SalesTaxDistrict { get; set; }

        public string DocumentFolderPath { get; set; }
        public bool HasBidDocumentLibrary { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string InsuranceType { get; set; }
        public string Bonded { get; set; }
        public string TaxExempt { get; set; }

        public string InvoiceSubmittal { get; set; }
        public string CertifiedPayroll { get; set; }

        public DateTime? NetTermsDate { get; set; }

        public decimal RetainagePercentage { get; set; }
    }
    public class BidAmounts
    {
        public int BidAmountID { get; set; }

        public int InternalBidID { get; set; }

        public DateTime? BidDate { get; set; }

        public decimal BidAmount { get; set; }

        public string Status { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
    public class BidAccessSummary
    {
        public int BidAccessID { get; set; }

        public string EmployeeName { get; set; }

        public string RoleName { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
    public class UpdateBidDetailsRequest
    {
        public int InternalBidID { get; set; }

        // Bid Overview
        public string Department { get; set; }
        public string Division { get; set; }
        public string BidName { get; set; }
        public string BidPhase { get; set; }
        public string BidStatus { get; set; }

        // Property Address
        public string Jobsite { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string SalesTaxDistrict { get; set; }

        // General Contractor
        public string ClientName { get; set; }
        public string GC_Address1 { get; set; }
        public string GC_Address2 { get; set; }
        public string GC_City { get; set; }
        public string GC_State { get; set; }
        public string GC_ZipCode { get; set; }
        public string GC_SalesTaxDistrict { get; set; }

        // Project Information
        public string InsuranceType { get; set; }
        public string Bonded { get; set; }
        public string TaxExempt { get; set; }
        public string InvoiceSubmittal { get; set; }
        public string CertifiedPayroll { get; set; }

        public DateTime? NetTermsDate { get; set; }

        public decimal? RetainagePercentage { get; set; }

        // Audit
        public string ModifiedBy { get; set; }
    }
    public class AddBidAmountRequest
    {
        public int InternalBidID { get; set; }

        public DateTime BidDate { get; set; }

        public decimal BidAmount { get; set; }

        public string Status { get; set; }

        public string ModifiedBy { get; set; }
    }
    public class UpdateBidAmountRequest
    {
        public int BidAmountID { get; set; }

        public DateTime? BidDate { get; set; }

        public decimal? BidAmount { get; set; }

        public string Status { get; set; }

        public string ModifiedBy { get; set; }
    }
    public class DeleteBidAmountRequest
    {
        public int BidAmountID { get; set; }

        public string ModifiedBy { get; set; }
    }
    public class AddBidAccessRequest
    {
        public int InternalBidID { get; set; }

        public string RoleName { get; set; }

        public string EmployeeName { get; set; }

        public string ModifiedBy { get; set; }
    }
    public class RemoveBidAccessRequest
    {
        public int BidAccessID { get; set; }

        public string ModifiedBy { get; set; }
    }
    public class ConvertBidToProjectRequest
    {
        public int InternalBidID { get; set; }

        public string ModifiedBy { get; set; }
    }
    public class CloseBidAsLostRequest
    {
        public int InternalBidID { get; set; }

        public string ModifiedBy { get; set; }
    }
    public class AddBidAmountResponse
    {
        public string Message { get; set; }
        public int NewBidAmountID { get; set; }
    }
    public class AddBidAccessResponse
    {
        public string Message { get; set; }
        public int NewBidAccessID { get; set; }
    }
    public class ConvertBidResponse
    {
        public string Message { get; set; }
        public string NewProjectID { get; set; }
    }
    public class SimpleResponse
    {
        public string Message { get; set; }
    }
    public class NextDisplayBidIDResponse
    {
        public bool Success { get; set; }
        public string? DisplayBidID { get; set; }
    }
    public class Employee
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
    }
}
