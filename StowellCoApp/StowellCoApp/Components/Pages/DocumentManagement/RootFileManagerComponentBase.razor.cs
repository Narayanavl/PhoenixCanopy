using Microsoft.AspNetCore.Components;
using StowellCoApp.Components.Pages.StowellAdmin;
using StowellCoApp.Services;

namespace StowellCoApp.Components.Pages.DocumentManagement
{
    public class RootFileManagerComponentBase : ComponentBase
    {
        [Inject]
        public IConfiguration Config { get; set; }
        [Inject]
        public IProjectOverviewService _overviewservice { get; set; }
        protected string? apiUrl;
        protected string? selectedFolderName { get; set; }
        protected override async Task OnInitializedAsync()
        {
            apiUrl = Config["APIBaseUrl"];
            selectedFolderName = Config["FileSettings:FolderName"];

        }
    }
}
