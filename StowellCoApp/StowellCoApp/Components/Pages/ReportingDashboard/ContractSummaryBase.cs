using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Radzen.Blazor;
using StowellCoApp.DTO;
using StowellCoApp.Models;
using StowellCoApp.Services;
using System.Globalization;
using System.Text;

namespace StowellCoApp.Components.Pages.ReportingDashboard
{
    public class ContractSummaryBase : ComponentBase
    {
        public RadzenDataGrid<StowellCoApp.DTO.ContractSummary> grid;
        public IEnumerable<StowellCoApp.DTO.ContractSummary> jobs = Array.Empty<StowellCoApp.DTO.ContractSummary>();
        public IEnumerable<StowellCoApp.DTO.ContractSummary> filteredJobs = Array.Empty<StowellCoApp.DTO.ContractSummary>();
        public IEnumerable<StowellCoApp.DTO.ContractSummary> pagedJobs = Array.Empty<StowellCoApp.DTO.ContractSummary>();
        public string searchTerm = string.Empty;
        [Inject] IJSRuntime JS { get; set; } = default!;
        public CancellationTokenSource cts = new();
        public string selectedPageSize = "10";
        public string[] pageSizes = new string[] { "10", "15", "20", "50", "All" };
        public int pageSize = 10;
        public int currentPage = 0;
        public IEnumerable<CostCodeRecord> _costCodeRecords { get; set; }
        [Inject]
        public ICostCodeService _costCodeService { get; set; }
        [Inject]
        public ILogger<ContractSummaryBase> logger { get; set; }
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
                jobs = await _costCodeService.GetContractSummaryData(selectedProjectId.ToString());
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

                jobs = await _costCodeService.GetContractSummaryData(selectedProjectId.ToString());
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
        //            "ContractSummary.pdf",
        //            "application/pdf",
        //            base64
        //        );
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
        byte[] GeneratePdfBytes()
        {
            using var ms = new MemoryStream();

            var document = new Document(PageSize.A4, 36, 36, 36, 36);

            PdfWriter.GetInstance(document, ms); // ✅ CORRECT USAGE

            document.Open();

            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
            var bodyFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);

            document.Add(new Paragraph("Contract Summary", titleFont));
            document.Add(new Paragraph(" ")); // spacer

            PdfPTable table = new PdfPTable(7);
            table.WidthPercentage = 100;

            AddCell(table, "Original Contract", true);
            AddCell(table, "Changes To Date", true);
            AddCell(table, "New Contract", true);
            AddCell(table, "Invoiced To Date", true);
            AddCell(table, "Balance on Contract", true);
            AddCell(table, "Balance With Tax", true);
            AddCell(table, "Retained", true);
            AddCell(table, "Net Due With Tax", true);

            foreach (var r in filteredJobs)
            {
                AddCell(table, r.OriginalContractAmount.ToString("N2"));
                AddCell(table, r.ChangesToDate.ToString("N2"));
                AddCell(table, r.NewContract.ToString("N2"));
                AddCell(table, r.InvoicedToDate.ToString("N2"));
                AddCell(table, r.BalanceOnContract.ToString("N2"));
                AddCell(table, "$0.00");
                AddCell(table, "$0.00");
                AddCell(table, "$0.00");
            }

            document.Add(table);
            document.Close();

            return ms.ToArray();
        }

        void AddCell(PdfPTable table, string text, bool isHeader = false)
        {
            var font = isHeader
                ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9)
                : FontFactory.GetFont(FontFactory.HELVETICA, 9);

            var cell = new PdfPCell(new Phrase(text ?? string.Empty, font))
            {
                Padding = 5,
                HorizontalAlignment = Element.ALIGN_LEFT
            };

            table.AddCell(cell);
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

            sb.AppendLine("Original Contract\tChanges To Date\tNew Contract\tInvoiced To Date\tBalance on Contract\tBalance With Tax\tRetained\tNet Due With Tax");

            // All rows
            foreach (var r in filteredJobs)
            {
                sb.AppendLine(
                    $"{PdfReportHelper.Money(r.OriginalContractAmount)}\t" +
                    $"{PdfReportHelper.Money(r.ChangesToDate)}\t" +
                    $"{PdfReportHelper.Money(r.NewContract)}\t" +
                    $"{PdfReportHelper.Money(r.InvoicedToDate)}\t" +
                    $"{PdfReportHelper.Money(r.BalanceOnContract)}\t" +
                    $"{PdfReportHelper.Money(0)}\t" +
                    $"{PdfReportHelper.Money(0)}\t" +
                    $"{PdfReportHelper.Money(0)}\t" 

                );
            }

            return sb.ToString();
        }

        protected async Task ExportExcel()

        {

            using var workbook = new XLWorkbook();

            var worksheet = workbook.Worksheets.Add("CashFlowToDateMultiple");

            // Header

            worksheet.Cell(1, 1).Value = "Original Contract";
            worksheet.Cell(1, 2).Value = "Changes To Date";
            worksheet.Cell(1, 3).Value = "New Contract";
            worksheet.Cell(1, 4).Value = "Invoiced To Date";
            worksheet.Cell(1, 5).Value = "Balance on Contract";
            worksheet.Cell(1, 6).Value = "Balance With Tax";
            worksheet.Cell(1, 7).Value = "Retained";
            worksheet.Cell(1, 8).Value = "Net Due With Tax";
            // 🔹 Header styling (PDF-like)
            var headerRange = worksheet.Range(1, 1, 1, 8);
            headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            int row = 2;

            foreach (var r in filteredJobs)

            {
                worksheet.Cell(row, 1).Value = PdfReportHelper.Money(r.OriginalContractAmount);
                worksheet.Cell(row, 2).Value = PdfReportHelper.Money(r.ChangesToDate);
                worksheet.Cell(row, 3).Value = PdfReportHelper.Money(r.NewContract);
                worksheet.Cell(row, 4).Value = PdfReportHelper.Money(r.InvoicedToDate);
                worksheet.Cell(row, 5).Value = PdfReportHelper.Money(r.BalanceOnContract);
                worksheet.Cell(row, 6).Value = PdfReportHelper.Money(0);
                worksheet.Cell(row, 7).Value = PdfReportHelper.Money(0);
                worksheet.Cell(row, 8).Value = PdfReportHelper.Money(0);
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

                    "ContractSummary.xlsx",

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
                "Original Contract,Changes To Date,New Contract,Invoiced To Date,Balance on Contract,"+
                "Balance With Tax,Retained,Net Due With Tax"
            );

            foreach (var r in filteredJobs)
            {
                csv.AppendLine(
                    $"\"{PdfReportHelper.Money(r.OriginalContractAmount)}\"," +
                    $"\"{PdfReportHelper.Money(r.ChangesToDate)}\"," +
                    $"\"{PdfReportHelper.Money(r.NewContract)}\"," +
                    $"\"{PdfReportHelper.Money(r.InvoicedToDate)}\"," +
                    $"\"{PdfReportHelper.Money(r.BalanceOnContract)}\"," +
                    $"\"{PdfReportHelper.Money(0)}\"," +
                    $"\"{PdfReportHelper.Money(0)}\"," +
                    $"\"{PdfReportHelper.Money(0)}\"," 
                
                );
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            var base64 = Convert.ToBase64String(bytes);
            await JS.InvokeVoidAsync("downloadFileFromBase64", "ContractSummary.csv", base64);
        }
        protected async Task ExportPdf()
        {
            var headers = new[]
            {
        "Original Contract",
        "Changes To Date",
        "New Contract",
        "Invoiced To Date",
        "Balance on Contract",
        "Balance With Tax",
        "Retained",
        "Net Due With Tax"
    };

            var rows = filteredJobs.Select(r => new[]
            {
        PdfReportHelper.Money(r.OriginalContractAmount),
        PdfReportHelper.Money(r.ChangesToDate),
        PdfReportHelper.Money(r.NewContract),
        PdfReportHelper.Money(r.InvoicedToDate),
        PdfReportHelper.Money(r.BalanceOnContract),
        PdfReportHelper.Money(0),
        PdfReportHelper.Money(0),
        PdfReportHelper.Money(0)
    }).ToList();

            byte[] pdfBytes = PdfReportHelper.BuildPdf(
                "Contract Summary",
                headers,
                rows,
                landscape: true
            );

            var base64 = Convert.ToBase64String(pdfBytes);

            await JS.InvokeVoidAsync(
                "downloadPDFFileFromBase64",
                "ContractSummary.pdf",
                "application/pdf",
                base64
            );
        }
        

    }
    //public static class PdfReportHelper
    //{
    //    public static byte[] BuildPdf(
    //        string title,
    //        string[] headers,
    //        List<string[]> rows,
    //        bool landscape = true
    //    )
    //    {
    //        using var ms = new MemoryStream();

    //        var pageSize = landscape ? PageSize.A4.Rotate() : PageSize.A4;
    //        var document = new Document(pageSize, 36, 36, 36, 36);

    //        PdfWriter.GetInstance(document, ms);
    //        document.Open();

    //        // Title
    //        var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
    //        document.Add(new Paragraph(title, titleFont));
    //        document.Add(new Paragraph(" "));

    //        // Table
    //        PdfPTable table = new PdfPTable(headers.Length)
    //        {
    //            WidthPercentage = 100
    //        };

    //        table.SetWidths(Enumerable.Repeat(1f, headers.Length).ToArray());

    //        // Headers
    //        foreach (var h in headers)
    //            AddCell(table, h, true);

    //        // Rows
    //        foreach (var row in rows)
    //            foreach (var cell in row)
    //                AddCell(table, cell);

    //        document.Add(table);
    //        document.Close();

    //        return ms.ToArray();
    //    }

    //    private static void AddCell(PdfPTable table, string text, bool isHeader = false)
    //    {
    //        var font = isHeader
    //            ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9)
    //            : FontFactory.GetFont(FontFactory.HELVETICA, 9);

    //        var cell = new PdfPCell(new Phrase(text ?? string.Empty, font))
    //        {
    //            Padding = 5,
    //            HorizontalAlignment = Element.ALIGN_CENTER,
    //            VerticalAlignment = Element.ALIGN_MIDDLE,
    //            NoWrap = isHeader,
    //            BackgroundColor = isHeader ? BaseColor.LIGHT_GRAY : BaseColor.WHITE
    //        };

    //        table.AddCell(cell);
    //    }
    //    public static string Money(decimal value)
    //    {
    //        return $"${value.ToString("0.00", CultureInfo.InvariantCulture)}";
    //    }
    //}
    public static class PdfReportHelper
    {
        public static byte[] BuildPdf(
            string title,
            string[] headers,
            List<string[]> rows,
            bool landscape = true
        )
        {
            using var ms = new MemoryStream();

            var pageSize = landscape ? PageSize.A4.Rotate() : PageSize.A4;
            var document = new Document(pageSize, 36, 36, 36, 36);

            PdfWriter.GetInstance(document, ms);
            document.Open();

            // Title
            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14);
            document.Add(new Paragraph(title, titleFont));
            document.Add(new Paragraph(" "));

            // Table
            PdfPTable table = new PdfPTable(headers.Length)
            {
                WidthPercentage = 100
            };

            // 🔹 Smart default widths (long headers get more space)
            table.SetWidths(GetAutoColumnWidths(headers));

            // Headers
            foreach (var h in headers)
                AddCell(table, h, true);

            // Rows
            foreach (var row in rows)
                foreach (var cell in row)
                    AddCell(table, cell);

            document.Add(table);
            document.Close();

            return ms.ToArray();
        }

        // 🔹 Auto-size columns based on header length
        private static float[] GetAutoColumnWidths(string[] headers)
        {
            return headers
                .Select(h => Math.Max(1.2f, h.Length / 6f))
                .ToArray();
        }

        private static void AddCell(PdfPTable table, string text, bool isHeader = false)
        {
            var font = isHeader
                ? FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8)
                : FontFactory.GetFont(FontFactory.HELVETICA, 9);

            var cell = new PdfPCell(new Phrase(text ?? string.Empty, font))
            {
                Padding = 5,
                HorizontalAlignment = Element.ALIGN_CENTER,
                VerticalAlignment = Element.ALIGN_MIDDLE,
                BackgroundColor = isHeader ? BaseColor.LIGHT_GRAY : BaseColor.WHITE
                // ❌ No NoWrap — allows wrapping globally
            };

            table.AddCell(cell);
        }

        public static string Money(decimal value)
        {
            return $"${value.ToString("0.00", CultureInfo.InvariantCulture)}";
        }
    }

}
