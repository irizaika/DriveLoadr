using CommunityToolkit.Maui.Views;
using DriveLoadr.Models;

namespace DriveLoadr.Pages.PopupPages;

public partial class AddContractorPopup : Popup
{
    private Contractor _contractor = new();

    private readonly ContractorRepository _contractorsRepository;
    private readonly RoleRepository _roleRepository;

    public List<string> StatusOptions { get; set; }
    public List<Role> RoleOptions { get; set; }


    private bool _isEditMode;

    public AddContractorPopup( ContractorRepository contractorsRepository, RoleRepository roleRepository)
    {
        _contractorsRepository = contractorsRepository;
        _roleRepository = roleRepository;

        InitializeComponent();

        StatusOptions = Enum.GetNames( typeof(ContractorStatus)).ToList();
        RoleOptions = _roleRepository.GetRolesAsync().Result; // ?? .Result is blocking, for quick test only; better use async.

    }

    public void Initialize(Contractor contractor = null)
    {
        _isEditMode = contractor != null;
        _contractor = contractor ?? new Contractor();

        BindingContext = this;

        if (_isEditMode)
        {
            // Pre-fill form (BindingContext already does this if XAML uses {Binding ...})
            NameEntry.Text = _contractor.Name;
            ShortNameEntry.Text = _contractor.ShortName;
            AddressEntry.Text = _contractor.Address;
            PhoneEntry.Text = _contractor.Phone;
            EmailEntry.Text = _contractor.Email;
            BankEntry.Text = _contractor.BankAccount;
            DayRateEntry.Text = _contractor.DayRate.ToString();

            StatusPicker.SelectedItem = _contractor.Status?.ToString();

            var selectedRole = RoleOptions.FirstOrDefault(r => r.Id == _contractor.RoleId);
            if (selectedRole != null)
                RolePicker.SelectedItem = selectedRole;
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text) && string.IsNullOrWhiteSpace(ShortNameEntry.Text))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Name and short name are required", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(NameEntry.Text))
         {
            await Application.Current.MainPage.DisplayAlert("Error", "Name is required", "OK");
            return;
        }
        if (string.IsNullOrWhiteSpace(ShortNameEntry.Text))
        {
            await Application.Current.MainPage.DisplayAlert("Error", "Short name is required", "OK");
            return;
        }

        _contractor.Name = NameEntry.Text;
        _contractor.ShortName = ShortNameEntry.Text;
        _contractor.Address = AddressEntry.Text;
        _contractor.Phone = PhoneEntry.Text;
        _contractor.Email = EmailEntry.Text;
        _contractor.BankAccount = BankEntry.Text;

        if (StatusPicker.SelectedItem is string selectedStatus &&
             Enum.TryParse<ContractorStatus>(selectedStatus, out var status))
        {
            _contractor.Status = status.ToString(); // Or store as enum
        }
        _contractor.DayRate = decimal.TryParse(DayRateEntry.Text, out var rate) ? rate : 0;

        if (RolePicker.SelectedItem is Role selectedRole)
        {
            _contractor.RoleId = selectedRole.Id;
        }

        await _contractorsRepository.SaveContractorAsync(_contractor);
        Close(true); // returns true to caller
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        Close(false);
    }
}


