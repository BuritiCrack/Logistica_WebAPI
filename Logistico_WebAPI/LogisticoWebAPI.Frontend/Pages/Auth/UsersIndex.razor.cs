using CurrieTechnologies.Razor.SweetAlert2;
using LogisticoWebAPI.Frontend.Repositories;
using LogisticoWebAPI.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System.Net;

namespace LogisticoWebAPI.Frontend.Pages.Auth
{
    [Authorize(Roles = "Admin")]
    public partial class UsersIndex
    {
        public List<User>? Users { get; set; }
        private int CurrentPage = 1;
        private int TotalPages;

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Parameter, SupplyParameterFromQuery] public string Page { get; set; } = string.Empty;
        [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task OnPageChangedAsync(int page)
        {
            CurrentPage = page;
            await LoadAsync(page);
        }

        private async Task LoadAsync(int page = 1)
        {
            if (!string.IsNullOrWhiteSpace(Page))
            {
                page = Convert.ToInt32(Page);
            }

            var ok = await LoadListAync(page);
            if (ok)
            {
                await LoadPagesAsync();
            }
        }

        private async Task<bool> LoadListAync(int page)
        {
            var url = $"api/accounts/all?page={page}";
            if (!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"&filter={Filter}";
            }

            var responseHttp = await Repository.GetAsync<List<User>>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return false;
            }
            Users = responseHttp.Response;
            return true;
        }

        private async Task LoadPagesAsync()
        {
            var url = $"api/accounts/totalpages";
            if(!string.IsNullOrWhiteSpace(Filter))
            {
                url += $"?filter={Filter}";
            }
            var responseHttp = await Repository.GetAsync<int>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            TotalPages = responseHttp.Response;
        }
        private async Task CleanFilterAsync()
        {
            Filter = string.Empty;
            await ApplyfilterAsync();
        }

        private async Task ApplyfilterAsync()
        {
            int page = 1;
            await LoadAsync(page);
            await OnPageChangedAsync(page);
        }

        private async Task DeactivateUser(User user)
        {
            var action = user.IsActive ? "desactivar" : "activar";
            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = $"¿Estás seguro de que deseas {action} a {user.FullName}?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true,
                ConfirmButtonText = $"Sí, {action}",
                CancelButtonText = "Cancelar"
            });

            var confirm = string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }

            var responseHttp = await Repository.PutAsync($"api/accounts/{user.Id}", user);
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/Users");
                    return;
                }
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Errro", message, SweetAlertIcon.Error);
                return;
            }

            await LoadAsync();
            var toas = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Icon = SweetAlertIcon.Success,
                Toast = true,
                Position = SweetAlertPosition.TopEnd,
                ShowConfirmButton = false,
                Timer = 3000
            });
            await toas.FireAsync(message: "Registro actualizado con éxito");
        }
    }
}