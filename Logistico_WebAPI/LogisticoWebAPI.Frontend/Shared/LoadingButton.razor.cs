using Microsoft.AspNetCore.Components;

namespace LogisticoWebAPI.Frontend.Shared
{
    public partial class LoadingButton
    {
        [Parameter] public bool IsLoading { get; set; } = false;
        [Parameter] public string Text { get; set; } = "Guardar";
        [Parameter] public string LoadingText { get; set; } = "Procesando...";
        [Parameter] public string Icon { get; set; } = "";
        [Parameter] public string CssClass { get; set; } = "btn btn-primary";
        [Parameter] public string ButtonType { get; set; } = "submit";
        [Parameter] public bool Disabled { get; set; } = false;
        [Parameter] public EventCallback OnClick { get; set; }
    }
}