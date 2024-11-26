using System.Text.Json.Serialization;

namespace IPStackCommunicationLibrary;

public class IPDetails : IIPDetails
{  
    [JsonPropertyName("city")]
    public required string City { get; set; }
    [JsonPropertyName("country_name")]
    public required string Country {get; set;}
    [JsonPropertyName("continent_name")]
    public required string Continent { get; set; }
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }
    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }
}
