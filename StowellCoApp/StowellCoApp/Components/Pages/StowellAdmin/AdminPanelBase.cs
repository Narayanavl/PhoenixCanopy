using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using NPOI.SS.Formula.Functions;
using Radzen;
using Radzen.Blazor;
using StowellCoApp.DTO;
using StowellCoApp.Models;
using StowellCoApp.Services;

namespace StowellCoApp.Components.Pages.StowellAdmin
{
    public class AdminPanelBase : ComponentBase
    {
        protected RadzenDataGrid<ProjectManagementUser> projectManagementGrid;

        public int? SelectedRoleId { get; set; }
        public int? SelectedUserId { get; set; }
        public List<Project> projects;
        [Inject]
        public NotificationService NotificationService { get; set; }

        public IEnumerable<ProjectManagementUser> _projectManagementUsers { get; set; }
        [Inject]

        public IAdminPanelService _adminPanelService { get; set; }
        private ILogger<AdminPanelBase> logger { get; set; } = default!;
        public IEnumerable<ContactGroupMembers> _stowellUsers { get; set; }
        public IEnumerable<ProjectRoles> _projectRoles { get; set; }


        IEnumerable<int> pageSizeOptions = new int[] { 10, 20, 50 };
        bool showPagerSummary = true;
        private IEnumerable<ProjectManagementUser> pagedProjectManagementUsers;
        protected string selectedPageSize = "15"; // default
        protected string[] pageSizes = new string[] { "10", "15", "20", "50", "All" };
        protected int pageSize = 15;
        private int currentPage = 0;

        protected string searchTerm = string.Empty;
        private CancellationTokenSource cts = new();
        protected IEnumerable<ProjectManagementUser> filteredProjectManagementUsers;
        [Inject]
        public IContactService _contactService { get; set; }
        protected override async Task OnInitializedAsync()
        {


        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender) // important to avoid infinite loops
            {
                try
                {
                    // _stowellUsers = await _adminPanelService.GetAdminPanelUsers();
                    var contactGroups = await _contactService.GetAllGroups();
                    string[] groupdIds =
                {

 "21e5091d-cb71-4bdb-8d63-f98d8bce3017"
                };
                    contactGroups = contactGroups.Where(x => groupdIds.Contains(x.Id)).ToList();
                    if (contactGroups != null && contactGroups.Count() > 0)
                    {
                        var contactGroupMembers = await _contactService.GetGroupUsers(contactGroups.FirstOrDefault().Id);
                        _stowellUsers = contactGroupMembers;
                    }
                    await InvokeAsync(StateHasChanged);
                    _projectRoles = await _adminPanelService.GetUserRoles();
                    var apiData = await _adminPanelService.GetProjectEmails();
                    if (apiData == null)
                        return;

                    _projectManagementUsers = apiData
     .GroupBy(x => x.JobNumber)
     .Select(g => new ProjectManagementUser
     {
         JobNumber = g.Key,
         Assignments = g.Select(x => new ProjectManagementUser
         {
             Id = x.Id,
             JobNumber = x.JobNumber,
             EmailId = x.EmailId,
             UserRole = x.UserRole,
             UserRoleName = x.UserRoleName,
             DateAdded = x.DateAdded,
             IsDisabled = x.IsDisabled
         }).ToList()
     })
     .ToList();
                    filteredProjectManagementUsers = _projectManagementUsers;
                    //_projectManagementUsers
                    //filteredProjectManagementUsers = _projectManagementUsers;
                    await InvokeAsync(StateHasChanged);

                }
                catch (Exception ex)
                {
                    logger.LogInformation($"Error loading jobs: {ex.Message}");
                    Console.WriteLine($"Error loading jobs: {ex.Message}");
                }
            }
            // await projectManagementGrid.Reload();
        }


        protected async Task AddAssignment(ProjectManagementUser project)
        {
            // Prevent multiple new rows
            if (project.Assignments.Any(a => a.IsNew))
                return;
            var newRow = new ProjectManagementUser
            {
                JobNumber = project.JobNumber,
                IsNew = true
            };

            // 🔴 IMPORTANT: Reassign list
            project.Assignments = project.Assignments
                .Prepend(newRow)
                .ToList();

            await InvokeAsync(StateHasChanged);

            //await projectManagementGrid.ExpandRow(project);
            await projectManagementGrid.Reload();
        }
        protected async Task SaveAssignment(ProjectManagementUser row)
        {
            try
            {
                if (row.SelectedUserId == null || row.SelectedRoleId == null)
                    return;

                var user = _stowellUsers.First(u => u.Id == row.SelectedUserId);
                var role = _projectRoles.First(r => r.RoleId == row.SelectedRoleId);

                row.EmailId = user.Mail!;
                row.UserRole = role.RoleId;
                row.UserRoleName = role.RoleName!;
                row.IsNew = false;
                var response = await _adminPanelService.InsertProjectManagementUser(row);
                if (response)
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = true
                           ? NotificationSeverity.Success
                           : NotificationSeverity.Error,

                        Summary = true ? "Success" : "Error",
                        Detail = true ? "Details Saved Successfully" : "Error while saving details",
                        Duration = 4000,

                    });
                    var apiData = await _adminPanelService.GetProjectEmails();
                    if (apiData == null)
                        return;
                    _projectManagementUsers = apiData
    .GroupBy(x => x.JobNumber)
    .Select(g => new ProjectManagementUser
    {
        JobNumber = g.Key,
        Assignments = g.Select(x => new ProjectManagementUser
        {
            Id = x.Id,
            JobNumber = x.JobNumber,
            EmailId = x.EmailId,
            UserRole = x.UserRole,
            UserRoleName = x.UserRoleName,
            DateAdded = x.DateAdded,
            IsDisabled = x.IsDisabled
        }).ToList()
    })
    .ToList();
                    await InvokeAsync(StateHasChanged);
                    //await projectManagementGrid.Reload();
                }
                else
                {
                    NotificationService.Notify(new NotificationMessage
                    {
                        Severity = NotificationSeverity.Error,

                        Summary = "Error",
                        Detail = "Error while saving.",
                        Duration = 4000
                    });

                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,

                    Summary = "Error",
                    Detail = "Error while saving details.",
                    Duration = 4000
                });
            }
        }
        protected async Task CancelAssignment(ProjectManagementUser parent, ProjectManagementUser assignment)
        {
            parent.Assignments = parent.Assignments
                .Where(a => a != assignment)
                .ToList();

            await InvokeAsync(StateHasChanged);
        }

        protected void EditAssignment(ProjectManagementUser assignment)
        {
            Console.WriteLine($"Edit assignment: {assignment.EmailId}");
        }

        protected async Task DeleteAssignment(ProjectManagementUser assignment)
        {
            try
            {
                var response = await _adminPanelService
                    .DeleteProjectManagementUser(assignment.JobNumber, assignment.EmailId);

                if (!response)
                    return;

                // 🔥 find existing parent reference
                var parent = filteredProjectManagementUsers
                    .FirstOrDefault(p => p.JobNumber == assignment.JobNumber);

                if (parent != null)
                {
                    parent.Assignments = parent.Assignments
                        .Where(a => a.EmailId != assignment.EmailId)
                        .ToList();
                }

                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Success,
                    Summary = "Success",
                    Detail = "Deleted Successfully",
                    Duration = 4000
                });

                await InvokeAsync(StateHasChanged); // ✅ NO Reload, NO reassignment
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = "Error",
                    Detail = "Error while deleting",
                    Duration = 3000
                });
            }
        }



        //protected void AddNewRow(Project project)
        //{
        //    if (project.Assignments.Any(a => a.IsEditing))
        //        return;

        //    project.Assignments.Add(new ProjectAssignment
        //    {
        //        IsEditing = true,
        //        IsNew = true
        //    });
        //}

        //protected void EditRow(Project project, ProjectAssignment row)
        //{
        //    if (project.Assignments.Any(a => a.IsEditing))
        //        return;

        //    row.Backup();
        //    row.IsEditing = true;
        //}

        //protected void SaveRow(Project project, ProjectAssignment row)
        //{
        //    row.IsEditing = false;
        //    row.IsNew = false;
        //}

        //protected void CancelEdit(Project project, ProjectAssignment row)
        //{
        //    if (row.IsNew)
        //    {
        //        project.Assignments.Remove(row);
        //    }
        //    else
        //    {
        //        row.Restore();
        //        row.IsEditing = false;
        //    }
        //}

        //protected void DeleteRow(Project project, ProjectAssignment row)
        //{
        //    project.Assignments.Remove(row);
        //}
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

                FilterJobs();
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
        }
        private void FilterJobs()
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                filteredProjectManagementUsers = _projectManagementUsers;
            }
            else
            {
                filteredProjectManagementUsers = _projectManagementUsers.Where(j =>
                    (j.JobNumber.ToString()?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (j.EmailId?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            projectManagementGrid.Reload();

        }

        private void OnPageSizeChanged(int newPageSize)
        {
            pageSize = newPageSize;
            currentPage = 0; // reset to first page
            ApplyPaging();
        }
        protected async void OnPageSizeChangedInput(object value)
        {
            selectedPageSize = value.ToString();

            if (selectedPageSize == "All")
            {
                // Show all records
                pageSize = filteredProjectManagementUsers?.Count() ?? 0;
            }
            else
            {
                pageSize = Convert.ToInt32(selectedPageSize);
            }

            await InvokeAsync(StateHasChanged);

            //currentPage = 0;

            //// Refresh the grid and pager
            //projectManagementGrid.GoToPage(0);
            //projectManagementGrid.Reload();
        }

        private void ApplyPaging()
        {
            if (pageSize == 0)
            {
                filteredProjectManagementUsers = _projectManagementUsers;
            }
            else
            {
                filteredProjectManagementUsers = _projectManagementUsers
                    .Skip(currentPage * pageSize)
                    .Take(pageSize);
            }
            projectManagementGrid?.Reload();
        }
    }
    public class Project
    {
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public List<ProjectAssignment> Assignments { get; set; }
    }

    public class ProjectAssignment
    {
        public string Role { get; set; }
        public string Employee { get; set; }

        public bool IsEditing { get; set; }
        public bool IsNew { get; set; }

        string _roleBackup;
        string _employeeBackup;

        public void Backup()
        {
            _roleBackup = Role;
            _employeeBackup = Employee;
        }

        public void Restore()
        {
            Role = _roleBackup;
            Employee = _employeeBackup;
        }
    }
}
