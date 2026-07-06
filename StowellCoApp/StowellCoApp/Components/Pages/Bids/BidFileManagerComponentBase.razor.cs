using Microsoft.AspNetCore.Components;
using StowellCoApp.DTO;
using StowellCoApp.Services;
using System.Threading.Tasks;

namespace StowellCoApp.Components.Pages.Bids
{
    public class BidFileManagerComponentBase : ComponentBase
    {
        [Parameter] public string BidId { get; set; }
        [Parameter] public string Type { get; set; }
        [Parameter] public string rootPath { get; set; }
        protected string? apiUrl;
        [Inject]
        public HttpClient Http { get; set; }
        [Inject]
        public IBidService _bidservice { get; set; }
        [Parameter] public BidRecords _bidRecords { get; set; }
        [Inject]
        public IConfiguration Config { get; set; }
        [Inject]
        public IProjectOverviewService _overviewservice { get; set; }
        protected override async Task OnInitializedAsync()
        {
            apiUrl = Config["APIBaseUrl"];
            if (!string.IsNullOrEmpty(BidId))
            {
                // _bidRecords = await _bidservice.GetBidById(BidId);
                //if (_bidRecords != null)
                //{
                // rootPath = "\\\\DESKTOP-C86LJD9\\AnuShareFolder\\10064 - Fairmont Hotel";
                rootPath = Config["BidsDocSettings:RootSharePath"] + "\\" + BidId;
                //}
            }
        }

    }
}
