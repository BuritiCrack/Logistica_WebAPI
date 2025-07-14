using CurrieTechnologies.Razor.SweetAlert2;
using LogisticoWebAPI.Frontend.Repositories;
using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Enums;
using Microsoft.AspNetCore.Components;
using System.Diagnostics.Tracing;

namespace LogisticoWebAPI.Frontend.Pages.Events
{
    public partial class MyApplications
    {
        private List<EventUser>? Applications { get; set; }

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            await LoadApplicationsAsync();
        }

        private async Task LoadApplicationsAsync()
        {
            try
            {
                var responseHttp = await Repository.GetAsync<List<EventUser>>("api/eventapplications/myapplications");
                if (responseHttp.Error)
                {
                    var message = await responseHttp.GetErrorMessageAsync();
                    await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                    return;
                }
                Applications = responseHttp.Response; 
                //!.OrderByDescending(a => a.RegistrationDate).ToList();
            }
            catch (Exception ex)
            {
                await SweetAlertService.FireAsync("Error", ex.Message, SweetAlertIcon.Error);
            }
        }

        private async Task CancelApplicationAsync(int eventUserId)
        {
            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "¿Estás seguro?",
                Text = "¿Deseas cancelar tu aplicación a este evento?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true,
                ConfirmButtonText = "Sí, cancelar",
                CancelButtonText = "No"
            });

            if (result.IsConfirmed)
            {
                try
                {
                    var responseHttp = await Repository.DeleteAsync<EventUser>(
                        $"api/eventapplications/cancel/{eventUserId}");

                    if (responseHttp.Error)
                    {
                        var message = await responseHttp.GetErrorMessageAsync();
                        await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                        return;
                    }

                    await SweetAlertService.FireAsync("Éxito", "Tu aplicación ha sido cancelada correctamente.", SweetAlertIcon.Success);
                    await LoadApplicationsAsync();
                }
                catch (Exception ex)
                {
                    await SweetAlertService.FireAsync("Error", ex.Message, SweetAlertIcon.Error);
                }
            }
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
    }
}
