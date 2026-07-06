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
    public class ARInvoiceListBase : ComponentBase
    {
        [Inject] IJSRuntime JS { get; set; } = default!;
        public RadzenDataGrid<StowellCoApp.DTO.InvoicePayment> grid;
        public IEnumerable<StowellCoApp.DTO.InvoicePayment> jobs = Array.Empty<StowellCoApp.DTO.InvoicePayment>();
        public IEnumerable<StowellCoApp.DTO.InvoicePayment> filteredJobs = Array.Empty<StowellCoApp.DTO.InvoicePayment>();
        public IEnumerable<StowellCoApp.DTO.InvoicePayment> pagedJobs = Array.Empty<StowellCoApp.DTO.InvoicePayment>();
        public string searchTerm = string.Empty;
        public CancellationTokenSource cts = new();
        public string selectedPageSize = "10";
        public string[] pageSizes = new string[] { "10", "15", "20", "50", "All" };
        public int pageSize = 10;
        public int currentPage = 0;
        public IEnumerable<CostCodeRecord> _costCodeRecords { get; set; }
        [Inject]
        public ICostCodeService _costCodeService { get; set; }
        [Inject]
        public ILogger<ARInvoiceListBase> logger { get; set; }
        [Inject]
        public IJSRuntime jSRuntime { get; set; } = default!;
        public int selectedProjectId;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    _costCodeRecords = await _costCodeService.GetAllUserJobs();
                  // await Print();
                    StateHasChanged();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error loading jobs");
                }
            }
        }
        async Task Print()
        {
            await jSRuntime.InvokeVoidAsync("window.print");
        }
        protected async void OnJobChanged(object value)
        {
            selectedProjectId = Convert.ToInt32(value);
            try
            {
                jobs = await _costCodeService.GetARInvoice(selectedProjectId.ToString());
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

                jobs = await _costCodeService.GetARInvoice(selectedProjectId.ToString());
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
        //            "ARInvoiceList.pdf",
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

        //    document.Add(new Paragraph("AR Invoice List", titleFont));
        //    document.Add(new Paragraph(" ")); // spacer

        //    PdfPTable table = new PdfPTable(7);
        //    table.WidthPercentage = 100;

        //    AddCell(table, "Record#", true);
        //    AddCell(table, "Invoice#", true);
        //    AddCell(table, "Job", true);
        //    AddCell(table, "Date", true);
        //    AddCell(table, "Due Date", true);
        //    AddCell(table, "Invoice Total", true);
        //    AddCell(table, "Balance", true);

        //    foreach (var r in filteredJobs)
        //    {
        //        AddCell(table, r.RecNum.ToString());
        //        AddCell(table, r.JobName);
        //        AddCell(table, r.InvNum.ToString());
        //        AddCell(table, r.InvDate);
        //        AddCell(table, r.DueDate.ToString());
        //        AddCell(table, r.InvoiceTotal.ToString("N2"));
        //        AddCell(table, r.Balance.ToString("N2"));
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
            "Invoice#",
            "Job",
            "Date",
            "Due Date",
            "Invoice Total",
            "Balance"
        };

            var rows = filteredJobs.Select(r => new string[]
 {
    r.RecNum.ToString(),
    r.JobName.ToString(),
    r.InvNum.ToString(),
    r.InvDate,
  r.DueDate,
    PdfReportHelper.Money(r.InvoiceTotal),
    PdfReportHelper.Money(r.Balance)
 }).ToList();


            byte[] pdfBytes = PdfReportHelper.BuildPdf(
                "AR Invoice List",
                headers,
                rows,
                landscape: true
            );

            var base64 = Convert.ToBase64String(pdfBytes);

            await JS.InvokeVoidAsync(
                "downloadPDFFileFromBase64",
                "ARInvoiceList.pdf",
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

            sb.AppendLine("Record#\tInvoice#\tJob\tDate\tDue Date\tInvoice Total\tBalance");

            // All rows
            foreach (var r in filteredJobs)
            {
                sb.AppendLine(
                    $"{r.RecNum}\t" +
                    $"{r.JobName}\t" +
                    $"{r.InvNum}\t" +
                    $"{r.InvDate}\t" +
                    $"{r.DueDate}\t" +
                    $"{PdfReportHelper.Money(r.InvoiceTotal)}\t" +
                    $"{PdfReportHelper.Money(r.Balance)}\t" 

                );
            }

            return sb.ToString();
        }

        protected async Task ExportExcel()

        {

            using var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("AR Invoice List");

            // Header

            worksheet.Cell(1, 1).Value = "Record#";
            worksheet.Cell(1, 2).Value = "Invoice#";
            worksheet.Cell(1, 3).Value = "Job";
            worksheet.Cell(1, 4).Value = "Date";
            worksheet.Cell(1, 5).Value = "Due Date";
            worksheet.Cell(1, 6).Value = "Invoice Total";
            worksheet.Cell(1, 7).Value = "Balance";
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
                worksheet.Cell(row, 2).Value = r.JobName;
                worksheet.Cell(row, 3).Value = r.InvNum;
                worksheet.Cell(row, 4).Value = r.InvDate;
                worksheet.Cell(row, 5).Value = r.DueDate;
                worksheet.Cell(row, 6).Value = PdfReportHelper.Money(r.InvoiceTotal);
                worksheet.Cell(row, 7).Value = PdfReportHelper.Money(r.Balance);
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

                    "ARInvoiceList.xlsx",

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
                "Record#,Invoice#,Job,Date,Due Date,Invoice Total,Balance"
            );

            foreach (var r in filteredJobs)
            {
                csv.AppendLine(
                    $"\"{r.RecNum}\"," +
                    $"\"{r.JobName}\"," +
                    $"\"{r.InvNum}\"," +
                    $"\"{r.InvDate}\"," +
                    $"\"{r.DueDate}\"," +
                    $"\"{PdfReportHelper.Money(r.InvoiceTotal)}\"," +
                    $"\"{PdfReportHelper.Money(r.Balance)}\"," 
                    
                );
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            var base64 = Convert.ToBase64String(bytes);
            await JS.InvokeVoidAsync("downloadFileFromBase64", "ARInvoiceList.csv", base64);
        }
    }
}
