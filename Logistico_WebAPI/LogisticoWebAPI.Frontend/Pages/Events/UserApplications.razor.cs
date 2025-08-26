using CurrieTechnologies.Razor.SweetAlert2;
using LogisticoWebAPI.Frontend.Repositories;
using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System.Net;

namespace LogisticoWebAPI.Frontend.Pages.Events
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public partial class UserApplications
    {
        private int CurrentPage = 1;
        private int TotalPages;
        private List<EventUser>? Applications { get; set; }
        private EventStatisticsDTO? Statistics { get; set; }
        public bool IsLoading { get; set; } = true;

        [Parameter] public int Id { get; set; }
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Parameter, SupplyParameterFromQuery] public string Page { get; set; } = string.Empty;
        [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

        protected override async Task OnParametersSetAsync()
        {
            await LoadAsync();
            await LoadStatisticsAsync();
        }

        private async Task OnFilterChangedAsync(string filter)
        {
            Filter = filter;
            await ApplyfilterAsync();
            StateHasChanged(); // Forzar re-renderizado
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

        private async Task LoadStatisticsAsync()
        {
            var responseHttp = await Repository.GetAsync<EventStatisticsDTO>($"api/eventapplications/statistics/{Id}");
            if (!responseHttp.Error)
            {
                Statistics = responseHttp.Response;
            }
        }

        private async Task<bool> LoadListAsync(int page)
        {
            var url = $"api/eventapplications/event/{Id}?page={page}";
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"&filter={Filter}";
            }
            var responseHttp = await Repository.GetAsync<List<EventUser>>(url);
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
                    return false;
                }
            }

            Applications = responseHttp.Response;
            IsLoading = false;
            return true;
        }

        private async Task LoadPagesAsync()
        {
            var url = $"api/eventapplications/totalpages/{Id}";
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"?filter={Filter}";
            }
            var responseHttp = await Repository.GetAsync<int>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            TotalPages = responseHttp.Response;
        }

        private async Task ApplyfilterAsync()
        {
            int page = 1;
            await LoadAsync(page);
            await OnPageCngedAsync(page);
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
            await LoadStatisticsAsync();
            await LoadListAsync(CurrentPage);
        }

        private async Task UpdateAttendanceAsync(int applicationId, bool didAttend)
        {
            try
            {
                var attendDTO = new AttendDTO
                {
                    EventUserId = applicationId,
                    DidAttend = didAttend
                };

                var httpResponse = await Repository.PutAsync($"/api/eventApplications/attendance", attendDTO);

                if (httpResponse.Error)
                {
                    var message = await httpResponse.GetErrorMessageAsync();
                    await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);

                    // Revertir el cambio en la UI si hay error
                    await LoadAsync();
                    return;
                }

                // Actualizar localmente para feedback inmediato
                var application = Applications?.FirstOrDefault(a => a.Id == applicationId);
                if (application != null)
                {
                    application.DidAttend = didAttend;
                }

                var successMessage = didAttend
                    ? "Asistencia confirmada exitosamente"
                    : "Asistencia marcada como no confirmada";

                var toas = SweetAlertService.Mixin(new SweetAlertOptions
                {
                    Icon = SweetAlertIcon.Success,
                    Toast = true,
                    Position = SweetAlertPosition.TopEnd,
                    ShowConfirmButton = false,
                    Timer = 3000
                });

                await toas.FireAsync(message: successMessage);
                await LoadStatisticsAsync();
                // await LoadListAsync(CurrentPage);
            }
            catch (Exception ex)
            {
                await SweetAlertService.FireAsync("Error", $"Error al actualizar asistencia: {ex.Message}", SweetAlertIcon.Error);
                await LoadAsync(); // Recargar datos en caso de error
            }
        }

        //private int GetAttendeesCount()
        //{
        //    if (Applications == null) return 0;

        //    return Applications.Count(a => a.Status == ApplicationStatus.Accepted && a.DidAttend);
        //}

        private string GetStatusBadgeClass(ApplicationStatus status)
        {
            return status switch
            {
                ApplicationStatus.Pending => "bg-warning text-dark",
                ApplicationStatus.Accepted => "bg-success",
                ApplicationStatus.Rejected => "bg-danger",
                ApplicationStatus.CancelledByUser => "bg-secondary",
                _ => "bg-light text-dark",
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