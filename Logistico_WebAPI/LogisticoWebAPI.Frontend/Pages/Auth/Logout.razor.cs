using LogisticoWebAPI.Frontend.Services;
using Microsoft.AspNetCore.Components;

namespace LogisticoWebAPI.Frontend.Pages.Auth
{
    public partial class Logout
    {
        [Inject] private ILoginService LoginService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        protected override async Task OnInitializedAsync()
        {
            await LoginService.LogoutAsync();
            NavigationManager.NavigateTo("/");
        }
    }

}