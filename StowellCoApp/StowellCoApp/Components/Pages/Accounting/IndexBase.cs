using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using StowellCoApp.DTO;
using StowellCoApp.Services;

namespace StowellCoApp.Components.Pages.Accounting
{
    public class IndexBase : ComponentBase
    {
        [Inject]
        public IAccountingService accountingService { get; set; }
        [Inject]
        protected ILogger<AccountingItem> logger { get; set; } = default!;
        protected RadzenDataGrid<AccountingItem> grid;
        protected IEnumerable<AccountingItem> filteredaccounting;
        protected string searchTerm = string.Empty;
        protected IEnumerable<AccountingItem> accounting;
        protected IEnumerable<AccountingItem> pagedaccounting;
        protected CancellationTokenSource cts = new();
        protected RadzenDataGrid<AccountingItem> dataGrid;
        protected IEnumerable<int> pageSizeOptions = new int[] { 10, 20, 50 };
        protected IEnumerable<AccountingItem> orderDetails;
        protected bool showPagerSummary = true;
        protected bool isLoading = true;
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
        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            try
            {
                accounting = await accountingService.GetRecords();
                filteredaccounting = accounting;
                orderDetails = accounting;
                logger.LogInformation($"filteredaccounting end{filteredaccounting.Count()}");
                StateHasChanged(); // optional, usually not needed in firstRender
            }
            catch (Exception ex)
            {
                logger.LogInformation($"Error loading accounting: {ex.Message}");
                Console.WriteLine($"Error loading accounting: {ex.Message}");
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

                Filteraccounting();
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
        }

        protected void Filteraccounting()
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                filteredaccounting = accounting;
            }
            else
            {
                filteredaccounting = accounting.Where(j =>
                    (j.JobID?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (j.JobName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (j.Address?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (j.Submitter?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (j.JobType?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            grid.Reload();
        }
        protected string selectedPageSize = "10"; // default
        protected string[] pageSizes = new string[] { "10", "20", "50", "All" };
        protected int pageSize = 10;
        protected int currentPage = 0;
        protected void OnPageSizeChanged(int newPageSize)
        {
            pageSize = newPageSize;
            currentPage = 0; // reset to first page
            ApplyPaging();
        }
        protected void OnPageSizeChangedInput(object value)
        {
            selectedPageSize = value.ToString();

            if (selectedPageSize == "All")
            {
                // Show all records
                pageSize = filteredaccounting?.Count() ?? 0;
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
                pagedaccounting = filteredaccounting;
            }
            else
            {
                pagedaccounting = filteredaccounting
                    .Skip(currentPage * pageSize)
                    .Take(pageSize);
            }
            grid?.Reload();
        }
        protected async Task OnLoadData(LoadDataArgs args)
        {
            // Handle search
            filteredaccounting = string.IsNullOrWhiteSpace(searchTerm)
                ? accounting
                : accounting.Where(j =>
                   (j.JobID?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (j.JobName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (j.Address?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (j.Submitter?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (j.JobType?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false));

            // Handle paging
            pagedaccounting = pageSize == 0
                ? filteredaccounting
                : filteredaccounting.Skip(args.Skip ?? 0).Take(args.Top ?? pageSize);

            StateHasChanged();
        }


        protected void OnPageChanged(int newPageIndex)
        {
            currentPage = newPageIndex;
            ApplyPaging();
        }
    }
}
