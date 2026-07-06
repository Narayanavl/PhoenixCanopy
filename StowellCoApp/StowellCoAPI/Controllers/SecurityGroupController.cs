using Azure.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Graph;
using StowellCoAPI.DTO;
using StowellCoAPI.Models;
using StowellCoAPI.Services;
using System.Data;

namespace StowellCoAPI.Controllers
{
    [ApiController]
    public class SecurityGroupController : ControllerBase
    {
        private readonly ILogger<SecurityGroupController> _logger;
        private readonly IConfiguration _configuration;
        private readonly GraphServiceClient _graphServiceClient;

        public SecurityGroupController(ILogger<SecurityGroupController> logger, IConfiguration configuration, GraphServiceClient graphServiceClient)
        {
            _configuration = configuration;
            _graphServiceClient = graphServiceClient;
            _logger = logger;
        }

        [HttpGet("api/[controller]/GetAllGroups", Name = "GetAllGroups")]
        public async Task<IActionResult> GetAllGroups()
        {
            try
            {

                var userList = new List<ContactGroups>();

                // Fetch all users with pagination
                var groups = await _graphServiceClient.Groups
                    .Request()
                    .GetAsync();

                while (groups != null)
                {
                    // Process the current page
                    userList.AddRange(groups.CurrentPage
                        .Where(group => group.GroupTypes == null || !group.GroupTypes.Contains("Unified")) // Only include security groups
                        .Select(group => new ContactGroups
                        {
                            DisplayName = group.DisplayName,
                            Id = group.Id
                        }).OrderBy(group => group.DisplayName));
                    // Get the next page
                    groups = groups.NextPageRequest != null ?
                            await groups.NextPageRequest.GetAsync() :
                            null;
                }

                return Ok(userList);
            }
            catch (SqlException sqlEx)
            {
                // Log SQL-specific errors (e.g., connection or syntax issues)
                return StatusCode(500, new
                {
                    Message = "A database error occurred while retrieving jobs.",
                    Details = sqlEx.Message
                });
                _logger.LogError(sqlEx, sqlEx.Message);
            }
            catch (Exception ex)
            {
                // Catch all unexpected exceptions
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while retrieving jobs.",
                    Details = ex.Message
                });
                _logger.LogError(ex, ex.Message);
            }

        }

        [HttpGet("api/[controller]/GetGroupUsers/{groupId}", Name = "GetGroupUsers")]
        public async Task<IActionResult> GetUsersInGroup(string groupId)
        {
            try
            {
                var groupMembers = new List<ContactGroupMembers>();

                // Request to get members of the group
                var members = await _graphServiceClient.Groups[groupId].Members
                    .Request()
                    //.Select("id,displayName,jobTitle,officeLocation,companyName,mobilePhone,mail,onPremisesExtensionAttributes")
                    .GetAsync();
                foreach (var member in members.CurrentPage.OfType<User>())
                {
                    var fullUser = await _graphServiceClient.Users[member.Id]
                        .Request()
                        .Select("id,displayName,jobTitle,city,state,zip,businessPhones,mail,officeLocation,companyName,postalCode,streetAddress,onPremisesExtensionAttributes")
                        .GetAsync();
                    string? photoBase64 = null;
                    try
                    {
                        byte[]? photoBytes;
                        var photoStream = await _graphServiceClient.Users[member.Id]
    .Photo
    .Content
    .Request()
    .GetAsync();

                        using (var ms = new MemoryStream())
                        {
                            await photoStream.CopyToAsync(ms);
                            photoBytes = ms.ToArray();
                        }
                        // Convert photo bytes to base64 string (if available)

                        if (photoBytes != null)
                        {
                            photoBase64 = Convert.ToBase64String(photoBytes);
                        }
                    }
                    catch (ServiceException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        // Photo not found, leave photoBytes null
                    }



                    groupMembers.Add(new ContactGroupMembers
                    {
                        Id = fullUser.Id,
                        DisplayName = fullUser.DisplayName,
                        JobTitle = fullUser.JobTitle,
                        City = fullUser.City,
                        Mail = fullUser.Mail,
                        StreetAddress = fullUser.StreetAddress,
                        State = fullUser.State,
                        PostalCode = fullUser.PostalCode,
                        Location = fullUser.OfficeLocation,
                        Company = fullUser.CompanyName,
                        Phone = fullUser.BusinessPhones.Any() ? fullUser.BusinessPhones.FirstOrDefault() : fullUser.MobilePhone,
                        AddressLine1 = fullUser.OnPremisesExtensionAttributes?.ExtensionAttribute1,
                        AddressLine2 = fullUser.OnPremisesExtensionAttributes?.ExtensionAttribute2,
                        PhotoBase64 = photoBase64
                    });

                }

                return Ok(groupMembers);
            }
            catch (SqlException sqlEx)
            {
                // Log SQL-specific errors (e.g., connection or syntax issues)
                return StatusCode(500, new
                {
                    Message = "A database error occurred while retrieving jobs.",
                    Details = sqlEx.Message
                });
                _logger.LogError(sqlEx, sqlEx.Message);
            }
            catch (Exception ex)
            {
                // Catch all unexpected exceptions
                return StatusCode(500, new
                {
                    Message = "An unexpected error occurred while retrieving jobs.",
                    Details = ex.Message
                });
                _logger.LogError(ex, ex.Message);
            }

        }
        [HttpGet("api/[controller]/friend-photo/{email}", Name = "friend-photo")]
        private async Task<IActionResult> GetFriendPhotoAsync([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email ID is required.");


            try
            {
                var user = await _graphServiceClient.Users
                    .Request()
                    .Filter($"mail eq '{email}'")
                    .Select("id")
                    .GetAsync();

                var userId = user?.FirstOrDefault()?.Id;
                if (string.IsNullOrEmpty(userId))
                    return NotFound($"User not found with email: {email}");

                var photoStream = await _graphServiceClient.Users[userId].Photo.Content.Request().GetAsync();
                if (photoStream != null)
                    return File(photoStream, "image/jpeg");

                return NotFound($"Photo not found for user with email: {email}");
            }
            catch (ServiceException ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return NotFound($"User or photo not found for email: {email}");

                Console.WriteLine($"Graph API error: {ex.Message}");
                return StatusCode(500, "Internal server error while fetching user photo.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }
    }
}