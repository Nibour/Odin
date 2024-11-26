using System.Text.Json.Serialization;

namespace WebApi.Dto;

public class UpdateJobDto
{
    [JsonPropertyName("jobId")]
    public Guid JobId { get; set;}
    [JsonPropertyName("isCompleted")]
    public bool IsCompleted { get; set;}
    [JsonPropertyName("pendingItems")]
    public int PendingItems { get; set; }
}
