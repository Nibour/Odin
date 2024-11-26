
using System.Text.Json;
using IPStackCommunicationLibrary;

public class IPInfoprovider : IPPInfoprovider
{
    private readonly string _apiKey;
    private readonly HttpClient _httpClient;

    public IPInfoprovider(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("API Key cannot be null or empty", nameof(apiKey));

        _apiKey = apiKey;
        _httpClient = new HttpClient();
    }

    public IPDetails GetDetails(string ip)
    {
        try
        {
            var response = _httpClient.GetAsync($"http://api.ipstack.com/{ip}?access_key={_apiKey}").Result;

            if (!response.IsSuccessStatusCode)
                throw new IPServiceNotAvailableException($"Failed to fetch details for IP {ip}. API returned status code {response.StatusCode}");

            var jsonResponse = response.Content.ReadAsStringAsync().Result;
            var ipDetails = JsonSerializer.Deserialize<IPDetails>(jsonResponse);

            if (ipDetails == null)
                throw new IPServiceNotAvailableException($"Failed to parse API response for IP {ip}.");

            return ipDetails;
        }
        catch (Exception ex) when (ex is HttpRequestException || ex is JsonException)
        {
            throw new IPServiceNotAvailableException("An error occurred while communicating with the IPStack API.", ex);
        }
    }
}