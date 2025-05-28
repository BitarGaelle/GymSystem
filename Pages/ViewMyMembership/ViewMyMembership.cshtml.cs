using GymSystem.Models;
using GymSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace GymSystem.Pages.ViewMyMembership {


    public class ViewMyMembershipModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IGymService _gymService;

        public ViewMyMembershipModel(IHttpClientFactory httpClientFactory, IConfiguration configuration, IGymService gymService)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _gymService = gymService;
        }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        public List<MembershipDetailsDto> MembershipData { get; set; } = new List<MembershipDetailsDto>();

        public string ErrorMessage { get; set; } = string.Empty;
        public bool HasSearched { get; set; } = false;

        public bool Loading { get; set; } = false;

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Please enter a valid email.";
                return Page();
            }

            HasSearched = true;

            Loading = true;
            try
            {
                MembershipData = await _gymService.GetMembershipByEmailAsync(Email);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error fetching data: {ex.Message}";
            }
            finally
            {
                Loading = false;
            }

            return Page();
        }

    }


}
