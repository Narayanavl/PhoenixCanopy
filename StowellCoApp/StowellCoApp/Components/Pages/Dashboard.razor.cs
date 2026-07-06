using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using StowellCoApp.DTO;
using StowellCoApp.Services;
using static StowellCoApp.Components.ColumnChart;
using static StowellCoApp.Components.ColumnChart1;
using static StowellCoApp.Components.Pages.CalendarPage;

namespace StowellCoApp.Components.Pages
{
    public partial class Dashboard : ComponentBase
    {
        [Inject]
        public IJobService JobService { get; set; }
        [Inject]
        private ILogger<Dashboard> logger { get; set; } = default!;
        [Inject]
        private IConfiguration _configuration { get; set; }
        private IEnumerable<ChartData> chartDatasets;
        private IEnumerable<ChartData> cashCollectedDatasets;
        private IEnumerable<ChartData> sampleChartDatasets;
        
       

        private List<CashDataItem> cashData = new();

        //private List<CashDataItem> TransformToCashData(IEnumerable<ChartData> chartDataList)
        //{
        //    var result = new List<CashDataItem>();

        //    if (chartDataList == null) return result;

        //    // Assuming only one ChartData item or merge all (adjust as needed)
        //    var chartData = chartDataList.FirstOrDefault();
        //    if (chartData == null) return result;

        //    for (int i = 0; i < chartData.Labels.Length; i++)
        //    {
        //        var category = chartData.Labels[i];
        //        decimal cashCollected = 0;
        //        decimal cashPaid = 0;

        //        foreach (var dataset in chartData.Datasets)
        //        {
        //            if (i < dataset.Data.Length)
        //            {
        //                if (dataset.Label == "Cash Collected")
        //                    cashCollected = dataset.Data[i];
        //                else if (dataset.Label == "Cash Paid")
        //                    cashPaid = dataset.Data[i];
        //            }
        //        }

        //        result.Add(new CashDataItem
        //        {
        //            Category = category,
        //            CashCollected = cashCollected,
        //            CashPaid = cashPaid
        //        });
        //    }

        //    return result;
        //}
        //public enum CalendarView
        //{
        //    Month,
        //    Week,
        //    Day
        //}
        //IEnumerable<MyEventViewModel> FilteredEvents
        //{
        //    get
        //    {
        //        if (currentView == CalendarView.Day)
        //        {
        //            return calendarEvents
        //                .Where(e => e.Start.Date == DateTime.Today);
        //        }

        //        if (currentView == CalendarView.Week)
        //        {
        //            var start = StartOfWeek(DateTime.Today);
        //            var end = start.AddDays(6);

        //            return calendarEvents
        //                .Where(e => e.Start.Date >= start && e.Start.Date <= end);
        //        }

        //        // Month (default)
        //        return calendarEvents;
        //    }
        //}
        //DateTime StartOfWeek(DateTime date)
        //{
        //    DayOfWeek startDay = DayOfWeek.Monday; // change if needed

        //    int diff = (7 + (date.DayOfWeek - startDay)) % 7;
        //    return date.AddDays(-diff).Date;
        //}

        //CalendarView currentView = CalendarView.Month;

        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
        //    if (firstRender) // important to avoid infinite loops
        //    {
        //        try
        //        {
        //            //logger.LogInformation("OnAfterRenderAsync start");
        //            //jobs = await JobService.GetCurrentJobs();
        //            //logger.LogInformation("OnAfterRenderAsync end");
        //            //chartDatasets = await JobService.GetSampleChartDatasets();
        //            // cashCollectedDatasets = await JobService.GetCashCollectedDatasets();
        //            //  cashData = TransformToCashData(cashCollectedDatasets);

        //            //cashCollectedDatasets = apiData.Select(d => new LineChart.ChartData
        //            //{
        //            //    Labels = d.Labels,
        //            //    Datasets = d.Datasets.Select(ds => new LineChart.ChartDataset
        //            //    {
        //            //        Label = ds.Label,
        //            //        Data = ds.Data,
        //            //        BackgroundColor = ds.BackgroundColor,
        //            //        BorderColor = ds.BorderColor
        //            //    }).ToList()
        //            //}).ToList();

        //            //            cashCollectedDatasets = new List<LineChart.ChartData>
        //            //{
        //            //    new LineChart.ChartData
        //            //    {
        //            //        Labels = new[] { "Jan", "Feb", "Mar", "Apr" },
        //            //        Datasets = new List<LineChart.ChartDataset>
        //            //        {
        //            //            new LineChart.ChartDataset
        //            //            {
        //            //                Label = "2023",
        //            //                Data = new decimal[] { 234000, 269000, 233000, 244000 },
        //            //                BackgroundColor = "#4caf50",
        //            //                BorderColor = "#388e3c"
        //            //            },
        //            //            new LineChart.ChartDataset
        //            //            {
        //            //                Label = "2024",
        //            //                Data = new decimal[] { 334000, 369000, 333000, 344000 },
        //            //                BackgroundColor = "#2196f3",
        //            //                BorderColor = "#1976d2"
        //            //            }
        //            //        }
        //            //    }
        //            //            };
        //            //sampleChartDatasets = await JobService.GetSampleChartDatasets();
        //            //logger.LogInformation("filteredJobs start");
        //            //filteredJobs = jobs;
        //           // logger.LogInformation($"filteredJobs end{filteredJobs.Count()}");
        //            StateHasChanged(); // optional, usually not needed in firstRender
        //        }
        //        catch (Exception ex)
        //        {
        //            logger.LogInformation($"Error loading jobs: {ex.Message}");
        //            Console.WriteLine($"Error loading jobs: {ex.Message}");
        //        }
        //    }
        //}
        private List<CashDataItem> TransformToCashData(IEnumerable<ChartData> chartDataList)
        {
            var result = new List<CashDataItem>();

            if (chartDataList == null) return result;

            // Assuming only one ChartData item or merge all (adjust as needed)
            var chartData = chartDataList.FirstOrDefault();
            if (chartData == null) return result;

            for (int i = 0; i < chartData.Labels.Length; i++)
            {
                var category = chartData.Labels[i];
                decimal cashCollected = 0;
                decimal cashPaid = 0;

                foreach (var dataset in chartData.Datasets)
                {
                    if (i < dataset.Data.Length)
                    {
                        if (dataset.Label == "Cash Collected")
                            cashCollected = dataset.Data[i];
                        else if (dataset.Label == "Cash Paid")
                            cashPaid = dataset.Data[i];
                    }
                }

                result.Add(new CashDataItem
                {
                    Category = category,
                    CashCollected = cashCollected,
                    CashPaid = cashPaid
                });
            }

            return result;
        }

        public class Appointment
        {
            public string Title { get; set; } = "";
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
        }

        List<Appointment> appointments = new()
                {
                new Appointment { Title = "Team Meeting", Start = DateTime.Today.AddHours(9), End = DateTime.Today.AddHours(10) },
                new Appointment { Title = "Project Review", Start = DateTime.Today.AddDays(1).AddHours(13), End = DateTime.Today.AddDays(1).AddHours(14) },
                new Appointment { Title = "Client Presentation", Start = DateTime.Today.AddDays(2).AddHours(15), End = DateTime.Today.AddDays(2).AddHours(16) }
                };
        void ShowList()
        {
            Console.WriteLine("List clicked!");
            // your logic here
        }
        protected override async Task OnInitializedAsync()
        {
            logger.LogInformation("Dashboard OnInitializedAsync start");
            calendarEvents = await ApiClient.GetUserCalendarAsync(_configuration["EventsEmail"]);
            // var view = GetCurrentView();
            // // GroupEvents();
            // var test = scheduler.SelectedView.Text;
            // GroupEventsByView(view);
            logger.LogInformation("Dashboard OnInitializedAsync end");
        }
    }
}
