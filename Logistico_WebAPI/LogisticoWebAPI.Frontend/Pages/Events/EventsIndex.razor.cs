using CurrieTechnologies.Razor.SweetAlert2;
using LogisticoWebAPI.Frontend.Repositories;
using LogisticoWebAPI.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System.Net;

namespace LogisticoWebAPI.Frontend.Pages.Events
{
    [Authorize(Roles = "User,Admin")]
    public partial class EventsIndex
    {
        private int CurrentPage = 1;
        private int TotalPages;
        public List<Event>? Events { get; set; }
        private HashSet<int> AppliedEventIds { get; set; } = new();

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            await LoadAsycn();
            await LoadUserApplicationsAsync();
        }

        private async Task OnPageCngedAsync(int page)
        {
            CurrentPage = page;
            await LoadAsycn(page);
        }

        private async Task LoadAsycn(int page = 1)
        {
            var ok = await LoadListAync(page);
            if (ok)
            {
                await LoadPagesAsync();
            }
        }

        private async Task<bool> LoadListAync(int page)
        {
            var responseHttp = await Repository.GetAsync<List<Event>>($"api/events?page={page}");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return false;
            }
            Events = responseHttp.Response;
            return true;
        }

        private async Task LoadPagesAsync()
        {
            var responseHttp = await Repository.GetAsync<int>("api/events/totalpages");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            TotalPages = responseHttp.Response;
        }

        private async Task LoadUserApplicationsAsync()
        {
            var responseHttp = await Repository.GetAsync<List<EventUser>>("api/eventapplications/myapplications");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            AppliedEventIds = responseHttp.Response!
                    .Select(eu => eu.EventId)
                    .ToHashSet();
        }

        private async Task DeleteAsync(Event @event)
        {
            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmacion",
                Text = $"¿Deseas eliminar el evento: {@event.Name}?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true
            });

            var confirm = string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }

            var responseHttp = await Repository.DeleteAsync<Event>($"api/events/{@event.Id}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/events");
                }
                else
                {
                    var message = await responseHttp.GetErrorMessageAsync();
                    await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                }
                return;
            }

            await LoadAsycn();
            var toas = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Icon = SweetAlertIcon.Success,
                Toast = true,
                Position = SweetAlertPosition.TopEnd,
                ShowConfirmButton = false,
                Timer = 3000
            });
            await toas.FireAsync(message: "Registro borrado con éxito");
        }

        private async Task AppyToEventAsync(int eventId)
        {
            var ApplyToEventDTO = new ApplyToEventDTO
            {
                EventId = eventId
            };
            var responseHttp = await Repository.PostAsync("api/EventApplications/apply", ApplyToEventDTO);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            // Agregar el ID del evento a la lista de aplicados
            AppliedEventIds.Add(eventId);
            StateHasChanged(); // Forzar re-renderizado

            var toas = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.TopEnd,
                ShowConfirmButton = false,
                Timer = 3000
            });
            await toas.FireAsync(icon: SweetAlertIcon.Success, message: "Postulación enviada con éxito");
        }

        private bool HasAppliedToEvent(int eventId)
        {
            return AppliedEventIds.Contains(eventId);
        }
    }
}