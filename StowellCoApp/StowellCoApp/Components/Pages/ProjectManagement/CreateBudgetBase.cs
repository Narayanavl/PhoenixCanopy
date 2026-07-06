using Microsoft.AspNetCore.Components;
using NPOI.XSSF.UserModel;
using Radzen;
using Radzen.Blazor;
using StowellCoApp.DTO;
using StowellCoApp.Models;
using StowellCoApp.Services;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using static System.Net.WebRequestMethods;

namespace StowellCoApp.Components.Pages.ProjectManagement
{
    public class CreateBudgetBase : ComponentBase
    {
        Phase paramPhase = new Phase();
        public readonly CultureInfo UsCulture = new("en-US");

        public int selectedProjectId;
        [Inject]
        public NotificationService NotificationService { get; set; }
        protected bool isSubmitDisabled = true;
        [Parameter] public int ProjectId { get; set; }

        [Parameter] public string Action { get; set; }

        public string selectedPhaseId { get; set; }

        public string selectedPhaseNum { get; set; }

        protected RadzenDataGrid<StowellCoApp.DTO.BudgetRecord> budgetgrid;

        [Inject]
        public ICostCodeService _costCodeService { get; set; }
        [Inject]
        public IProjectBudget _projectBudgetService { get; set; }

        [Inject]
        public IAccountingService _accountingService { get; set; }

        public IEnumerable<Phase> _phaseModel { get; set; }

        public IEnumerable<CostCodeRecord> _costCodeRecords { get; set; }

        public IEnumerable<StowellCoApp.DTO.BudgetRecord> budgetRecords { get; set; } = new List<StowellCoApp.DTO.BudgetRecord>();

        public bool IsSaving = false;
        protected RadzenUpload radzenUpload;
        protected override async Task OnInitializedAsync()
        {

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
                paramPhase.Source = Action == "CreateBudget" ? "Local" : "Sage";
                budgetRecords = await _projectBudgetService.BudgetData(paramPhase);
            }
            await InvokeAsync(StateHasChanged);
        }

        protected void AddRow()
        {
            budgetRecords = budgetRecords.Append(new StowellCoApp.DTO.BudgetRecord()).ToList();
            budgetgrid.Reload();
        }
        //public void UpdateJobNumberAndPhaseNum(int selectedProjectId, int selectedPhaseId)
        //{
        //    // Loop through the budget records and update the JobNumber and PhaseNum
        //    foreach (var record in budgetRecords)
        //    {
        //        //if(budgetRecords.CostCodeDescription==null && budgetRecords.TotalBudget==null)
        //        // Check if the current record needs updating (e.g., if it's empty or matches specific criteria)
        //        // If you want to update **all records**, just update them unconditionally
        //        //record.CostCode = "Test";
        //        record.JobNumber = selectedProjectId.ToString();  // Set the JobNumber to selectedProjectId
        //        record.PhaseNum = selectedPhaseId;  // Set the PhaseNum to selectedPhaseId
        //    }
        //}
        public bool UpdateJobNumberAndPhaseNum(int selectedProjectId, int selectedPhaseId)
        {
            // Validate all budget records before updating
            bool hasInvalidRows = budgetRecords
                .Any(r => string.IsNullOrWhiteSpace(r.CostCodeDescription) || r.TotalBudget == null);

            if (hasInvalidRows)
            {
                return false;
            }

            // Update records if validation is successful
            foreach (var record in budgetRecords)
            {
                record.JobNumber = selectedProjectId.ToString();
                record.PhaseNum = selectedPhaseId;
                record.Source = Action == "CreateBudget" ? "Local" : "Sage";
                record.ActionType = Action;
            }
            return true;
        }

        protected void OnSubmit(BudgetRecord record)
        {
            // This fires only if validation passes
            SaveBudget();
        }

        protected async Task SaveBudget()
        {
            try
            {
                IsSaving = true;              // 🔥 Show spinner overlay
                StateHasChanged();            // Important for Blazor Server


                //if(budgetRecords.CostCodeDescription==null && budgetRecords.TotalBudget==null)
                var valid = UpdateJobNumberAndPhaseNum(selectedProjectId, Convert.ToInt32(selectedPhaseId));
                if (valid)
                {
                    var result = await _projectBudgetService.SaveCreateBatchCostCodes(budgetRecords.ToList());
                    // Optionally refresh
                    budgetRecords = result.ToList();

                    if (budgetRecords != null)
                    {
                        NotificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Success,
                            Summary = "Success",
                            Detail = "Saved successfully.",
                            Duration = 10000
                        });
                        isSubmitDisabled = false;
                        await budgetgrid.Reload();
                        await InvokeAsync(StateHasChanged);
                    }
                }
                else
                {
                    NotificationService.Notify(NotificationSeverity.Error,
                   "Validation Error",
                   "Please enter Cost Code and Budget for all rows.", 10000);
                    isSubmitDisabled = true;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                IsSaving = false;         // 🔥 Hide spinner overlay
                StateHasChanged();        // Refresh UI
            }
        }

        protected async Task SubmitBudget()
        {
            try
            {
                IsSaving = true;         // 🔥 Hide spinner overlay

                StateHasChanged();        // Refresh UI
                //if(budgetRecords.CostCodeDescription==null && budgetRecords.TotalBudget==null)
                var valid = UpdateJobNumberAndPhaseNum(selectedProjectId, Convert.ToInt32(selectedPhaseId));
                if (valid)
                {
                    var result = await _projectBudgetService.SubmitBatchCostCodes(budgetRecords.ToList());
                    // Optionally refresh
                    budgetRecords = result.ToList();

                    if (budgetRecords != null)
                    {
                        NotificationService.Notify(new NotificationMessage
                        {
                            Severity = NotificationSeverity.Success,
                            Summary = "Success",
                            Detail = "Submitted successfully.",
                            Duration = 10000
                        });
                        isSubmitDisabled = false;
                        await budgetgrid.Reload();
                        await InvokeAsync(StateHasChanged);
                    }
                }
                else
                {
                    NotificationService.Notify(NotificationSeverity.Error,
                   "Validation Error",
                   "Please enter Cost Code and Budget for all rows.", 10000);
                    isSubmitDisabled = true;
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                IsSaving = false;         // 🔥 Hide spinner overlay

                StateHasChanged();        // Refresh UI
            }

        }

        protected async void OnJobChanged(object value)
        {
            selectedProjectId = Convert.ToInt32(value);
            if (selectedProjectId > 0 && selectedPhaseId != null)
            {
                paramPhase.Recnum = selectedProjectId.ToString();
                paramPhase.PhaseNum = selectedPhaseId.ToString();
                paramPhase.PhaseName = selectedPhaseNum;
                paramPhase.Source = Action == "CreateBudget" ? "Local" : "Sage";
                budgetRecords = await _projectBudgetService.BudgetData(paramPhase);
                await InvokeAsync(StateHasChanged);
            }

        }
        protected async void OnPhaseChanged(object value)
        {
            selectedPhaseId = value.ToString();
            if (selectedProjectId > 0 && selectedPhaseId != null)
            {
                paramPhase.Recnum = selectedProjectId.ToString();
                paramPhase.PhaseNum = selectedPhaseId.ToString();
                paramPhase.PhaseName = selectedPhaseNum;
                paramPhase.Source = Action == "CreateBudget" ? "Local" : "Sage";
                budgetRecords = await _projectBudgetService.BudgetData(paramPhase);
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
        Radzen.FileInfo? selectedFile;

        protected void OnFileChange(UploadChangeEventArgs args)

        {

            if (args.Files.Count() > 0)

            {

                selectedFile = args.Files.FirstOrDefault();

            }

            else

            {

                selectedFile = null;

            }

        }

        protected async Task OnUpload()

        {

            if (selectedFile == null)

            {

                Console.WriteLine("No file selected.");
                NotificationService.Notify(NotificationSeverity.Error,
                "Required",
                "Please select an Excel file.", 10000);
                return;

            }

            try

            {

                IsSaving = true;              // 🔥 Show spinner overlay

                StateHasChanged();

                var file = selectedFile;

                using var stream = file.OpenReadStream();

                using var memoryStream = new MemoryStream();

                await stream.CopyToAsync(memoryStream);

                memoryStream.Position = 0;

                await ProcessExcel(memoryStream, selectedProjectId.ToString(), selectedPhaseId.ToString(), true);

                if (selectedProjectId > 0 && selectedPhaseId != null)

                {

                    paramPhase.Recnum = selectedProjectId.ToString();

                    paramPhase.PhaseName = selectedPhaseNum;

                    paramPhase.PhaseNum = selectedPhaseId.ToString();

                    paramPhase.Source = Action == "CreateBudget" ? "Local" : "Sage";

                    budgetRecords = await _projectBudgetService.BudgetData(paramPhase);

                }

                await InvokeAsync(StateHasChanged);
                radzenUpload.ClearFiles();
                await budgetgrid.Reload();

            }

            catch (Exception ex)

            {

                Console.WriteLine("Upload failed: " + ex.Message);

            }

            finally

            {

                IsSaving = false;         // 🔥 Hide spinner overlay

                StateHasChanged();        // Refresh UI

            }

        }
        protected async Task ProcessExcel(Stream excelStream, string jobId, string phaseNumber, bool isNew)
        {
            var result = new List<BudgetRecord>();

            excelStream.Position = 0;

            // IMPORTANT: Load XSSFWorkbook from the MemoryStream
            XSSFWorkbook workbook = new XSSFWorkbook(excelStream);
            var sheet = workbook.GetSheetAt(0);

            for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
            {
                var row = sheet.GetRow(rowIndex);
                if (row == null) continue;

                string fullCostCode = row.GetCell(0)?.ToString()?.Trim();
                string newAmount = row.GetCell(1)?.ToString()?.Trim();
                string updatedAmount = row.GetCell(2)?.ToString()?.Trim();

                if (string.IsNullOrWhiteSpace(fullCostCode))
                    continue;

                decimal updateBudget = decimal.TryParse(updatedAmount, out _) ? Convert.ToDecimal(updatedAmount) : 0;
                decimal totalBudget = decimal.TryParse(newAmount, out _) ? Convert.ToDecimal(newAmount) : 0;

                // For new upload: ignore rows without amounts
                if (isNew && totalBudget == 0)
                    continue;

                // For update: skip if both are zero
                if (!isNew && totalBudget == 0 && updateBudget == 0)
                    continue;

                // Extract Cost Code and Description
                var hyphenIndex = fullCostCode.IndexOf('-');
                string code = hyphenIndex > 0 ? fullCostCode[..hyphenIndex].Trim() : fullCostCode;
                string description = fullCostCode;
                //hyphenIndex > 0 ? fullCostCode[(hyphenIndex + 1)..].Trim() : "";

                result.Add(new BudgetRecord
                {
                    CostCode = code,
                    CostCodeDescription = description,
                    TotalBudget = totalBudget,
                    UpdateBudget = updateBudget,
                    JobNumber = jobId,
                    PhaseNum = decimal.TryParse(phaseNumber, out var pn) ? pn : 0
                });
            }
            var response = await _projectBudgetService.UploadAddExcelAsync(result);
            if (response != null)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = "Success",
                    Detail = "Uploaded successfully.",
                    Duration = 10000
                });
            }

        }

        protected void OnUploadComplete(UploadCompleteEventArgs args)
        {
            Console.WriteLine("Upload complete!");
        }

        protected void OnUploadError(UploadErrorEventArgs args)
        {
            Console.WriteLine("Upload error: " + args.Message);
        }
        protected void OnProgress(UploadProgressArgs args)
        {
            // console.Log($"Upload progress: {args.Progress}% / {args.Loaded} of {args.Total} bytes.");

            if (args.Progress == 100)
            {
                foreach (var file in args.Files)
                {
                    // console.Log($"Uploaded: {file.Name} / {file.Size} bytes");
                }
            }
        }
    }
}
