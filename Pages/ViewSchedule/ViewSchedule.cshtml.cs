using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace GymSystem.Pages.ViewSchedule
{
    public class ViewScheduleModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ViewScheduleModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public List<Schedule> Schedules { get; set; } = new List<Schedule>();

        public string ErrorMessage { get; set; } = string.Empty;

        public bool Loading { get; set; } = false;

        public async Task<IActionResult> OnGetAsync()
        {
            Loading = true;

            try
            {
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(10);
                var response = await client.GetAsync("http://localhost:3003/activityschedule");

                if (response.IsSuccessStatusCode)
                {
                    var responseStream = await response.Content.ReadAsStreamAsync();
                    var schedules = await JsonSerializer.DeserializeAsync<List<Schedule>>(responseStream);

                    Schedules = schedules ?? new List<Schedule>();
                }
                else
                {
                    ErrorMessage = "Failed to fetch schedules.";
                }
            }
            catch (Exception)
            {
                ErrorMessage = "An unexpected error occurred while fetching data.";
            }
            finally
            {
                Loading = false;
            }

            return Page();
        }

        public class Schedule
        {
            public string ActivityName { get; set; }
            public string DayOfWeek { get; set; }
            public string StartHour { get; set; }
            public string EndHour { get; set; }
        }
    }
}
