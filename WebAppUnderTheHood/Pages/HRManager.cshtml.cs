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
        var httpClient = _httpClientFactory.CreateClient("OurWebAPI");
        var res = await httpClient.PostAsJsonAsync("Auth",
            new Credential { Username = "admin", Password = "password" });
        res.EnsureSuccessStatusCode();
        var strJwt = await res.Content.ReadAsStringAsync();
        var token = JsonConvert.DeserializeObject<JwtToken>(strJwt);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token?.AccessToken);
        WeatherForecastItems = await httpClient.GetFromJsonAsync<List<WeatherForecastDto>>("WeatherForecast") ?? [];
    }
}