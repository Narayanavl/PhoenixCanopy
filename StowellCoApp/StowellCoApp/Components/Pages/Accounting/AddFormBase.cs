using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using Radzen.Blazor.Rendering;
using StowellCoApp.Components.Pages.Estimation;
using StowellCoApp.DTO;
using StowellCoApp.Models;
using StowellCoApp.Services;
using System.Globalization;
using System.Security.Claims;

namespace StowellCoApp.Components.Pages.Accounting
{
    public class AddFormBase : ComponentBase
    {
        protected bool isLoading = false;

        [Inject] DialogService DialogService { get; set; }
        public readonly CultureInfo UsCulture = new("en-US");

        [Parameter]
        public int JobId { get; set; }
        [Inject]
        NavigationManager NavigationManager { get; set; }
        [Inject]
        public NotificationService NotificationService { get; set; }
        protected string txtJobName;
        protected string txtJobShortName;
        protected string txtContractNumber;
        protected DateTime? txtContractDate;
        protected decimal? txtContractAmount;

        protected DateTime? txtEstStartDate;
        protected DateTime? txtEstCompletionDate;
        protected string txtJobSite;
        protected string txtJobSiteAddress;
        protected string txtJobSiteAddress2;
        protected string txtJobSiteCity;
        protected string txtJobSiteState;
        protected string txtJobSiteSalesTaxDistrict;


        protected DateTime? txtNetTermsDate;
        protected decimal? txtRetainage;

        protected string selectedDepartment = null;
        protected string selectedJobType;

        protected string? selectedBidStatus = null;

        protected string? selectedJobPhase = null;
        protected string? selectedDivision = null;

        protected string? selectedInsuranceType;
        protected string? selectedBonded;
        protected string? selectedTaxExempt;
        protected string? selectedInvoiceSubmittal;
        protected string? selectedCertifiedPayroll;
        public int? selectedClientId { get; set; }
        public int? selectedEmployeeId { get; set; }
        public string selectedEmpName { get; set; }


        protected RadzenButton button;
        protected Popup popup;
        protected Popup Phasespopup;
        protected Popup Employeepopup;

        protected RadzenDataGrid<StowellCoApp.Models.BidAmount> _bidAmountGrid;
        protected RadzenDataGrid<StowellCoApp.Models.BidPhase> _bidPhaseGrid;
        protected RadzenDataGrid<StowellCoApp.Models.BidEmployee> _bidEmployeeGrid;



        public string txtBidderDisplay { get; set; }
        public string txtJobId { get; set; }

        public string selectedPhaseName { get; set; }
        public string selectedPhaseNum { get; set; }


        public DateTime? BidDate { get; set; }
        public decimal? BidAmount { get; set; }
        public IEnumerable<StowellCoApp.Models.BidAmount> _bidAmountRecords { get; set; } = new List<StowellCoApp.Models.BidAmount>();
        public IEnumerable<StowellCoApp.Models.BidPhase> _bidphaseRecords { get; set; } = new List<StowellCoApp.Models.BidPhase>();
        public IEnumerable<StowellCoApp.Models.BidEmployee> _bidEmployeeRecords { get; set; } = new List<StowellCoApp.Models.BidEmployee>();

        public IEnumerable<StowellCoApp.DTO.ContractorClient> _contractClientRecords { get; set; } = new List<StowellCoApp.DTO.ContractorClient>();

        public IEnumerable<StowellCoApp.DTO.ContractorClient> _contractClientdroplist { get; set; } = new List<StowellCoApp.DTO.ContractorClient>();

        public ContractorClient selectedClient = new ContractorClient();
        [Inject]
        private ILogger<AddFormBase> logger { get; set; } = default!;
        public StowellCoApp.Models.BidAmount _bidAmount = new();
        public StowellCoApp.Models.BidPhase _bidphase = new();
        public StowellCoApp.Models.BidEmployee _bidEmployee = new();

        public StowellCoApp.DTO.BidInfoDto _bidInfoDto = new();
        public ProjectLocationDto _projectLocationDto = new();
        public ContractorInfoDto _contractorInfoDto = new();
        public ProjectInfoDto _projectInfoDto = new();
        protected bool isProcessing = false;
        public IEnumerable<Phase> _phases { get; set; }
        [Inject]
        public ICostCodeService _costCodeService { get; set; }

        List<ContractorClient> _employees = new();

        [Inject]
        public IEstimationService _estimationService { get; set; }

        [Inject]
        public IAccountingService _accountingService { get; set; }

        public string selectedEmpRole = "Sr. PM"; // optional default selection

        public List<string> empRoles = new()
       {
         "Sr. PM",
         "PM"
       };

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
        public List<DivisionOption> divisions = new()
    {
        new DivisionOption { Value = "BlockConcreteGroupId", Text = "BlockConcrete" },
        new DivisionOption { Value = "a539add2-52ab-4545-acb6-5d23c703403f", Text = "Jacksonville" },
        new DivisionOption { Value = "0a6836b5-aa1f-4c7e-b1e9-77be7b5a85d1", Text = "Naples" },
        new DivisionOption { Value = "3d76dbfb-27c8-4c95-836e-e5caed001a0b", Text = "Orlando" },
        new DivisionOption { Value = "b222364a-0ca8-4415-ac9e-36ba0bfcabe1", Text = "Tampa" },
        new DivisionOption { Value = "1446eb90-6189-47cf-b242-b6f84f9f1d56", Text = "Texas" }
    };
        protected List<string> jobTypes = new();
        //protected override async Task OnInitializedAsync()
        //{


        //}
        protected override async Task OnInitializedAsync()
        {
            _bidAmount ??= new BidAmount();
            _bidphase ??= new BidPhase();
            try
            {
                txtBidderDisplay = await _estimationService.GetLoggedUserEmail();
                txtJobId = JobId.ToString();//await _estimationService.GetNewBidNumber();
                _phases = await _costCodeService.GetAllPhases();
                _contractClientRecords = await _estimationService.GetAllEmployees();
                _contractClientdroplist = await _estimationService.GetAllContractorClients();
                _bidEmployeeRecords = await _estimationService.GetBidEmployees(txtJobId);
                _bidAmountRecords = await _estimationService.GetBidsAmountAsync(txtJobId);
                _bidphaseRecords = await _estimationService.GetPhases(txtJobId);
                _bidInfoDto = await _estimationService.GetBidDetails(JobId);

                if (_bidInfoDto != null)
                {
                    txtJobId = _bidInfoDto.JobID;
                    txtBidderDisplay = _bidInfoDto.Bidder;
                    selectedJobPhase = _bidInfoDto.Phase;
                    selectedBidStatus = _bidInfoDto.BidStatus;
                    selectedDivision = _bidInfoDto.Division;
                    selectedDepartment = _bidInfoDto.Department;
                    selectedJobType = _bidInfoDto.JobType;
                    txtJobName = _bidInfoDto.JobName;
                    txtJobShortName = _bidInfoDto.ShortName;
                    txtContractNumber = _bidInfoDto.ContractNumber;
                    txtContractDate = DateTime.TryParse(_bidInfoDto.ContractDate, out var contractDate) ? contractDate : null;
                    txtContractAmount = _bidInfoDto.ContractAmount;
                    txtEstStartDate = DateTime.TryParse(_bidInfoDto.EstStartDate, out var estStartDate) ? estStartDate : null;
                    txtEstCompletionDate = DateTime.TryParse(_bidInfoDto.EstCompletionDate, out var estCompletionDate) ? estCompletionDate : null;
                    _projectLocationDto = _bidInfoDto.ProjectLocation;
                    _contractorInfoDto = _bidInfoDto.ContractorInfo;
                    _projectInfoDto = _bidInfoDto.ProjectInfo;
                }
                if (_projectLocationDto != null)
                {
                    // Reverse the ProjectLocationDto fields
                    txtJobSite = _projectLocationDto.Jobsite;
                    txtJobSiteAddress = _projectLocationDto.Address;
                    txtJobSiteAddress2 = _projectLocationDto.Address2;
                    txtJobSiteCity = _projectLocationDto.City;
                    txtJobSiteState = _projectLocationDto.State;
                    txtJobSiteSalesTaxDistrict = _projectLocationDto.SalesTaxDistrict;
                }
                if (_contractorInfoDto != null)
                {
                    // Reverse the ContractorInfoDto fields
                    selectedClientId = int.TryParse(_contractorInfoDto.Client, out var clientId) ? clientId : null;
                    selectedClient = new ContractorClient
                    {
                        Address1 = _contractorInfoDto.Address,
                        Address2 = _contractorInfoDto.Address2,
                        City = _contractorInfoDto.City,
                        State = _contractorInfoDto.State,
                        ZipCode = _contractorInfoDto.Zip
                    };
                }
                if (_projectInfoDto != null)
                {
                    // Reverse the ProjectInfoDto fields
                    selectedInsuranceType = _projectInfoDto.InsuranceType.ToString();
                    selectedBonded = _projectInfoDto.Bonded.ToString();
                    selectedTaxExempt = _projectInfoDto.TaxExcempt.ToString();
                    selectedInvoiceSubmittal = _projectInfoDto.InvoiceSubmittal.ToString();
                    selectedCertifiedPayroll = _projectInfoDto.CertifiedPayroll.ToString();
                    txtNetTermsDate = DateTime.TryParse(_projectInfoDto.NetTermsDate, out var netTermsDate) ? netTermsDate : null;
                    txtRetainage = Convert.ToDecimal(_projectInfoDto.RetainagePerc);

                }

                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Error loading jobs: {ex.Message}");
                Console.WriteLine($"Error loading jobs: {ex.Message}");
            }
        }
        protected async Task CloseClick()
        {
            BidDate = null;
            BidAmount = null;
            await popup.CloseAsync();
        }
        protected async Task SaveBidsClick()
        {
            //var CurrentUser = HttpContext.User.Identity.Name;
            var UserName = await _estimationService.GetLoggedUserEmail();
            var BidNumber = JobId.ToString();// await _estimationService.GetNewBidNumber();
            _bidAmount ??= new BidAmount();
            _bidAmount.Id = 0;
            _bidAmount.JobId = BidNumber;
            _bidAmount.BidDate = BidDate.HasValue ? DateOnly.FromDateTime(BidDate.Value) : null;
            _bidAmount.BidAmount1 = BidAmount;
            var response = await _estimationService.InsertBid(_bidAmount);
            if (response != null)
            {
                _bidAmountRecords = await _estimationService.GetBidsAmountAsync(Convert.ToString(BidNumber));
                BidDate = null;
                BidAmount = null;
                await popup.CloseAsync();
                await _bidAmountGrid.Reload();
                await InvokeAsync(StateHasChanged);
            }
        }

        protected async Task ClosephaseClick()
        {
            selectedPhaseNum = null;
            await Phasespopup.CloseAsync();
        }
        protected async Task SavePhaseClick()
        {
            //var CurrentUser = HttpContext.User.Identity.Name;
            var UserName = await _estimationService.GetLoggedUserEmail();
            var BidNumber = JobId.ToString();// await _estimationService.GetNewBidNumber();
            selectedPhaseName = _phases.FirstOrDefault(x => x.PhaseNum == selectedPhaseNum)?.PhaseName;
            _bidphase ??= new BidPhase();
            _bidphase.Id = 0;
            _bidphase.JobId = BidNumber;
            _bidphase.Phase = selectedPhaseNum;
            _bidphase.Description = selectedPhaseName;
            var response = await _estimationService.InsertPhase(_bidphase);
            if (response != null)
            {
                _bidphaseRecords = await _estimationService.GetPhases(BidNumber);
                selectedPhaseNum = null;
                await Phasespopup.CloseAsync();
                await _bidPhaseGrid.Reload();
                await InvokeAsync(StateHasChanged);
            }
        }

        protected async Task CloseEmployeeClick()
        {
            selectedEmpRole = null;
            selectedEmpName = null;
            await Employeepopup.CloseAsync();
        }

        protected async Task SaveEmployeeClick()
        {
            var UserName = await _estimationService.GetLoggedUserEmail();
            var BidNumber = JobId.ToString();// await _estimationService.GetNewBidNumber();
            //  selectedEmpName = _contractClientRecords.FirstOrDefault(x => x.Id == selectedEmployeeId)?.Name;
            _bidEmployee ??= new BidEmployee();
            _bidEmployee.Id = 0;
            _bidEmployee.JobId = BidNumber;
            _bidEmployee.Role = selectedEmpRole;
            _bidEmployee.EmployeeId = selectedEmployeeId;
            var response = await _estimationService.InsertEmployee(_bidEmployee);
            if (response != null)
            {
                _bidEmployeeRecords = await _estimationService.GetBidEmployees(JobId.ToString());
                selectedEmpName = null;
                selectedEmpRole = null;
                await Employeepopup.CloseAsync();
                await _bidEmployeeGrid.Reload();
                await InvokeAsync(StateHasChanged);
            }
        }

        protected List<string> departments = new()
    {
        "Drywall",
        "Concrete"
    };

        protected Dictionary<string, List<string>> ddlJobTypes = new()
        {
            ["Drywall"] = new() { "Apartment", "Condo", "Medical", "Residential" },
            ["Concrete"] = new() { "Block", "Tilt Up", "Foundation" }
        };

        protected void OnDepartmentChange(object value)
        {
            selectedDepartment = value?.ToString();
            selectedJobType = null;

            jobTypes = ddlJobTypes.ContainsKey(selectedDepartment)
                ? ddlJobTypes[selectedDepartment]
                : new List<string>();
        }

        public async Task OnClientChange(int? clientId)
        {
            //if (clientId!=null)
            //{
            selectedClient = await _estimationService.GetContractorClientById(clientId.ToString());
            //}
            //else
            //{
            //    _contractClientdroplist = new ContractorClient(); // reset fields
            //}
        }

        public async Task SubmitBidAsync()
        {
            isLoading = true;
            StateHasChanged();
            try
            {
                if (_bidInfoDto == null)
                    _bidInfoDto = new BidInfoDto();

                _bidInfoDto.JobID = txtJobId;
                _bidInfoDto.Bidder = txtBidderDisplay;
                _bidInfoDto.Phase = selectedJobPhase;
                _bidInfoDto.BidStatus = selectedBidStatus;
                _bidInfoDto.Division = selectedDivision;
                _bidInfoDto.Department = selectedDepartment;
                _bidInfoDto.JobType = selectedJobType;
                _bidInfoDto.JobName = txtJobName;
                _bidInfoDto.ShortName = txtJobShortName;
                _bidInfoDto.ContractNumber = txtContractNumber;
                _bidInfoDto.ContractDate = txtContractDate.ToString();
                _bidInfoDto.ContractAmount = txtContractAmount;
                _bidInfoDto.EstStartDate = txtEstStartDate.ToString();
                _bidInfoDto.EstCompletionDate = txtEstCompletionDate.ToString();

                if (_projectLocationDto == null)
                    _projectLocationDto = new ProjectLocationDto();

                _projectLocationDto.JobID = txtJobId;
                _projectLocationDto.Jobsite = txtJobSite;
                _projectLocationDto.Address = txtJobSiteAddress;
                _projectLocationDto.Address2 = txtJobSiteAddress2;
                _projectLocationDto.City = txtJobSiteCity;
                _projectLocationDto.State = txtJobSiteState;
                _projectLocationDto.SalesTaxDistrict = txtJobSiteSalesTaxDistrict;

                if (_contractorInfoDto == null)
                    _contractorInfoDto = new ContractorInfoDto();

                _contractorInfoDto.JobID = txtJobId;
                _contractorInfoDto.Client = selectedClientId.ToString();
                _contractorInfoDto.Address = selectedClient.Address1;
                _contractorInfoDto.Address2 = selectedClient.Address2;
                _contractorInfoDto.City = selectedClient.City;
                _contractorInfoDto.State = selectedClient.State;
                _contractorInfoDto.Zip = selectedClient.ZipCode;

                if (_projectInfoDto == null)
                    _projectInfoDto = new ProjectInfoDto();

                _projectInfoDto.JobID = txtJobId;
                _projectInfoDto.InsuranceType = Convert.ToInt32(selectedInsuranceType);
                _projectInfoDto.Bonded = Convert.ToInt32(selectedBonded);
                _projectInfoDto.TaxExcempt = Convert.ToInt32(selectedTaxExempt);
                _projectInfoDto.InvoiceSubmittal = Convert.ToInt32(selectedInvoiceSubmittal);
                _projectInfoDto.CertifiedPayroll = Convert.ToInt32(selectedCertifiedPayroll);
                _projectInfoDto.NetTermsDate = txtNetTermsDate.ToString();
                _projectInfoDto.RetainagePerc = Convert.ToInt32(txtRetainage);

                _bidInfoDto.ProjectLocation = _projectLocationDto;
                _bidInfoDto.ContractorInfo = _contractorInfoDto;
                _bidInfoDto.ProjectInfo = _projectInfoDto;

                var response = await _accountingService.SubmitToSage(_bidInfoDto);
                isLoading = false;
                StateHasChanged();
                if (response != null && response.Success)
                {

                    if (!response.IsExist)
                    {
                        await DialogService.Alert(
         "Project Details updated Successfully",
         "Success",
         new AlertOptions() { OkButtonText = "Ok" }
     );

                        // Navigate immediately after OK
                        NavigationManager.NavigateTo("/Accounting");
                    }

                    else
                    {
                        await DialogService.Alert(
             response?.Message,
            "",
            new AlertOptions() { OkButtonText = "Ok" }
        );

                    }

                    //NotificationService.Notify(new NotificationMessage
                    //{
                    //    Severity = !response.IsExist
                    //       ? NotificationSeverity.Success
                    //       : NotificationSeverity.Warning,

                    //    Summary = !response.IsExist ? "Success" : "",
                    //    Detail = true ? response.Message : "Error while submitting project details",
                    //    // Duration = 10000,

                    //});
                    // Wait for the notification to finish (wait for 4 seconds) before navigating
                    //await Task.Delay(500);
                    //    .ContinueWith(t =>
                    //{
                    // Navigate to the target page after the notification duration
                    //if (!response.IsExist)
                       //NavigationManager.NavigateTo("/Accounting", forceLoad: true);
                    //});
                }
                else
                {
                    //NotificationService.Notify(new NotificationMessage
                    //{
                    //    Severity = NotificationSeverity.Error,

                    //    Summary = "Error",
                    //    Detail = "Error while submitting project details.",
                    //    // Duration = 4000
                    //});
                    await DialogService.Alert(
        "Error while updating project details.",
        "Error"

    );
                }
            }
            catch (Exception ex)
            {
                isLoading = false;
                StateHasChanged();
                Console.Error.WriteLine(ex.Message);
                //NotificationService.Notify(new NotificationMessage
                //{
                //    Severity = NotificationSeverity.Error,

                //    Summary = "Error",
                //    Detail = "Error while submitting project details.",
                //    Duration = 4000
                //});
                await DialogService.Alert(
"Error while updating project details.",
"Error"

);

            }
            finally
            {
                //isProcessing = false;
                //StateHasChanged(); // refresh UI to hide spinner
            }
        }
        public async Task RejectBidAsync()
        {
            try
            {
                if (_bidInfoDto == null)
                    _bidInfoDto = new BidInfoDto();

                _bidInfoDto.JobID = txtJobId;
                _bidInfoDto.Bidder = txtBidderDisplay;
                _bidInfoDto.Phase = selectedJobPhase;
                _bidInfoDto.BidStatus = selectedBidStatus;
                _bidInfoDto.Division = selectedDivision;
                _bidInfoDto.Department = selectedDepartment;
                _bidInfoDto.JobType = selectedJobType;
                _bidInfoDto.JobName = txtJobName;
                _bidInfoDto.ShortName = txtJobShortName;
                _bidInfoDto.ContractNumber = txtContractNumber;
                _bidInfoDto.ContractDate = txtContractDate.ToString();
                _bidInfoDto.ContractAmount = txtContractAmount;
                _bidInfoDto.EstStartDate = txtEstStartDate.ToString();
                _bidInfoDto.EstCompletionDate = txtEstCompletionDate.ToString();

                if (_projectLocationDto == null)
                    _projectLocationDto = new ProjectLocationDto();

                _projectLocationDto.JobID = txtJobId;
                _projectLocationDto.Jobsite = txtJobSite;
                _projectLocationDto.Address = txtJobSiteAddress;
                _projectLocationDto.Address2 = txtJobSiteAddress2;
                _projectLocationDto.City = txtJobSiteCity;
                _projectLocationDto.State = txtJobSiteState;
                _projectLocationDto.SalesTaxDistrict = txtJobSiteSalesTaxDistrict;

                if (_contractorInfoDto == null)
                    _contractorInfoDto = new ContractorInfoDto();

                _contractorInfoDto.JobID = txtJobId;
                _contractorInfoDto.Client = selectedClientId.ToString();
                _contractorInfoDto.Address = selectedClient.Address1;
                _contractorInfoDto.Address2 = selectedClient.Address2;
                _contractorInfoDto.City = selectedClient.City;
                _contractorInfoDto.State = selectedClient.State;
                _contractorInfoDto.Zip = selectedClient.ZipCode;

                if (_projectInfoDto == null)
                    _projectInfoDto = new ProjectInfoDto();

                _projectInfoDto.JobID = txtJobId;
                _projectInfoDto.InsuranceType = Convert.ToInt32(selectedInsuranceType);
                _projectInfoDto.Bonded = Convert.ToInt32(selectedBonded);
                _projectInfoDto.TaxExcempt = Convert.ToInt32(selectedTaxExempt);
                _projectInfoDto.InvoiceSubmittal = Convert.ToInt32(selectedInvoiceSubmittal);
                _projectInfoDto.CertifiedPayroll = Convert.ToInt32(selectedCertifiedPayroll);
                _projectInfoDto.NetTermsDate = txtNetTermsDate.ToString();
                _projectInfoDto.RetainagePerc = Convert.ToInt32(txtRetainage);

                _bidInfoDto.ProjectLocation = _projectLocationDto;
                _bidInfoDto.ContractorInfo = _contractorInfoDto;
                _bidInfoDto.ProjectInfo = _projectInfoDto;

                var response = await _estimationService.UpdateBid(_bidInfoDto);

                if (response != null && response.Success)
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = true
                           ? NotificationSeverity.Success
                           : NotificationSeverity.Error,

                        Summary = true ? "Success" : "Error",
                        Detail = true ? "Project Details Updated Successfully" : "Error while submitting project details",
                       // Duration = 10000,

                    });
                    // Wait for the notification to finish (wait for 4 seconds) before navigating
                    //Task.Delay(500).ContinueWith(t =>
                    //{
                        // Navigate to the target page after the notification duration
                        NavigationManager.NavigateTo("/Estimation/BidQueue", forceLoad: true);
                    //});
                }
                else
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,

                        Summary = "Error",
                        Detail = "Error while submitting project details.",
                        Duration = 4000
                    });
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,

                    Summary = "Error",
                    Detail = "Error while submitting project details.",
                    Duration = 4000
                });
            }
        }
        protected async Task DeleteBidAmountRow(BidAmount item)
        {
            var confirmed = await DialogService.Confirm(
                "Are you sure you want to delete this record?",
                "Delete",
                new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });

            if (confirmed == true)
            {
                var response = await _estimationService.DeleteBidAmount(item.Id.ToString());
                if (response != null)
                {
                    _bidAmountRecords = await _estimationService.GetBidsAmountAsync(txtJobId);
                }
                _bidAmountGrid?.Reload();
            }
        }
        protected async Task DeleteBidPhaseRow(BidPhase item)
        {
            var confirmed = await DialogService.Confirm(
                "Are you sure you want to delete this record?",
                "Delete",
                new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });

            if (confirmed == true)
            {
                var response = await _estimationService.DeletePhase(item.Id.ToString());
                if (response != null)
                {
                    _bidphaseRecords = await _estimationService.GetPhases(txtJobId);
                }
                _bidPhaseGrid?.Reload();
            }
        }
        protected async Task DeleteEmployeeRow(BidEmployee item)
        {
            var confirmed = await DialogService.Confirm(
                "Are you sure you want to delete this record?",
                "Delete",
                new ConfirmOptions() { OkButtonText = "Yes", CancelButtonText = "No" });

            if (confirmed == true)
            {
                var response = await _estimationService.DeleteEmployee(item.Id.ToString());
                if (response != null)
                {
                    _bidEmployeeRecords = await _estimationService.GetBidEmployees(txtJobId);
                }
                _bidEmployeeGrid?.Reload();
            }
        }
        protected async Task OpenAddBidPopup()
        {
            // 🔹 Clear existing values
            BidDate = null;
            BidAmount = null;

            // 🔹 Force UI refresh before opening popup
            await InvokeAsync(StateHasChanged);

            // 🔹 Open popup
            await popup!.ToggleAsync(button!.Element);
        }
    }
}
