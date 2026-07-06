using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen.Blazor;
using StowellCoApp.DTO;
using StowellCoApp.Services;
using System.Globalization;
using System.Text;

namespace StowellCoApp.Components.Pages.ReportingDashboard
{
    public class PrimeChangeListBase : ComponentBase
    {
        public RadzenDataGrid<StowellCoApp.DTO.PrimeChangeList> grid;
        public IEnumerable<StowellCoApp.DTO.PrimeChangeList> jobs = Array.Empty<StowellCoApp.DTO.PrimeChangeList>();
        public IEnumerable<StowellCoApp.DTO.PrimeChangeList> filteredJobs = Array.Empty<StowellCoApp.DTO.PrimeChangeList>();
        public IEnumerable<StowellCoApp.DTO.PrimeChangeList> pagedJobs = Array.Empty<StowellCoApp.DTO.PrimeChangeList>();
        public string searchTerm = string.Empty;
        public CancellationTokenSource cts = new();
        public string selectedPageSize = "10";
        public string[] pageSizes = new string[] { "10", "15", "20", "50", "All" };
        public int pageSize = 10;
        public int currentPage = 0;
        [Inject] IJSRuntime JS { get; set; } = default!;
        public IEnumerable<CostCodeRecord> _costCodeRecords { get; set; }
        [Inject]
        public ICostCodeService _costCodeService { get; set; }
        [Inject]
        public ILogger<PrimeChangeListBase> logger { get; set; }
        public int selectedProjectId;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    _costCodeRecords = await _costCodeService.GetAllUserJobs();
                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error loading jobs");
                }
            }
        }
        protected async void OnJobChanged(object value)
        {
            selectedProjectId = Convert.ToInt32(value);
            try
            {
                jobs = await _costCodeService.GetPrimeChangeList(selectedProjectId.ToString());
                filteredJobs = jobs;
                ApplyPaging();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading jobs");
            }
        }
        private void ApplyPaging()
        {
            pagedJobs = pageSize == 0
                ? filteredJobs
                : filteredJobs.Skip(currentPage * pageSize).Take(pageSize);

            grid?.Reload();
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

                jobs = await _costCodeService.GetPrimeChangeList(selectedProjectId.ToString());
                filteredJobs = jobs;

            }
            catch (TaskCanceledException) { }
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
        //protected async Task ExportPdf()
        //{
        //    try
        //    {
        //        // Example: create PDF bytes (replace with your real PDF generator)
        //        byte[] pdfBytes = GeneratePdfBytes();

        //        var base64 = Convert.ToBase64String(pdfBytes);

        //        await JS.InvokeVoidAsync(
        //            "downloadPDFFileFromBase64",
        //            "PrimeChangeListReport.pdf",
        //            "application/pdf",
        //            base64
        //        );
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
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

        //    AddCell(table, "Record#", true);
        //    AddCell(table, "Order#", true);
        //    AddCell(table, "Date", true);
        //    AddCell(table, "Description", true);
        //    AddCell(table, "Status", true);
        //    AddCell(table, "Requested", true);
        //    AddCell(table, "Approved", true);


        //    foreach (var r in filteredJobs)
        //    {
        //        AddCell(table, r.RecNum.ToString());
        //        AddCell(table, r.ChgNum);
        //        AddCell(table, r.ChgDate.ToString());
        //        AddCell(table, r.Description);
        //        AddCell(table, r.Status);
        //        AddCell(table, r.ReqAmt.ToString("N2"));
        //        AddCell(table, r.AppAmt.ToString("N2"));
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
        protected async Task ExportPdf()
        {
            var headers = new[]
            {
           "Record#",
            "Order#",
           "Date",
           "Description",
            "Status",
            "Requested",
            "Approved"
        };

            var rows = filteredJobs.Select(r => new string[]
 {
    r.RecNum.ToString(),
    r.ChgNum.ToString(),
    r.ChgDate.ToString(),
    r.Description,
    r.Status,
    PdfReportHelper.Money(r.ReqAmt),
    PdfReportHelper.Money(r.AppAmt)
 }).ToList();


            byte[] pdfBytes = PdfReportHelper.BuildPdf(
                "Prime Change List",
                headers,
                rows,
                landscape: true
            );

            var base64 = Convert.ToBase64String(pdfBytes);

            await JS.InvokeVoidAsync(
                "downloadPDFFileFromBase64",
                "PrimeChangeListReport.pdf",
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

            sb.AppendLine("Record#\tOrder#\tDate\tDescription\tStatus\tRequested\tApproved");

            // All rows
            foreach (var r in filteredJobs)
            {
                sb.AppendLine(
                    $"{r.RecNum}\t" +
                    $"{r.ChgNum}\t" +
                    $"{r.ChgDate}\t" +
                    $"{r.Description}\t" +
                    $"{r.Status}\t" +
                    $"{PdfReportHelper.Money(r.ReqAmt)}\t" +
                    $"{PdfReportHelper.Money(r.AppAmt)}" 
                     
                );
            }

            return sb.ToString();
        }

        protected async Task ExportExcel()

        {

            using var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add(" CashFlowToDateMultiple");

            // Header

            worksheet.Cell(1, 1).Value = "Record#";
            worksheet.Cell(1, 2).Value = "Order#";
            worksheet.Cell(1, 3).Value = "Date";
            worksheet.Cell(1, 4).Value = "Description";
            worksheet.Cell(1, 5).Value = "Status";
            worksheet.Cell(1, 6).Value = "Requested";
            worksheet.Cell(1, 7).Value = "Approved";
            // 🔹 Header styling (PDF-like)
            var headerRange = worksheet.Range(1, 1, 1, 7);
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            int row = 2;

            foreach (var r in filteredJobs)

            {
                worksheet.Cell(row, 1).Value = r.RecNum;
                worksheet.Cell(row, 2).Value = r.ChgNum;
                worksheet.Cell(row, 3).Value = r.ChgDate;
                worksheet.Cell(row, 4).Value = r.Description;
                worksheet.Cell(row, 5).Value = r.Status;
                worksheet.Cell(row, 6).Value = PdfReportHelper.Money(r.ReqAmt);
                worksheet.Cell(row, 7).Value = PdfReportHelper.Money(r.AppAmt);

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

                    "PrimeChangeListReport.xlsx",

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
            csv.AppendLine("Record#,Order#,Date,Description,Status,Requested,Approved");

            foreach (var r in filteredJobs)
            {
                csv.AppendLine(
                    $"{r.RecNum}," +
                    $"{r.ChgNum}," +
                    $"{r.ChgDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}," +
                    $"\"{r.Description}\"," +
                    $"{r.Status}," +
                    $"{PdfReportHelper.Money(r.ReqAmt)}," +
                    $"{PdfReportHelper.Money(r.AppAmt)}"
                );
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            var base64 = Convert.ToBase64String(bytes);
            await JS.InvokeVoidAsync("downloadFileFromBase64", "PrimeChangeListReport.csv", base64);
        }
    }
}
