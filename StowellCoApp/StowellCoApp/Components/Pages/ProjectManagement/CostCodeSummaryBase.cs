using ClosedXML.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using StowellCoApp.Components.Pages.ReportingDashboard;
using StowellCoApp.DTO;
using StowellCoApp.Services;
using System.Globalization;
using System.Text;

namespace StowellCoApp.Components.Pages.ProjectManagement
{
    public class CostCodeSummaryBase : ComponentBase
    {
        public readonly CultureInfo UsCulture = new("en-US");

        [Inject] IJSRuntime JS { get; set; } = default!;
        [Parameter] public int ProjectId { get; set; }
        public string selectedProjectId;
        public int selectedCostCodeId { get; set; }
        public List<string> SelectedCostCodes { get; set; } = new();
        protected DateTime? StartDate { get; set; }
        protected DateTime? EndDate { get; set; }
        public string selectedPhaseId { get; set; }
        [Inject]
        public IJobSummaryService _jobSummaryService { get; set; }
        [Inject]
        public ICostCodeService _costCodeService { get; set; }
        [Inject]
        protected ILogger<JobSummary> logger { get; set; } = default!;
        protected RadzenDataGrid<CostCodeSummaryRecord> grid;
        //protected IEnumerable<CostCodeSummaryRecord> filteredJobs;
        protected string searchTerm = string.Empty;
        ///protected IEnumerable<CostCodeSummaryRecord> jobs;
       // protected IEnumerable<CostCodeSummaryRecord> pagedJobs;
        protected CancellationTokenSource cts = new();
        RadzenDataGrid<CostCodeSummaryRecord> dataGrid;
        IEnumerable<int> pageSizeOptions = new int[] { 10, 20, 50 };
        IEnumerable<CostCodeSummaryRecord> orderDetails;
        // Initialize EVERYTHING to avoid nulls
        protected IEnumerable<CostCodeSummaryRecord> filteredJobs = Enumerable.Empty<CostCodeSummaryRecord>();
        protected IEnumerable<CostCodeSummaryRecord> jobs = Enumerable.Empty<CostCodeSummaryRecord>();
        protected IEnumerable<CostCodeSummaryRecord> pagedJobs = Enumerable.Empty<CostCodeSummaryRecord>();
        IEnumerable<int> values = new int[] { 1, 2 };
        public IEnumerable<CostCodeRecord> _costCodeRecords { get; set; }
        // Dropdown with "Select All" prepended
        public List<StowellCoApp.DTO.CostCode> CostCodesWithSelectAll { get; set; } = new();
        public List<Phase> phasesList { get; set; } = new();
        public CurrentCostSummaryViewModel Model { get; set; } =
            new CurrentCostSummaryViewModel
            {
                Jobs = new List<Job>(),
                CostCodes = new List<StowellCoApp.DTO.CostCode>(),
                CostCodeSummaryModel = new CostCodeSummaryModel()
            };
        bool showPagerSummary = true;

        protected async Task FirstPage()
        {
            await dataGrid.FirstPage();
        }
        protected async Task TenthPage()
        {
            await dataGrid.GoToPage(9);
        }
        protected async Task LastPage()
        {
            await dataGrid.LastPage();
        }
       protected void OnPage(PagerEventArgs args)
        {
            //
        }
        protected async void OnJobChanged(object value)
        {
            selectedProjectId = Convert.ToString(value);

            var costcodesList = await _costCodeService
                .GetCostCodesForReport(selectedProjectId);

            SelectedCostCodes = costcodesList?
                .Select(x => x.Description)
                .ToList() ?? new List<string>();
            Model.CostCodes = costcodesList.ToList();
            Model.Phases = await _costCodeService
                .GetPhasesForReport(selectedProjectId);
            StateHasChanged();
        }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                selectedProjectId = ProjectId.ToString();
                Model.Jobs = await _costCodeService.GetJobCodesForReport();
                //Model.CostCodes = new List<CostCode>();
                //Model.CostCodeSummaryModel = new CostCodeSummaryModel();
                var costcodesList = await _costCodeService
                .GetCostCodesForReport(selectedProjectId);

                SelectedCostCodes = costcodesList?
                    .Select(x => x.Description)
                    .ToList() ?? new List<string>();
                Model.CostCodes = costcodesList.ToList();
                Model.Phases = await _costCodeService
                    .GetPhasesForReport(selectedProjectId);
                Model.CostCodeSummaryModel.CostCodeSummary = new List<CostCodeSummaryRecord>();
                Model = await _costCodeService.GetCurrentCostSummaryViewModel(Convert.ToString(selectedProjectId));
                if (Model != null && Model.CostCodeSummaryModel != null)
                {
                    filteredJobs = Model.CostCodeSummaryModel.CostCodeSummary;
                }
                // OnSubmitClick();
            }
            catch (Exception ex)
            {
                logger.LogError($"Error loading CostCodeSummaryReport OnInitializedAsync: {ex.Message}");
            }
        }
        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
        //    if (!firstRender) return;

        //    try
        //    {
        //        logger.LogInformation("Loading job summary records…");
        //       // jobs = await _jobSummaryService.GetJobCostSummaryRecords(ProjectId.ToString());
        //        Model = await _costCodeService.GetCurrentCostSummaryViewModel(Convert.ToString(ProjectId));
        //        filteredJobs = Model.CostCodeSummaryModel.CostCodeSummary;
        //        selectedProjectId = ProjectId.ToString();
        //        if (Model != null)
        //        {
        //            var phasesmodel = Model.Phases;
        //            // Add the "All" option to the beginning of the list
        //            phasesList.Add(new Phase { PhaseNum = string.Empty, PhaseName = "All" });

        //            // Add the actual fetched phases
        //            phasesList.AddRange(phasesmodel);

        //            // Optionally, set the default selected value to "All" (null)
        //            selectedPhaseId = string.Empty;  // This selects "All"
        //        }
        //        StateHasChanged();
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError($"Error loading jobs: {ex.Message}");
        //    }
        //}

        protected void OnCostCodeChange(object value)
        {
            var selectedObjects = value as IEnumerable<object>;

            // Check if selectedObjects is null or empty
            if (selectedObjects == null || !selectedObjects.Any())
            {
                SelectedCostCodes = new List<string>(); // Clear selection
                StateHasChanged(); // Ensure UI is updated
                return;
            }

            // Convert selected objects to strings
            SelectedCostCodes = selectedObjects
                .Select(x => x?.ToString())
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList();

            // Debug log to check what was selected
            Console.WriteLine("Selected Cost Codes: " + string.Join(", ", SelectedCostCodes));

            // Update the UI
            StateHasChanged();
        }


        protected async Task OnInputChanged(ChangeEventArgs args)
        {
            searchTerm = args.Value?.ToString() ?? string.Empty;

            cts.Cancel();
            cts = new CancellationTokenSource();
            var token = cts.Token;

            try
            {
                await Task.Delay(300, token);
                if (token.IsCancellationRequested) return;

                FilterJobs();
            }
            catch (TaskCanceledException) { }
        }

        protected void FilterJobs()
        {
            filteredJobs = string.IsNullOrWhiteSpace(searchTerm)
                ? jobs
                : jobs.Where(j =>
                      (j.JobNumber?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                      (j.CostCode?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                      (j.CostCodeDescription?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false));

            grid?.Reload();
        }

        protected void OnPageSizeChangedInput(object value)
        {
            selectedPageSize = value.ToString();

            if (selectedPageSize == "All")
                pageSize = filteredJobs.Count();
            else
                pageSize = Convert.ToInt32(selectedPageSize);

            currentPage = 0;

            grid?.GoToPage(0);
            grid?.Reload();
        }
        protected string selectedPageSize = "15"; // default
        protected string[] pageSizes = new string[] { "10", "15", "20", "50", "All" };
        protected int pageSize = 15;
        protected int currentPage = 0;
        protected void OnPageSizeChanged(int newPageSize)
        {
            pageSize = newPageSize;
            currentPage = 0; // reset to first page
            ApplyPaging();
        }
        

        protected void ApplyPaging()
        {
            if (pageSize == 0)
            {
                pagedJobs = filteredJobs;
            }
            else
            {
                pagedJobs = filteredJobs
                    .Skip(currentPage * pageSize)
                    .Take(pageSize);
            }
            grid?.Reload();
        }
        protected async Task OnLoadData(LoadDataArgs args)
        {
            // Handle search
            filteredJobs = string.IsNullOrWhiteSpace(searchTerm)
                ? jobs
                : jobs.Where(j =>
                    (j.JobNumber?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (j.CostCode?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (j.CostCodeDescription?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false));

            // Handle paging
            pagedJobs = pageSize == 0
                ? filteredJobs
                : filteredJobs.Skip(args.Skip ?? 0).Take(args.Top ?? pageSize);

            StateHasChanged();
        }


        protected void OnPageChanged(int newPageIndex)
        {
            currentPage = newPageIndex;
            ApplyPaging();
        }
        protected async void OnSubmitClick()
        {
            try
            {
                Model = await _costCodeService.GetCurrentCostSummaryViewModel(Convert.ToString(ProjectId), SelectedCostCodes, selectedPhaseId, StartDate.HasValue ? Convert.ToString(StartDate.Value) : "", EndDate.HasValue ? Convert.ToString(EndDate.Value) : "");
                if (Model != null && Model.CostCodeSummaryModel != null)
                {
                    filteredJobs = Model.CostCodeSummaryModel.CostCodeSummary;
                }
                StateHasChanged();
            }
            catch (Exception ex)
            {
                logger.LogError($"Error loading jobs: {ex.Message}");
            }
        }
        protected async Task ExportPdf()
        {
            var headers = new[]
            {
           "Job ID",
            "Cost Code",
            "Cost Code Description",
            "Budget & Changes",
            "To Date",
            "This Period",
            "Remaining",
        };

            var rows = filteredJobs.Select(r => new string[]
 {
    r.JobNumber,
    r.CostCode,
    r.CostCodeDescription,
    PdfReportHelper.Money(r.BudgetAndChanges),
  PdfReportHelper.Money(r.ToDate),
    PdfReportHelper.Money(r.ThisPeriod),
    PdfReportHelper.Money(r.Remaining)
 }).ToList();


            byte[] pdfBytes = PdfReportHelper.BuildPdf(
                "Cost Code Summary Report",
                headers,
                rows,
                landscape: true
            );

            var base64 = Convert.ToBase64String(pdfBytes);

            await JS.InvokeVoidAsync(
                "downloadPDFFileFromBase64",
                "CostCodeSummaryReport.pdf",
                "application/pdf",
                base64
            );
        }
        protected void CopyAllToClipboard()
        {
            try
            {
                // Build the clipboard text synchronously
                var text = BuildAllRowsClipboardText();

                // Call JS immediately — no await
                JS.InvokeVoidAsync("copyTextToClipboard", text);
                JS.InvokeVoidAsync("showToast", "Copied " + filteredJobs.Count() + " rows to clipboard!");
            }
            catch (Exception ex)
            {

            }
        }

        string BuildAllRowsClipboardText()
        {
            var sb = new StringBuilder();

            // Header row
            sb.AppendLine("Job Number\tCost Code\tCost Code Description\tBudget & Changes\tTo Date\tThis Period\tRemaining");

            // All rows
            foreach (var r in filteredJobs)
            {
                sb.AppendLine(
                    $"{r.JobNumber}\t" +
                    $"{r.CostCode}\t" +
                    $"{r.CostCodeDescription}\t" +
                    $"{PdfReportHelper.Money(r.BudgetAndChanges)}\t" +
                    $"{PdfReportHelper.Money(r.ToDate)}\t" +
                    $"{PdfReportHelper.Money(r.ThisPeriod)}\t" +
                    $"{PdfReportHelper.Money(r.Remaining)}"
                );
            }

            return sb.ToString();
        }

        protected async Task ExportExcel()

        {

            using var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Cost Code Summary");

            // Header

            worksheet.Cell(1, 1).Value = "Job ID";

            worksheet.Cell(1, 2).Value = "Cost Code";

            worksheet.Cell(1, 3).Value = "Cost Code Description";

            worksheet.Cell(1, 4).Value = "Budget & Changes";

            worksheet.Cell(1, 5).Value = "To Date";

            worksheet.Cell(1, 6).Value = "This Period";

            worksheet.Cell(1, 7).Value = "Remaining";
            // 🔹 Header styling (PDF-like)
            var headerRange = worksheet.Range(1, 1, 1, 7);
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            int row = 2;

            foreach (var r in filteredJobs)

            {

                worksheet.Cell(row, 1).Value = r.JobNumber;

                worksheet.Cell(row, 2).Value = r.CostCode;

                worksheet.Cell(row, 3).Value = r.CostCodeDescription;

                worksheet.Cell(row, 4).Value = PdfReportHelper.Money(r.BudgetAndChanges);

                worksheet.Cell(row, 5).Value = PdfReportHelper.Money(r.ToDate);

                worksheet.Cell(row, 6).Value = PdfReportHelper.Money(r.ThisPeriod);

                worksheet.Cell(row, 7).Value = PdfReportHelper.Money(r.Remaining);

                row++;

            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();

            workbook.SaveAs(stream);

            var base64 = Convert.ToBase64String(stream.ToArray());
            try
            {
                await JS.InvokeVoidAsync(

                    "downloadExcelFileFromBase64",

                    "CostCodeSummaryReport.xlsx",

                    base64

                );
            }
            catch (JSDisconnectedException)
            {
                // User left page – safe to ignore
            }
            catch (Exception ex)
            {

            }

        }
        protected async Task ExportCsvManual()
        {
            var csv = new StringBuilder();
            csv.AppendLine("Job Number,Cost Code,Cost Code Description,Budget & Changes,To Date,This Period,Remaining");

            foreach (var r in filteredJobs)
            {
                csv.AppendLine(
                    $"\"{r.JobNumber}\"," +
                    $"\"{r.CostCode}\"," +
                    $"\"{r.CostCodeDescription}\"," +
                    $"\"{PdfReportHelper.Money(r.BudgetAndChanges)}\"," +
                    $"\"{PdfReportHelper.Money(r.ToDate)}\"," +
                    $"\"{PdfReportHelper.Money(r.ThisPeriod)}\"," +
                    $"\"{PdfReportHelper.Money(r.Remaining)}\""
                );
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            var base64 = Convert.ToBase64String(bytes);
            await JS.InvokeVoidAsync("downloadFileFromBase64", "CostCodeSummaryReport.csv", base64);
        }
    }
}

