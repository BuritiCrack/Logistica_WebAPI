using CurrieTechnologies.Razor.SweetAlert2;
using LogisticoWebAPI.Frontend.Repositories;
using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using LogisticoWebAPI.Shared.Enums;
using Microsoft.AspNetCore.Components;

namespace LogisticoWebAPI.Frontend.Pages.Auth
{
    public partial class Register
    {
        private UserDTO userDTO = new();
        private List<State>? states;
        private List<City>? cities;
        private bool isLoading;
        private string? imageUrl;

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            await LoadStatesAsync();
        }

        private void ImageSelected(string imagenBase64)
        {
            userDTO.Photo = imagenBase64;
            imageUrl = null;
        }

        private async Task LoadStatesAsync()
        {
            var responseHttp = await Repository.GetAsync<List<State>>("/api/states/combo");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            states = responseHttp.Response;
        }

        private async Task StateChangedAsync(ChangeEventArgs e)
        {
            var selectedState = Convert.ToInt32(e.Value!);
            cities = null;
            userDTO.CityId = 0;
            await LoadCitiesAsyn(selectedState);
        }

        private async Task LoadCitiesAsyn(int stateId)
        {
            var responseHttp = await Repository.GetAsync<List<City>>($"/api/cities/combo/{stateId}");
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            cities = responseHttp.Response;
        }

        private async Task CreateUserAsync()
        {
            isLoading = true;
            userDTO.UserName = userDTO.Email;
            userDTO.UserType = UserType.User;
            var responseHttp = await Repository.PostAsync<UserDTO>("/api/accounts/CreateUser", userDTO);
            isLoading = false;
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            await SweetAlertService.FireAsync("Éxito", "Usuario creado exitosamente. Por favor, confirme " +
                "su cuenta a través del correo electrónico.", SweetAlertIcon.Info);
            NavigationManager.NavigateTo("/");
        }
    }
}