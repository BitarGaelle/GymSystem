using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace YourNamespace.Pages.UserPage
{
    public class PaymentPageModel : PageModel
    {
        [BindProperty]
        [Required]
        public string CardholderName { get; set; }

        [BindProperty]
        [Required]
        [RegularExpression(@"\d{4}\s?\d{4}\s?\d{4}\s?\d{4}", ErrorMessage = "Invalid card number format")]
        public string CardNumber { get; set; }

        [BindProperty]
        [Required]
        public string ExpiryDate { get; set; }

        [BindProperty]
        [Required]
        [RegularExpression(@"\d{3,4}", ErrorMessage = "Invalid CVV")]
        public string CVV { get; set; }

        [BindProperty]
        [Required]
        public string BillingAddress { get; set; }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page(); // Re-render the form with validation errors
            }

            TempData["PaymentMessage"] = "Payment successful 💪";
            return RedirectToPage("/homepage/home");
        }
    }
}
