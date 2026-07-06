using Microsoft.AspNetCore.Components;
using StowellCoApp.Services;
using System.Threading.Tasks;

namespace StowellCoApp.Components.Pages.DocumentManagement
{
    public class BidFileManagerComponentBase : ComponentBase
    {
        [Parameter] public int ProjectId { get; set; }
        protected string? apiUrl;
        protected string selectedJobId { get; set; }
        protected string selectedJobName { get; set; }
        protected string selectedFolderName { get; set; }
        [Inject]
        public IConfiguration Config { get; set; }
        [Inject]
        public IProjectOverviewService _overviewservice { get; set; }
        protected override async Task OnInitializedAsync()
        {
            apiUrl = Config["APIBaseUrl"];
            var jobcodedetails = await _overviewservice.GetJobCodeDetails(ProjectId.ToString());
            if (jobcodedetails != null)
            {
                selectedJobId = jobcodedetails.Recnum;
                selectedJobName = jobcodedetails.JobName;
                selectedFolderName =jobcodedetails.FullJob;
            }
        }

    }
}
