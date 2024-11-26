namespace WebApi.Model;

public class UpdateJob
{
    public Guid JobId { get; } = Guid.NewGuid();
    public List<IPEntity> Buffer { get; set; } = [];
    public bool IsCompleted { get; set; } = false;
}
