using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel; // optional for MVVM attributes
using DriveLoadr.Models;
using DriveLoadr.Pages.PopupPages;
using System.Collections.ObjectModel;

namespace DriveLoadr.PageModels;

public class ContractorDisplay : Contractor
{
    public string RoleName { get; set; } = "";
}

public class ContractorsPageModel : ObservableObject // from CommunityToolkit.Mvvm.ComponentModel
{
    private readonly ContractorRepository _contractorsRepository;
    private readonly RoleRepository _roleRepository;
    private readonly IServiceProvider _serviceProvider;

    // Observable collection of contractors
    private ObservableCollection<ContractorDisplay> _contractors = new();
    public ObservableCollection<ContractorDisplay> Contractors
    {
        get => _contractors;
        set => SetProperty(ref _contractors, value); // automatically raises PropertyChanged
    }

    public ContractorsPageModel(IServiceProvider serviceProvider, ContractorRepository contractorsRepository,
        RoleRepository roleRepository)
    {
        _serviceProvider = serviceProvider; 
        _contractorsRepository = contractorsRepository;
        _roleRepository = roleRepository;
        Contractors = new ObservableCollection<ContractorDisplay>();
    }

    // Load contractors from database
    public async Task LoadContractors()
    {
        var contractors = await _contractorsRepository.GetContractorsAsync();
        var roles = await _roleRepository.GetRolesAsync(); 

        Contractors = new ObservableCollection<ContractorDisplay>(
            contractors.Select(c => new ContractorDisplay
            {
                Id = c.Id,
                Name = c.Name,
                ShortName = c.ShortName,
                Phone = c.Phone,
                Address = c.Address,
                Email = c.Email,
                BankAccount = c.BankAccount,
                Status = c.Status,
                DayRate = c.DayRate,
                RoleId = c.RoleId,
                RoleName = roles.FirstOrDefault(r => r.Id == c.RoleId)?.RoleName ?? "Unknown"
            })
        );
    }

    public async Task AddContractorAsync(ContentPage page)
    {
        var popup = _serviceProvider.GetRequiredService<AddContractorPopup>();
        popup.Initialize();

        var result = await page.ShowPopupAsync(popup);

        if (result is bool saved && saved)
        {
            await LoadContractors();
        }
    }

    public async Task EditContractorAsync(ContentPage page, ContractorDisplay row)
    {
        // Convert to the DB entity
        var dbContractor = new Contractor
        {
            Id = row.Id,
            Name = row.Name,
            ShortName = row.ShortName,
            Phone = row.Phone,
            Address = row.Address,
            Email = row.Email,
            BankAccount = row.BankAccount,
            Status = row.Status,
            DayRate = row.DayRate,
            RoleId = row.RoleId
        };

        var popup = _serviceProvider.GetRequiredService<AddContractorPopup>();
        popup.Initialize(dbContractor);

        var result = await page.ShowPopupAsync(popup);

        if (result is bool saved && saved)
        {
            await LoadContractors();
        }
    }

    public async Task DeleteContractorAsync(ContentPage page, ContractorDisplay row)
    {
        // Convert to the DB entity
        var dbContractor = new Contractor
        {
            Id = row.Id,
            Name = row.Name,
            ShortName = row.ShortName,
            Phone = row.Phone,
            Address = row.Address,
            Email = row.Email,
            BankAccount = row.BankAccount,
            Status = row.Status,
            DayRate = row.DayRate,
            RoleId = row.RoleId
        };

        var answer = await page.DisplayAlert(
                $"Confirm to delete record",
                $"{dbContractor.Name}",
                "Confirm",
                "Cancel");
        if (answer)
        {
            var result = await _contractorsRepository.DeleteContractorAsync(dbContractor);
            if (result > 0)
                await page.DisplayAlert("Success", "Contractor deleted successfully", "OK");
            else
                await page.DisplayAlert("Error", "Failed to delete contractor", "OK");

            await LoadContractors();
        }
    }
    
    public async Task AddJobTypeAsync(ContentPage page)
    {
        var popup = _serviceProvider.GetRequiredService<AddJobTypePopup>();
        await popup.Initialize();

        var result = await page.ShowPopupAsync(popup);

        if (result is bool saved && saved)
        {
         //   await LoadJobTypes();
        }
    }
}
