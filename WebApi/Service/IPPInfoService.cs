using IPStackCommunicationLibrary;
using WebApi.Model;

public interface IIPInfoService
{
    Task<IPDetails> GetIPDetailsAsync(string ip);
    public Guid CreateUpdateJob(IEnumerable<IPEntity> details);
    Task ProcessJobsAsync();
    public UpdateJob? GetJobStatus(Guid jobId);
}
