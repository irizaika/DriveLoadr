using CommunityToolkit.Maui.Views;
using DriveLoadr.Models;

namespace DriveLoadr.Pages.PopupPages;

public partial class AddVanPopup : Popup
{
    private readonly VanRepository _vanRepository;

    private bool _isEditMode;
    private Van _van = new();
    public AddVanPopup(VanRepository vanRepository)
    {
        InitializeComponent();
        _vanRepository = vanRepository;
    }

    public void Initialize(Van van = null)
    {
        _van = van ?? new Van();
        _isEditMode = van != null;

        BindingContext = _van;

        if (_isEditMode)
        {
            VanNameEntry.Text = _van.VanName;
            DetailsEntry.Text = _van.Details;
            PlateEntry.Text = _van.Plate;
        }

    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(VanNameEntry.Text))
        {
            await App.Current.MainPage.DisplayAlert("Error", "Name is required", "OK");
            return;
        }

        _van.VanName = VanNameEntry.Text;
        _van.Details = DetailsEntry.Text;
        _van.Plate = PlateEntry.Text;
 

        await _vanRepository.SaveVanAsync(_van);
        Close(true); // Return success to caller
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        Close(false);
    }
}
