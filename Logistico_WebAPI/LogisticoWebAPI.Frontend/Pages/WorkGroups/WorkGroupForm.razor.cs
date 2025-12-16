using CurrieTechnologies.Razor.SweetAlert2;
using LogisticoWebAPI.Frontend.Repositories;
using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;

namespace LogisticoWebAPI.Frontend.Pages.WorkGroups
{
    [Authorize(Roles = "Admin,SuperAdmin,Coordinator")]
    public partial class WorkGroupForm
    {
        private EditContext editContext = null!;
        private bool IsLoadingCoordinator;
        [Inject] public IRepository Repository { get; set; } = null!;
        [Inject] public SweetAlertService SweetAlertService { get; set; } = null!;
        [EditorRequired, Parameter] public WorkGroupDTO WorkGroupDTO { get; set; } = null!;
        [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
        [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }
        [Parameter] public bool IsLoading { get; set; }
        [Parameter] public List<User>? Coordinators { get; set; } = new();
        public bool FormPostedSuccessfully { get; set; } = false;

        protected override void OnInitialized()
        {
            editContext = new(WorkGroupDTO);
        }

        private async Task OnBeforeInternalNavigation(LocationChangingContext context)
        {
            var formWassEdited = editContext.IsModified();
            if (!formWassEdited || FormPostedSuccessfully)
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
            if (confirm)
            {
                return;
            }

            context.PreventNavigation();
        }
    }
}