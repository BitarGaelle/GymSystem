using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace GymSystem.Pages.AdminPage
{
    public class DashboardModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DashboardModel> _logger;

        public DashboardModel(HttpClient httpClient, ILogger<DashboardModel> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public int TotalClients { get; set; }
        public int TotalMemberships { get; set; }
        public int TotalActivities { get; set; }

        public async Task OnGetAsync()
        {
            await FetchDashboardData();
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            // Clear any authentication cookies or session data
            HttpContext.Session.Clear();

            // If using Cookie Authentication
            // await HttpContext.SignOutAsync();

            return RedirectToPage("/Index"); // or wherever your login page is
        }

        private async Task FetchDashboardData()
        {
            try
            {
                var response = await _httpClient.GetAsync("http://localhost:3003/totalcount");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<DashboardData>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (data != null)
                    {
                        TotalClients = data.TotalClients;
                        TotalMemberships = data.TotalMemberships;
                        TotalActivities = data.TotalActivities;
                    }
                }
                else
                {
                    _logger.LogWarning("Failed to fetch dashboard data. Status: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching dashboard data");
                // Set default values or handle error appropriately
                TotalClients = 0;
                TotalMemberships = 0;
                TotalActivities = 0;
            }
        }
    }

    public class DashboardData
    {
        public int TotalClients { get; set; }
        public int TotalMemberships { get; set; }
        public int TotalActivities { get; set; }
    }
}