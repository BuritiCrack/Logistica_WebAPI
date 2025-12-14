using CurrieTechnologies.Razor.SweetAlert2;
using LogisticoWebAPI.Frontend.Repositories;
using LogisticoWebAPI.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System.Net;

namespace LogisticoWebAPI.Frontend.Pages.WorkGroups
{
    [Authorize(Roles = "Admin,SuperAdmin,Coordinator")]
    public partial class WorkGroupsIndex
    {
        private int CurrentPage = 1;
        private int TotalPages;
        public List<WorkGroup>? workGroups;
        private bool IsLoading { get; set; } = true;
        public List<WorkGroupMember> Members { get; set; } = [];
        private string EventName { get; set; } = string.Empty;
        [Inject] public IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Parameter, SupplyParameterFromQuery] public string Page { get; set; } = string.Empty;
        [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;
        [Parameter] public int EventId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
            await LoadEventInfoAsycn();
            //await LoadMembersASync();
        }

        private async Task OnFilterChangedAsync(string filter)
        {
            Filter = filter;
            await ApplyfilterAsync();
            StateHasChanged(); // Forzar re-renderizado
        }

        private async Task ApplyfilterAsync()
        {
            int page = 1;
            await LoadAsync(page);
            await OnPageCngedAsync(page);
        }

        private async Task OnPageCngedAsync(int page)
        {
            CurrentPage = page;
            await LoadAsync(page);
        }

        private async Task LoadAsync(int page = 1)
        {
            if (!string.IsNullOrWhiteSpace(Page))
            {
                page = Convert.ToInt32(Page);
            }

            var ok = await LoadListAsync(page);
            if (ok)
            {
                await LoadPagesAsync();
            }
        }

        private async Task<bool> LoadListAsync(int page)
        {
            var url = $"api/workgroups?eventId={EventId}&page={page}";
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"&filter={Filter}";
            }
            IsLoading = true;
            var responsehttp = await Repository.GetAsync<List<WorkGroup>>(url);
            if (responsehttp.Error)
            {
                var message = await responsehttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return false;
            }
            workGroups = responsehttp.Response;
            IsLoading = false;
            return true;
        }

        private async Task LoadPagesAsync()
        {
            var url = $"api/workgroups/totalPages?eventId={EventId}";
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"?filter={Filter}";
            }
            IsLoading = true;
            var responsehttp = await Repository.GetAsync<int>(url);
            if (responsehttp.Error)
            {
                var message = await responsehttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            TotalPages = responsehttp.Response;
            IsLoading = false;
        }

        /*
        private async Task LoadMembersASync()
        {
        }
        */

        private async Task DeleteAsync(WorkGroup workGroup)
        {
            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "¿Estás seguro?",
                Text = $"¿Deseas eliminar el grupo de trabajo '{workGroup.Name}'?",
                Icon = SweetAlertIcon.Warning,
                ShowCancelButton = true,
                ConfirmButtonText = "Sí, eliminar",
                CancelButtonText = "Cancelar"
            });

            var confirm = string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }

            var responseHttp = await Repository.DeleteAsync<WorkGroup>($"/api/workgroups/{workGroup.Id}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/workGroups");
                }
                else
                {
                    var message = await responseHttp.GetErrorMessageAsync();
                    await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                    return;
                }

                await LoadAsync(CurrentPage);
                var toast = SweetAlertService.Mixin(new SweetAlertOptions
                {
                    Icon = SweetAlertIcon.Success,
                    Toast = true,
                    Position = SweetAlertPosition.TopEnd,
                    ShowConfirmButton = false,
                    Timer = 3000
                });
                await toast.FireAsync(message: "Registro borrado con éxito");
            }
        }

        private async Task LoadEventInfoAsycn()
        {
            var responsehttp = await Repository.GetAsync<Event>($"/api/events/{EventId}");
            if (responsehttp.Error)
            {
                var message = await responsehttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                NavigationManager.NavigateTo("/events");
                return;
            }

            EventName = responsehttp.Response!.Name;
        }
    }
}