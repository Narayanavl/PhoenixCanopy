using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using Radzen.Blazor.Rendering;
using StowellCoApp.DTO;
using StowellCoApp.Models;
using StowellCoApp.Services;
using System.Globalization;
using System.Security.Cryptography;

namespace StowellCoApp.Components.Pages.Bids
{
    public partial class BidProjectDetailsTabBase : ComponentBase
    {
        [Parameter] public string BidId { get; set; }
        [Parameter] public string Type { get; set; }
        [Inject] DialogService DialogService { get; set; }
        [Inject]
        public HttpClient Http { get; set; }
        public DateTime? BidDate { get; set; }
        public decimal? BidAmount { get; set; }

        protected RadzenButton button;
        protected Popup popup;
        protected Popup Phasespopup;
        protected Popup Employeepopup;
        [Inject]
        public IBidService _bidservice { get; set; }
        [Parameter] public BidRecords _bidRecords { get; set; }
        [Parameter] public IEnumerable<BidAccessSummary> _bidAccessSummary { get; set; }
        public IEnumerable<StowellCoApp.DTO.ContractorClient> _contractClientRecords { get; set; } = new List<StowellCoApp.DTO.ContractorClient>();
        public IEnumerable<StowellCoApp.DTO.ContractorClient> _contractClientdroplist { get; set; } = new List<StowellCoApp.DTO.ContractorClient>();
        public IEnumerable<StowellCoApp.DTO.Employee> _employeeClientdroplist { get; set; } = new List<StowellCoApp.DTO.Employee>();
        [Inject]
        public IEstimationService _estimationService { get; set; }
        protected RadzenDataGrid<BidAccessSummary> bidAccessSummaryGrid;
        public string? selectedClientName { get; set; }
        public ContractorClient selectedClient = new ContractorClient();
        [Inject]
        public NotificationService NotificationService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        public readonly CultureInfo UsCulture = new("en-US");
        [Parameter] public int InternalBidId { get; set; }
        public AddBidAccessRequest request { get; set; }
        public string? SelectedRoleName { get; set; }
        public string? SelectedEmployeeName { get; set; }
        protected bool IsClosed => Type == "Closed";
        protected override async Task OnInitializedAsync()
        {
            if (!string.IsNullOrEmpty(BidId))
            {
                _employeeClientdroplist =await _bidservice.GetBidAccessEmployees();
                _contractClientdroplist = await _estimationService.GetAllContractorClients();
                _bidRecords = await _bidservice.GetBidById(BidId);
                _bidAccessSummary = await _bidservice.GetBidAccessSummary(BidId);
                if (_bidRecords != null)
                {
                    InternalBidId = _bidRecords.InternalBidID;
                    selectedClientName = _bidRecords.ClientName;
                }
            }
        }
        public async Task OnClientChange(string? selectedClientName)
        {
            if (string.IsNullOrEmpty(selectedClientName))
                return;

            selectedClient = await _bidservice.GetContractorClientByName(selectedClientName);

            if (selectedClient != null)
            {
                _bidRecords.GC_Address1 = selectedClient.Address1;
                _bidRecords.GC_Address2 = selectedClient.Address2;
                _bidRecords.GC_City = selectedClient.City;
                _bidRecords.GC_State = selectedClient.State;
                _bidRecords.GC_ZipCode = selectedClient.ZipCode;
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
        protected async Task SaveBidAccessClick()
        {
            if (request == null)
            {
                request = new AddBidAccessRequest();
            }

            if (_bidRecords == null)
            {
                return;
            }

            request.InternalBidID = _bidRecords.InternalBidID;

            request.RoleName = SelectedRoleName;
            request.EmployeeName = SelectedEmployeeName;

            var (message, newId) = await _bidservice.AddBidAccess(request);

            if (newId > 0)
            {
                await popup.CloseAsync();
                _bidAccessSummary = await _bidservice.GetBidAccessSummary(BidId);
                await bidAccessSummaryGrid.Reload();
                await InvokeAsync(StateHasChanged);
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
                request.ClientName = selectedClientName;//_bidRecords.ClientName;
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
        protected async Task CloseClick()
        {
            SelectedRoleName = null;
            SelectedEmployeeName = null;
            await popup.CloseAsync();
        }
        protected async Task DeleteBidAccessRow(BidAccessSummary item)
        {
            var confirmed = await DialogService.Confirm(
                "Are you sure you want to delete this record?",
                "Delete",
                new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });

            if (confirmed == true)
            {
                RemoveBidAccessRequest request = new RemoveBidAccessRequest();
                request.BidAccessID = item.BidAccessID;
                var response = await _bidservice.RemoveBidAccess(request);
                if (response != null)
                {
                    _bidAccessSummary = await _bidservice.GetBidAccessSummary(BidId);
                }
                bidAccessSummaryGrid?.Reload();
            }
        }
    }
}
