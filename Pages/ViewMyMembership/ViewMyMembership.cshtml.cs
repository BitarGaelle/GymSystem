using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace GymSystem.Pages.ViewMyMembership {


    public class ViewMyMembershipModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ViewMyMembershipModel(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        public List<Membership> MembershipData { get; set; } = new List<Membership>();

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
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"http://localhost:3003/membership?email={Email}");
                if (response.IsSuccessStatusCode)
                {
                    var responseStream = await response.Content.ReadAsStreamAsync();
                    var responseData = await JsonSerializer.DeserializeAsync<ApiResponse>(responseStream);

                    if (responseData != null)
                    {
                        MembershipData = responseData.Memberships;
                    }
                    else
                    {
                        ErrorMessage = "No membership data found.";
                    }
                }
                else
                {
                    ErrorMessage = "No membership found for this email.";
                }
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

        public class Membership
        {
            public string ActivityName { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string PaymentMethod { get; set; }
            public decimal Amount { get; set; }
        }

        public class ApiResponse
        {
            public List<Membership> Memberships { get; set; }
        }
    }


}
