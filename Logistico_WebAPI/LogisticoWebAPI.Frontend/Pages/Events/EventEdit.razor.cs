using CurrieTechnologies.Razor.SweetAlert2;
using LogisticoWebAPI.Frontend.Repositories;
using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System.Net;

namespace LogisticoWebAPI.Frontend.Pages.Events
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public partial class EventEdit
    {
        private EventDTO EventDTO = new();
        private Event? Event;
        private EventForm? eventForm;
        private bool IsLoading;
        [Inject] public IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Parameter] public int Id { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            var responseHttp = await Repository.GetAsync<Event>($"/api/events/{Id}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    await SweetAlertService.FireAsync("Error", "Evento no encontrado");
                    NavigationManager.NavigateTo("/events");
                    return;
                }
                else
                {
                    var message = await responseHttp.GetErrorMessageAsync();
                    await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                    NavigationManager.NavigateTo("/events");
                    return;
                }
            }
            else
            {
                Event = responseHttp.Response!;
                EventDTO = ToEventDTO(Event);
                StateHasChanged();
            }
        }

        private EventDTO ToEventDTO(Event newEvent)
        {
            return new EventDTO
            {
                Id = newEvent.Id,
                Name = newEvent.Name,
                Place = newEvent.Place,
                Description = newEvent.Description,
                StartDate = newEvent.StartDate,
                EndDate = newEvent.EndDate,
                PaymentDate = newEvent.PaymentDate.Date,
                MealType = newEvent.MealType,
                Payment = newEvent.Payment,
                Photo = newEvent.Photo
            };
        }

        private async Task EditAsync()
        {
            IsLoading = true;
            var responseHttp = await Repository.PutAsync($"/api/events/full", EventDTO);
            IsLoading = false;
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message);
                return;
            }

            Return();
            var toas = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Icon = SweetAlertIcon.Success,
                Toast = true,
                Position = SweetAlertPosition.TopEnd,
                ShowConfirmButton = false,
                Timer = 3000
            });
            await toas.FireAsync(message:"Evento actualizado con éxito");
        }

        private void Return()
        {
            eventForm!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo("/events");
        }
    }
}
