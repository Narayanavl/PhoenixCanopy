namespace StowellCoAPI.Models
{
    public class Bids
    {
        public string BidId { get; set; }
        public string BidName { get; set; }
        public string BidManager { get; set; }
        public string Bidder { get; set; }
        public string Status { get; set; }
    }
    public class BidQueue
    {
        public List<Bids> CurrentBids { get; set; }
        public List<Bids> ClosedBids { get; set; }
    }
    public class CreateBidRequest
    {
        public string BidId{get;set;}
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
    public class BidStatus
    {
        public string Uid { get; set; }
        public string Name { get; set; }
    }
}
