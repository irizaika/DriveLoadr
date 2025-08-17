using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ClosedXML.Excel;

namespace DriveLoadr.PageModels;

public partial class ContractorReportPageModel : ObservableObject
{
    private readonly JobRepository _jobRepository;
    private readonly PartnerRepository _partnerRepository;
    private readonly JobTypeRepository _jobTypeRepository;
    private readonly ContractorRepository _contractorRepository;


    [ObservableProperty]
    private DateTime _startDate = DateTime.Today.AddDays(-3);

    [ObservableProperty]
    private DateTime _endDate = DateTime.Today.AddDays(25);

    [ObservableProperty]
    private ObservableCollection<ContractorReportItem> _reportItems = new();


    public ContractorReportPageModel(JobRepository jobRepository, PartnerRepository partnerRepository, JobTypeRepository jobTypeRepository,
        ContractorRepository contractorRepository)
    {
        _jobRepository = jobRepository;
        _partnerRepository = partnerRepository;
        _jobTypeRepository = jobTypeRepository;
        _contractorRepository = contractorRepository;
    }

    public string DateRangeText => $"{StartDate:dd MMM yyyy} - {EndDate:dd MMM yyyy}";

    [RelayCommand]
    private async Task GenerateReportGroupByDate()
    {
        ReportItems.Clear();

        // Lookups
        var partners = await _partnerRepository.GetPartnersListAsync();
        var partnerDict = partners.ToDictionary(p => p.Id, p => p.CompanyName);

        var jobTypes = await _jobTypeRepository.GetJobTypesAsync();
        var jobTypeDict = jobTypes.ToDictionary(jt => jt.Id, jt => jt.Name);

        var contractors = await _contractorRepository.GetContractorsAsync();
        var contractorsDict = contractors.ToDictionary(c => c.Id, c => c.Name);
        var contractorRates = contractors.ToDictionary(c => c.Id, c => c.DayRate);

        // Get jobs
        var jobs = await _jobRepository.GetJobsInRangeAsync(StartDate, EndDate);

        var jobsWithContractors = jobs
            .SelectMany(j =>
                (j.ContractorList ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(idStr => int.TryParse(idStr, out var contractorId) ? contractorId : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => new
                {
                    ContractorId = id.Value,
                    j.Date,
                    j.JobTypeId,
                    j.Details,
                    j.Count
                })
            );

        var groupedByContractor = jobsWithContractors
            .GroupBy(jc => jc.ContractorId);

        foreach (var contractorGroup in groupedByContractor)
        {
            var contractorId = contractorGroup.Key;
            var contractorName = contractorsDict.TryGetValue(contractorId, out var name) ? name : "(Unknown)";
            var dailyRate = contractorRates.TryGetValue(contractorId, out var rate) ? rate : 0m;

            var reportItem = new ContractorReportItem
            {
                Name = contractorName,
                Jobs = new List<ContractorJobReportRow>()
            };

            // Group jobs by date
            var jobsByDay = contractorGroup.GroupBy(j => j.Date.Date);

            foreach (var dayGroup in jobsByDay)
            {
                int index = 0;
                decimal dailySubtotal = 0m;

                foreach (var job in dayGroup)
                {
                    decimal jobPay = 0m;

                    if (dailyRate > 0)
                    {
                        if (index == 0)
                            jobPay = dailyRate;        // first job
                        else
                            jobPay = dailyRate * 1.2m; // extra job = +20%
                    }

                    dailySubtotal += jobPay;

                    reportItem.Jobs.Add(new ContractorJobReportRow
                    {
                        Date = job.Date,
                        JobDetails = job.Details,
                        JobType = jobTypeDict.TryGetValue(job.JobTypeId, out var jobName) ? jobName : "(Job type not set)",
                        DefaultPay = jobPay,
                        Count = job.Count
                    });

                    index++;
                }

                // Add a "subtotal row" for this day
                reportItem.Jobs.Add(new ContractorJobReportRow
                {
                    Date = dayGroup.Key,
                    JobDetails = $"Subtotal for {dayGroup.Key:yyyy-MM-dd}",
                    JobType = string.Empty,
                    DefaultPay = dailySubtotal,
                    Count = index
                });
            }

            ReportItems.Add(reportItem);
        }
    }

    [RelayCommand]
    private async Task GenerateReport()
    {
        ReportItems.Clear();

        // Get partner, job type, and contractor lookup dictionaries
        var partners = await _partnerRepository.GetPartnersListAsync();
        var partnerDict = partners.ToDictionary(p => p.Id, p => p.CompanyName);

        var jobTypes = await _jobTypeRepository.GetJobTypesAsync();
        var jobTypeDict = jobTypes.ToDictionary(jt => jt.Id, jt => jt.Name);

        var contractors = await _contractorRepository.GetContractorsAsync();
        var contractorsDict = contractors.ToDictionary(c => c.Id, c => c.Name);

        // Assuming each contractor has a DefaultDailyRate property
        var contractorRates = contractors.ToDictionary(c => c.Id, c => c.DayRate);

        // Get jobs
        var jobs = await _jobRepository.GetJobsInRangeAsync(StartDate, EndDate);

        var jobsWithContractors = jobs
            .SelectMany(j =>
                (j.ContractorList ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(idStr => int.TryParse(idStr, out var contractorId) ? contractorId : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => new
                {
                    ContractorId = id.Value,
                    j.Date,
                    j.JobTypeId,
                    j.Details,
                    j.Count
                })
            );

        var groupedByContractor = jobsWithContractors
            .GroupBy(jc => jc.ContractorId);

        foreach (var contractorGroup in groupedByContractor)
        {
            var contractorId = contractorGroup.Key;
            var contractorName = contractorsDict.TryGetValue(contractorId, out var name) ? name : "(Unknown)";
            var dailyRate = contractorRates.TryGetValue(contractorId, out var rate) ? rate : 0m;

            var reportItem = new ContractorReportItem
            {
                Name = contractorName,
                Jobs = new List<ContractorJobReportRow>()
            };

            // Group jobs by day for pay calculation
            var jobsByDay = contractorGroup.GroupBy(j => j.Date.Date);

            foreach (var dayGroup in jobsByDay)
            {
                int index = 0;
                foreach (var job in dayGroup)
                {
                    decimal jobPay = 0m;

                    if (dailyRate > 0)
                    {
                        if (index == 0)
                            jobPay = dailyRate; // first job of the day
                        else
                            jobPay = dailyRate * 0.2m; // subsequent jobs add 20%
                    }

                    reportItem.Jobs.Add(new ContractorJobReportRow
                    {
                        Date = job.Date,
                        JobDetails = job.Details,
                        JobType = jobTypeDict.TryGetValue(job.JobTypeId, out var jobName) ? jobName : "(Job type not set)",
                        DefaultPay = jobPay,
                        Count = job.Count
                    });

                    index++;
                }
            }

            ReportItems.Add(reportItem);
        }
    }




    [RelayCommand]
    private async Task ExportReport()
    {
        if (ReportItems == null || ReportItems.Count == 0)
        {
            await Application.Current.MainPage.DisplayAlert("No Data", "Please generate the report first.", "OK");
            return;
        }

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Report");

        int row = 1;

        // Title
        worksheet.Cell(row, 1).Value = $"Report: {DateRangeText}";
        worksheet.Range(row, 1, row, 5).Merge().Style
            .Font.SetBold()
            .Font.FontSize = 14;
        row += 2;

        foreach (var company in ReportItems)
        {
            // Company name
            worksheet.Cell(row, 1).Value = company.Name;
            worksheet.Cell(row, 1).Style.Font.SetBold();
            row++;

            // Column headers
            worksheet.Cell(row, 1).Value = "Date";
            worksheet.Cell(row, 2).Value = "Job Type";
            worksheet.Cell(row, 3).Value = "Count";
            worksheet.Cell(row, 4).Value = "DefaultPay";
            worksheet.Cell(row, 5).Value = "Details";
            worksheet.Row(row).Style.Font.SetBold();
            row++;

            // Job rows
            foreach (var job in company.Jobs)
            {
                worksheet.Cell(row, 1).Value = job.Date;
                worksheet.Cell(row, 1).Style.DateFormat.Format = "dd MMM yyyy";
                worksheet.Cell(row, 2).Value = job.JobType;
                worksheet.Cell(row, 3).Value = job.Count;
                worksheet.Cell(row, 4).Value = job.DefaultPay;
                worksheet.Cell(row, 5).Value = job.JobDetails;
                row++;
            }

            // Empty line after each company
            row++;
        }

        worksheet.Columns().AdjustToContents();

        string reportName = $"Report_{DateRangeText}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

        // Save file
#if ANDROID
        var filePath = Path.Combine(FileSystem.Current.AppDataDirectory, reportName);
#elif WINDOWS
    var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), reportName);
#else
    var filePath = Path.Combine(FileSystem.Current.AppDataDirectory, reportName);
#endif

        workbook.SaveAs(filePath);

        await Application.Current.MainPage.DisplayAlert("Exported", $"Report saved to:\n{filePath}", "OK");

        // Optionally open the file
        await Launcher.OpenAsync(new OpenFileRequest
        {
            File = new ReadOnlyFile(filePath)
        });
    }

}

public class ContractorReportItem
{
    public string Name { get; set; }
    public List<ContractorJobReportRow> Jobs { get; set; }
}

public class ContractorJobReportRow
{
    public DateTime Date { get; set; }
    public string JobType { get; set; }
    public string JobDetails { get; set; }
    public decimal Count { get; set; }
    public decimal DefaultPay { get; set; }

}