using Blazored.Modal.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using LogisticoWebAPI.Frontend.Repositories;
using Microsoft.AspNetCore.Components;

namespace LogisticoWebAPI.Frontend.Pages.Auth
{
    public partial class ConfirmEmail
    {
        private string? message;

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;

        [Parameter, SupplyParameterFromQuery] public string UserId { get; set; } = string.Empty;
        [Parameter, SupplyParameterFromQuery] public string Token { get; set; } = string.Empty;
        [CascadingParameter] private IModalService Modal { get; set; } = default!;

        private void ShowLogin()
        {
            Modal.Show<Login>();
        }
        protected async Task ConfirmAccountAsync()
        {
            var responseHttp = await Repository.GetAsync($"/api/accounts/ConfirmEmail/?userId={UserId}&token={Token}");
            if (responseHttp.Error)
            {
                message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                NavigationManager.NavigateTo("/");
                return;
            }

            await SweetAlertService.FireAsync("Éxito", "Cuenta confirmada exitosamente.", SweetAlertIcon.Success);
            Modal.Show<Login>();
        }
    }
}