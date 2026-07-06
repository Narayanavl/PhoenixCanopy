using Microsoft.AspNetCore.Components;
using StowellCoApp.DTO;
using StowellCoApp.Components.Pages.ProjectManagement;

namespace StowellCoApp.Components.Pages.ProjectManagement
{

    public class ProjectTabBase : ComponentBase
    {
        [Parameter] public CurrentCostSummaryViewModel Model { get; set; }
    }

}