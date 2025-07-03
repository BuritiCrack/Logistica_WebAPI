using CurrieTechnologies.Razor.SweetAlert2;
using LogisticoWebAPI.Frontend.Repositories;
using LogisticoWebAPI.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace LogisticoWebAPI.Frontend.Pages.Events
{
    [Authorize(Roles = "Admin")]
    public partial class EventCreate
    {
        private Event Event { get; set; } = new Event();
        private EventForm? eventForm;

        [Inject] public IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

        protected override void OnInitialized()
        {
            SetDefaultDates();
        }

        private void SetDefaultDates()
        {
            var now = DateTime.Now;

            // Establecer fecha y hora de inicio
            var startTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0).AddHours(1);
            Event.FechaHoraInicio = startTime;

            // Establecer fecha y hora de fin: 12 horas después del inicio
            Event.FechaHoraFin = startTime.AddHours(12);
        }

        private async Task CreateAsync()
        { 
            var responseHttp = await Repository.PostAsync("api/events", Event);
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
            await toas.FireAsync(message:"Registro creado con éxito");
        }

        private void Return()
        {
            eventForm!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo("/events");
        }
    }
}