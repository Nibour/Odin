using System.Collections.Concurrent;
using IPStackCommunicationLibrary;
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

    public async Task<IPDetails> GetIPDetailsAsync(string ip)
    {
        // Validate IP address
        if (string.IsNullOrWhiteSpace(ip))
            throw new ArgumentException("Invalid IP address");

        // Get details from repository
        return await _repository.GetDetailsAsync(ip);
    }

    public UpdateJob? GetJobStatus(Guid jobId)
    {
        return _jobs.TryGetValue(jobId, out var job) ? job : null;
    }
    
    public Guid CreateUpdateJob(IEnumerable<IPEntity> details)
    {
        var job = new UpdateJob();
        job.Buffer.AddRange(details);

        _jobs[job.JobId] = job;
        return job.JobId; // Return the JobId to the client
    }
    
    public async Task ProcessJobsAsync()
    {
        foreach (var job in _jobs.Values.Where(j => !j.IsCompleted))
        {
            while (job.Buffer.Count > 0)
            {
                var batch = job.Buffer.Take(200).ToList(); // Get a batch of 200
                job.Buffer = job.Buffer.Skip(200).ToList(); // Remove the processed items

                // Update database and cache
                await _repository.UpdateDatabaseAsync(batch);
                _repository.UpdateCache(batch);
            }

            job.IsCompleted = true; // Mark the job as complete
        }
    }
}
