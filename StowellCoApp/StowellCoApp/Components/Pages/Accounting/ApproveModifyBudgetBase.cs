using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using StowellCoApp.DTO;
using StowellCoApp.Models;
using StowellCoApp.Services;
using System.Globalization;

namespace StowellCoApp.Components.Pages.Accounting
{
    public class ApproveModifyBudgetBase: ComponentBase
    {
        public readonly CultureInfo UsCulture = new("en-US");
        [Parameter]
        public int ProjectId { get; set; }
        public int selectedProjectId { get; set; }
        [Inject]
        public NotificationService NotificationService { get; set; }
        [Inject]
        public ICostCodeService _costCodeService { get; set; }
        [Inject]
        public IProjectBudget _projectBudgetService { get; set; }
        [Inject]
        private ILogger<ApproveNewBudget> logger { get; set; } = default!;
        [Inject]
        public IAccountingService _accountingService { get; set; }
        public IEnumerable<CostCodeRecord> _costCodeRecords { get; set; }
        public IEnumerable<Phase> _phaseModel { get; set; }

        public string selectedPhaseId { get; set; }
        protected RadzenDataGrid<StowellCoApp.DTO.BudgetRecord> budgetgrid;
        public string selectedPhaseNum { get; set; }
        public IEnumerable<StowellCoApp.DTO.BudgetRecord> budgetRecords { get; set; } = new List<StowellCoApp.DTO.BudgetRecord>();
        Phase paramPhase = new Phase();
        public bool IsSaving = false;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender) // important to avoid infinite loops
            {
                try
                {
                    selectedProjectId = ProjectId;
                    _phaseModel = await _costCodeService.GetJobPhasesData(ProjectId);
                    selectedPhaseId = _phaseModel != null && _phaseModel.Any() ? _phaseModel.First().PhaseNum : "0";
                    selectedPhaseNum = _phaseModel != null && _phaseModel.Any() ? _phaseModel.First().PhaseName : string.Empty;
                    _costCodeRecords = await _costCodeService.GetAllUserJobs();
                    selectedProjectId = ProjectId;
                    if (selectedProjectId > 0 && selectedPhaseId != null)
                    {
                        paramPhase.Recnum = selectedProjectId.ToString();
                        paramPhase.PhaseName = selectedPhaseNum;
                        paramPhase.PhaseNum = selectedPhaseId.ToString();
                        budgetRecords = await _accountingService.GetModifyBudgetData(paramPhase);
                    }
                    await InvokeAsync(StateHasChanged);
                }
                catch (Exception ex)
                {
                    logger.LogInformation($"Error loading jobs: {ex.Message}");
                    Console.WriteLine($"Error loading jobs: {ex.Message}");
                }
            }
        }
        protected async void OnJobChanged(object value)
        {
            selectedProjectId = Convert.ToInt32(value);
            if (selectedProjectId > 0 && selectedPhaseId != null)
            {
                paramPhase.Recnum = selectedProjectId.ToString();
                paramPhase.PhaseName = selectedPhaseNum;
                paramPhase.PhaseNum = selectedPhaseId.ToString();
                budgetRecords = await _accountingService.GetModifyBudgetData(paramPhase);
                await InvokeAsync(StateHasChanged);
            }
        }
        protected async void OnPhaseChanged(object value)
        {
            selectedPhaseId = value.ToString();
            if (selectedProjectId > 0 && selectedPhaseId != null)
            {
                paramPhase.Recnum = selectedProjectId.ToString();
                paramPhase.PhaseName = selectedPhaseNum;
                paramPhase.PhaseNum = selectedPhaseId.ToString();
                budgetRecords = await _accountingService.GetModifyBudgetData(paramPhase);
                await InvokeAsync(StateHasChanged);
            }
        }
        protected IEnumerable<CostCodeList> costCodeDescriptions = new List<CostCodeList>();

        // Example list of Cost Code Descriptions
        protected async Task LoadCostCodeDescriptions(LoadDataArgs args)
        {
            string filter = args.Filter ?? string.Empty;
            costCodeDescriptions = await _accountingService.GetCostCodes(filter);
            StateHasChanged();
        }
        protected async Task ApproveNewBudget()
        {
            try
            {
                IsSaving = true;         // 🔥 show spinner overlay
                StateHasChanged();        // Refresh UI
                if (selectedProjectId > 0 && selectedPhaseId != null)
                {
                    paramPhase.Recnum = selectedProjectId.ToString();
                    paramPhase.PhaseName = selectedPhaseNum;
                    paramPhase.PhaseNum = selectedPhaseId.ToString();
                    ApiResponse response =
                await _accountingService.BudgetApproveRequestAsync(paramPhase);

                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = response.Success
                            ? NotificationSeverity.Success
                            : NotificationSeverity.Error,

                        Summary = response.Success ? "Success" : "Error",
                        Detail = response.Message ?? "Approve Budget failed.",
                        Duration = 4000
                    });

                    await InvokeAsync(StateHasChanged);
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,

                    Summary = "Error",
                    Detail = "Approve Budget failed.",
                    Duration = 4000
                });
            }
            finally
            {
                IsSaving = false;         // 🔥 Hide spinner overlay
                StateHasChanged();        // Refresh UI
            }
        }
        protected async Task RejectNewBudget()
        {
            try
            {
                IsSaving = true;         // 🔥 show spinner overlay
                StateHasChanged();        // Refresh UI
                if (selectedProjectId > 0 && selectedPhaseId != null)
                {
                    paramPhase.Recnum = selectedProjectId.ToString();
                    paramPhase.PhaseName = selectedPhaseNum;
                    paramPhase.PhaseNum = selectedPhaseId.ToString();
                    ApiResponse response =
                await _accountingService.BudgetRejectRequest(paramPhase);

                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = response.Success
                            ? NotificationSeverity.Success
                            : NotificationSeverity.Error,

                        Summary = response.Success ? "Success" : "Error",
                        Detail = response.Message ?? "Reject Budget failed.",
                        Duration = 4000
                    });

                    await InvokeAsync(StateHasChanged);
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,

                    Summary = "Error",
                    Detail = "Reject Budget failed.",
                    Duration = 4000
                });
            }
            finally
            {
                IsSaving = false;         // 🔥 Hide spinner overlay
                StateHasChanged();        // Refresh UI
            }
        }
    }
}