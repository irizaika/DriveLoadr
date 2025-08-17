using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel; // optional for MVVM attributes
using DriveLoadr.Models;
using DriveLoadr.Pages.PopupPages;
using System.Collections.ObjectModel;

namespace DriveLoadr.PageModels;

public class VanListPageModel : ObservableObject // from CommunityToolkit.Mvvm.ComponentModel
{
    private readonly IServiceProvider _serviceProvider;
    private readonly VanRepository _vanRepository;

    // Observable collection of contractors
    private ObservableCollection<Van> _vanList;
    public ObservableCollection<Van> VanList
    {
        get => _vanList;
        set => SetProperty(ref _vanList, value); // automatically raises PropertyChanged
    }

    public VanListPageModel(IServiceProvider serviceProvider, VanRepository vanRepository)
    {
        VanList = new ObservableCollection<Van>();
        _serviceProvider = serviceProvider;
        _vanRepository = vanRepository;
    }

    // Load contractors from database
    public async Task LoadVanList()
    {
        // Example:
         VanList = new ObservableCollection<Van>(await _vanRepository.GetVanListAsync());
    }

    // Command methods can be wired from XAML or code-behind
    public async Task AddVanAsync(ContentPage page)
    {
        var popup = _serviceProvider.GetRequiredService<AddVanPopup>();
        popup.Initialize();

        var result = await page.ShowPopupAsync(popup);

        if (result is bool saved && saved)
        {
            await LoadVanList();
        }
    }

    public async Task EditVanAsync(ContentPage page, Van van)
    {
        var popup = _serviceProvider.GetRequiredService<AddVanPopup>();
        popup.Initialize(van);
        var result = await page.ShowPopupAsync(popup);

        if (result is bool saved && saved)
        {
            await LoadVanList();
        }
    }

    public async Task DeleteVanAsync(ContentPage page, Van van)
    {
        var answer = await page.DisplayAlert(
            $"Confirm to delete record",
            $"{van.VanName}: {van.Details} {van.Plate}",
            "Confirm",
            "Cancel");
        if (answer)
        {
            var result = await _vanRepository.DeleteVanAsync(van);
            if (result > 0)
                await page.DisplayAlert("Success", "Van deleted successfully", "OK");
            else
                await page.DisplayAlert("Error", "Failed to delete van", "OK");

            await LoadVanList();
        }
    }
}
