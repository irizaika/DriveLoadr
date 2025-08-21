using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel; // optional for MVVM attributes
using DriveLoadr.Models;
using DriveLoadr.Pages.PopupPages;
using System.Collections.ObjectModel;

namespace DriveLoadr.PageModels;

public class JobTypesDisplay : JobType
{
    public string PartnterName { get; set; } = "Custom"; // if partners id is 0;
}

public class JobTypesPageModel : ObservableObject
{
    private readonly JobTypeRepository _jobTypeRepository;
    private readonly PartnerRepository _partnerRepository;
    private readonly IServiceProvider _serviceProvider;

    // Observable collection of contractors
    private ObservableCollection<JobTypesDisplay> _jobTypes = new();
    public ObservableCollection<JobTypesDisplay> JobTypes
    {
        get => _jobTypes;
        set => SetProperty(ref _jobTypes, value); // automatically raises PropertyChanged
    }
    public ObservableCollection<Partner> Partners { get; set; } = new();

    public JobTypesPageModel(IServiceProvider serviceProvider, JobTypeRepository jobTypesRepository,
        PartnerRepository partnerRepository)
    {
        _serviceProvider = serviceProvider; 
        _jobTypeRepository = jobTypesRepository;
        _partnerRepository = partnerRepository;

        JobTypes = new ObservableCollection<JobTypesDisplay>();
        Partners = new ObservableCollection<Partner>();
    }

    // Load job types from database
    public async Task LoadJobTypes()
    {
        var contractors = await _jobTypeRepository.GetJobTypesAsync();
        var partners = await _partnerRepository.GetPartnersListAsync();

        // fill partner list for the filter (include "All")
        Partners.Clear();
        Partners.Add(new Partner { Id = Constants.All, CompanyName = "All" });
        Partners.Add(new Partner { Id = 0, CompanyName = "Custom" });
        foreach (var p in partners)
            Partners.Add(p);

        JobTypes = new ObservableCollection<JobTypesDisplay>(
            contractors.Select(c => new JobTypesDisplay
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                PartnerID = c.PartnerID,
                NumberOfPeople = c.NumberOfPeople,
                NumberOfVans = c.NumberOfVans,
                PayRate = c.PayRate,
                PartnterName = partners.FirstOrDefault(r => r.Id == c.PartnerID)?.CompanyName ?? "Custom"
            })
        );
    }

    // Command methods can be wired from XAML or code-behind
    public async Task AddJobTypeAsync(ContentPage page, int? selectedPartnerId = null )
    {
        //await Shell.Current.GoToAsync(nameof(JobTypesPage));
        var popup = _serviceProvider.GetRequiredService<AddJobTypePopup>();
        await popup.Initialize(null, selectedPartnerId);

        var result = await page.ShowPopupAsync(popup);

        if (result is bool saved && saved)
        {
            await LoadJobTypes();
        }
    }

    public async Task EditJobTypeAsync(ContentPage page, JobTypesDisplay row)
    {
        // Convert to the DB entity
        var dbJobType= new JobType
        {
            Id = row.Id,
            Name = row.Name,
            Description = row.Description,
            PartnerID = row.PartnerID,
            NumberOfPeople = row.NumberOfPeople,
            NumberOfVans = row.NumberOfVans,
            PayRate = row.PayRate
        };

        var popup = _serviceProvider.GetRequiredService<AddJobTypePopup>();
        await popup.Initialize(dbJobType);

        var result = await page.ShowPopupAsync(popup);

        if (result is bool saved && saved)
        {
            await LoadJobTypes();
        }
    }

    public async Task DeleteJobTypeAsync(ContentPage page, JobTypesDisplay row)
    {
        // Convert to the DB entity
        var dbJobType = new JobType
        {
            Id = row.Id,
            Name = row.Name,
            Description = row.Description,
            PartnerID = row.PartnerID,
            NumberOfPeople = row.NumberOfPeople,
            NumberOfVans = row.NumberOfVans,
            PayRate = row.PayRate
        };
        
        var answer = await page.DisplayAlert(
                $"Confirm to delete record",
                $"{dbJobType.Name}",
                "Confirm",
                "Cancel");
        if (answer)
        {
            var result = await _jobTypeRepository.DeleteJobTypeAsync(dbJobType);
            if (result > 0)
                await page.DisplayAlert("Success", "Job type deleted successfully", "OK");
            else
                await page.DisplayAlert("Error", "Failed to delete job type", "OK");

            await LoadJobTypes();
        }
    }
}
