using CommunityToolkit.Maui;
using DriveLoadr.Pages.PopupPages;
using Microsoft.Extensions.Logging;
using SQLite;
using Syncfusion.Maui.Toolkit.Hosting;

namespace DriveLoadr;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureSyncfusionToolkit()
			.ConfigureMauiHandlers(handlers =>
			{
			})
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("SegoeUI-Semibold.ttf", "SegoeSemibold");
				fonts.AddFont("FluentSystemIcons-Regular.ttf", FluentUI.FontFamily);
			});

#if DEBUG
		builder.Logging.AddDebug();
		builder.Services.AddLogging(configure => configure.AddDebug());
#endif

        // Register your page models and pages
        builder.Services.AddSingleton<DashboardPageModel>();
        builder.Services.AddTransient<DashboardPage>();

        builder.Services.AddSingleton<ContractorsPageModel>();
        builder.Services.AddTransient<ContractorsPage>();

        builder.Services.AddSingleton<PartnersPageModel>();
        builder.Services.AddTransient<PartnersPage>();

        builder.Services.AddSingleton<JobPlanningPageModel>();
        builder.Services.AddTransient<JobPlanningPage>();

        builder.Services.AddSingleton<VanListPageModel>();
        builder.Services.AddTransient<VanListPage>();

        builder.Services.AddSingleton<PartnerReportPageModel>();
        builder.Services.AddTransient<PartnerReportPage>();

        builder.Services.AddSingleton<CustomerReportPageModel>();
        builder.Services.AddTransient<CustomerReportPage>();

        builder.Services.AddSingleton<ContractorReportPageModel>();
        builder.Services.AddTransient<ContractorReportPage>();

        builder.Services.AddSingleton<JobTypesPageModel>();
        builder.Services.AddTransient<JobTypesPage>();

        // Database path
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "logistics.db3");
        var connection = new SQLiteAsyncConnection(dbPath);

        // Register SQLite connection
        builder.Services.AddSingleton(connection);

        // Register repositories
        builder.Services.AddSingleton<ContractorRepository>();
        builder.Services.AddSingleton<RoleRepository>();
		builder.Services.AddSingleton<PartnerRepository>();
		builder.Services.AddSingleton<VanRepository>();
		builder.Services.AddSingleton<JobTypeRepository>();
		builder.Services.AddSingleton<JobRepository>();

        builder.Services.AddTransient<AddContractorPopup>();
        builder.Services.AddTransient<AddJobPopup>();
        builder.Services.AddTransient<AddJobTypePopup>();
        builder.Services.AddTransient<AddPartnerPopup>();
        builder.Services.AddTransient<AddVanPopup>();

        Routing.RegisterRoute(nameof(DashboardPage), typeof(DashboardPage));
        Routing.RegisterRoute(nameof(ContractorsPage), typeof(ContractorsPage));
        Routing.RegisterRoute(nameof(VanListPage), typeof(VanListPage));
        Routing.RegisterRoute(nameof(PartnersPage), typeof(PartnersPage));
        Routing.RegisterRoute(nameof(JobPlanningPage), typeof(JobPlanningPage));
        Routing.RegisterRoute(nameof(PartnerReportPage), typeof(PartnerReportPage));
        Routing.RegisterRoute(nameof(CustomerReportPage), typeof(CustomerReportPage));
        Routing.RegisterRoute(nameof(ContractorReportPage), typeof(ContractorReportPage));
        Routing.RegisterRoute(nameof(JobTypesPage), typeof(JobTypesPage));

        return builder.Build();
	}
}
