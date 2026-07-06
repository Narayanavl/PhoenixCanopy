using Microsoft.AspNetCore.Components;
using StowellCoApp.DTO;
using StowellCoApp.Services;

namespace StowellCoApp.Components.Pages.ProjectManagement
{
    public class ProjectOverviewBase : ComponentBase
    {
        [Parameter] public int ProjectId { get; set; }

        [Inject] public IProjectOverviewService ProjectService { get; set; }

        public ProjectOverviewModel Model { get; set; } = new();
        public string ActiveTab { get; set; } = "overview";

        protected override async Task OnInitializedAsync()
        {
            Model = new ProjectOverviewModel();
           // await ProjectService.GetProjectOverviewAsync(ProjectId);
        }

        protected void SetTab(string tab)
        {
            ActiveTab = tab;
        }
    }
    public class ProjectOverviewModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Customer { get; set; }
        public string Estimator { get; set; }

        // Overview fields
        public string Status { get; set; }
        public DateTime CreatedOn { get; set; }

        // Details tab
        public string JobAddress { get; set; }
        public string JobCity { get; set; }
        public string JobState { get; set; }
        public string JobZip { get; set; }

        // Financials
        public decimal ContractAmount { get; set; }
        public decimal Cost { get; set; }
        public decimal Margin => ContractAmount - Cost;

        // Reports
        public IEnumerable<ProjectReportItem> Reports { get; set; } = new List<ProjectReportItem>();
        public string SelectedJobId { get; set; }
        public string SelectedJobName { get; set; }
        public string ShareFolderLink { get; set; }
        public List<string> SelectedCostCodeIds { get; set; }
        public string SelectedPhaseId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public List<Job> Jobs { get; set; }
        public List<CostCode> CostCodes { get; set; }
        public List<Phase> Phases { get; set; }
        public CostCodeSummaryModel CostCodeSummaryModel { get; set; }
        public CashFlowRecord CashFlowRecord { get; set; }
        public BudgetPayment BudgetPayment { get; set; }
    }

    public class ProjectReportItem
    {
        public string Title { get; set; }
        public DateTime Date { get; set; }
    }

}
