using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using Radzen.Blazor.Rendering;
using StowellCoApp.Components.Pages.StowellAdmin;
using StowellCoApp.DTO;
using StowellCoApp.Services;
using System.Globalization;
using System.Security.Claims;

namespace StowellCoApp.Components.Pages.Bids
{
    public partial class BidQueueOverviewTabBase : ComponentBase
    {
        [Inject] DialogService DialogService { get; set; }
        [Parameter] public string BidId { get; set; }
        [Parameter] public string Type { get; set; }
        [Parameter] public int InternalBidId { get; set; }
        protected RadzenDataGrid<BidAmounts> bidAmountsGrid;

        private readonly IHttpContextAccessor _httpContextAccessor;
        [Inject]
        public HttpClient Http { get; set; }
        [Inject]
        public IBidService _bidservice { get; set; }
        [Parameter] public BidRecords _bidRecords { get; set; }
        [Parameter] public IEnumerable<BidAmounts> _bidAmounts { get; set; }
        [Inject]
        public NotificationService NotificationService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public readonly CultureInfo UsCulture = new("en-US");
 public DateTime? BidDate { get; set; }
        public decimal? BidAmount { get; set; }

        public AddBidAmountRequest request { get; set; }

        protected RadzenButton button;
        protected Popup popup;
        protected Popup Phasespopup;
        protected Popup Employeepopup;
        protected bool IsClosed => Type == "Closed";
        public IEnumerable<string> _phases { get; set; }
        public IEnumerable<string> _bidStatus { get; set; }
        public List<string> jobPhases = new()
    {
        "RFI",
        "Estimate",
        "Bid",
        "Project"
    };
        public List<string> bidStatuses = new()
    {
        "Open Bid",
        "Won",
        "Lost",
        "Pending"
    };
        protected override async Task OnInitializedAsync()
        {
            if (!string.IsNullOrEmpty(BidId))
            {
                _bidStatus = jobPhases;//await _bidservice.GetAllBidStatus();
                _phases = bidStatuses;//await _bidservice.GetAllBidPhases();
                _bidRecords = await _bidservice.GetBidById(BidId);
                if (_bidRecords != null)
                {
                    InternalBidId = _bidRecords.InternalBidID;
                }
                _bidAmounts = await _bidservice.GetBidAmounts(BidId);
            }
        }
        protected async Task GetBidWon()
        {
            if (!string.IsNullOrEmpty(BidId))
            {
                try
                {
                    ConvertBidToProjectRequest request = new ConvertBidToProjectRequest
                    {
                        InternalBidID = InternalBidId
                    };

                    var responseData = await _bidservice.ConvertBidToProject(request);

                    if (!string.IsNullOrEmpty(responseData.NewProjectId))
                    {
                        NotificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Success,
                            Summary = "Success",
                            Detail = $"{responseData.NewProjectId} created successfully.",
                            Duration = 3000
                        });

                        await Task.Delay(3000);

                        NavigationManager.NavigateTo("/Bids/BidQueue");
                    }
                    else
                    {
                        NotificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = "Error",
                            Detail = responseData.Message ?? "Unable to create bid.",
                            Duration = 5000
                        });
                    }
                }
                catch (Exception ex)
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = ex.Message,
                        Duration = 5000
                    });
                }
            }
        }
        
        protected async Task GetBidLost()
        {
            if (!string.IsNullOrEmpty(BidId))
            {
                try
                {
                    CloseBidAsLostRequest request = new CloseBidAsLostRequest
                    {
                        InternalBidID = InternalBidId
                    };

                    var responseData = await _bidservice.CloseBidAsLost(request);

                    if (!string.IsNullOrEmpty(responseData))
                    {
                        NotificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Success,
                            Summary = "Success",
                            Detail = $"Bid closed successfully",
                            Duration = 3000
                        });

                        await Task.Delay(3000);

                        NavigationManager.NavigateTo("/Bids/BidQueue");
                    }
                    else
                    {
                        NotificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Error,
                            Summary = "Error",
                            Detail = "Unable to close bid.",
                            Duration = 5000
                        });
                    }
                }
                catch (Exception ex)
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = ex.Message,
                        Duration = 5000
                    });
                }
            }
        }
        protected async Task SaveBidData()
        {
            try
            {
                UpdateBidDetailsRequest request = new UpdateBidDetailsRequest();
                request.Address1 = _bidRecords.Address1 ?? string.Empty;
                request.Address2 = _bidRecords.Address2 ?? string.Empty;
                request.BidName = _bidRecords.BidName ?? string.Empty;
                request.BidPhase = _bidRecords.BidPhase ?? string.Empty;
                request.BidStatus = _bidRecords.BidStatus ?? string.Empty;
                request.Bonded = _bidRecords.Bonded;
                request.CertifiedPayroll = _bidRecords.CertifiedPayroll;
                request.City = _bidRecords.City;
                request.ClientName = _bidRecords.ClientName;
                request.Department = _bidRecords.Department;
                request.Division = _bidRecords.Division;
                request.GC_Address1 = _bidRecords.GC_Address1 ?? string.Empty;
                request.GC_Address2 = _bidRecords.GC_Address2 ?? string.Empty;
                request.GC_City = _bidRecords.GC_City ?? string.Empty;
                request.GC_SalesTaxDistrict = _bidRecords.GC_SalesTaxDistrict ?? string.Empty;
                request.GC_State = _bidRecords.GC_State;
                request.GC_ZipCode = _bidRecords.ZipCode;
                request.InsuranceType = _bidRecords.InsuranceType;
                request.InternalBidID = _bidRecords.InternalBidID;
                request.InvoiceSubmittal = _bidRecords.InvoiceSubmittal;
                request.Jobsite = _bidRecords.Jobsite;
                request.NetTermsDate = _bidRecords.NetTermsDate;
                request.RetainagePercentage = _bidRecords.RetainagePercentage;
                request.SalesTaxDistrict = _bidRecords.SalesTaxDistrict;
                request.State = _bidRecords.State;
                request.TaxExempt = _bidRecords.TaxExempt;
                request.ZipCode = _bidRecords.ZipCode;
                var responseData = await _bidservice.UpdateBidDetails(request);
                if (responseData)
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = $"Bid details saved successfully",
                        Duration = 3000
                    });

                    //await Task.Delay(3000);

                    //NavigationManager.NavigateTo("/Bids/BidQueue");
                }
                else
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Unable to save bid details.",
                        Duration = 5000
                    });
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = ex.Message,
                    Duration = 5000
                });
            }
        }
		protected async Task SaveBidsClick()
        {
            if (request == null)
            {
                request = new AddBidAmountRequest(); // or your actual class
            }

            if (_bidRecords == null)
            {
                // handle properly
                return;
            }
            
                request.InternalBidID = _bidRecords.InternalBidID;

           request.BidDate= BidDate.Value;
            request.BidAmount=BidAmount.Value;
            request.Status = _bidRecords.BidStatus;
            var (message, newId) = await _bidservice.AddBidAmount(request);
            if (newId > 0)
            {
                //_bidAmountRecords = await _estimationService.GetBidsAmountAsync(Convert.ToString(BidNumber));
                //BidDate = null;
                //BidAmount = null;
                await popup.CloseAsync();
                _bidAmounts = await _bidservice.GetBidAmounts(BidId);
                await bidAmountsGrid.Reload();
                await InvokeAsync(StateHasChanged);
            }
        }
        protected async Task CloseClick()
        {
            BidDate = null;
            BidAmount = null;
            await popup.CloseAsync();
        }
        protected async Task DeleteBidAmountRow(BidAmounts item)
        {
            var confirmed = await DialogService.Confirm(
                "Are you sure you want to delete this record?",
                "Delete",
                new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });

            if (confirmed == true)
            {
                DeleteBidAmountRequest request = new DeleteBidAmountRequest();
                request.BidAmountID = item.BidAmountID;
                var response = await _bidservice.DeleteBidAmount(request);
                if (response != null)
                {
                    _bidAmounts = _bidAmounts = await _bidservice.GetBidAmounts(BidId);
                }
                bidAmountsGrid?.Reload();
            }
        }
    }
}
