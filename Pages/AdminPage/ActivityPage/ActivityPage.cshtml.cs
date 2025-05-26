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

public class ActivityPageModel : PageModel
{
    private readonly IGymService _gymService;

    public ActivityPageModel( IGymService gymService)
    {
        _gymService = gymService;
    }

    public List<Activity> Activities { get; set; } = new();
    public List<Activity> FilteredActivities { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string SearchQuery { get; set; }

    public string Error { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            Activities = await _gymService.GetAllActivitiesAsync();

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                var query = SearchQuery.ToLower();
                FilteredActivities = Activities
                    .Where(c => $"{c.activity_name}".ToLower().Contains(query))
                    .ToList();
            }
            else
            {
                FilteredActivities = Activities;
            }
        }
        catch (HttpRequestException ex)
        {
            Error = "Error fetching activities data";
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int activityId)
    {
        try
        {
            bool deleted = await _gymService.DeleteActivityAsync(activityId);
            if (!deleted)
            {
                Error = "Activity not found or already deleted.";
                return Page();
            }
        }
        catch (Exception ex)
        {
            Error = "Error deleting activity.";
            // Optionally log the error
            return Page();
        }

        return RedirectToPage(); // Refresh the page after deletion
    }



}
