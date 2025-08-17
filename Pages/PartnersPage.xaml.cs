using CommunityToolkit.Maui.Views;
using DriveLoadr.Models;
namespace DriveLoadr.Pages;

public partial class PartnersPage : ContentPage
{
    private PartnersPageModel ViewModel;
    private readonly IServiceProvider _serviceProvider;

    public PartnersPage(PartnersPageModel model, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        ViewModel = model;
        BindingContext = ViewModel;
        _serviceProvider = serviceProvider;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.LoadPartners();
    }

    private async void AddPartner_Clicked(object sender, EventArgs e)
    {
        await ViewModel.AddPartnerAsync(this);
    }

    private async void EditPartner_Clicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is Partner partner)
        {
            await ViewModel.EditPartnerAsync(this, partner);
        }
    }

    private async void DeletePartner_Clicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is Partner partner)
        {
            await ViewModel.DeletePartnerAsync(this, partner);
        }
    }

    private async void AddJobType_Clicked(object sender, EventArgs e)
    {
        // await Shell.Current.GoToAsync(nameof(JobTypesPage));

        if (sender is Button button && button.BindingContext is Partner partner)
        {
            await Shell.Current.GoToAsync($"{nameof(JobTypesPage)}?PartnerId={partner.Id}");
        }
    }
}
