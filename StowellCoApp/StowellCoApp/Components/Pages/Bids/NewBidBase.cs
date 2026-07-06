using Microsoft.AspNetCore.Components;
using Radzen;
using StowellCoApp.DTO;
using StowellCoApp.Services;

namespace StowellCoApp.Components.Pages.Bids
{
    public class NewBidBase : ComponentBase
    {
        private bool _isLoaded = false;
        [Inject]
        public IBidService _bidService { get; set; }
        public CreateBidRequest createBidRequest { get; set; } = new();
        [Inject]
        public NotificationService NotificationService { get; set; }
        [Inject]
        public NavigationManager NavigationManager { get; set; }

        public bool IsSaving = false;
        //protected override async Task OnInitializedAsync()
        //{
        //    try
        //    {
        //        if (createBidRequest.BidId==null)
        //        {
        //            // var bidId = await _bidService.GetNextDisplayBidID();
        //            createBidRequest.BidId = await _bidService.GetNextDisplayBidID();
        //            //  await InvokeAsync(StateHasChanged);
        //            _isLoaded = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                if (firstRender)
                {
                    createBidRequest.BidId =
                        await _bidService.GetNextDisplayBidID();

                    await InvokeAsync(StateHasChanged);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected async Task SaveBid()
        {
            try
            {
                IsSaving = true;

               // createBidRequest.CreatedBy = "admin";

                var result = await _bidService.CreateNewBid(createBidRequest);

                if (result)
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Success,
                        Summary = "Success",
                        Detail = "Opportunity created successfully.",
                        Duration = 3000
                    });
                    await Task.Delay(3000);
                    NavigationManager.NavigateTo("/Bids/CurrentBids");
                }
                else
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,
                        Summary = "Error",
                        Detail = "Unable to create Opportunity.",
                        Duration = 5000
                    });
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Exception",
                    Detail = ex.Message,
                    Duration = 5000
                });
            }
            finally
            {
                IsSaving = false;
              //  await InvokeAsync(StateHasChanged);
            }
        }
        protected async Task Cancel()
        {
            createBidRequest = new CreateBidRequest();

            // Reload next Bid ID
            createBidRequest.BidId = await _bidService.GetNextDisplayBidID();
            // createBidRequest.Department = "";
            // createBidRequest.Division = "";
            createBidRequest.BidName = "";
            createBidRequest.Address1 = "";
            createBidRequest.City = "";
            // BidModel.State = "";
            createBidRequest.ZipCode = "";

            await InvokeAsync(StateHasChanged);
        }
    }
}
