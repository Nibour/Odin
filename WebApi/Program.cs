using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using WebApi.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(sp => new IPInfoprovider(builder.Configuration["IPStackAPIKey"]??""));

builder.Services.AddSingleton<ConcurrentDictionary<Guid, UpdateJob>>();

builder.Services.AddScoped<IIPInfoRepository, IPInfoRepository>();
builder.Services.AddScoped<IIPInfoService, IPInfoService>();

builder.Services.AddMemoryCache();

builder.Services.AddControllers();

builder.Services.AddSingleton<IHostedService, JobProcessingHostedService>();

builder.Services.AddDbContext<IPInfoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();

app.MapControllers();

app.Run();
