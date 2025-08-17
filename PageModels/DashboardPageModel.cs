using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriveLoadr.Pages;

namespace DriveLoadr.PageModels;

public partial class DashboardPageModel : ObservableObject
{
    [ObservableProperty]
    private string _today = DateTime.Now.ToString("dddd, MMM d");

    [RelayCommand]
    private async Task ViewContractorsData()
    {
        // Navigate to your data list page
        await Shell.Current.GoToAsync(nameof(ContractorsPage));
    }

    [RelayCommand]
    private async Task ViewVansData()
    {
        // Navigate to your data list page
        await Shell.Current.GoToAsync(nameof(VanListPage));
    }

    [RelayCommand]
    private async Task ViewPartnersData()
    {
        // Navigate to your data list page
        await Shell.Current.GoToAsync(nameof(PartnersPage));
    }

    [RelayCommand]
    private async Task ViewJobTypesData()
    {
        // Navigate to your data list page
        await Shell.Current.GoToAsync(nameof(JobTypesPage));
    }

    [RelayCommand]
    private async Task GoToShedule()
    {
        // Navigate to your data list page
        await Shell.Current.GoToAsync(nameof(JobPlanningPage));
    }

    [RelayCommand]
    private async Task GeneratePartnerReports()
    {
        // Navigate to reports page or popup
        await Shell.Current.GoToAsync(nameof(PartnerReportPage));
    }

    [RelayCommand]
    private async Task GenerateCustomerReports()
    {
        // Navigate to reports page or popup
        await Shell.Current.GoToAsync(nameof(CustomerReportPage));
    }

    [RelayCommand]
    private async Task GenerateContractorReports()
    {
        // Navigate to reports page or popup
        await Shell.Current.GoToAsync(nameof(ContractorReportPage));
    }

    [RelayCommand]
    private async Task CreateInvoice()
    {
        // Navigate to invoice creation page
      //  await Shell.Current.GoToAsync(nameof(InvoicePage));
    }
}
