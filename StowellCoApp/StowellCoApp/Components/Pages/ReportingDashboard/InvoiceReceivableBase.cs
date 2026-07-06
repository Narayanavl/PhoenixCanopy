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
    public class InvoiceReceivableBase : ComponentBase
    {
        public RadzenDataGrid<StowellCoApp.DTO.InvoiceReceivable> grid;
        public IEnumerable<StowellCoApp.DTO.InvoiceReceivable> jobs = Array.Empty<StowellCoApp.DTO.InvoiceReceivable>();
        public IEnumerable<StowellCoApp.DTO.InvoiceReceivable> filteredJobs = Array.Empty<StowellCoApp.DTO.InvoiceReceivable>();
        public IEnumerable<StowellCoApp.DTO.InvoiceReceivable> pagedJobs = Array.Empty<StowellCoApp.DTO.InvoiceReceivable>();
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
        public ILogger<InvoiceReceivableBase> logger { get; set; }
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
                jobs = await _costCodeService.GetInvoiceReceivable(selectedProjectId.ToString());
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

                jobs = await _costCodeService.GetInvoiceReceivable(selectedProjectId.ToString());
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
        protected async Task ExportPdf()
        {
            var headers = new[]
            {

            "Record#",
        "Invoice#",
        "Invoice Date",
        "Description",
        "Due Date",
        "Requested",
        "Invoice Total",
        "Paid",
"Disc/Cred",
                "Balance",
                "Retention",
                "Net Due"
    };

            var rows = filteredJobs.Select(r => new[]
            {
        r.RecNum.ToString(),
             r.InvNum,
             r.InvDate.ToString(),
             r.Description,
             r.DueDate,
             r.Status.ToString(),
             PdfReportHelper.Money(r.InvTtl),
             PdfReportHelper.Money(r.AmtPad),
             PdfReportHelper.Money(r.DiscCred),
             PdfReportHelper.Money(r.InvBal),
             PdfReportHelper.Money(r.Retain),
             PdfReportHelper.Money(r.InvNet)

        }).ToList();

            byte[] pdfBytes = PdfReportHelper.BuildPdf(
                "Invoice Receivable",
                headers,
                rows,
                landscape: true
            );

            var base64 = Convert.ToBase64String(pdfBytes);

            await JS.InvokeVoidAsync(
                "downloadPDFFileFromBase64",
                "InvoiceReceivable.pdf",
                "application/pdf",
                base64
            );
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
        //            "InvoiceReceivable.pdf",
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

        //    document.Add(new Paragraph("Invoice Receivable", titleFont));
        //    document.Add(new Paragraph(" ")); // spacer

        //    PdfPTable table = new PdfPTable(7);
        //    table.WidthPercentage = 100;

        //    AddCell(table, "Record#", true);
        //    AddCell(table, "Invoice#", true);
        //    AddCell(table, "Invoice Date", true);
        //    AddCell(table, "Description", true);
        //    AddCell(table, "Due Date", true);
        //    AddCell(table, "Requested", true);
        //    AddCell(table, "Invoice Total", true);
        //    AddCell(table, "Paid", true); 
        //    AddCell(table, "Disc/Cred", true); 
        //    AddCell(table, "Balance", true);
        //    AddCell(table, "Retention", true);
        //    AddCell(table, "Net Due", true);

        //    foreach (var r in filteredJobs)
        //    {
        //        AddCell(table, r.RecNum.ToString());
        //        AddCell(table, r.InvNum);
        //        AddCell(table, r.InvDate.ToString());
        //        AddCell(table, r.Description);
        //        AddCell(table, r.DueDate);
        //        AddCell(table, r.Status.ToString());
        //        AddCell(table, r.InvTtl.ToString("N2"));
        //        AddCell(table, r.AmtPad.ToString("N2"));
        //        AddCell(table, r.DiscCred.ToString("N2"));
        //        AddCell(table, r.InvBal.ToString("N2"));
        //        AddCell(table, r.Retain.ToString("N2"));
        //        AddCell(table, r.InvNet.ToString("N2"));
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

            sb.AppendLine("Record#\tInvoice#\tInvoice Date\tDescription\tDue Date\tRequested\tInvoice Total\tPaid\tDisc/Cred\tBalance\tRetention\tNet Due");

            // All rows
            foreach (var r in filteredJobs)
            {
                sb.AppendLine(
                    $"{r.RecNum}\t" +
                    $"{r.InvNum}\t" +
                    $"{r.InvDate}\t" +
                    $"{r.Description}\t" +
                    $"{r.DueDate}\t" +
                    $"{r.Status}\t" +
                    $"{PdfReportHelper.Money(r.InvTtl)}\t" +
                    $"{PdfReportHelper.Money(r.AmtPad)}\t"+
                    $"{PdfReportHelper.Money(r.DiscCred)}\t"+
                    $"{PdfReportHelper.Money(r.InvBal)}\t"+
                    $"{PdfReportHelper.Money(r.Retain)}\t"+
                     $"{PdfReportHelper.Money(r.InvNet)}\t"

                );
            }

            return sb.ToString();
        }

        protected async Task ExportExcel()

        {

            using var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("CashFlowToDateMultiple");

            // Header

            worksheet.Cell(1, 1).Value = "Record#";
            worksheet.Cell(1, 2).Value = "Invoice#";
            worksheet.Cell(1, 3).Value = "Invoice Date";
            worksheet.Cell(1, 4).Value = "Description";
            worksheet.Cell(1, 5).Value = "Due Date";
            worksheet.Cell(1, 6).Value = "Requested";
            worksheet.Cell(1, 7).Value = "Invoice Total";
            worksheet.Cell(1, 8).Value = "Paid";
            worksheet.Cell(1, 9).Value = "Disc/Cred";
            worksheet.Cell(1, 10).Value = "Balance";
            worksheet.Cell(1, 11).Value = "Retention";
            worksheet.Cell(1, 12).Value = "Net Due";

            // 🔹 Header styling (PDF-like)
            var headerRange = worksheet.Range(1, 1, 1, 12);
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            int row = 2;

            foreach (var r in filteredJobs)

            {
                worksheet.Cell(row, 1).Value = r.RecNum;
                worksheet.Cell(row, 2).Value = r.InvNum;
                worksheet.Cell(row, 3).Value = r.InvDate;
                worksheet.Cell(row, 4).Value = r.Description;
                worksheet.Cell(row, 5).Value = r.DueDate;
                worksheet.Cell(row, 6).Value = r.Status;
                worksheet.Cell(row, 7).Value = PdfReportHelper.Money(r.InvTtl);
                worksheet.Cell(row, 8).Value = PdfReportHelper.Money(r.AmtPad);
                worksheet.Cell(row, 9).Value = PdfReportHelper.Money(r.DiscCred);
                worksheet.Cell(row, 10).Value = PdfReportHelper.Money(r.InvBal);
                worksheet.Cell(row, 11).Value = PdfReportHelper.Money(r.Retain);
                worksheet.Cell(row, 12).Value = PdfReportHelper.Money(r.InvNet);
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

                    "InvoiceReceivable.xlsx",

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
                "Record#,Invoice#,Invoice Date,Description,Due Date,Status," +
                "Invoice Total,Paid,Disc/Cred,Balance,Retention,Net Due"
            );

            foreach (var r in filteredJobs)
            {
                csv.AppendLine(
                    $"\"{r.RecNum}\"," +
                    $"\"{r.InvNum}\"," +
                    $"\"{r.InvDate}\"," +
                    $"\"{r.Description}\"," +
                    $"\"{r.DueDate}\"," +
                    $"\"{r.Status}\"," +
                    $"\"{PdfReportHelper.Money(r.InvTtl)}\"," +
                    $"\"{PdfReportHelper.Money(r.AmtPad)}\"," +
                    $"\"{PdfReportHelper.Money(r.DiscCred)}\"," +
                    $"\"{PdfReportHelper.Money(r.InvBal)}\"," +
                    $"\"{PdfReportHelper.Money(r.Retain)}\"," +
                    $"\"{PdfReportHelper.Money(r.InvNet)}\""
                );
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            var base64 = Convert.ToBase64String(bytes);
            await JS.InvokeVoidAsync("downloadFileFromBase64", "InvoiceReceivable.csv", base64);
        }
    }
}
