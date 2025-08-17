namespace DriveLoadr.Pages;

public partial class CustomerReportPage : ContentPage
{
	public CustomerReportPage(CustomerReportPageModel model)
    {
		InitializeComponent();
        BindingContext = model;
    }
}