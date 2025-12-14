using CurrieTechnologies.Razor.SweetAlert2;
using LogisticoWebAPI.Frontend.Repositories;
using LogisticoWebAPI.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace LogisticoWebAPI.Frontend.Pages.WorkGroups
{
    [Authorize(Roles = "Admin,SuperAdmin,Coordinator")]
    public partial class WorkGroupsIndex
    {
        private List<WorkGroup>? workGroups;
        private string? EventName;
        [Inject] public IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Parameter] public int EventId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            var eventResponse = await Repository.GetAsync<Event>($"/api/events/{EventId}");
            if (eventResponse.Error)
            {
                var message = await eventResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                NavigationManager.NavigateTo("/events");
                return;
            }
            else
            {
                EventName = eventResponse.Response!.Name;
            }

            var responsehttp = await Repository.GetAsync<List<WorkGroup>>($"/api/workgroups/byEvent/{EventId}");
            if (responsehttp.Error)
            {
                var message = await responsehttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            else
            {
                workGroups = responsehttp.Response;
            }
        }

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
            if (result.IsConfirmed)
            {
                var responseHttp = await Repository.DeleteAsync<WorkGroup>($"/api/workgroups/{workGroup.Id}");
                if (responseHttp.Error)
                {
                    var message = await responseHttp.GetErrorMessageAsync();
                    await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                    return;
                }
                else
                {
                    await SweetAlertService.FireAsync("Eliminado", "El grupo de trabajo ha sido eliminado.", SweetAlertIcon.Success);
                    await LoadAsync();
                }
            }
        }
    }
}