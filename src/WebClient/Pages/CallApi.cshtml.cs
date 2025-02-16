using System.Text.Json;
using Duende.AccessTokenManagement.OpenIdConnect;
using Duende.IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyApp.Namespace
{
    public class CallApiModel : PageModel
    {
        public string Json = string.Empty;

        public async Task OnGet()
        {
            UserToken tokenInfo = await HttpContext.GetUserAccessTokenAsync();
            HttpClient client = new HttpClient();
            client.SetBearerToken(tokenInfo.AccessToken!);

            string content = await client.GetStringAsync("https://localhost:6001/identity");

            JsonDocument parsed = JsonDocument.Parse(content);
            string formatted = JsonSerializer.Serialize(parsed, new JsonSerializerOptions {WriteIndented = true});

            Json = formatted;
        }
    }
}