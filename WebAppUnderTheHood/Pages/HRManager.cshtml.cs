using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using WebAppUnderTheHood.Authorization;
using WebAppUnderTheHood.DTO;
using WebAppUnderTheHood.Pages.Account;

namespace WebAppUnderTheHood.Pages;

[Authorize(Policy = "HRManagerOnly")]
public class HRManager : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    [BindProperty]
    public List<WeatherForecastDto> WeatherForecastItems { get; set; }

    public HRManager(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task OnGet()
    {
        // Get token from the session
        JwtToken token = null;

        var strTokenObj = HttpContext.Session.GetString("access_token");
        if (string.IsNullOrEmpty(strTokenObj))
        {
            token = await Authenticate();
        }
        else
        {
            token = JsonConvert.DeserializeObject<JwtToken>(strTokenObj) ?? new JwtToken();
        }

        if (string.IsNullOrWhiteSpace(token.AccessToken) || token.ExpireAt <= DateTime.UtcNow)
        {
            token = await Authenticate();
        }

        var httpClient = _httpClientFactory.CreateClient("OurWebAPI");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken);
        WeatherForecastItems = await httpClient.GetFromJsonAsync<List<WeatherForecastDto>>("WeatherForecast") ?? [];
    }

    private async Task<JwtToken> Authenticate()
    {
        // Authentication and getting the token
        var httpClient = _httpClientFactory.CreateClient("OurWebAPI");
        var res = await httpClient.PostAsJsonAsync("Auth",
            new Credential { Username = "admin", Password = "password" });
        res.EnsureSuccessStatusCode();
        var strJwt = await res.Content.ReadAsStringAsync();
        HttpContext.Session.SetString("access_token", strJwt);

        return JsonConvert.DeserializeObject<JwtToken>(strJwt) ?? new JwtToken();
    }
}