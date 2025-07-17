using Microsoft.AspNetCore.Components;
using System.Reflection.Metadata.Ecma335;

namespace LogisticoWebAPI.Frontend.Shared
{
    public partial class Pagination
    {
        private List<PageModel> links = null!;

        [Parameter] public int CurrentPage { get; set; } = 1;

        [Parameter] public int TotalPages { get; set; } = 1;

        [Parameter] public int Radio { get; set; } = 10;

        [Parameter] public EventCallback<int> OnPageChanged { get; set; }

        protected override void OnParametersSet()
        {
            links = [];

            // Botón anterior
            links.Add(new PageModel
            {
                Text = "«",
                Page = CurrentPage - 1,
                IsEnable = CurrentPage > 1,
                IsCurrent = false
            });

            // Determinar el rango de páginas a mostrar
            int startPage, endPage;

            if (TotalPages <= Radio)
            {
                // Mostrar todas las páginas
                startPage = 1;
                endPage = TotalPages;
            }
            else
            {
                // Calcular rango basado en la página actual
                int halfRadio = Radio / 2;

                if (CurrentPage <= halfRadio)
                {
                    startPage = 1;
                    endPage = Radio;
                }
                else if (CurrentPage + halfRadio >= TotalPages)
                {
                    startPage = TotalPages - Radio + 1;
                    endPage = TotalPages;
                }
                else
                {
                    startPage = CurrentPage - halfRadio;
                    endPage = CurrentPage + halfRadio;
                }
            }

            // Agregar las páginas numéricas
            for (int i = startPage; i <= endPage; i++)
            {
                links.Add(new PageModel
                {
                    Page = i,
                    IsEnable = true, // Todas las páginas son clicables
                    IsCurrent = i == CurrentPage,
                    Text = $"{i}"
                });
            }

            // Botón siguiente
            links.Add(new PageModel
            {
                Text = "»",
                Page = CurrentPage + 1,
                IsEnable = CurrentPage < TotalPages,
                IsCurrent = false
            });
        }

        private async Task InternalSelectedPage(PageModel pageModel)
        {
            if (pageModel.Page == CurrentPage || pageModel.Page < 1 || pageModel.Page > TotalPages)
            {
                return;
            }
            await OnPageChanged.InvokeAsync(pageModel.Page);
        }

        private class PageModel
        {
            public string Text { get; set; } = null!;
            public int Page { get; set; }
            public bool IsCurrent { get; set; } = false;
            public bool IsEnable { get; set; } = true;
        }
    }
}