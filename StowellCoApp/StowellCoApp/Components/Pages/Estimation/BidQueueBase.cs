using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using StowellCoApp.Components.Pages.ProjectManagement;
using StowellCoApp.DTO;
using StowellCoApp.Services;
using System.Globalization;

namespace StowellCoApp.Components.Pages.Estimation
{
    public class BidQueueBase : ComponentBase
    {
        public readonly CultureInfo UsCulture = new("en-US");

        [Inject]
        public IEstimationService estimationService { get; set; }
        [Inject]
        private ILogger<BidQueueBase> logger { get; set; } = default!;
        protected RadzenDataGrid<BidItem> openBidQueueGrid;
        protected RadzenDataGrid<BidItem> closedBidQueueGrid;
        protected RadzenDataGrid<BidItem> pendingBidQueueGrid;
        protected IEnumerable<BidItem> filteredOpenBids;
        protected IEnumerable<BidItem> openBids;
        protected IEnumerable<BidItem> filteredClosedBids;
        protected IEnumerable<BidItem> closedBids;
        protected IEnumerable<BidItem> filteredPendingBids;
        protected IEnumerable<BidItem> pendingBids;
        protected string searchTerm = string.Empty;
        private IEnumerable<BidItem> pagedCodes;
        private CancellationTokenSource cts = new();
        protected bool isLoading = true;
        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            try
            {
                openBids = await estimationService.GetOpenBids();
                filteredOpenBids = openBids;
                closedBids = await estimationService.GetClosedBids();
                filteredClosedBids = closedBids;
                pendingBids = await estimationService.GetPendingBids();
                filteredPendingBids = pendingBids;
                StateHasChanged(); // optional, usually not needed in firstRender
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Error loading jobs: {ex.Message}");
                Console.WriteLine($"Error loading jobs: {ex.Message}");
            }
            finally
            {
                isLoading = false;
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

                FilterBidQueues();
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
        }

        protected void FilterBidQueues()
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                filteredOpenBids = openBids;
            }
            else
            {
                filteredOpenBids = openBids.Where(j =>
                     j.Address.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                   // (j.BidDate?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (j.JobID?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (j.BidStatus?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) 
                  //  (j.ID?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            openBidQueueGrid.Reload();
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
                pageSize = filteredOpenBids?.Count() ?? 0;
            }
            else
            {
                pageSize = Convert.ToInt32(selectedPageSize);
            }

            currentPage = 0;

            // Refresh the grid and pager
            openBidQueueGrid.GoToPage(0);
            openBidQueueGrid.Reload();
        }

        protected void ApplyPaging()
        {
            if (pageSize == 0)
            {
                pagedCodes = filteredOpenBids;
            }
            else
            {
                pagedCodes = filteredOpenBids
                    .Skip(currentPage * pageSize)
                    .Take(pageSize);
            }
            openBidQueueGrid?.Reload();
        }
        protected async Task OnLoadData(LoadDataArgs args)
        {
            // Handle search
            filteredOpenBids = string.IsNullOrWhiteSpace(searchTerm)
                ? openBids
                : openBids.Where(j =>
                      j.Address.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    // (j.BidDate?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (j.JobID?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (j.BidStatus?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                //  (j.ID?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                );

            // Handle paging
            pagedCodes = pageSize == 0
                ? filteredOpenBids
                : filteredOpenBids.Skip(args.Skip ?? 0).Take(args.Top ?? pageSize);

            // StateHasChanged();
        }
    }
}
