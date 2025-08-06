using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace LogisticoWebAPI.Frontend.Shared
{
    public partial class InputImg
    {
        private string? imageBase64;
        private bool isDragOver = false;
        private InputFile? fileInput;

        [Parameter] public string Label { get; set; } = "Seleccionar Imagen";
        [Parameter] public string? ImageURL { get; set; }
        [Parameter] public EventCallback<string> ImageSelected { get; set; }
        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;

        private async Task OnChange(InputFileChangeEventArgs e)
        {
            var imagenes = e.GetMultipleFiles();

            foreach (var imagen in imagenes)
            {
                // Validar tamaño del archivo (5MB máximo)
                if (imagen.Size > 5 * 1024 * 1024)
                {
                    // Aquí podrías mostrar un mensaje de error
                    continue;
                }

                var arrBytes = new byte[imagen.Size];
                await imagen.OpenReadStream(5 * 1024 * 1024).ReadAsync(arrBytes);
                imageBase64 = Convert.ToBase64String(arrBytes);
                ImageURL = null;
                await ImageSelected.InvokeAsync(imageBase64);
                StateHasChanged();
            }
        }

        private async Task TriggerFileInput()
        {
            if (fileInput?.Element != null)
            {
                await JSRuntime.InvokeVoidAsync("triggerFileInput", fileInput.Element);
            }
        }

        private async Task RemoveImage()
        {
            imageBase64 = null;
            ImageURL = null;
            await ImageSelected.InvokeAsync(string.Empty);
            StateHasChanged();
        }

        private void HandleDragEnter()
        {
            isDragOver = true;
        }

        private void HandleDragLeave()
        {
            isDragOver = false;
        }

        private async Task HandleDrop(DragEventArgs e)
        {
            isDragOver = false;

            try
            {
                // Usar JavaScript para obtener los archivos del evento drop
                var fileDataArray = await JSRuntime.InvokeAsync<string[]>("handleFileDrop", e);

                if (fileDataArray != null && fileDataArray.Length > 0)
                {
                    var fileData = fileDataArray[0]; // Tomar el primer archivo

                    if (!string.IsNullOrEmpty(fileData))
                    {
                        imageBase64 = fileData;
                        ImageURL = null;
                        await ImageSelected.InvokeAsync(imageBase64);
                        StateHasChanged();
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar errores silenciosamente o mostrar mensaje
                Console.WriteLine($"Error al procesar archivo: {ex.Message}");
            }
        }
    }
}