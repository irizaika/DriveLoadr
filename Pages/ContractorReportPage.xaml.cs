namespace DriveLoadr.Pages;

public partial class ContractorReportPage : ContentPage
{
	public ContractorReportPage(ContractorReportPageModel model)
    {
		InitializeComponent();
        BindingContext = model;
    }
}