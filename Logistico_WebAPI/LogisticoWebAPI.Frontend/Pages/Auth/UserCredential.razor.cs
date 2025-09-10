using CurrieTechnologies.Razor.SweetAlert2;
using LogisticoWebAPI.Frontend.Repositories;
using LogisticoWebAPI.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;

namespace LogisticoWebAPI.Frontend.Pages.Auth
{
    [Authorize(Roles = "User,Coordinator")]
    public partial class UserCredential
    {
        private User? user;
        private bool isLoading;

        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            var responseHttp = await Repository.GetAsync<User>("api/accounts");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            user = responseHttp.Response;
            isLoading = false;
        }
    }
}