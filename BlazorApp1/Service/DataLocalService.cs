using BlazorApp1.Components;
using BlazorApp1.Factories;
using BlazorApp1.Models;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;

namespace BlazorApp1.Service
{
    public class DataLocalService : IDataService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigationManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public DataLocalService(
            ILocalStorageService localStorage,
            HttpClient http,
            IWebHostEnvironment webHostEnvironment,
            NavigationManager navigationManager)
        {
            _localStorage = localStorage;
            _http = http;
            _webHostEnvironment = webHostEnvironment;
            _navigationManager = navigationManager;
        }

        public async Task Add(ItemModel model)
        {
            // Get the current data
            var currentData = await _localStorage.GetItemAsync<List<Item>>("data");

            // Simulate the Id
            model.Id = currentData.Max(s => s.Id) + 1;

            // Add the item to the current data
            currentData.Add(ItemFactory.Create(model));

           /* // Save the image Supprimer si API
            var imagePathInfo = new DirectoryInfo($"{_webHostEnvironment.WebRootPath}/images");

            // Check if the folder "images" exist
            if (!imagePathInfo.Exists)
            {
                imagePathInfo.Create();
            }

            // Determine the image name
            var fileName = new FileInfo($"{imagePathInfo}/{model.Name}.png");

            // Write the file content
            await File.WriteAllBytesAsync(fileName.FullName, model.ImageContent);*/

            // Save the data
            await _localStorage.SetItemAsync("data", currentData);
        }

        public async Task<int> Count()
        {
            return (await _localStorage.GetItemAsync<Item[]>("data")).Length;
        }

        public async Task<List<Item>> List(int currentPage, int pageSize)
        {
            // Load data from the local storage
            var currentData = await _localStorage.GetItemAsync<Item[]>("data");

            // Check if data exist in the local storage
            if (currentData == null)
            {
                // this code add in the local storage the fake data
                var originalData = await _http.GetFromJsonAsync<Item[]>($"{_navigationManager.BaseUri}fake-data.json");
                await _localStorage.SetItemAsync("data", originalData);
            }

            return (await _localStorage.GetItemAsync<Item[]>("data")).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        }
        public async Task<Item> GetById(int id)
        {
            // Get the current data
            var currentData = await _localStorage.GetItemAsync<List<Item>>("data");

            // Get the item int the list
            var item = currentData.FirstOrDefault(w => w.Id == id);

            // Check if item exist
            if (item == null)
            {
                throw new Exception($"Unable to found the item with ID: {id}");
            }

            return item;
        }

        public async Task Update(int id, ItemModel model)
        {
            // Get the current data
            var currentData = await _localStorage.GetItemAsync<List<Item>>("data");

            // Get the item int the list
            var item = currentData.FirstOrDefault(w => w.Id == id);

            // Check if item exist
            if (item == null)
            {
                throw new Exception($"Unable to found the item with ID: {id}");
            }

            /*// Save the image Supprimer si API
            var imagePathInfo = new DirectoryInfo($"{_webHostEnvironment.WebRootPath}/images");

            // Check if the folder "images" exist
            if (!imagePathInfo.Exists)
            {
                imagePathInfo.Create();
            }

            // Delete the previous image
            if (item.Name != model.Name)
            {
                var oldFileName = new FileInfo($"{imagePathInfo}/{item.Name}.png");

                if (oldFileName.Exists)
                {
                    File.Delete(oldFileName.FullName);
                }
            }

            // Determine the image name
            var fileName = new FileInfo($"{imagePathInfo}/{model.Name}.png");

            // Write the file content
            await File.WriteAllBytesAsync(fileName.FullName, model.ImageContent);*/

            // Modify the content of the item
            ItemFactory.Update(item, model);

            // Save the data
            await _localStorage.SetItemAsync("data", currentData);
        }

        public async Task Delete(int id)
        {
            // Get the current data
            var currentData = await _localStorage.GetItemAsync<List<Item>>("data");

            // Get the item int the list
            var item = currentData.FirstOrDefault(w => w.Id == id);

            // Delete item in
            currentData.Remove(item);

            /*// Delete the image Supprimer si API
            var imagePathInfo = new DirectoryInfo($"{_webHostEnvironment.WebRootPath}/images");
            var fileName = new FileInfo($"{imagePathInfo}/{item.Name}.png");

            if (fileName.Exists)
            {
                File.Delete(fileName.FullName);
            }*/

            // Save the data
            await _localStorage.SetItemAsync("data", currentData);
        }
        public Task<List<CraftingRecipe>> GetRecipes()
        {
            var items = new List<CraftingRecipe>
        {
            new CraftingRecipe
            {
                Give = new Item { DisplayName = "Diamond", Name = "diamond" },
                Have = new List<List<string>>
                {
                    new List<string> { "dirt", "dirt", "dirt" },
                    new List<string> { "dirt", null, "dirt" },
                    new List<string> { "dirt", "dirt", "dirt" }
                }
            }
        };

            return Task.FromResult(items);
        }
    }
}
