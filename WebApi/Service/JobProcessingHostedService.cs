public class JobProcessingHostedService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public JobProcessingHostedService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Create a new scope to resolve the scoped service IIPInfoService
            using (var scope = _scopeFactory.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<IIPInfoService>();

                // Call the method to process jobs
                await service.ProcessJobsAsync();
            }

            // Poll every 10 seconds
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }
}
