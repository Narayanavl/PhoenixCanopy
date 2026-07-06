using Microsoft.AspNetCore.Components;
using Radzen;
using StowellCoApp.DTO;
using StowellCoApp.Services;

namespace StowellCoApp.Components.Pages.Contacts
{
    public partial class IndexBase:ComponentBase
    {
        [Inject]
        public NotificationService NotificationService { get; set; }
        [Inject]
        public IContactService _contactService { get; set; }

        [Inject]
        private ILogger<IndexBase> logger { get; set; } = default!;
        public IEnumerable<ContactGroups> contactGroups;
        public IEnumerable<ContactGroupMembers> contactGroupMembers=new List<ContactGroupMembers>();
        protected bool isLoading = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender) // important to avoid infinite loops
            {
                try
                {
                    contactGroups= await _contactService.GetAllGroups();
                    string[] groupdIds =
                {
                      "00c89364-0f28-402e-a905-3080af09b97b",
                "3e93348e-a38a-4fff-b4f4-7fc5d7b59d2e",
                "9b10cfc1-1de4-47a5-ba0c-6e11fa60493f",
                "a539add2-52ab-4545-acb6-5d23c703403f",
                "0a6836b5-aa1f-4c7e-b1e9-77be7b5a85d1",
                "3d76dbfb-27c8-4c95-836e-e5caed001a0b",
                "b222364a-0ca8-4415-ac9e-36ba0bfcabe1",
                    "1446eb90-6189-47cf-b242-b6f84f9f1d56",
                    "e322ddc9-e2d3-468f-bbd2-c7a4bb86908b",
                    "4220517b-c129-4baa-8a4c-7c408a44414d",
 "21e5091d-cb71-4bdb-8d63-f98d8bce3017"
                };
                    contactGroups = contactGroups.Where(x => groupdIds.Contains(x.Id)).ToList();
                    contactGroupMembers = await _contactService.GetGroupUsers(contactGroups.FirstOrDefault().Id);
                    await InvokeAsync(StateHasChanged);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex,ex.Message);
                }
            }
        }

        // Handle when the tab changes
        public async Task OnTabIndexChanged(string Id)
        {
            // Get the Id of the selected tab
            contactGroupMembers = await _contactService.GetGroupUsers(Id);

            // Additional logic can be added here
        }
         protected int selectedIndex = 0; // default first tab
        // private IEnumerable<ContactMember> currentTabMembers;

        //void OnTabIndexChanged(int index)
        //{
        //    selectedIndex = index;

        //    // Get the selected tab
        //    var selectedTab = contactGroups.ElementAt(index);

        //    // Filter members for this tab
        //    contactGroupMembers = contactGroupMembers
        //                        .Where(m => m.Id == selectedTab.Id)
        //                        .ToList();
        //}
        protected int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                if (selectedIndex != value)
                {
                    selectedIndex = value;
                    OnTabIndexChanged(selectedIndex);
                }
            }
        }
        protected async void OnTabIndexChanged(int index)
        {
            isLoading = true; // show loading
            contactGroupMembers = new List<ContactGroupMembers>();
            selectedIndex = index;
            var selectedTab = contactGroups.ElementAt(index);
            if (selectedTab != null)
            {
                // Get the Id of the selected tab
                contactGroupMembers = await _contactService.GetGroupUsers(selectedTab.Id);
            }
            // Additional logic can be added here
            isLoading = false; // hide loading
            await InvokeAsync(StateHasChanged); // refresh UI
        }
    }
}
