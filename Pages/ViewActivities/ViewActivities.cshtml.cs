using GymSystem.Services;
using GymSystem.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace GymSystem.Pages.ViewActivities
{
    public class ViewActivitiesModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IGymService _gymService;

        public ViewActivitiesModel(IHttpClientFactory httpClientFactory, IGymService gymService)
        {
            _httpClientFactory = httpClientFactory;
            _gymService = gymService;
        }

        public List<Activity> Activities { get; set; } = new List<Activity>();
        public string Error { get; set; } = string.Empty;
        public bool Loading { get; set; } = true;

        public Dictionary<string, string> ActivityImages { get; } = new()
        {
            { "Boxing", "/images/Boxing.jpg" },
            { "MuayThai", "/images/MuayThai.jpg" },
            { "MMABJJ", "/images/MMABJJ.jpg" },
            { "Kickboxing", "/images/Kickboxing.jpg" },
            { "HipHop", "/images/HipHop.jpg" },
            { "Kpop", "/images/Kpop.jpg" },
            { "Ballet", "/images/Ballet.jpg" },
            { "Salsa", "/images/Salsa.jpg" },
            { "Basketball", "/images/Basketball.jpg" },
            { "Football", "/images/Football.jpg" },
            { "Swimming", "/images/Swimming.jpg" },
            { "Pilates", "/images/Pilates.jpg" },
            { "Yoga", "/images/Yoga.jpg" },
            { "RegularGym", "/images/RegularGym.jpg" },
            { "Tennis", "/images/Tennis.jpg" }
        };

        public string FallbackImage { get; } = "/images/Fallback.jpg";

        public async Task OnGetAsync()
        {
            
            try
            {
                Activities = await _gymService.GetAllActivitiesAsync();


            }
            catch
            {
                Error = "Error fetching activities";
                Activities.Clear();
            }
            finally
            {
                Loading = false;
            }
        }
    }


}
