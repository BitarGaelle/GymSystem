using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

public class ClientPageModel : PageModel
{
    private readonly HttpClient _httpClient;

    public ClientPageModel(IHttpClientFactory clientFactory)
    {
        _httpClient = clientFactory.CreateClient();
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
            var response = await _httpClient.GetAsync("http://localhost:3003/client");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            Clients = JsonSerializer.Deserialize<List<Client>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                var query = SearchQuery.ToLower();
                FilteredClients = Clients
                    .Where(c => $"{c.FirstName} {c.LastName}".ToLower().Contains(query))
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

//issue i encountered, the [JsonPropertyName] wasn't the same as the database column name
//so fname and lname weren't showing
public class Client
{
    [JsonPropertyName("client_fname")]
    public string FirstName { get; set; }

    [JsonPropertyName("client_lname")]
    public string LastName { get; set; }

    [JsonPropertyName("address")]
    public string Address { get; set; }

    [JsonPropertyName("phone")]
    public string Phone { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }
}