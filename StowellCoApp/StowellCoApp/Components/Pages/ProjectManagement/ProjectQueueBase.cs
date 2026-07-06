using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using StowellCoApp.Common;
using StowellCoApp.DTO;
using StowellCoApp.Models;
using StowellCoApp.Services;
using System.Globalization;
using System.Net.Http;
using static StowellCoApp.Components.ColumnChart1;

namespace StowellCoApp.Components.Pages.ProjectManagement
{
    public class ProjectQueueBase :ComponentBase
    {
        public readonly CultureInfo UsCulture = new("en-US");

        [Inject]
        public ICostCodeService CostCode { get; set; }
        [Inject]
        private ILogger<ProjectQueueBase> logger { get; set; } = default!;
        protected RadzenDataGrid<CostCodeRecord> grid;
        //protected IEnumerable<CostCodeRecord> filteredJobs;
        private IEnumerable<CostCodeRecord> codes;
        protected IEnumerable<CostCodeRecord> filteredCostCodes;
        protected string searchTerm = string.Empty;
        private IEnumerable<CostCodeRecord> pagedCodes;
        private CancellationTokenSource cts = new();
        protected RadzenDataGrid<BidItem> bidItemGrid;
        protected IEnumerable<BidItem> filteredBidItem;
        private IEnumerable<BidItem> bidItems;
        private IEnumerable<BidItem> bidItemFiltered;
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender) // important to avoid infinite loops
            {
                try
                {
                    logger.LogInformation("OnAfterRenderAsync start");
                    codes = await CostCode.GetCostCodeRecords();
                    filteredBidItem = await CostCode.GetBids();
                    logger.LogInformation("OnAfterRenderAsync end");
                    logger.LogInformation("filteredJobs start");
                    filteredCostCodes = codes;
                    logger.LogInformation($"filteredJobs end{filteredCostCodes.Count()}");
                    StateHasChanged(); // optional, usually not needed in firstRender
                }
                catch (Exception ex)
                {
                    logger.LogInformation($"Error loading jobs: {ex.Message}");
                    Console.WriteLine($"Error loading jobs: {ex.Message}");
                }
            }
        }
        protected async Task OnInputChanged(ChangeEventArgs args)
        {
            searchTerm = args.Value?.ToString() ?? string.Empty;

            // Cancel previous debounce
            cts.Cancel();
            cts = new CancellationTokenSource();
            var token = cts.Token;

            try
            {
                await Task.Delay(300, token); // debounce 300ms

                if (token.IsCancellationRequested) return;

                FilterCostCodes();
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
        }

        protected void FilterCostCodes()
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                filteredCostCodes = codes;
            }
            else
            {
                filteredCostCodes = codes.Where(j =>
                     j.RecNum.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (j.JobNumber?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (j.Address?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (j.City?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (j.State?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)||
                     (j.ZipCode?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            grid.Reload();
        }

        protected string selectedPageSize = "10"; // default
        protected string[] pageSizes = new string[] { "10", "20", "50", "All" };
        protected int pageSize = 10;
        private int currentPage = 0;
        protected void OnPageSizeChanged(object value)
        {
            selectedPageSize = value.ToString();

            if (selectedPageSize == "All")
            {
                // Show all records
                pageSize = filteredCostCodes?.Count() ?? 0;
            }
            else
            {
                pageSize = Convert.ToInt32(selectedPageSize);
            }

            currentPage = 0;

            // Refresh the grid and pager
            grid.GoToPage(0);
            grid.Reload();
        }

        protected void ApplyPaging()
        {
            if (pageSize == 0)
            {
                pagedCodes = filteredCostCodes;
            }
            else
            {
                pagedCodes = filteredCostCodes
                    .Skip(currentPage * pageSize)
                    .Take(pageSize);
            }
            grid?.Reload();
        }
        protected async Task OnLoadData(LoadDataArgs args)
        {
            // Handle search
            filteredCostCodes = string.IsNullOrWhiteSpace(searchTerm)
                ? codes
                : codes.Where(j =>
                    j.RecNum.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (j.JobNumber?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (j.Address?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (j.City?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (j.State?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                     (j.ZipCode?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    );

            // Handle paging
            pagedCodes = pageSize == 0
                ? filteredCostCodes
                : filteredCostCodes.Skip(args.Skip ?? 0).Take(args.Top ?? pageSize);

           // StateHasChanged();
        }
    }
}
