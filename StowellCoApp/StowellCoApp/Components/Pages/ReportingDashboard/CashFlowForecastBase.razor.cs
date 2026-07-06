using ClosedXML.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NPOI.SS.Formula.Functions;
using Radzen.Blazor;
using StowellCoApp.Components.Pages.ProjectManagement;
using StowellCoApp.Models;
using StowellCoApp.Services;
using System.Text;
using static System.Net.WebRequestMethods;

namespace StowellCoApp.Components.Pages.ReportingDashboard
{
    public class CashFlowForecastBase : ComponentBase
    {
        protected CashFlowForecastResponse forecastData = new();
        protected IEnumerable<CashFlowForecastRow> filteredBudgetForecast;
        protected IEnumerable<CashFlowForecastRow> filteredContractForecast;
        [Inject]
        private ILogger<CashFlowForecastBase> logger { get; set; } = default!;
        protected RadzenDataGrid<CashFlowForecastRow> BudgetForecastGrid;
        protected RadzenDataGrid<CashFlowForecastRow> ContractForecastGrid;
        protected List<string> MonthColumnsBudget = new();
        protected List<string> MonthColumnsContract = new();
        private CancellationTokenSource cts = new();
        protected string searchTermBudget = string.Empty;
        protected string searchTermContract = string.Empty;
        [Inject] IJSRuntime JS { get; set; } = default!;
        [Inject]
        public IReportingService _reportingService { get; set; }
        //protected override async Task OnInitializedAsync()
        //{
        //    forecastData = await Http.GetFromJsonAsync<CashFlowForecastResponse>(
        //        "api/Dashboard/CashFlowForecast");

        //    filteredBudgetForecast = forecastData.BudgetForecast;
        //    filteredContractForecast = forecastData.ContractForecast;

        //    MonthColumns = forecastData.BudgetForecast
        //        .FirstOrDefault()?
        //        .ForecastMonths
        //        .Keys
        //        .ToList() ?? new();
        //}
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender) // important to avoid infinite loops
            {
                try
                {
                    logger.LogInformation("OnAfterRenderAsync start");
                    forecastData = await _reportingService.GetCashFlowForecastReportData();
                    filteredBudgetForecast = forecastData.BudgetForecast;
                    MonthColumnsBudget = forecastData.BudgetForecast
                        .FirstOrDefault()?
                        .ForecastMonths
                        .Keys
                        .ToList() ?? new();
                    filteredContractForecast = forecastData.ContractForecast;
                    MonthColumnsContract = forecastData.ContractForecast
                        .FirstOrDefault()?
                        .ForecastMonths
                        .Keys
                        .ToList() ?? new();
                    
                    StateHasChanged(); // optional, usually not needed in firstRender
                }
                catch (Exception ex)
                {
                    logger.LogInformation($"Error loading jobs: {ex.Message}");
                    Console.WriteLine($"Error loading jobs: {ex.Message}");
                }
            }
        }
        protected async Task OnInputChangedBudget(ChangeEventArgs args)
        {
            searchTermBudget = args.Value?.ToString() ?? string.Empty;

            // Cancel previous debounce
            cts.Cancel();
            cts = new CancellationTokenSource();
            var token = cts.Token;

            try
            {
                await Task.Delay(300, token); // debounce 300ms

                if (token.IsCancellationRequested) return;

                FilteredBudgetForecastByList();
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
        }
        protected async Task OnInputChangedContract(ChangeEventArgs args)
        {
            searchTermContract = args.Value?.ToString() ?? string.Empty;

            // Cancel previous debounce
            cts.Cancel();
            cts = new CancellationTokenSource();
            var token = cts.Token;

            try
            {
                await Task.Delay(300, token); // debounce 300ms

                if (token.IsCancellationRequested) return;

                FilteredBudgetForecastByList();
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
        }
        protected string selectedPageSizeBudget = "10"; // default
        protected string selectedPageSizeContract = "10"; // default
        protected string[] pageSizes = new string[] { "10", "20", "50", "All" };
        protected int pageSize = 10;
        private int currentPage = 0;
        protected void OnPageSizeChangedBudget(object value)
        {
            selectedPageSizeBudget = value.ToString();

            if (selectedPageSizeBudget == "All")
            {
                // Show all records
                pageSize = filteredBudgetForecast?.Count() ?? 0;
            }
            else
            {
                pageSize = Convert.ToInt32(selectedPageSizeBudget);
            }

            currentPage = 0;

            // Refresh the grid and pager
            BudgetForecastGrid.GoToPage(0);
            BudgetForecastGrid.Reload();
        }
        protected void FilteredBudgetForecastByList()
        {
            if (string.IsNullOrWhiteSpace(searchTermBudget))
            {
                filteredBudgetForecast = filteredBudgetForecast;
            }
            else
            {
                filteredBudgetForecast = filteredBudgetForecast.Where(j =>
                     j.JobName.ToString().Contains(searchTermBudget, StringComparison.OrdinalIgnoreCase) ||
                    (j.JobNumber?.Contains(searchTermBudget, StringComparison.OrdinalIgnoreCase) ?? false) 
                );
            }

            BudgetForecastGrid.Reload();
        }
        protected void OnPageSizeChangedContract(object value)
        {
            selectedPageSizeContract = value.ToString();

            if (selectedPageSizeContract == "All")
            {
                // Show all records
                pageSize = filteredContractForecast?.Count() ?? 0;
            }
            else
            {
                pageSize = Convert.ToInt32(selectedPageSizeContract);
            }

            currentPage = 0;

            // Refresh the grid and pager
            ContractForecastGrid.GoToPage(0);
            ContractForecastGrid.Reload();
        }
        protected void FilteredContractForecastByList()
        {
            if (string.IsNullOrWhiteSpace(searchTermContract))
            {
                filteredContractForecast = filteredContractForecast;
            }
            else
            {
                filteredBudgetForecast = filteredContractForecast.Where(j =>
                     j.JobName.ToString().Contains(searchTermContract, StringComparison.OrdinalIgnoreCase) ||
                    (j.JobNumber?.Contains(searchTermContract, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            ContractForecastGrid.Reload();
        }
        protected void CopyAllToClipboardBudget()
        {
            try
            {
                // Build the clipboard text synchronously
                var text = BuildBudgetForecastClipboardText();

                // Call JS immediately — no await
                JS.InvokeVoidAsync("copyTextToClipboard", text);
                JS.InvokeVoidAsync("showToast", "Copied " + filteredBudgetForecast.Count() + " rows to clipboard!");
            }
            catch (Exception ex)
            {

            }
        }
        protected string BuildBudgetForecastClipboardText()
        {
            var sb = new StringBuilder();

            // Header
            sb.Append("Job Number\tJob Name\tBudget Amount\tCosts As Of\tRemaining");

            foreach (var month in MonthColumnsBudget)
            {
                sb.Append($"\t{month}");
            }

            sb.AppendLine();

            // Rows
            foreach (var row in filteredBudgetForecast)
            {
                sb.Append(
                    $"{row.JobNumber}\t" +
                    $"{row.JobName}\t" +
                    $"{PdfReportHelper.Money(row.TotalAmount)}\t" +
                    $"{PdfReportHelper.Money(row.CurrentAmount)}\t" +
                    $"{PdfReportHelper.Money(row.Remaining)}"
                );

                foreach (var month in MonthColumnsBudget)
                {
                    var value = row.ForecastMonths.TryGetValue(month, out var amount)
                        ? amount
                        : 0;

                    sb.Append($"\t{PdfReportHelper.Money(value)}");
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
        protected void CopyAllToClipboardContract()
        {
            try
            {
                // Build the clipboard text synchronously
                var text = BuildContractForecastClipboardText();

                // Call JS immediately — no await
                JS.InvokeVoidAsync("copyTextToClipboard", text);
                JS.InvokeVoidAsync("showToast", "Copied " + filteredBudgetForecast.Count() + " rows to clipboard!");
            }
            catch (Exception ex)
            {

            }
        }
        private string BuildContractForecastClipboardText()
        {
            var sb = new StringBuilder();

            // Header
            sb.Append("Job Number\tJob Name\tTotal Contract Amount\tBilled As Of\tRemaining");

            foreach (var month in MonthColumnsContract)
            {
                sb.Append($"\t{month}");
            }

            sb.AppendLine();

            // Rows
            foreach (var row in filteredContractForecast)
            {
                sb.Append(
                    $"{row.JobNumber}\t" +
                    $"{row.JobName}\t" +
                    $"{PdfReportHelper.Money(row.TotalAmount)}\t" +
                    $"{PdfReportHelper.Money(row.CurrentAmount)}\t" +
                    $"{PdfReportHelper.Money(row.Remaining)}"
                );

                foreach (var month in MonthColumnsContract)
                {
                    var value = row.ForecastMonths.TryGetValue(month, out var amount)
                        ? amount
                        : 0;

                    sb.Append($"\t{PdfReportHelper.Money(value)}");
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
        protected async Task ExportBudgetForecastCsv()
        {
            var csv = new StringBuilder();

            // Header
            csv.Append("Job Number,Job Name,Budget Amount,Costs As Of,Remaining");

            foreach (var month in MonthColumnsBudget)
            {
                csv.Append($",{month}");
            }

            csv.AppendLine();

            // Rows
            foreach (var r in filteredBudgetForecast)
            {
                csv.Append(
                    $"\"{r.JobNumber}\"," +
                    $"\"{r.JobName}\"," +
                    $"\"{PdfReportHelper.Money(r.TotalAmount)}\"," +
                    $"\"{PdfReportHelper.Money(r.CurrentAmount)}\"," +
                    $"\"{PdfReportHelper.Money(r.Remaining)}\""
                );

                foreach (var month in MonthColumnsBudget)
                {
                    var value = r.ForecastMonths.TryGetValue(month, out var amt)
                        ? amt
                        : 0;

                    csv.Append($",\"{PdfReportHelper.Money(value)}\"");
                }

                csv.AppendLine();
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            var base64 = Convert.ToBase64String(bytes);

            await JS.InvokeVoidAsync("downloadFileFromBase64", "BudgetForecast.csv", base64);
        }
        protected async Task ExportContractForecastCsv()
        {
            var csv = new StringBuilder();

            // Header
            csv.Append("Job Number,Job Name,Total Contract Amount,Billed As Of,Remaining");

            foreach (var month in MonthColumnsContract)
            {
                csv.Append($",{month}");
            }

            csv.AppendLine();

            // Rows
            foreach (var r in filteredContractForecast)
            {
                csv.Append(
                    $"\"{r.JobNumber}\"," +
                    $"\"{r.JobName}\"," +
                    $"\"{PdfReportHelper.Money(r.TotalAmount)}\"," +
                    $"\"{PdfReportHelper.Money(r.CurrentAmount)}\"," +
                    $"\"{PdfReportHelper.Money(r.Remaining)}\""
                );

                foreach (var month in MonthColumnsContract)
                {
                    var value = r.ForecastMonths.TryGetValue(month, out var amt)
                        ? amt
                        : 0;

                    csv.Append($",\"{PdfReportHelper.Money(value)}\"");
                }

                csv.AppendLine();
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            var base64 = Convert.ToBase64String(bytes);

            await JS.InvokeVoidAsync("downloadFileFromBase64", "ContractForecast.csv", base64);
        }
        protected async Task ExportBudgetForecastPdf()
        {
            var headers = new List<string>
    {
        "Job Number",
        "Job Name",
        "Budget Amount",
        "Costs As Of",
        "Remaining"
    };

            // add dynamic month headers
            headers.AddRange(MonthColumnsBudget);

            var rows = filteredBudgetForecast.Select(r =>
            {
                var row = new List<string>
        {
            r.JobNumber,
            r.JobName,
            PdfReportHelper.Money(r.TotalAmount),
            PdfReportHelper.Money(r.CurrentAmount),
            PdfReportHelper.Money(r.Remaining)
        };

                foreach (var month in MonthColumnsBudget)
                {
                    var value = r.ForecastMonths.TryGetValue(month, out var amt)
                        ? amt
                        : 0;

                    row.Add(PdfReportHelper.Money(value));
                }

                return row.ToArray();
            }).ToList();

            byte[] pdfBytes = PdfReportHelper.BuildPdf(
                "Cash Flow Budget Forecast",
                headers.ToArray(),
                rows,
                landscape: true
            );

            var base64 = Convert.ToBase64String(pdfBytes);

            await JS.InvokeVoidAsync(
                "downloadPDFFileFromBase64",
                "CashFlowBudgetForecast.pdf",
                "application/pdf",
                base64
            );
        }
        protected async Task ExportContractForecastPdf()
        {
            var headers = new List<string>
    {
        "Job Number",
        "Job Name",
        "Total Contract Amount",
        "Billed As Of",
        "Remaining"
    };

            headers.AddRange(MonthColumnsContract);

            var rows = filteredContractForecast.Select(r =>
            {
                var row = new List<string>
        {
            r.JobNumber,
            r.JobName,
            PdfReportHelper.Money(r.TotalAmount),
            PdfReportHelper.Money(r.CurrentAmount),
            PdfReportHelper.Money(r.Remaining)
        };

                foreach (var month in MonthColumnsContract)
                {
                    var value = r.ForecastMonths.TryGetValue(month, out var amt)
                        ? amt
                        : 0;

                    row.Add(PdfReportHelper.Money(value));
                }

                return row.ToArray();
            }).ToList();

            byte[] pdfBytes = PdfReportHelper.BuildPdf(
                "Cash Flow Contract Forecast",
                headers.ToArray(),
                rows,
                landscape: true
            );

            var base64 = Convert.ToBase64String(pdfBytes);

            await JS.InvokeVoidAsync(
                "downloadPDFFileFromBase64",
                "CashFlowContractForecast.pdf",
                "application/pdf",
                base64
            );
        }
        protected async Task ExportBudgetForecastExcel()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("CashFlowBudgetForecast");

            int col = 1;

            // Fixed headers
            worksheet.Cell(1, col++).Value = "Job Number";
            worksheet.Cell(1, col++).Value = "Job Name";
            worksheet.Cell(1, col++).Value = "Budget Amount";
            worksheet.Cell(1, col++).Value = "Costs As Of";
            worksheet.Cell(1, col++).Value = "Remaining";

            // Dynamic month headers
            foreach (var month in MonthColumnsBudget)
            {
                worksheet.Cell(1, col++).Value = month;
            }

            // Header styling
            var headerRange = worksheet.Range(1, 1, 1, col - 1);
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            int row = 2;

            foreach (var r in filteredBudgetForecast)
            {
                col = 1;

                worksheet.Cell(row, col++).Value = r.JobNumber;
                worksheet.Cell(row, col++).Value = r.JobName;
                worksheet.Cell(row, col++).Value = PdfReportHelper.Money(r.TotalAmount);
                worksheet.Cell(row, col++).Value = PdfReportHelper.Money(r.CurrentAmount);
                worksheet.Cell(row, col++).Value = PdfReportHelper.Money(r.Remaining);

                foreach (var month in MonthColumnsBudget)
                {
                    var value = r.ForecastMonths.TryGetValue(month, out var amt) ? amt : 0;
                    worksheet.Cell(row, col++).Value = PdfReportHelper.Money(value);
                }

                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            var base64 = Convert.ToBase64String(stream.ToArray());

            await JS.InvokeVoidAsync(
                "downloadExcelFileFromBase64",
                "CashFlowBudgetForecast.xlsx",
                base64
            );
        }
        protected async Task ExportContractForecastExcel()
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("CashFlowContractForecast");

            int col = 1;

            // Fixed headers
            worksheet.Cell(1, col++).Value = "Job Number";
            worksheet.Cell(1, col++).Value = "Job Name";
            worksheet.Cell(1, col++).Value = "Total Contract Amount";
            worksheet.Cell(1, col++).Value = "Billed As Of";
            worksheet.Cell(1, col++).Value = "Remaining";

            // Dynamic months
            foreach (var month in MonthColumnsContract)
            {
                worksheet.Cell(1, col++).Value = month;
            }

            // Header style
            var headerRange = worksheet.Range(1, 1, 1, col - 1);
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            int row = 2;

            foreach (var r in filteredContractForecast)
            {
                col = 1;

                worksheet.Cell(row, col++).Value = r.JobNumber;
                worksheet.Cell(row, col++).Value = r.JobName;
                worksheet.Cell(row, col++).Value = PdfReportHelper.Money(r.TotalAmount);
                worksheet.Cell(row, col++).Value = PdfReportHelper.Money(r.CurrentAmount);
                worksheet.Cell(row, col++).Value = PdfReportHelper.Money(r.Remaining);

                foreach (var month in MonthColumnsContract)
                {
                    var value = r.ForecastMonths.TryGetValue(month, out var amt) ? amt : 0;
                    worksheet.Cell(row, col++).Value = PdfReportHelper.Money(value);
                }

                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            var base64 = Convert.ToBase64String(stream.ToArray());

            await JS.InvokeVoidAsync(
                "downloadExcelFileFromBase64",
                "CashFlowContractForecast.xlsx",
                base64
            );
        }
    }
}
