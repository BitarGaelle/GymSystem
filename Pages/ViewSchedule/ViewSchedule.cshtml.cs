using GymSystem.Models;
using GymSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ViewScheduleModel> _logger;
        private readonly IGymService _gymService;

        public ViewScheduleModel(IHttpClientFactory httpClientFactory, ILogger<ViewScheduleModel> logger, IGymService gymService)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _gymService = gymService;
        }

        public List<ActivityScheduleDto> Schedules { get; set; } = new List<ActivityScheduleDto>();


        public string ErrorMessage { get; set; } = string.Empty;

        public bool Loading { get; set; } = false;

        public async Task<IActionResult> OnGetAsync()
        {
            Loading = true;

            try
            {
                Schedules = await _gymService.GetActivityScheduleAsync();

            }
            catch (TaskCanceledException tex)
            {
                _logger.LogError(tex, "Request timed out while fetching schedules.");
                ErrorMessage = "Request timed out. Please try again later.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while fetching schedules.");
                ErrorMessage = "An unexpected error occurred while fetching data.";
            }
            finally
            {
                Loading = false;
            }

            return Page();
        }
    }
}
