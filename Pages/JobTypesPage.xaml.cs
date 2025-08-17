using DriveLoadr.Models;
using DriveLoadr.PageModels;

namespace DriveLoadr.Pages;

[QueryProperty(nameof(PartnerId), "PartnerId")]
public partial class JobTypesPage : ContentPage
{
    private JobTypesPageModel ViewModel;

    public int PartnerId { get; set; }

    public JobTypesPage(JobTypesPageModel model)
	{
    InitializeComponent();
        ViewModel = model;
        BindingContext = model;
	}

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.LoadJobTypes();

        if (BindingContext is JobTypesPageModel vm)
        {
            await vm.LoadJobTypes();
            PartnerFilterPicker.ItemsSource = vm.Partners;
            PartnerFilterPicker.ItemDisplayBinding = new Binding("CompanyName");
            PartnerFilterPicker.SelectedIndex = 0; // default to "All"
            // set selected filter if passed
            if (PartnerId > 0)
            {
                var match = vm.Partners.FirstOrDefault(p => p.Id == PartnerId);
                if (match != null)
                {
                    PartnerFilterPicker.SelectedItem = match;
                }
            }
            else
            {
                PartnerFilterPicker.SelectedIndex = 0; // All
            }
        }
    }
    private async void AddJobType_Clicked(object sender, EventArgs e)
    {
        await ViewModel.AddJobTypeAsync(this);
    }

    private async void EditJobType_Clicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is JobTypesDisplay jobType)
        {
            await ViewModel.EditJobTypeAsync(this, jobType);
        }
    }

    private async void DeleteJobType_Clicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is JobTypesDisplay jobType)
        {
            await ViewModel.DeleteJobTypeAsync(this, jobType);
        }
    }

    private void PartnerFilterPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (BindingContext is not JobTypesPageModel vm) return;
        if (PartnerFilterPicker.SelectedItem is not Partner selected) return;

        if (selected.Id == 0)
        {
            // show all
            JobTypesView.ItemsSource = vm.JobTypes;
        }
        else
        {
            // filter
            JobTypesView.ItemsSource = vm.JobTypes
                .Where(j => j.PartnerID == selected.Id)
                .ToList();
        }
    }


}