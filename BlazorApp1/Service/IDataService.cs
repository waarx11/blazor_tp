using BlazorApp1.Components;
using BlazorApp1.Models;

namespace BlazorApp1.Service
{
    public interface IDataService
    {
        Task Add(ItemModel model);
        Task<int> Count();
        Task<List<Item>> List(int currentPage, int pageSize);
        Task<Item> GetById(int id);
        Task Update(int id, ItemModel model);

        Task Delete(int id);
        Task<List<CraftingRecipe>> GetRecipes();
    }
}
