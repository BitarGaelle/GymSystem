using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

public class LoginModel : PageModel
{
    private readonly ILogger<LoginModel> _logger;

    [BindProperty]
    public string Username { get; set; }

    [BindProperty]
    public string Password { get; set; }

    public string Error { get; set; }

    public LoginModel(ILogger<LoginModel> logger)
    {
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        return Page();
    }

    public IActionResult OnPost()
    {
        if (Username == "admin" && Password == "admin")
        {
            return RedirectToPage("/AdminPage/Dashboard/Dashboard");
        }
        else
        {
            Error = "Invalid username or password";
            return Page();
        }
    }
}
