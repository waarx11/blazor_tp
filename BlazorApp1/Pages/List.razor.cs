using BlazorApp1.Models;
using Blazored.LocalStorage;
using Blazorise.DataGrid;
using Microsoft.AspNetCore.Components;
using Blazored.Modal.Services;
using Blazored.Modal;
using BlazorApp1.Modals;
using BlazorApp1.Service;
using Microsoft.Extensions.Localization;

namespace BlazorApp1.Pages;

public partial class List
{
    [Inject]
    public IStringLocalizer<List> Localizer { get; set; }
    private List<Item> items;

    private int totalItem;

    [Inject]
    public HttpClient Http { get; set; }

    [Inject]
    public ILocalStorageService LocalStorage { get; set; }

    [Inject]
    public IWebHostEnvironment WebHostEnvironment { get; set; }

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [CascadingParameter]
    public IModalService Modal { get; set; }

    [Inject]
    public IDataService DataService { get; private set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        // Do not treat this action if is not the first render
        if (!firstRender)
        {
            return;
        }

        var currentData = await LocalStorage.GetItemAsync<Item[]>("data");

        // Check if data exist in the local storage
        if (currentData == null)
        {
            // this code add in the local storage the fake data (we load the data sync for initialize the data before load the OnReadData method)
            var originalData = Http.GetFromJsonAsync<Item[]>($"{NavigationManager.BaseUri}fake-data.json").Result;
            await LocalStorage.SetItemAsync("data", originalData);
        }
    }

    private async Task OnReadData(DataGridReadDataEventArgs<Item> e)
    {
        if (e.CancellationToken.IsCancellationRequested)
        {
            return;
        }

        // When you use a real API, we use this follow code
        //var response = await Http.GetJsonAsync<Data[]>( $"http://my-api/api/data?page={e.Page}&pageSize={e.PageSize}" );
        var response = (await LocalStorage.GetItemAsync<Item[]>("data")).Skip((e.Page - 1) * e.PageSize).Take(e.PageSize).ToList();

        if (!e.CancellationToken.IsCancellationRequested)
        {
            totalItem = (await LocalStorage.GetItemAsync<List<Item>>("data")).Count;
            items = new List<Item>(response); // an actual data for the current page
        }
    }

    private async void OnDelete(int id)
    {
        var parameters = new ModalParameters();
        parameters.Add(nameof(Item.Id), id);

        var modal = Modal.Show<DeleteConfirmation>("Delete Confirmation", parameters);
        var result = await modal.Result;

        if (result.Cancelled)
        {
            return;
        }

        await DataService.Delete(id);

        // Reload the page
        NavigationManager.NavigateTo("list", true);
    }
}