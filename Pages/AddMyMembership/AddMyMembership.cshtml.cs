using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text;
using System.Net.Http;

public class AddMyMembershipModel : PageModel
{
    [BindProperty] public string FirstName { get; set; } = "";
    [BindProperty] public string LastName { get; set; } = "";
    [BindProperty] public string Address { get; set; } = "";
    [BindProperty] public string Email { get; set; } = "";
    [BindProperty] public string Contact { get; set; } = "";
    [BindProperty] public string MembershipType { get; set; } = "";
    [BindProperty] public string PaymentMethod { get; set; } = "creditCard";
    [BindProperty] public DateTime StartDate { get; set; } = DateTime.Today;
    [BindProperty] public DateTime EndDate { get; set; } = DateTime.Today.AddMonths(1);
    public Dictionary<string, decimal> Prices { get; set; } = new();
    public decimal TotalPrice { get; set; }
    public bool ShowForm { get; set; } = true;

    public async Task OnGetAsync()
    {
        await FetchPricesAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await FetchPricesAsync();
        CalculateTotalPrice();
        UpdateEndDate();

        if (!ModelState.IsValid || !ValidateInputs())
        {
            ModelState.AddModelError("", "Invalid input.");
            return Page();
        }

        var client = new
        {
            client_fname = FirstName,
            client_lname = LastName,
            address = Address,
            email = Email,
            phone = Contact,
            membership_type = MembershipType,
            payment_method = PaymentMethod,
            total_price = TotalPrice,
            start_date = StartDate.ToString("yyyy-MM-dd"),
            end_date = EndDate.ToString("yyyy-MM-dd")
        };

        try
        {
            using var http = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(client), Encoding.UTF8, "application/json");
            var response = await http.PostAsync("http://localhost:3003/client", content);

            if (response.IsSuccessStatusCode)
            {
                ShowForm = false;
                return Page();
            }

            ModelState.AddModelError("", "Failed to add membership.");
        }
        catch
        {
            ModelState.AddModelError("", "Server error.");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostResetAsync()
    {
        FirstName = LastName = Address = Email = Contact = MembershipType = "";
        PaymentMethod = "creditCard";
        StartDate = DateTime.Today;
        EndDate = StartDate.AddMonths(1);
        ShowForm = true;

        await FetchPricesAsync();

        return Page();
    }

    private async Task FetchPricesAsync()
    {
        try
        {
            using var http = new HttpClient();
            var response = await http.GetAsync("http://localhost:3003/membershipprices");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            Prices = JsonSerializer.Deserialize<Dictionary<string, decimal>>(json) ?? new();
        }
        catch
        {
            Prices = new();
        }
    }

    private void CalculateTotalPrice()
    {
        if (!string.IsNullOrEmpty(MembershipType) && Prices.TryGetValue(MembershipType, out var price))
        {
            TotalPrice = price;
        }
    }

    private void UpdateEndDate()
    {
        EndDate = StartDate.AddMonths(1);
    }

    private bool ValidateInputs()
    {
        return
            !string.IsNullOrWhiteSpace(FirstName) &&
            !string.IsNullOrWhiteSpace(LastName) &&
            !string.IsNullOrWhiteSpace(Address) &&
            new EmailAddressAttribute().IsValid(Email) &&
            System.Text.RegularExpressions.Regex.IsMatch(Contact, @"^\+?[1-9]\d{1,14}$") &&
            !string.IsNullOrWhiteSpace(MembershipType);
    }
}
