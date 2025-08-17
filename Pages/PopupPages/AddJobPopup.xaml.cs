using CommunityToolkit.Maui.Views;
using DriveLoadr.Models;
using System.ComponentModel;

namespace DriveLoadr.Pages.PopupPages;

public partial class AddJobPopup : Popup
{
    private Job _editingJob = new();

    private readonly JobRepository _jobRepository;
    private readonly ContractorRepository _contractorsRepository;
    private readonly PartnerRepository _partnerRepository;
    private readonly JobTypeRepository _jobTypeRepository;
    private readonly VanRepository _vanRepository;


    public AddJobPopup(JobRepository jobRepository, ContractorRepository contractorsRepository,
        PartnerRepository partnerRepository, JobTypeRepository jobTypeRepository, VanRepository vanRepository)
    {
        _jobRepository = jobRepository;
        _contractorsRepository = contractorsRepository;
        _partnerRepository = partnerRepository;
        _jobTypeRepository = jobTypeRepository;
        _vanRepository = vanRepository;

        InitializeComponent();
    }

    public async void Initialize(DateTime? date, Job editingJob = null)
    {
        _editingJob = editingJob;
        await LoadData(date);

        if (_editingJob == null)// obly for the adding jobs
        {
            JobTypePicker.SelectedIndexChanged += OnJobTypeChanged;
        }

        if (_editingJob != null)
        {
            LoadExistingJob();
        }
    }

    private async Task LoadData(DateTime? date)
    {
        if (date != null)
            DatePicker.Date = date.Value;

        // Partners
        var partners = await _partnerRepository.GetPartnersListAsync();
        partners.Insert(0, new Partner { Id = 0, CompanyName = "Not set" });
        PartnerPicker.ItemsSource = partners;
        PartnerPicker.SelectedIndex = 0; // default to "Not set"

        // Job Types
        var jobTypes = await _jobTypeRepository.GetJobTypesAsync();
        jobTypes.Insert(0, new JobType { Id = 0, Name = "Not set" });
        JobTypePicker.ItemsSource = jobTypes;
        JobTypePicker.SelectedIndex = 0;


        // Vans
        var vans = await _vanRepository.GetVanListAsync();
        VanListCollection.ItemsSource = vans.Select(c => new VanSelectable
        {
            Id = c.Id,
            Name = c.VanName,
            IsSelected = false
        }).ToList();


        // Contractors
        var contractors = await _contractorsRepository.GetContractorsAsync();
        ContractorsCollection.ItemsSource = contractors.Select(c => new ContractorSelectable
        {
            Id = c.Id,
            Name = c.Name,
            IsSelected = false
        }).ToList();
    }


    private void LoadExistingJob()
    {
        try
        {
            DatePicker.Date = _editingJob.Date;
            PartnerPicker.SelectedItem = ((List<Partner>)PartnerPicker.ItemsSource).FirstOrDefault(p => p.Id == _editingJob.PartnerId);
            JobTypePicker.SelectedItem = ((List<JobType>)JobTypePicker.ItemsSource).FirstOrDefault(j => j.Id == _editingJob.JobTypeId);
            CountEntry.Text = _editingJob.Count.ToString();
            PayReceived.Text = _editingJob.PayReceived.ToString();
            CustomerNameEntry.Text = _editingJob.CustomerName;
            DetailsEntry.Text = _editingJob.Details;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        var selectedIds = _editingJob.ContractorList.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Select(int.Parse).ToList();
        foreach (var contractor in (List<ContractorSelectable>)ContractorsCollection.ItemsSource)
            contractor.IsSelected = selectedIds.Contains(contractor.Id);

        var selectedVanIds = _editingJob.VanList.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Select(int.Parse).ToList();
        foreach (var van in (List<VanSelectable>)VanListCollection.ItemsSource)
            van.IsSelected = selectedVanIds.Contains(van.Id);

    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var selectedContractors = ((List<ContractorSelectable>)ContractorsCollection.ItemsSource)
            .Where(c => c.IsSelected)
            .Select(c => c.Id.ToString());

        var selectedVanList = ((List<VanSelectable>)VanListCollection.ItemsSource)
            .Where(c => c.IsSelected)
            .Select(c => c.Id.ToString());

        var job = _editingJob ?? new Job();
        job.Date = DatePicker.Date;
        job.CustomerName = CustomerNameEntry.Text;
        job.Details = DetailsEntry.Text;
        job.PartnerId = ((Partner)PartnerPicker.SelectedItem)?.Id ?? 0;
        job.JobTypeId = ((JobType)JobTypePicker.SelectedItem)?.Id ?? 0;
        job.Count = decimal.TryParse(CountEntry.Text, out var amt) ? amt : 0;
        job.PayReceived = decimal.TryParse(PayReceived.Text, out var pay) ? pay : 0;
        job.ContractorList = string.Join(",", selectedContractors);
        job.VanList = string.Join(",", selectedVanList);

        await _jobRepository.SaveJobAsync(job);
        Close(job); // return success
    }
    private async void OnJobTypeChanged(object sender, EventArgs e)
    {
        try
        {
            if (JobTypePicker.SelectedItem is not JobType selectedJobType)
                return;

            // Query the repository for rate
            var jobType = await _jobTypeRepository.GetJobfByIdAsync(selectedJobType.Id);

            if (jobType != null)
            {
                // auto fill 
                PayReceived.Text = jobType.PayRate.ToString("0.00");
                PartnerPicker.SelectedItem = ((List<Partner>)PartnerPicker.ItemsSource).FirstOrDefault(p => p.Id == jobType.PartnerID);
                DetailsEntry.Text = jobType.Description;
            }
            else
            {
                // no found
                PayReceived.Text = "";
                PartnerPicker.SelectedItem = 0;
                DetailsEntry.Text = "";

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading default rate: {ex.Message}");
        }
    }



    private void OnCancelClicked(object sender, EventArgs e) => Close(false);

    private class ContractorSelectable : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }


    private class VanSelectable : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

}
