using GymSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace GymSystem.Pages.AdminPage
{
    public class DashboardModel : PageModel
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DashboardModel> _logger;
        private readonly IGymService _gymServices;

        public DashboardModel(HttpClient httpClient, ILogger<DashboardModel> logger, IGymService gymServices)
        {
            _httpClient = httpClient;
            _logger = logger;
            _gymServices = gymServices;
        }

        public int TotalClients { get; set; }
        public int TotalMemberships { get; set; }
        public int TotalActivities { get; set; }

        public async Task OnGetAsync()
        {
            await FetchDashboardData();
        }


        private async Task FetchDashboardData()
        {
            try
            {

                 var totalCounts = await _gymServices.GetTotalCountAsync();

                TotalClients = totalCounts.TotalClients;
                TotalMemberships = totalCounts.TotalMemberships;
                TotalActivities = totalCounts.TotalActivities;
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