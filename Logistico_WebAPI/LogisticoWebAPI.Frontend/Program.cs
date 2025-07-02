using CurrieTechnologies.Razor.SweetAlert2;
using LogisticoWebAPI.Frontend.AuthenticationProviders;
using LogisticoWebAPI.Frontend.Repositories;
using LogisticoWebAPI.Frontend.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace LogisticoWebAPI.Frontend;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7172/") });
        builder.Services.AddSweetAlert2();
        builder.Services.AddScoped<IRepository, Repository>();
        builder.Services.AddAuthorizationCore();

        builder.Services.AddScoped<AuthenticationProviderJWT>();
        builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationProviderJWT>(x => x.GetRequiredService<AuthenticationProviderJWT>());
        builder.Services.AddScoped<ILoginService, AuthenticationProviderJWT>(x => x.GetRequiredService<AuthenticationProviderJWT>());


        await builder.Build().RunAsync();
    }
}
