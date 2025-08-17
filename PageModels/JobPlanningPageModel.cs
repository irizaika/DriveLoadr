using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DriveLoadr.Models;
using DriveLoadr.Pages.PopupPages;
using System.Collections.ObjectModel;
using DayOfWeek = System.DayOfWeek;

namespace DriveLoadr.PageModels;
public partial class DayJobs : ObservableObject
{
    public DateTime Date { get; set; }
    public bool IsWeekend => Date.DayOfWeek == DayOfWeek.Saturday || Date.DayOfWeek == DayOfWeek.Sunday;

    [ObservableProperty]
    public List<JobDisplay> jobs = new();
}

public partial class JobDisplay 
{
    public string PartnerName { get; set; }
    public string ContractorsDisplay { get; set; }
    public string JobTypeName { get; set; }
    public string VanName { get; set; }
    public string VanDisplay { get; set; }
    public decimal Count { get; set; }
    public Job OriginalJob { get; set; }

    //[ObservableProperty]
    public string JobSummary => $"{JobTypeName}({Count}) | {VanDisplay} | {PartnerName} | {ContractorsDisplay} | Pay: {OriginalJob.PayReceived} | {OriginalJob.Details}";

}

public partial class JobPlanningPageModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    private readonly JobRepository _jobRepository;
    private readonly PartnerRepository _partnerRepository;
    private readonly JobTypeRepository _jobTypeRepository;
    private readonly VanRepository _vanRepository;
    private readonly ContractorRepository _contractorRepository;

    [ObservableProperty]
    private ObservableCollection<DayJobs> _monthDays;

    [ObservableProperty]
    private DateTime _startDate = DateTime.Today.AddDays(-3);

    [ObservableProperty]
    private DateTime _endDate = DateTime.Today.AddDays(25);


    public JobPlanningPageModel(IServiceProvider serviceProvider, JobRepository jobRepository,
        PartnerRepository partnerRepository, JobTypeRepository jobTypeRepository, VanRepository vanRepository,
        ContractorRepository contractorRepository)
    {
        _serviceProvider = serviceProvider;
        _jobRepository = jobRepository;
        _partnerRepository = partnerRepository;
        _jobTypeRepository = jobTypeRepository;
        _vanRepository = vanRepository;
        _contractorRepository = contractorRepository;

        //GenerateMonthView(DateTime.Now);
        RefreshJobs();
    }


    [RelayCommand]
    private void RefreshJobs()
    {
        // regenerate MonthDays based on StartDate and EndDate
        GenerateMonthView(StartDate, EndDate);
    }

    // Update GenerateMonthView to accept start/end range
    private void GenerateMonthView(DateTime start, DateTime end)
    {
        MonthDays = new ObservableCollection<DayJobs>();

        var jobTypes = _jobTypeRepository.GetJobTypesAsync().Result;
        var jobTypeDict = jobTypes.ToDictionary(jt => jt.Id, jt => jt.Name);

        var locations = _partnerRepository.GetPartnersListAsync().Result;
        var locationsDict = locations.ToDictionary(jt => jt.Id, jt => jt.ShortName);

        var vans = _vanRepository.GetVanListAsync().Result;
        var vansDict = vans.ToDictionary(jt => jt.Id, jt => jt.VanName);

        var contractors = _contractorRepository.GetContractorsAsync().Result;
        var contractorsDict = contractors.ToDictionary(jt => jt.Id, jt => jt.ShortName);

        for (var date = start.Date; date <= end.Date; date = date.AddDays(1))
        {
            var jobs = _jobRepository.GetDayJobs(date).Result;

            var jobDisplays = jobs.Select(j => new JobDisplay
            {
                PartnerName = (locationsDict.TryGetValue(j.PartnerId, out var companyName) &&
                         !string.IsNullOrWhiteSpace(companyName) &&
                         !string.Equals(companyName, "Custom", StringComparison.OrdinalIgnoreCase))
                            ? companyName
                            : (!string.IsNullOrWhiteSpace(j.CustomerName) ? j.CustomerName : "Custom"),
                ContractorsDisplay = string.Join(", ",
                    (j.ContractorList ?? "")
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(idStr => int.TryParse(idStr, out var id) && contractorsDict.ContainsKey(id) ? contractorsDict[id] : null)
                        .Where(name => !string.IsNullOrEmpty(name))),
                JobTypeName = jobTypeDict.TryGetValue(j.JobTypeId, out var jobName) ? jobName : "(Job type not set)",
                VanName = vansDict.TryGetValue(j.Id, out var vanName) ? vanName : "(No van added)",
                VanDisplay = string.Join(", ",
                    (j.VanList ?? "")
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(idStr => int.TryParse(idStr, out var id) && vansDict.ContainsKey(id) ? vansDict[id] : null)
                        .Where(name => !string.IsNullOrEmpty(name))),
                Count = j.Count,
                OriginalJob = j
            }).ToList();

            MonthDays.Add(new DayJobs
            {
                Date = date,
                jobs = jobDisplays
            });
        }
    }


    //private void GenerateMonthViewOld(DateTime month)
    //{
    //    MonthDays = new ObservableCollection<DayJobs>();

    //    var start = new DateTime(month.Year, month.Month, 1);
    //    var daysInMonth = DateTime.DaysInMonth(month.Year, month.Month);

    //    var jobTypes = _jobTypeRepository.GetJobTypesAsync().Result;
    //    var jobTypeDict = jobTypes.ToDictionary(jt => jt.Id, jt => jt.Name);

    //    var locations = _partnerRepository.GetPartnersListAsync().Result;
    //    var locationsDict = locations.ToDictionary(jt => jt.Id, jt => jt.CompanyName);

    //    var vans = _vanRepository.GetVanListAsync().Result;
    //    var vansDict = vans.ToDictionary(jt => jt.Id, jt => jt.VanName);

    //    var contarctors = _contractorRepository.GetContractorsAsync().Result;
    //    var contractorsDict = contarctors.ToDictionary(jt => jt.Id, jt => jt.Name);

    //    for (int i = 0; i < daysInMonth; i++)
    //    {
    //        var date = start.AddDays(i);
    //        var job = _jobRepository.GetDayJobs(date).Result;

    //         List<JobDisplay> jobdisplay = job.Select(j => new JobDisplay
    //        {
    //            PartnerName = locationsDict.TryGetValue(j.PartnerId, out var companyName) ? companyName : "(Job owner not set)",

    //             ContractorsDisplay = string.Join(", ",
    //             (j.ContractorList ?? "")
    //            .Split(',', StringSplitOptions.RemoveEmptyEntries)
    //            .Select(idStr => int.TryParse(idStr, out var id) && contractorsDict.ContainsKey(id)
    //                ? contractorsDict[id]
    //                : null)
    //            .Where(name => !string.IsNullOrEmpty(name))),


    //             JobTypeName = jobTypeDict.TryGetValue(j.JobTypeId, out var jobName) ? jobName : "(Job type not set)",
    //            VanName = vansDict.TryGetValue(j.Id, out var vanName) ? vanName : "(No van added)",
    //            Count = j.Count,
    //            OriginalJob = j
    //        }).ToList();

    //        MonthDays.Add(new DayJobs
    //        {
    //            Date = date,
    //            jobs = jobdisplay
    //        });
    //    }
    //}

    private void RefreshDay(DateTime date)
    {
        RefreshJobs();
       // GenerateMonthView(DateTime.Now); // todo refresh only one day

        // this code not working
        //var day = MonthDays.FirstOrDefault(d => d.Date.Date == date.Date);
        //if (day != null)
        //{
        //    var jobTypes = _jobTypeRepository.GetJobTypesAsync().Result;
        //    var jobTypeDict = jobTypes.ToDictionary(jt => jt.Id, jt => jt.TypeName);

        //    var locations = _partnerRepository.GetPartnersListAsync().Result;
        //    var locationsDict = locations.ToDictionary(jt => jt.Id, jt => jt.CompanyName);

        //    var vans = _vanRepository.GetVanListAsync().Result;
        //    var vansDict = vans.ToDictionary(jt => jt.Id, jt => jt.VanName);

        //    var contractors = _contractorRepository.GetContractorsAsync().Result;
        //    var contractorsDict = contractors.ToDictionary(jt => jt.Id, jt => jt.Name);

        //    var jobs = _jobRepository.GetDayJobs(date).Result;

        //    var newjobs =  jobs.Select(j => new JobDisplay
        //    {
        //        LocationName = locationsDict.TryGetValue(j.LocationId, out var companyName) ? companyName : "(Unknown)",
        //        ContractorsDisplay = string.Join(", ",
        //            (j.ContractorList ?? "")
        //                .Split(',', StringSplitOptions.RemoveEmptyEntries)
        //                .Select(idStr => int.TryParse(idStr, out var id) && contractorsDict.ContainsKey(id)
        //                    ? contractorsDict[id]
        //                    : null)
        //                .Where(name => !string.IsNullOrEmpty(name))),
        //        JobTypeName = jobTypeDict.TryGetValue(j.JobTypeId, out var jobName) ? jobName : "(Unknown)",
        //        VanName = vansDict.TryGetValue(j.Id, out var vanName) ? vanName : "(Unknown)",
        //        Amount = j.Amount,
        //        OriginalJob = j
        //    }).ToList();


        //    day.Jobs.Clear();
        //    foreach (var jobDisplay in newjobs)
        //    {
        //        day.Jobs.Add(jobDisplay);
        //    }
        //}

    }




    [RelayCommand]
    public async Task AddJob(DateTime date)
    {
        var popup = _serviceProvider.GetRequiredService<AddJobPopup>();
        popup.Initialize(date);

     //   var result = await Shell.Current.CurrentPage.ShowPopupAsync(popup);
        var result = await Application.Current.MainPage.ShowPopupAsync(popup);



        //  var popupWindow = new Window(popup);
        //Application.Current.OpenWindow(popup);
        if (result is Job savedJob)
        {
            RefreshDay(savedJob.Date);
        }
    }

    [RelayCommand]
    public async Task EditJob(Job job)
    {
        var popup = _serviceProvider.GetRequiredService<AddJobPopup>();
        popup.Initialize(job.Date, job);

        var result = await Shell.Current.CurrentPage.ShowPopupAsync(popup);
        if (result is Job savedJob)
        {
            RefreshDay(savedJob.Date);
        }
    }

    [RelayCommand]
    public async Task DeleteJob(Job job)
    {
        var answer = await Shell.Current.DisplayAlert("Confirm", $"Delete job on {job.Date:d}?", "Yes", "No");
        if (answer)
        {
            await _jobRepository.DeleteJobAsync(job);
            RefreshDay(job.Date);
        }
    }
}