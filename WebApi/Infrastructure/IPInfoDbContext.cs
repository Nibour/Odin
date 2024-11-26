using Microsoft.EntityFrameworkCore;
using WebApi.Model;

public class IPInfoDbContext : DbContext
{
    public IPInfoDbContext(DbContextOptions<IPInfoDbContext> options)
        : base(options) { }

    public DbSet<IPEntity>? IPEntity { get; set; }
}
