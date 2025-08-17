
using DriveLoadr.Models;

namespace DriveLoadr.Pages;

public partial class VanListPage : ContentPage
{
    private VanListPageModel ViewModel;

    public VanListPage(VanListPageModel model)
    {
        InitializeComponent();
        ViewModel = model;
        BindingContext = ViewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.LoadVanList();
    }

    private async void AddVan_Clicked(object sender, EventArgs e)
    {
        await ViewModel.AddVanAsync(this);
    }

    private async void EditVan_Clicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is Van van)
        {
            await ViewModel.EditVanAsync(this, van);
        }
    }

    private async void DeleteVan_Clicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is Van van)
        {
            await ViewModel.DeleteVanAsync(this, van);
        }
    }
}
