using CurrieTechnologies.Razor.SweetAlert2;
using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;

namespace LogisticoWebAPI.Frontend.Pages.Events
{
    public partial class EventForm
    {
        private EditContext editContext = null!;
        private string? imageUrl;

        [EditorRequired, Parameter] public EventDTO EventDTO { get; set; } = null!;
        [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
        [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }
        [Inject] public SweetAlertService SweetAlertService { get; set; } = null!;
        public bool FormPostedSuccessfully { get; set; }

        protected override void OnInitialized()
        {
            editContext = new EditContext(EventDTO);
        }

        private void ImageSelected(string imageBase64)
        {
            EventDTO.Photo = imageBase64;
            imageUrl = null;
        }
        private async Task OnBeforeInternalNavigation(LocationChangingContext context)
        {
            var formWassEdited = editContext.IsModified();
            if(!formWassEdited || FormPostedSuccessfully) 
            {
                return;
            }

            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmacion",
                Text = "¿Deseas abandonar la página y perder los cambios?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true
            });

            var confirm = !string.IsNullOrEmpty(result.Value);
            if(confirm)
            {
                return;
            }

            context.PreventNavigation();
        }



    }
}