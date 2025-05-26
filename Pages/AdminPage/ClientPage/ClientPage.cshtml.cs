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

public class ClientPageModel : PageModel
{
    private readonly HttpClient _httpClient;
    private readonly IGymService _gymService;

    public ClientPageModel(IHttpClientFactory clientFactory, IGymService gymService)
    {
        _httpClient = clientFactory.CreateClient();
        _gymService = gymService;
    }

    public List<Client> Clients { get; set; } = new();
    public List<Client> FilteredClients { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string SearchQuery { get; set; }

    public string Error { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            Clients = await _gymService.GetAllClientsAsync();

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                var query = SearchQuery.ToLower();
                FilteredClients = Clients
                    .Where(c => $"{c.client_fname} {c.client_lname}".ToLower().Contains(query))
                    .ToList();
            }
            else
            {
                FilteredClients = Clients;
            }
        }
        catch (HttpRequestException ex)
        {
            Error = "Error fetching clients data";
        }
    }


}
