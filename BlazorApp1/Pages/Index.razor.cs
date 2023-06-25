using BlazorApp1.Components;
using BlazorApp1.Models;
using BlazorApp1.Service;
using Microsoft.AspNetCore.Components;

namespace BlazorApp1.Pages
{
    public partial class Index
    {
        /*private Cake CakeItem = new Cake
        {
            Id = 1,
            Name = "Black Forest",
            Cost = 50
        };

        public List<Cake> Cakes { get; set; }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            LoadCakes();
            StateHasChanged();
            return base.OnAfterRenderAsync(firstRender);
        }

        public void LoadCakes()
        {
            Cakes = new List<Cake>
        {
            // items hidden for display purpose
            new Cake
            {
                Id = 1,
                Name = "Red Velvet",
                Cost = 60
            },
        };
        }*/
        [Inject]
        public IDataService DataService { get; set; }

        public List<Item> Items { get; set; } = new List<Item>();

        private List<CraftingRecipe> Recipes { get; set; } = new List<CraftingRecipe>();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            base.OnAfterRenderAsync(firstRender);

            if (!firstRender)
            {
                return;
            }

            Items = await DataService.List(0, await DataService.Count());
            Recipes = await DataService.GetRecipes();

            StateHasChanged();
        }

    }
}
