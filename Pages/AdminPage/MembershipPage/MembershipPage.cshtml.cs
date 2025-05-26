using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using GymSystem.Models;
using GymSystem.Services;

public class MembershipPageModel : PageModel
{
    private readonly HttpClient _httpClient;
    private readonly IGymService _gymService;

    public MembershipPageModel(IHttpClientFactory clientFactory, IGymService gymService)
    {
        _httpClient = clientFactory.CreateClient();
        _gymService = gymService;
    }

    public List<Membership> Memberships { get; set; } = new();
    public List<Membership> FilteredMemberships { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string SearchQuery { get; set; }

    public string Error { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            Memberships = await _gymService.GetAllMembershipsAsync();

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                var query = SearchQuery.ToLower();
                FilteredMemberships = Memberships
                    .Where(c => $"{c.membership_type}".ToLower().Contains(query))
                    .ToList();
            }
            else
            {
                FilteredMemberships = Memberships;
            }
        }
        catch (HttpRequestException ex)
        {
            Error = "Error fetching membership data";
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int membershipId)
    {
        try
        {
            bool deleted = await _gymService.DeleteMembershipAsync(membershipId);
            if (!deleted)
            {
                Error = "Membership not found or already deleted.";
                return Page();
            }
        }
        catch (Exception ex)
        {
            Error = "Error deleting membership.";
            // Optionally log the error
            return Page();
        }

        return RedirectToPage(); // Refresh the page after deletion
    }



}
