using CurrieTechnologies.Razor.SweetAlert2;
using LogisticoWebAPI.Frontend.Repositories;
using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Enums;
using Microsoft.AspNetCore.Components;
using System.Net;

namespace LogisticoWebAPI.Frontend.Pages.Events
{
    public partial class UserApplications
    {
        private List<EventUser>? Applications { get; set; }
        public bool IsLoading { get; set; } = true;

        [Parameter] public int Id { get; set; }
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        protected override async Task OnParametersSetAsync()
        {
            var responseHttp = await Repository.GetAsync<List<EventUser>>($"api/eventapplications/applications/{Id}");
            IsLoading = true;
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/events");
                }
                else
                {
                    var message = responseHttp.GetErrorMessageAsync().Result;
                    await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                    return;
                }
            }
            else
            {
                Applications = responseHttp.Response;
                IsLoading = false;
            }
        }

        private async Task UpdateApplicationStatusAsync(int applicationId, ApplicationStatus newStatus)
        {
            var statusText = newStatus == ApplicationStatus.Accepted ? "aceptada" : "rechazada";
            var updateDto = new UpdateApplicationStatusDTO
            {
                ApplicationId = applicationId,
                NewStatus = newStatus
            };

            var responseHttp = await Repository.PutAsync("api/eventapplications/updatestatus", updateDto);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            var toas = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Icon = SweetAlertIcon.Success,
                Toast = true,
                Position = SweetAlertPosition.TopEnd,
                ShowConfirmButton = false,
                Timer = 3000
            });

            await toas.FireAsync(message: $"Persona {statusText} con éxito");
            await OnParametersSetAsync();
        }
        

        private string GetStatusBadgeClass(ApplicationStatus status)
        {
            return status switch
            {
                ApplicationStatus.Pending => "bg-warning text-dark",
                ApplicationStatus.Accepted => "bg-success",
                ApplicationStatus.Rejected => "bg-danger",
                ApplicationStatus.CancelledByUser => "bg-secondary",
                _ => "bg-light text-dark"
            };
        }

        private string GetStatusIcon(ApplicationStatus status)
        {
            return status switch
            {
                ApplicationStatus.Pending => "fa-clock",
                ApplicationStatus.Accepted => "fa-check",
                ApplicationStatus.Rejected => "fa-times",
                ApplicationStatus.CancelledByUser => "fa-ban",
                _ => "fa-question"
            };
        }

        private string GetStatusText(ApplicationStatus status)
        {
            return status switch
            {
                ApplicationStatus.Pending => "Pendiente",
                ApplicationStatus.Accepted => "Aceptado",
                ApplicationStatus.Rejected => "Rechazado",
                ApplicationStatus.CancelledByUser => "Cancelado",
                _ => "Desconocido"
            };
        }

        private void GoBack()
        {
            NavigationManager.NavigateTo("/events");
        }
    }
}