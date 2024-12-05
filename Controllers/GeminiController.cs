using GemBardPT.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

public class GeminiController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly GeminiApiSettings _geminiApiSettings;

    public GeminiController(IHttpClientFactory httpClientFactory, GeminiApiSettings geminiApiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _geminiApiSettings = geminiApiSettings;
    }

    [HttpPost]
    public async Task<IActionResult> GetResponseFromGemini(string input)
    {
        var client = _httpClientFactory.CreateClient();
        var apiUrl = "https://api.gemini.ai/v1/endpoint";

        var requestData = new { input = input };

        // Usando a chave da API no cabeçalho da solicitação
        client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _geminiApiSettings.ApiKey);

        var response = await client.PostAsync(
            apiUrl,
            new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json")
        );

        if (!response.IsSuccessStatusCode)
        {
            return View("Error");
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseContent);

        return View("Index", apiResponse);
    }
}
