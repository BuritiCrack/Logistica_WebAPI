using CurrieTechnologies.Razor.SweetAlert2;
using LogisticoWebAPI.Frontend.Repositories;
using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
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
        private bool IsLoadingCoordinator;
        private List<User> Coordinators = [];
        private CurrentUserDTO? CurrentUser;
        [Inject] public IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Parameter] public int EventId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadCurrentUserAsync();

            if (CurrentUser?.UserType == "Coordinator")
            {
                WorkGroupDTO.CoordinatorId = CurrentUser.UserId;
            }
            else
            {
                await LoadCoordinatorsAsync();
            }
        }

        private async Task LoadCurrentUserAsync()
        {
            IsLoading = true;
            var responseHttp = await Repository.GetAsync<CurrentUserDTO>("api/accounts/currentuser");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message);
                return;
            }
            CurrentUser = responseHttp.Response!;
            IsLoading = false;
        }

        private async Task LoadCoordinatorsAsync()
        {
            IsLoadingCoordinator = true;
            var responseHttp = await Repository.GetAsync<List<User>>($"api/workgroups/Coordinators/{EventId}");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message);
                return;
            }
            Coordinators = responseHttp.Response!;
            IsLoadingCoordinator = false;
        }

        private async Task CreateAsync()
        {
            if (CurrentUser?.UserType != "Coordinator" && string.IsNullOrEmpty(WorkGroupDTO.CoordinatorId))
            {
                await SweetAlertService.FireAsync("Error", "Debe seleccionar un coordinador para el grupo de trabajo.", SweetAlertIcon.Error);
                return;
            }
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