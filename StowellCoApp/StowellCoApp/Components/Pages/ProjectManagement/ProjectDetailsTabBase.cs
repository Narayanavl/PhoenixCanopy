using Microsoft.AspNetCore.Components;
using Radzen.Blazor;
using StowellCoApp.DTO;
using StowellCoApp.Services;
using System.Globalization;

namespace StowellCoApp.Components.Pages.ProjectManagement
{
    public class ProjectDetailsTabBase: ComponentBase
    {

        protected RadzenDataGrid<EmployeeDetails> employeeGrid;

        protected IEnumerable<EmployeeDetails> filteredemployees;

        protected IEnumerable<EmployeeDetails> employees;

        [Parameter] public int ProjectId { get; set; }

        [Inject]
        public IProjectOverviewService _overviewservice { get; set; }

        [Inject]
        public IEstimationService _estimateservice { get; set; }

        public CurrentCostSummaryViewModel _currencycostModel { get; set; }

        [Inject]
        public HttpClient Http { get; set; }

        [Parameter] public BidInfoDto _bidModel { get; set; }

        [Parameter] public IEnumerable<EmployeeDetails> _employeeModel { get; set; }

        protected string? selectedclient = null;
        protected string? selectedInsuranceType = null;
        protected DateTime? txtNetTermsDate;
        protected decimal? txtRetainage;
        protected string? selectedCertifiedPayroll = null;
        protected string? selectedInvoiceSubmittal = null;
        protected string? selectedTaxExempt = null;
        protected string? selectedBonded = null;
        protected int pageSize = 10;
        public readonly CultureInfo UsCulture = new("en-US");
        // public string JobId { get; set; }
        public IEnumerable<StowellCoApp.DTO.ContractorClient> _contractClientdroplist { get; set; } = new List<StowellCoApp.DTO.ContractorClient>();
        public int? selectedClientId { get; set; }
        [Inject]
        public IConfiguration _configuration { get; set; }
        protected string? projectfolderlibraryUrl = string.Empty;
        protected override async Task OnInitializedAsync()
        {
            projectfolderlibraryUrl = _configuration["ProjectFolderLibrary"];
            _contractClientdroplist = await _estimateservice.GetAllContractorClients();
            _currencycostModel = await _overviewservice.GetCurrentCostSummaryViewModel(Convert.ToString(ProjectId));
            _bidModel = await _estimateservice.GetBidDetails(ProjectId);
            filteredemployees = await _estimateservice.GetEmployees(ProjectId);
            if (_bidModel != null)
            {
                if (_bidModel.ContractorInfo != null)
                {
                    //selectedclient = _bidModel.ContractorInfo.Client;
                    selectedClientId = int.TryParse(_bidModel.ContractorInfo.Client, out var clientId) ? clientId : 0;
                }
                if (_bidModel.ProjectInfo != null)
                {
                    selectedInsuranceType = _bidModel.ProjectInfo.InsuranceType.ToString();
                    selectedBonded = _bidModel.ProjectInfo.Bonded.ToString();
                    selectedTaxExempt = _bidModel.ProjectInfo.TaxExcempt.ToString();
                    selectedInvoiceSubmittal = _bidModel.ProjectInfo.InvoiceSubmittal.ToString();
                    selectedCertifiedPayroll = _bidModel.ProjectInfo.CertifiedPayroll.ToString();
                    txtNetTermsDate = DateTime.TryParse(_bidModel.ProjectInfo.NetTermsDate, out var netTermsDate) ? netTermsDate : null;
                    txtRetainage = Convert.ToDecimal(_bidModel.ProjectInfo.RetainagePerc);
                }
                  
            }
            
        }

    }
}
