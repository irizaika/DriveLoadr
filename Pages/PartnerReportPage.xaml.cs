namespace DriveLoadr.Pages;

public partial class PartnerReportPage : ContentPage
{
	public PartnerReportPage(PartnerReportPageModel model)
    {
		InitializeComponent();
        BindingContext = model;
    }
}