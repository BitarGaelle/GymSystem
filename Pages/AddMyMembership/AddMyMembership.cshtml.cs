using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using GymSystem.Services;
using GymSystem.Models;

public class AddMyMembershipModel : PageModel
{
    private readonly IGymService _gymService;

    public AddMyMembershipModel(IGymService gymService)
    {
        _gymService = gymService;
    }

    [BindProperty]
    public string FirstName { get; set; } = "";

    [BindProperty]
    public string LastName { get; set; } = "";

    [BindProperty]
    public string Address { get; set; } = "";

    [BindProperty]
    public string Email { get; set; } = "";

    [BindProperty]
    public string Contact { get; set; } = "";

    [BindProperty]
    public string MembershipType { get; set; } = "";

    [BindProperty]
    public string PaymentMethod { get; set; } = "creditCard";

    [BindProperty]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [BindProperty]
    public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(1);

    [BindProperty]
    public bool IsPriceUpdate { get; set; } = false;

    public Dictionary<string, int> Prices { get; set; } = new();
    public int TotalPrice { get; set; }
    public bool ShowForm { get; set; } = true;

    public async Task OnGetAsync()
    {
        await FetchPricesAsync();
        CalculateTotalPrice();
        UpdateEndDate();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await FetchPricesAsync();

        // Update end date when start date changes
        UpdateEndDate();

        // Recalculate total price based on membership type
        CalculateTotalPrice();

        if (IsPriceUpdate)
        {
            return Page();
        }

        return Page();
    }


    public async Task<IActionResult> OnPostReset()
    {
        // Reset all form fields to their default values
        FirstName = "";
        LastName = "";
        Address = "";
        Email = "";
        Contact = "";
        MembershipType = "";
        PaymentMethod = "creditCard";
        StartDate = DateTime.Today;
        EndDate = DateTime.Today.AddMonths(1);
        ShowForm = true;
        IsPriceUpdate = false;

        await FetchPricesAsync();
        CalculateTotalPrice();

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSubmit()
    {
        await FetchPricesAsync();
        CalculateTotalPrice();
        UpdateEndDate();

        if (!ModelState.IsValid || !ValidateInputs())
        {
            ModelState.AddModelError("", "Please fill in all required fields correctly.");
            return Page();
        }

        AddMembershipDto membershipDto = new AddMembershipDto
        {
            ClientFname = FirstName,
            ClientLname = LastName,
            Address = Address,
            Email = Email,
            Phone = Contact,
            MembershipType = MembershipType,
            StartDate = StartDate.ToString("yyyy-MM-dd"),
            EndDate = EndDate.ToString("yyyy-MM-dd"),
            TotalPrice = TotalPrice.ToString(),
            PaymentMethod = PaymentMethod
        };

        try
        {
            int result = await _gymService.AddMembershipAsync(membershipDto, TotalPrice, PaymentMethod);

            if (result > 0)
            {
                if (PaymentMethod == "creditCard")
                {
                    return RedirectToPage("/PaymentPage/PaymentPage", new { email = Email, amount = TotalPrice });
                }
                else // onSite payment
                {
                    ShowForm = false;
                    return Page();
                }
            }

            ModelState.AddModelError("", "Failed to process your membership. Please try again.");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"An error occurred: {ex.Message}");
        }

        return Page();
    }

    private async Task FetchPricesAsync()
    {
        try
        {
            Prices = await _gymService.GetMembershipPricesAsync();
        }
        catch
        {
            Prices = new Dictionary<string, int>();
        }
    }

    private void CalculateTotalPrice()
    {
        if (!string.IsNullOrEmpty(MembershipType) && Prices.TryGetValue(MembershipType, out var price))
        {
            TotalPrice = price;
        }
        else
        {
            TotalPrice = 0;
        }
    }

    private void UpdateEndDate()
    {
        EndDate = StartDate.AddMonths(1);
    }

    private bool ValidateInputs()
    {
        return !string.IsNullOrWhiteSpace(FirstName) &&
               !string.IsNullOrWhiteSpace(LastName) &&
               !string.IsNullOrWhiteSpace(Address) &&
               new EmailAddressAttribute().IsValid(Email) &&
               System.Text.RegularExpressions.Regex.IsMatch(Contact, @"^\+?[1-9]\d{1,14}$") &&
               !string.IsNullOrWhiteSpace(MembershipType);
    }
}