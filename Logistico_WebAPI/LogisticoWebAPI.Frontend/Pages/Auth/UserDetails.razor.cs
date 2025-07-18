using CurrieTechnologies.Razor.SweetAlert2;
using LogisticoWebAPI.Frontend.Repositories;
using LogisticoWebAPI.Shared.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Threading.Tasks;

namespace LogisticoWebAPI.Frontend.Pages.Auth
{
    [Authorize(Roles = "Admin")]
    public partial class UserDetails
    {
        public User? User { get; set; }
        public bool IsLoading { get; set; } = true;

        [Parameter] public string Id { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

        protected override async Task OnParametersSetAsync()
        {
            var responseHttp = await Repository.GetAsync<User>($"api/accounts/user/{Id}");
            IsLoading = true;
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/users");
                    return;
                }
                else
                {
                    var message = await responseHttp.GetErrorMessageAsync();
                    await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                    return;
                }
            }
            else { User = responseHttp.Response; }
            IsLoading = false;
        }

        private void GoBack()
        {
            NavigationManager.NavigateTo("/users");
        }
    }
}