using Blazored.Modal;
using Blazored.Modal.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using LogisticoWebAPI.Frontend.Repositories;
using LogisticoWebAPI.Frontend.Services;
using LogisticoWebAPI.Shared.DTOs;
using Microsoft.AspNetCore.Components;

namespace LogisticoWebAPI.Frontend.Pages.Auth
{
    public partial class Login
    {
        private LoginDTO loginDTO = new();
        private bool wasClose;
        private bool isLoading;
        [Inject] private ILoginService LoginService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [CascadingParameter] private BlazoredModalInstance BlazoredModal { get; set; } = default!;

        private async Task CloseModalAsync()
        {
            wasClose = true;
            await BlazoredModal.CloseAsync(ModalResult.Ok());
        }
        private async Task LoginAsync()
        {
            if (wasClose)
            {
                NavigationManager.NavigateTo("/");
                return;
            }
            isLoading = true;
            var responseHttp = await Repository.PostAsync<LoginDTO, TokenDTO>("/api/accounts/Login", loginDTO);
            if (responseHttp.Error)
            {
                isLoading = false;
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Ups!", message, SweetAlertIcon.Error);
                return;
            }
            isLoading = false;
            await LoginService.LoginAsync(responseHttp.Response!.Token);
            NavigationManager.NavigateTo("/");
        }
    }
}