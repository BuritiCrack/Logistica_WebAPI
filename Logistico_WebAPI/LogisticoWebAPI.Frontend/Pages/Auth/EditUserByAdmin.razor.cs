using CurrieTechnologies.Razor.SweetAlert2;
using LogisticoWebAPI.Frontend.Repositories;
using LogisticoWebAPI.Frontend.Services;
using LogisticoWebAPI.Shared.DTOs;
using LogisticoWebAPI.Shared.Entities;
using Microsoft.AspNetCore.Components;
using System;
using System.Net;

namespace LogisticoWebAPI.Frontend.Pages.Auth
{
    public partial class EditUserByAdmin
    {
        private User? user;
        private List<State>? states;
        private List<City>? cities;
        private string? imageUrl;

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Parameter] public Guid Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await LoadUserAsync();
            if (user != null)
            {
                await LoadStatesAsync();
                if (user.City?.State?.Id != null)
                {
                    await LoadCitiesAsync(user.City.State.Id);
                }

                if (!string.IsNullOrEmpty(user.Photo))
                {
                    imageUrl = user.Photo;
                    user.Photo = null;
                }
            }
        }

        private void ImageSelected(string imagenBase64)
        {
            user!.Photo = imagenBase64;
            imageUrl = null;
        }

        private async Task LoadCitiesAsync(int stateId)
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

        private async Task LoadUserAsync()
        {
            var responseHttp = await Repository.GetAsync<User>($"/api/accounts/user/{Id}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/users");
                    return;
                }
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            user = responseHttp.Response;
        }
        private async Task StateChangedAsync(ChangeEventArgs change)
        {
            var selectedState = Convert.ToInt32(change.Value);
            cities = null;
            user!.CityId = 0;
            await LoadCitiesAsync(selectedState);
        }
        private async Task SaveUserAsync()
        {
            var responseHttp = await Repository.PutAsync<User>($"/api/accounts/edituser/{Id}", user!);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            await SweetAlertService.FireAsync("Éxito", "Usuario actualizado correctamente", SweetAlertIcon.Success);
            NavigationManager.NavigateTo("/users");
        }
    }
}