using IPStackCommunicationLibrary;
using WebApi.Model;

public interface IIPInfoRepository
{
    Task<IPDetails> GetDetailsAsync(string ip);
    public Task UpdateDatabaseAsync(List<IPEntity> batch);
    public void UpdateCache(List<IPEntity> batch);
}

