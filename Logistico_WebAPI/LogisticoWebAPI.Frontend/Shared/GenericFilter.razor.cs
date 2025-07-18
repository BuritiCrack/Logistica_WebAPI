using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace LogisticoWebAPI.Frontend.Shared
{
    public partial class GenericFilter
    {
        [Parameter]
        public string TexToFilter { get; set; } = string.Empty;

        [Parameter]
        public EventCallback<string> TexToFilterChanged { get; set; }

        [Parameter]
        public string TexFromFilter { get; set; } = string.Empty;

        [Parameter]
        public string PlaceHolder { get; set; } = string.Empty;

        [Parameter]
        public Func<string, Task> Callback { get; set; }
            = async (text) => await Task.CompletedTask;

        private async Task CleanFilter()
        {
            TexToFilter = string.Empty;
            await TexToFilterChanged.InvokeAsync(TexToFilter);
            await Callback(string.Empty);
        }

        private async Task ApplyFilterAsync()
        {
            await TexToFilterChanged.InvokeAsync(TexToFilter);
            await Callback(TexToFilter);
        }

        private async Task OnKeyPress(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await ApplyFilterAsync();
            }
        }

        private async Task OnInputChanged(ChangeEventArgs e)
        {
            TexToFilter = e.Value?.ToString() ?? string.Empty;
            await TexToFilterChanged.InvokeAsync(TexToFilter);
        }
    }
}