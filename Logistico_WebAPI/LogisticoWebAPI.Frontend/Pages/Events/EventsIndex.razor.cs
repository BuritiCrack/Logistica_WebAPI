using LogisticoWebAPI.Frontend.Repositories;
using LogisticoWebAPI.Shared.Entities;
using Microsoft.AspNetCore.Components;

namespace LogisticoWebAPI.Frontend.Pages.Events
{
    public partial class EventsIndex
    {
        [Inject] private IRepository Repository { get; set; } = null!;

        public List<Event>? Events { get; set; }

        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var responseHttp = await Repository.GetAsync<List<Event>>("api/events");
            Events = responseHttp.Response;
        }
    }
}