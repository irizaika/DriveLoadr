namespace DriveLoadr.Pages;

public partial class DashboardPage : ContentPage
{
    public DashboardPage(DashboardPageModel model)
	{
        InitializeComponent();
        BindingContext = model;
    }
}