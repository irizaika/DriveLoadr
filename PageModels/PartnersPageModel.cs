using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel; // optional for MVVM attributes
using DriveLoadr.Models;
using DriveLoadr.Pages.PopupPages;
using System.Collections.ObjectModel;

namespace DriveLoadr.PageModels;

public class PartnersPageModel : ObservableObject // from CommunityToolkit.Mvvm.ComponentModel
{
    private readonly PartnerRepository _partnerRepository;
    private readonly IServiceProvider _serviceProvider;


    // Observable collection of partners
    private ObservableCollection<Partner> _partner;
    public ObservableCollection<Partner> Partners
    {
        get => _partner;
        set => SetProperty(ref _partner, value); // automatically raises PropertyChanged
    }

    public PartnersPageModel(PartnerRepository partnerRepository, IServiceProvider serviceProvider)
    {
        Partners = new ObservableCollection<Partner>();
        _partnerRepository = partnerRepository;
        _serviceProvider = serviceProvider;
    }

    // Load contractors from database
    public async Task LoadPartners()
    {
        // Example:
         Partners = new ObservableCollection<Partner>(await _partnerRepository.GetPartnersListAsync());
    }

    // Command methods can be wired from XAML or code-behind
    public async Task AddPartnerAsync(ContentPage page)
    {
        var popup = _serviceProvider.GetRequiredService<AddPartnerPopup>();
        popup.Initialize();
        var result = await page.ShowPopupAsync(popup);

        if (result is bool saved && saved)
        {
            await LoadPartners();
        }
    }

    public async Task EditPartnerAsync(ContentPage page, Partner partner)
    {
        var popup = _serviceProvider.GetRequiredService<AddPartnerPopup>();
        popup.Initialize(partner);
        var result = await page.ShowPopupAsync(popup);

        if (result is bool saved && saved)
        {
            await LoadPartners();
        }
    }

    public async Task DeletePartnerAsync(ContentPage page, Partner partner)
    {
        var answer = await page.DisplayAlert(
                $"Confirm to delete record",
                $"{partner.CompanyName}",
                "Confirm",
                "Cancel");
        if (answer)
        {
            var result = await _partnerRepository.DeletePartnerAsync(partner);
            if (result > 0)
                await page.DisplayAlert("Success", "Partner deleted successfully", "OK");
            else
                await page.DisplayAlert("Error", "Failed to delete partner", "OK");

            await LoadPartners();
        }
    }

    public async Task AddJobTypeAsync(ContentPage page)
    {
        var popup = _serviceProvider.GetRequiredService<AddJobTypePopup>();
        popup.Initialize();

        var result = await page.ShowPopupAsync(popup);

        if (result is bool saved && saved)
        {
            //   await LoadJobTypes();
        }
    }
}
