using System.Text.Json.Serialization;
using IPStackCommunicationLibrary;

namespace WebApi.Dto;

public class IPEntityDto : IPDetails
{
    [JsonPropertyName("IP")]
    public required string IP { get; set; }
}
