using CommunityToolkit.Maui.Views;
using DriveLoadr.Models;

namespace DriveLoadr.Pages.PopupPages;

public partial class AddJobTypePopup : Popup
{
    private readonly JobTypeRepository _jobTypeRepository;
    private readonly PartnerRepository _partnerRepository;

    private bool _isEditMode;
    private JobType _jobType = new();
    public AddJobTypePopup(JobTypeRepository jobTypeRepository, PartnerRepository partnerRepository)
    {
        InitializeComponent();
        _jobTypeRepository = jobTypeRepository;
        _partnerRepository = partnerRepository;
    }

    public async Task Initialize(JobType jobType = null, int? selectedPartner = null)
    {
        _jobType = jobType ?? new JobType();
        _isEditMode = jobType != null;

        // Partners
        var partners = await _partnerRepository.GetPartnersListAsync();
        partners.Insert(0, new Partner { Id = 0, CompanyName = "Custom" });
        PartnerPicker.ItemsSource = partners;
        PartnerPicker.SelectedIndex = selectedPartner == null || selectedPartner == Constants.All ? 0 : selectedPartner.Value; // default to "General" if not passed from Patner page -> Job types page

        BindingContext = _jobType;

        if (_isEditMode)
        {
            PartnerPicker.SelectedItem = ((List<Partner>)PartnerPicker.ItemsSource).FirstOrDefault(p => p.Id == _jobType.PartnerID);
            JobTypeNameEntry.Text = _jobType.Name;
            DescriptionEntry.Text = _jobType.Description;
            NumberOfPeopleEntry.Text = _jobType.NumberOfPeople.ToString();
            NumberOfVansEntry.Text = _jobType.NumberOfVans.ToString();
            PayRateEntry.Text = _jobType.PayRate.ToString();
        }

    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(JobTypeNameEntry.Text))
        {
            await App.Current.MainPage.DisplayAlert("Error", "Name is required", "OK");
            return;
        }
        _jobType.Name = JobTypeNameEntry.Text;
        _jobType.Description = DescriptionEntry.Text;
        _jobType.PayRate = decimal.TryParse(PayRateEntry.Text, out var pay) ? pay : 0;
        _jobType.NumberOfPeople = int.TryParse(NumberOfPeopleEntry.Text, out var people) ? people : 0;
        _jobType.NumberOfVans = int.TryParse(NumberOfVansEntry.Text, out var vans) ? vans : 0;
        _jobType.PartnerID = ((Partner)PartnerPicker.SelectedItem)?.Id ?? 0;

        await _jobTypeRepository.SaveJobTypeAsync(_jobType);
        Close(true); // Return success to caller
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        Close(false);
    }
}
