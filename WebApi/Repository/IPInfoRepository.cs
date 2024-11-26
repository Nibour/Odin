using IPStackCommunicationLibrary;
using Microsoft.Extensions.Caching.Memory;
using WebApi.Model;
using Microsoft.EntityFrameworkCore;
using System.Data;


public class IPInfoRepository : IIPInfoRepository
{
    private readonly IPInfoprovider _ipProvider;
    private readonly IMemoryCache _cache;
    private readonly IPInfoDbContext _dbContext;
    
    public IPInfoRepository(IPInfoprovider ipProvider, IMemoryCache cache, IPInfoDbContext dbContext)
    {
        _ipProvider = ipProvider;
        _cache = cache;
        _dbContext = dbContext;
    }

    public async Task<IPDetails> GetDetailsAsync(string ip)
    {
        try
        {
            // --- Check if the result is already cached ---
            if (_cache.TryGetValue(ip, out IPDetails? details) && details != null)
            {
                return details; // Return cached result
            }

            // --- Check if it exists on DB ---
            var existingEntity = await _dbContext.IPEntity!.FindAsync(ip);
            if (existingEntity != null)
            {
                details = new IPDetails
                {
                    City = existingEntity.City,
                    Country = existingEntity.Country,
                    Continent = existingEntity.Continent,
                    Latitude = existingEntity.Latitude,
                    Longitude = existingEntity.Longitude
                };

                // Cache the details and return
                _cache.Set(ip, details, TimeSpan.FromMinutes(1));
                return details;
            }
            

            // --- Fetch details from the provider ---
            details = await Task.FromResult(_ipProvider.GetDetails(ip));

            if (details == null)
            {
                throw new Exception("No details found for the given IP address.");
            }

            // Save details to the database
            var newEntity = new IPEntity
            {
                IP = ip,
                City = details.City,
                Country = details.Country,
                Continent = details.Continent,
                Latitude = details.Latitude,
                Longitude = details.Longitude
            };
            _dbContext.IPEntity!.Add(newEntity);

            // Store the result in the cache
            _cache.Set(ip, details, TimeSpan.FromMinutes(1));

            return details;
        }
        catch (Exception ex)
        {
            throw new IPServiceNotAvailableException("Failed to fetch IP details from IPStack.", ex);
        }

    }

    public async Task UpdateDatabaseAsync(List<IPEntity> batch)
    {
        foreach (var details in batch)
        {   
            if(_dbContext == null || _dbContext.IPEntity == null){
                throw new DataException();
            }
            
            // Check if the entity already exists based on IP
            var existingEntity = await _dbContext.IPEntity
                .FirstOrDefaultAsync(entity => entity.IP == details.IP);

            if (existingEntity != null)
            {
                // Update existing entity
                existingEntity.City = details.City;
                existingEntity.Country = details.Country;
                existingEntity.Continent = details.Continent;
                existingEntity.Latitude = details.Latitude;
                existingEntity.Longitude = details.Longitude;
            }
            else
            {
                // Add new entity
                var newEntity = new IPEntity
                {
                    IP = details.IP,
                    City = details.City,
                    Country = details.Country,
                    Continent = details.Continent,
                    Latitude = details.Latitude,
                    Longitude = details.Longitude
                };
                await _dbContext.IPEntity.AddAsync(newEntity);
            }
        }

        // Save changes once after processing all entities
        await _dbContext.SaveChangesAsync();
    }


    public void UpdateCache(List<IPEntity> batch)
    {
        foreach (var details in batch)
        {
            _cache.Set(details.IP, details, TimeSpan.FromMinutes(1));
        }
    }
}
