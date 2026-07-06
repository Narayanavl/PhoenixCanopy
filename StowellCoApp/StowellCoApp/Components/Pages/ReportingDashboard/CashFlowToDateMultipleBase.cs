using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using NPOI.SS.Formula.Functions;
using Radzen;
using Radzen.Blazor;
using StowellCoApp.Components.Pages.ProjectManagement;
using StowellCoApp.Components.Pages.StowellAdmin;
using StowellCoApp.DTO;
using StowellCoApp.Services;
using System.Globalization;
using System.Text;

namespace StowellCoApp.Components.Pages.ReportingDashboard
{
    public class CashFlowToDateMultipleBase : ComponentBase
    {
        public readonly CultureInfo UsCulture = new("en-US");
        [Inject] IJSRuntime JS { get; set; } = default!;
        protected List<StatusModel> statusModels = new();
        protected IEnumerable<CostCodeRecord> jobsModel;
        protected IEnumerable<string> periodDropdown;
        protected string? selectedperiodDropdownId;
        // Selected value
        protected int? selectedStatusId;
        protected string? selectedJobIds;
        public List<string> selectedJobId { get; set; } = new();
        protected DateTime? StartDate { get; set; }
        protected DateTime? EndDate { get; set; }
        [Inject]
        public ICostCodeService _costCodeService { get; set; }
        protected IEnumerable<CashFlowRecord> cashFlowRecords = Enumerable.Empty<CashFlowRecord>();
        protected RadzenDataGrid<CashFlowRecord> grid;
        private ILogger<CashFlowToDateMultipleBase> logger { get; set; } = default!;
        protected CancellationTokenSource cts = new();
        protected string searchTerm = string.Empty;
        protected async Task OnStatusChanged(object value)
        {
            if (value == null)
            {
                selectedStatusId = 0;
            }
            selectedStatusId = Convert.ToInt32(value);
            jobsModel = await _costCodeService.GetAllUserJobsByStatus(selectedStatusId.Value);
            if (jobsModel == null)
            {
                selectedJobId = new List<string>();
            }
            else if (jobsModel != null && !jobsModel.Any())
            {
                selectedJobId = new List<string>();
            }
            Console.WriteLine("Selected Status: " + selectedStatusId);
            await InvokeAsync(StateHasChanged);
            // Example async call:
            // await LoadJobsForStatus(selectedStatusId.Value);
        }
        protected void OnJobNumberChange(object value)
        {
            var selectedObjects = value as IEnumerable<object>;

            // Check if selectedObjects is null or empty
            if (selectedObjects == null || !selectedObjects.Any())
            {
                selectedJobId = new List<string>(); // Clear selection
                StateHasChanged(); // Ensure UI is updated
                return;
            }
            selectedJobIds = string.Join(",", selectedJobId);
            // Debug log to check what was selected
            Console.WriteLine("Selected Cost Codes: " + string.Join(", ", selectedJobId));

            // Update the UI
            StateHasChanged();
        }

        public async Task GetReportData()
        {
            GetDatabyProjectId(selectedJobIds);
            // do something with result
        }
        public async void GetDatabyProjectId(string ProjectId)
        {
            CashFlowQueryInput input = new CashFlowQueryInput();
            input.recnum = ProjectId;
            input.StartDate = StartDate.Value;
            input.EndDate = EndDate.Value;
            // call your service, load data, update UI
            cashFlowRecords = await _costCodeService.CashFlowAllDataByMulitpleJobsAndPeriod(input);
            await InvokeAsync(StateHasChanged);
        }
        protected async override void OnInitialized()
        {
            var today = DateTime.Today;
            StartDate = new DateTime(today.Year, 1, 1);   // yyyy-01-01
            EndDate = today;
            // today's date
            try
            {
                statusModels = (await _costCodeService.GetAllStatusesData()).ToList();
                await InvokeAsync(StateHasChanged);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading statuses");
            }
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

                //if (ProjectId > 0)
                //{
                GetDatabyProjectId(Convert.ToString(selectedJobId));
                //}

            }
            catch (TaskCanceledException) { }
        }
        protected string selectedPageSize = "15"; // default
        protected string[] pageSizes = new string[] { "10", "15", "20", "50", "All" };
        protected int pageSize = 15;
        protected int currentPage = 0;
        protected void OnPageSizeChangedInput(object value)
        {
            selectedPageSize = value.ToString();

            if (selectedPageSize == "All")
                pageSize = cashFlowRecords.Count();
            else
                pageSize = Convert.ToInt32(selectedPageSize);

            currentPage = 0;

            grid?.GoToPage(0);
            grid?.Reload();
        }
        //protected async Task ExportPdf()
        //{
        //    try
        //    {
        //        // Example: create PDF bytes (replace with your real PDF generator)
        //        byte[] pdfBytes = GeneratePdfBytes();

        //        var base64 = Convert.ToBase64String(pdfBytes);

        //        await JS.InvokeVoidAsync(
        //            "downloadPDFFileFromBase64",
        //            "CashFlowReport.pdf",
        //            "application/pdf",
        //            base64
        //        );
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
        //       protected async Task ExportPdf()
        //       {
        //           var headers = new[]
        //           {
        //           "Rec Num",
        //           "Job Number",
        //           "Cash Collected",
        //           "Cash Paid",
        //           "Net Cash",
        //           "Invoice J2D",
        //           "A / R End Balance Today",
        //           "Job Costs To Date",
        //           "A/ P End Balance Today",
        //           "Cash Paid Out",
        //           "Net Cash In / Out"
        //       };

        //           var rows = cashFlowRecords.Select(r => new string[]
        //{
        //   r.RecNum.ToString(),
        //   r.JobNumber.ToString(),
        //   PdfReportHelper.Money(r.CashCollected),
        //   PdfReportHelper.Money(r.CashPaid),
        //   PdfReportHelper.Money(r.NetCash),
        //   PdfReportHelper.Money(r.InvoiceJ2D),
        //   PdfReportHelper.Money(r.A_REndBal_Today),
        //   PdfReportHelper.Money(r.jobcoststodate),
        //   PdfReportHelper.Money(r.A_PEndBal_Today),
        //   PdfReportHelper.Money(r.CashPaidOut),
        //   PdfReportHelper.Money(r.NetCashInOut)
        //}).ToList();


        //           byte[] pdfBytes = PdfReportHelper.BuildPdf(
        //               "Cash Flow To Date - Multiple",
        //               headers,
        //               rows,
        //               landscape: true
        //           );

        //           var base64 = Convert.ToBase64String(pdfBytes);

        //           await JS.InvokeVoidAsync(
        //               "downloadPDFFileFromBase64",
        //               "CashFlowToDateMultiple.pdf",
        //               "application/pdf",
        //               base64
        //           );
        //       }
        //byte[] GeneratePdfBytes()
        //{
        //    using var ms = new MemoryStream();

        //    var document = new Document(PageSize.A4, 36, 36, 36, 36);

        //    PdfWriter.GetInstance(document, ms); // ✅ CORRECT USAGE

        //    document.Open();

        //    var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
        //    var bodyFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

        //    document.Add(new Paragraph("Cash Flow To Date - Multiple", titleFont));
        //    document.Add(new Paragraph(" ")); // spacer

        //    PdfPTable table = new PdfPTable(7);
        //    table.WidthPercentage = 100;

        //    AddCell(table, "Rec Num", true);
        //    AddCell(table, "Job Number", true);
        //    AddCell(table, "Cash Collected", true);
        //    AddCell(table, "Cash Paid", true);
        //    AddCell(table, "Net Cash", true);
        //    AddCell(table, "Invoice J2D", true);
        //    AddCell(table, "A / R End Balance Today", true);
        //    AddCell(table, "Job Costs To Date", true);
        //    AddCell(table, "A/ P End Balance Today", true);
        //    AddCell(table, "Cash Paid Out", true);
        //    AddCell(table, "Net Cash In / Out", true);

        //    foreach (var r in cashFlowRecords)
        //    {
        //        AddCell(table, r.RecNum.ToString());
        //        AddCell(table, r.JobNumber);
        //        AddCell(table, r.CashCollected.ToString("N2"));
        //        AddCell(table, r.CashPaid.ToString("N2"));
        //        AddCell(table, r.NetCash.ToString("N2"));
        //        AddCell(table, r.InvoiceJ2D.ToString("N2"));
        //        AddCell(table, r.A_REndBal_Today.ToString("N2"));
        //        AddCell(table, r.jobcoststodate.ToString("N2"));
        //        AddCell(table, r.A_PEndBal_Today.ToString("N2"));
        //        AddCell(table, r.CashPaidOut.ToString("N2"));
        //        AddCell(table, r.NetCashInOut.ToString("N2"));
        //    }

        //    document.Add(table);
        //    document.Close();

        //    return ms.ToArray();
        //}

        //void AddCell(PdfPTable table, string text, bool isHeader = false)
        //{
        //    var font = isHeader
        //        ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9)
        //        : FontFactory.GetFont(FontFactory.HELVETICA, 9);

        //    var cell = new PdfPCell(new Phrase(text ?? string.Empty, font))
        //    {
        //        Padding = 5,
        //        HorizontalAlignment = Element.ALIGN_LEFT
        //    };

        //    table.AddCell(cell);
        //}


        //protected void CopyAllToClipboard()
        //{
        //    try
        //    {
        //        // Build the clipboard text synchronously
        //        var text = BuildAllRowsClipboardText();

        //        // Call JS immediately — no await
        //        JS.InvokeVoidAsync("copyTextToClipboard", text);
        //        JS.InvokeVoidAsync("showToast", "Copied " + cashFlowRecords.Count() + " rows to clipboard!");
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        //string BuildAllRowsClipboardText()
        //{
        //    var sb = new StringBuilder();

        //    // Header row

        //    sb.AppendLine("Rec Num\tJob Number\tCash Collected\tCash Paid\tNet Cash\tInvoice J2D\tA/R End Balance Today\tJob Costs To Date\tA/P End Balance Today\tCash Paid Out\tNet Cash In/Out");

        //    // All rows
        //    foreach (var r in cashFlowRecords)
        //    {
        //        sb.AppendLine(
        //            $"{r.RecNum}\t" +
        //            $"{r.JobNumber}\t" +
        //            $"{r.CashCollected.ToString("N2")}\t" +
        //            $"{r.CashPaid.ToString("N2")}\t" +
        //            $"{r.NetCash.ToString("N2")}\t" +
        //            $"{r.InvoiceJ2D.ToString("N2")}\t" +
        //            $"{r.A_REndBal_Today.ToString("N2")}\t" +
        //             $"{r.jobcoststodate.ToString("N2")}\t" +
        //              $"{r.A_PEndBal_Today.ToString("N2")}\t" +
        //               $"{r.CashPaidOut.ToString("N2")}\t" +
        //                $"{r.NetCashInOut.ToString("N2")}"
        //        );
        //    }

        //    return sb.ToString();
        //}

        //protected async Task ExportExcel()

        //{

        //    using var workbook = new XLWorkbook();

        //    var worksheet = workbook.Worksheets.Add(" CashFlowToDateMultiple");

        //    // Header

        //    worksheet.Cell(1, 1).Value = "Rec Num";
        //    worksheet.Cell(1, 2).Value = "Job Number";
        //    worksheet.Cell(1, 3).Value = "Cash Collected";
        //    worksheet.Cell(1, 4).Value = "Cash Paid";
        //    worksheet.Cell(1, 5).Value = "Net Cash";
        //    worksheet.Cell(1, 6).Value = "Invoice J2D";
        //    worksheet.Cell(1, 7).Value = "A / R End Balance Today";
        //    worksheet.Cell(1, 8).Value = "Job Costs To Date";
        //    worksheet.Cell(1, 9).Value = "A/ P End Balance Today";
        //    worksheet.Cell(1, 10).Value = "Cash Paid Out";
        //    worksheet.Cell(1, 11).Value = "Net Cash In / Out";

        //    int row = 2;

        //    foreach (var r in cashFlowRecords)

        //    {
        //        worksheet.Cell(row, 1).Value = r.RecNum;
        //        worksheet.Cell(row, 2).Value = r.JobNumber;
        //        worksheet.Cell(row, 3).Value = r.CashCollected.ToString("N2");
        //        worksheet.Cell(row, 4).Value = r.CashPaid.ToString("N2");
        //        worksheet.Cell(row, 5).Value = r.NetCash.ToString("N2");
        //        worksheet.Cell(row, 6).Value = r.InvoiceJ2D.ToString("N2");
        //        worksheet.Cell(row, 7).Value = r.A_REndBal_Today.ToString("N2");
        //        worksheet.Cell(row, 8).Value = r.jobcoststodate.ToString("N2");
        //        worksheet.Cell(row, 9).Value = r.A_PEndBal_Today.ToString("N2");
        //        worksheet.Cell(row, 10).Value = r.CashPaidOut.ToString("N2");
        //        worksheet.Cell(row, 11).Value = r.NetCashInOut.ToString("N2");

        //        row++;

        //    }

        //    worksheet.Columns().AdjustToContents();

        //    using var stream = new MemoryStream();

        //    workbook.SaveAs(stream);

        //    var base64 = Convert.ToBase64String(stream.ToArray());
        //    try
        //    {
        //        await JS.InvokeVoidAsync(

        //            "downloadExcelFileFromBase64",

        //            "CashFlowToDateMultiple.xlsx",

        //            base64

        //        );
        //    }
        //    catch (JSDisconnectedException)
        //    {
        //        // User left page – safe to ignore
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //}
        //protected async Task ExportCsvManual()
        //{
        //    var csv = new StringBuilder();
        //    csv.AppendLine(
        //        "Rec Num,Job Number,Cash Collected,Cash Paid,Net Cash,Invoice J2D," +
        //        "A/R End Balance Today,Job Costs To Date,A/P End Balance Today," +
        //        "Cash Paid Out,Net Cash In/Out"
        //    );

        //    foreach (var r in cashFlowRecords)
        //    {
        //        csv.AppendLine(
        //            $"\"{r.RecNum}\"," +
        //            $"\"{r.JobNumber}\"," +
        //            $"\"{r.CashCollected.ToString("0.00", CultureInfo.InvariantCulture)}\"," +
        //            $"\"{r.CashPaid.ToString("0.00", CultureInfo.InvariantCulture)}\"," +
        //            $"\"{r.NetCash.ToString("0.00", CultureInfo.InvariantCulture)}\"," +
        //            $"\"{r.InvoiceJ2D.ToString("0.00", CultureInfo.InvariantCulture)}\"," +
        //            $"\"{r.A_REndBal_Today.ToString("0.00", CultureInfo.InvariantCulture)}\"," +
        //            $"\"{r.jobcoststodate.ToString("0.00", CultureInfo.InvariantCulture)}\"," +
        //            $"\"{r.A_PEndBal_Today.ToString("0.00", CultureInfo.InvariantCulture)}\"," +
        //            $"\"{r.CashPaidOut.ToString("0.00", CultureInfo.InvariantCulture)}\"," +
        //            $"\"{r.NetCashInOut.ToString("0.00", CultureInfo.InvariantCulture)}\""
        //        );
        //    }

        //    var bytes = Encoding.UTF8.GetBytes(csv.ToString());
        //    var base64 = Convert.ToBase64String(bytes);
        //    await JS.InvokeVoidAsync("downloadFileFromBase64", "CashFlowToDateMultiple.csv", base64);
        //}
        protected async Task ExportPdf()
        {
            var headers = new[]
            {
            "Rec Num",
            "Job Number",
            "Cash Collected",
            "Cash Paid",
            "Net Cash",
            "Invoice J2D",
            "A / R End Balance Today",
            "Job Costs To Date",
            "A/ P End Balance Today",
            "Cash Paid Out",
            "Net Cash In / Out"
        };

            var rows = cashFlowRecords.Select(r => new string[]
 {
    r.RecNum.ToString(),
    r.JobNumber.ToString(),
    PdfReportHelper.Money(r.CashCollected),
    PdfReportHelper.Money(r.CashPaid),
    PdfReportHelper.Money(r.NetCash),
    PdfReportHelper.Money(r.InvoiceJ2D),
    PdfReportHelper.Money(r.A_REndBal_Today),
    PdfReportHelper.Money(r.jobcoststodate),
    PdfReportHelper.Money(r.A_PEndBal_Today),
    PdfReportHelper.Money(r.CashPaidOut),
    PdfReportHelper.Money(r.NetCashInOut)
 }).ToList();


            byte[] pdfBytes = PdfReportHelper.BuildPdf(
                "Cash Flow To Date",
                headers,
                rows,
                landscape: true
            );

            var base64 = Convert.ToBase64String(pdfBytes);

            await JS.InvokeVoidAsync(
                "downloadPDFFileFromBase64",
                "CashFlowToDateMultiple.pdf",
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
                JS.InvokeVoidAsync("showToast", "Copied " + cashFlowRecords.Count() + " rows to clipboard!");
            }
            catch (Exception ex)
            {

            }
        }

        string BuildAllRowsClipboardText()
        {
            var sb = new StringBuilder();

            // Header row

            sb.AppendLine("Rec Num\tJob Number\tCash Collected\tCash Paid\tNet Cash\tInvoice J2D\tA/R End Balance Today\tJob Costs To Date\tA/P End Balance Today\tCash Paid Out\tNet Cash In/Out");

            // All rows
            foreach (var r in cashFlowRecords)
            {
                sb.AppendLine(
                    $"{r.RecNum}\t" +
                    $"{r.JobNumber}\t" +
                    $"{PdfReportHelper.Money(r.CashCollected)}\t" +
                    $"{PdfReportHelper.Money(r.CashPaid)}\t" +
                    $"{PdfReportHelper.Money(r.NetCash)}\t" +
                    $"{PdfReportHelper.Money(r.InvoiceJ2D)}\t" +
                    $"{PdfReportHelper.Money(r.A_REndBal_Today)}\t" +
                     $"{PdfReportHelper.Money(r.jobcoststodate)}\t" +
                      $"{PdfReportHelper.Money(r.A_PEndBal_Today)}\t" +
                       $"{PdfReportHelper.Money(r.CashPaidOut)}\t" +
                        $"{PdfReportHelper.Money(r.NetCashInOut)}"
                );
            }

            return sb.ToString();
        }

        protected async Task ExportExcel()

        {

            using var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("Cash Flow Report");

            // Header

            worksheet.Cell(1, 1).Value = "Rec Num";
            worksheet.Cell(1, 2).Value = "Job Number";
            worksheet.Cell(1, 3).Value = "Cash Collected";
            worksheet.Cell(1, 4).Value = "Cash Paid";
            worksheet.Cell(1, 5).Value = "Net Cash";
            worksheet.Cell(1, 6).Value = "Invoice J2D";
            worksheet.Cell(1, 7).Value = "A / R End Balance Today";
            worksheet.Cell(1, 8).Value = "Job Costs To Date";
            worksheet.Cell(1, 9).Value = "A/ P End Balance Today";
            worksheet.Cell(1, 10).Value = "Cash Paid Out";
            worksheet.Cell(1, 11).Value = "Net Cash In / Out";
            // 🔹 Header styling (PDF-like)
            var headerRange = worksheet.Range(1, 1, 1, 11);
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            int row = 2;

            foreach (var r in cashFlowRecords)

            {
                worksheet.Cell(row, 1).Value = r.RecNum;
                worksheet.Cell(row, 2).Value = r.JobNumber;
                worksheet.Cell(row, 3).Value = PdfReportHelper.Money(r.CashCollected);
                worksheet.Cell(row, 4).Value = PdfReportHelper.Money(r.CashPaid);
                worksheet.Cell(row, 5).Value = PdfReportHelper.Money(r.NetCash);
                worksheet.Cell(row, 6).Value = PdfReportHelper.Money(r.InvoiceJ2D);
                worksheet.Cell(row, 7).Value = PdfReportHelper.Money(r.A_REndBal_Today);
                worksheet.Cell(row, 8).Value = PdfReportHelper.Money(r.jobcoststodate);
                worksheet.Cell(row, 9).Value = PdfReportHelper.Money(r.A_PEndBal_Today);
                worksheet.Cell(row, 10).Value = PdfReportHelper.Money(r.CashPaidOut);
                worksheet.Cell(row, 11).Value = PdfReportHelper.Money(r.NetCashInOut);

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

                    "CashFlowToDateMultiple.xlsx",

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
            csv.AppendLine(
                "Rec Num,Job Number,Cash Collected,Cash Paid,Net Cash,Invoice J2D," +
                "A/R End Balance Today,Job Costs To Date,A/P End Balance Today," +
                "Cash Paid Out,Net Cash In/Out"
            );

            foreach (var r in cashFlowRecords)
            {
                csv.AppendLine(
                    $"\"{r.RecNum}\"," +
                    $"\"{r.JobNumber}\"," +
                    $"\"{PdfReportHelper.Money(r.CashCollected)}\"," +
                    $"\"{PdfReportHelper.Money(r.CashPaid)}\"," +
                    $"\"{PdfReportHelper.Money(r.NetCash)}\"," +
                    $"\"{PdfReportHelper.Money(r.InvoiceJ2D)}\"," +
                    $"\"{PdfReportHelper.Money(r.A_REndBal_Today)}\"," +
                    $"\"{PdfReportHelper.Money(r.jobcoststodate)}\"," +
                    $"\"{PdfReportHelper.Money(r.A_PEndBal_Today)}\"," +
                    $"\"{PdfReportHelper.Money(r.CashPaidOut)}\"," +
                    $"\"{PdfReportHelper.Money(r.NetCashInOut)}\""
                );
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            var base64 = Convert.ToBase64String(bytes);
            await JS.InvokeVoidAsync("downloadFileFromBase64", "CashFlowToDateMultiple.csv", base64);
        }
    }

}