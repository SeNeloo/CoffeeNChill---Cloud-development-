using System.Net.Http;

namespace CoffeeNChill.Gateway.Services;

public class FunctionsProxyService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public FunctionsProxyService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<HttpResponseMessage> ForwardRequestAsync(
        HttpMethod method,
        string endpoint,
        HttpContent? content = null)
    {
        string baseUrl =
            _configuration["Functions:BaseUrl"]
            ?? throw new InvalidOperationException(
                "Functions BaseUrl is not configured.");

        var request = new HttpRequestMessage(
            method,
            new Uri(new Uri(baseUrl), endpoint));

        request.Content = content;

        return await _httpClient.SendAsync(request);
    }
}
