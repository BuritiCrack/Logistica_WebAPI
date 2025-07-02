using CurrieTechnologies.Razor.SweetAlert2;
using LogisticoWebAPI.Frontend.Repositories;
using LogisticoWebAPI.Frontend.Services;
using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Enums;
using Microsoft.AspNetCore.Components;

namespace LogisticoWebAPI.Frontend.Pages.Auth
{
    public partial class Register
    {
        private UserDTO userDTO = new();

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private ILoginService LoginService { get; set; } = null!;
        

        private async Task CreateUserAsync()
        {
            userDTO.UserName = userDTO.Email;
            userDTO.UserType = UserType.User;

            var responseHttp = await Repository.PostAsync<UserDTO, TokenDTO>("/api/accounts/CreateUser",userDTO);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            await LoginService.LoginAsync(responseHttp.Response!.Token);
            NavigationManager.NavigateTo("/");
        }
    }
}