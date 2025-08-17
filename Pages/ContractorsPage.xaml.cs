namespace DriveLoadr.Pages;

public partial class ContractorsPage : ContentPage
{
    private ContractorsPageModel ViewModel;

    public ContractorsPage(ContractorsPageModel model)
    {
        InitializeComponent();
        ViewModel = model;
        BindingContext = model;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.LoadContractors();
    }
    private async void AddContractor_Clicked(object sender, EventArgs e)
    {
        await ViewModel.AddContractorAsync(this);
    }

    private async void EditContractor_Clicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is ContractorDisplay contractor)
        {
            await ViewModel.EditContractorAsync(this, contractor);
        }
    }

    private async void DeleteContractor_Clicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is ContractorDisplay contractor)
        {
            await ViewModel.DeleteContractorAsync(this, contractor);
        }
    }

    private async void AddJobType_Clicked(object sender, EventArgs e)
    {
        await ViewModel.AddJobTypeAsync(this);
    }
}
