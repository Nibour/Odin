using System.Collections.Concurrent;
using IPStackCommunicationLibrary;
using WebApi.Dto;
using WebApi.Model;

public class IPInfoService : IIPInfoService
{
    private readonly IIPInfoRepository _repository;
    private readonly ConcurrentDictionary<Guid, UpdateJob> _jobs;

    public IPInfoService(IIPInfoRepository repository, ConcurrentDictionary<Guid, UpdateJob> jobs)
    {
        _repository = repository;
        _jobs = jobs;
    }

    public async Task<IPEntityDto> GetIPDetailsAsync(string ip)
    {
        // Validate IP address
        if (string.IsNullOrWhiteSpace(ip))
            throw new ArgumentException("Invalid IP address");

        // Get details from repository
        var details = await _repository.GetDetailsAsync(ip);
        var dto = new IPEntityDto
        {
            IP = ip,
            City = details.City,
            Country = details.Country,
            Continent = details.Continent,
            Latitude = details.Latitude,
            Longitude = details.Longitude
        };

    return dto;
    }

    public UpdateJobDto? GetJobStatus(Guid jobId)
{
    // Try to get the job from the dictionary
    if (_jobs.TryGetValue(jobId, out var job))
    {
        // If found, return the mapped UpdateJobDto
        return new UpdateJobDto
        {
            JobId = jobId,
            IsCompleted = job.IsCompleted,
            PendingItems = job.Buffer.Count
        };
    }
    
    // If not found, return null
    return null;
}

    
    public Guid CreateUpdateJob(IEnumerable<IPEntityDto> details)
    {   

        var entities = details.Select(ConvertToEntity);

        var job = new UpdateJob();
        job.Buffer.AddRange(entities);

        _jobs[job.JobId] = job;
        return job.JobId; // Return the JobId to the client
    }
    
    public async Task ProcessJobsAsync()
    {
        foreach (var job in _jobs.Values.Where(j => !j.IsCompleted))
        {
            while (job.Buffer.Count > 0)

            {
                var batch = job.Buffer.Take(10).ToList(); // Get a batch of 10
                job.Buffer = job.Buffer.Skip(10).ToList(); // Remove the processed items

                // Update database and cache
                await _repository.UpdateDatabaseAsync(batch);
                _repository.UpdateCache(batch);
            }

            job.IsCompleted = true; // Mark the job as complete
        }
    }

    private IPEntityDto ConvertToDto(IPEntity entity)
    {
        return new IPEntityDto
        {
            IP = entity.IP,
            City = entity.City,
            Country = entity.Country,
            Continent = entity.Continent,
            Latitude = entity.Latitude,
            Longitude = entity.Longitude
        };
    }

    private IPEntity ConvertToEntity(IPEntityDto dto)
    {
        return new IPEntity
        {
            IP = dto.IP,
            City = dto.City,
            Country = dto.Country,
            Continent = dto.Continent,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude
        };
    }
}
