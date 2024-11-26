using IPStackCommunicationLibrary;
using WebApi.Dto;
using WebApi.Model;

public interface IIPInfoService
{
    Task<IPEntityDto> GetIPDetailsAsync(string ip);
    public Guid CreateUpdateJob(IEnumerable<IPEntityDto> details);
    Task ProcessJobsAsync();
    public UpdateJobDto? GetJobStatus(Guid jobId);
}
