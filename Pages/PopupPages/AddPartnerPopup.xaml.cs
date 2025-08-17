using CommunityToolkit.Maui.Views;
using DriveLoadr.Models;

namespace DriveLoadr.Pages.PopupPages;

public partial class AddPartnerPopup : Popup
{
    private readonly PartnerRepository _partnerRepository;

    private Partner _partner = new();
    private bool _isEditMode;

    public AddPartnerPopup(PartnerRepository partnerRepository)
    {
        InitializeComponent();
        _partnerRepository = partnerRepository;
    }

    public void Initialize(Partner partner = null)
    {
        _isEditMode = partner != null;
        _partner = partner ?? new Partner();

        BindingContext = _partner;

        if (_isEditMode)
        {
            // Pre-fill form (BindingContext already does this if XAML uses {Binding ...})
            NameEntry.Text = _partner.CompanyName;
            ShortNameEntry.Text = _partner.ShortName;
            AddressEntry.Text = _partner.Address;
            Phone1Entry.Text = _partner.Phone1;
            Phone2Entry.Text = _partner.Phone2;
            EmailEntry.Text = _partner.Email;
            StatusPicker.SelectedItem = _partner.Status?.ToString();
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text))
        {
            await App.Current.MainPage.DisplayAlert("Error", "Name is required", "OK");
            return;
        }

        _partner.CompanyName = NameEntry.Text;
        _partner.Address = AddressEntry.Text;
        _partner.Phone1 = Phone1Entry.Text;
        _partner.Phone2 = Phone2Entry.Text;
        _partner.Email = EmailEntry.Text;

        if (StatusPicker.SelectedItem is string selectedStatus &&
             Enum.TryParse<ContractorStatus>(selectedStatus, out var status))
        {
            _partner.Status = status.ToString(); // Or store as enum
        }

        await _partnerRepository.SavePartnerAsync(_partner);
        Close(true); // returns true to caller
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        Close(false);
    }
}
