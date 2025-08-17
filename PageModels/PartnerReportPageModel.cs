using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using ClosedXML.Excel;

namespace DriveLoadr.PageModels;

public partial class PartnerReportPageModel : ObservableObject
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
    private ObservableCollection<ReportItem> _reportItems = new();


    public PartnerReportPageModel(JobRepository jobRepository, PartnerRepository partnerRepository, JobTypeRepository jobTypeRepository,
        ContractorRepository contractorRepository)
    {
        _jobRepository = jobRepository;
        _partnerRepository = partnerRepository;
        _jobTypeRepository = jobTypeRepository;
        _contractorRepository = contractorRepository;
    }

    public string DateRangeText => $"{StartDate:dd MMM yyyy} - {EndDate:dd MMM yyyy}";

    [RelayCommand]
    private async Task GenerateReport()
    {
        // Clear old data
        ReportItems.Clear();

        // Get partner info
        var partners = await _partnerRepository.GetPartnersListAsync();
        var partnerDict = partners.ToDictionary(p => p.Id, p => p.CompanyName);

        var jobTypes = _jobTypeRepository.GetJobTypesAsync().Result;
        var jobTypeDict = jobTypes.ToDictionary(jt => jt.Id, jt => jt.Name);

        var contarctors = _contractorRepository.GetContractorsAsync().Result;
        var contractorsDict = contarctors.ToDictionary(jt => jt.Id, jt => jt.Name);



        // Get all jobs in date range
        var allJobs = await _jobRepository.GetJobsInRangeAsync(StartDate, EndDate);

        var jobs = allJobs.Where(j => j.PartnerId > 0);

        // Group by company
        var groupedByCompany = jobs
            .GroupBy(j => j.PartnerId)
            .OrderBy(g => partnerDict.TryGetValue(g.Key, out var name) ? name : "(Unknown)");

        foreach (var group in groupedByCompany)
        {
            var companyName = partnerDict.TryGetValue(group.Key, out var name) ? name : "(Unknown)";

            // Create report entry
            var reportItem = new ReportItem
            {
                Name = companyName,
                Jobs = group.Select(j => new JobReportRow
                {
                    Date = j.Date,
                    JobDetails = j.Details,
                    JobType = jobTypeDict.TryGetValue(j.JobTypeId, out var jobName) ? jobName : "(Job type not set)",
                    Pay = j.PayReceived,
                    Count = j.Count,
                    Contractors = string.Join(", ",
                         (j.ContractorList ?? "")
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(idStr => int.TryParse(idStr, out var id) && contractorsDict.ContainsKey(id)
                            ? contractorsDict[id]
                            : null)
                        .Where(name => !string.IsNullOrEmpty(name))),
                }).ToList()
            };

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
            worksheet.Cell(row, 4).Value = "Pay";
            worksheet.Cell(row, 5).Value = "Contractors";
            worksheet.Cell(row, 6).Value = "Details";
            worksheet.Row(row).Style.Font.SetBold();
            row++;

            // Job rows
            foreach (var job in company.Jobs)
            {
                worksheet.Cell(row, 1).Value = job.Date;
                worksheet.Cell(row, 1).Style.DateFormat.Format = "dd MMM yyyy";
                worksheet.Cell(row, 2).Value = job.JobType;
                worksheet.Cell(row, 3).Value = job.Count;
                worksheet.Cell(row, 4).Value = job.Pay;
                worksheet.Cell(row, 5).Value = job.Contractors;
                worksheet.Cell(row, 6).Value = job.JobDetails;
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

public class ReportItem
{
    public string Name { get; set; }
    public List<JobReportRow> Jobs { get; set; }
}

public class JobReportRow
{
    public DateTime Date { get; set; }
    public string JobType { get; set; }
    public string JobDetails { get; set; }
    public decimal Count { get; set; }
    public decimal Pay { get; set; }
    public string Contractors { get; set; }
}