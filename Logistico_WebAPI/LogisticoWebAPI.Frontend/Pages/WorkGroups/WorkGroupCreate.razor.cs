using CurrieTechnologies.Razor.SweetAlert2;
using LogisticoWebAPI.Frontend.Repositories;
using LogisticoWebAPI.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace LogisticoWebAPI.Frontend.Pages.WorkGroups
{
    [Authorize(Roles = "Admin,SuperAdmin,Coordinator")]
    public partial class WorkGroupCreate
    {
        private WorkGroupDTO WorkGroupDTO = new();
        private WorkGroupForm? workGroupForm;
        private bool IsLoading;
        [Inject] public IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Parameter] public int EventId { get; set; }

        private async Task CreateAsync()
        {
            WorkGroupDTO.EventId = EventId;
            IsLoading = true;
            var responseHttp = await Repository.PostAsync("api/workgroups/full", WorkGroupDTO);
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
            await toas.FireAsync(message: "Registro creado con éxito");
        }

        private void Return()
        {
            workGroupForm!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo($"/events/workgroups/{EventId}");
        }
    }
}