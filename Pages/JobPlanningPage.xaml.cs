namespace DriveLoadr.Pages;

public partial class JobPlanningPage : ContentPage
{
    public JobPlanningPage(JobPlanningPageModel model)
    {
        InitializeComponent();
        BindingContext = model;
    }
}