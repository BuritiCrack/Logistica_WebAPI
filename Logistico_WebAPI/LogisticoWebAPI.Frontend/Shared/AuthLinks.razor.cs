using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace LogisticoWebAPI.Frontend.Shared
{
    public partial class AuthLinks
    {
        private string? photoUser;

        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationState { get; set; } = null!;

        protected override async Task OnParametersSetAsync()
        {
            var authenticationState = await AuthenticationState;
            var claims = authenticationState.User.Claims.ToList();
            var photoClaim = claims.FirstOrDefault(x => x.Type == "Photo");

            if (photoClaim is not null)
            {
                photoUser = photoClaim.Value;
            }
        }
    }
}