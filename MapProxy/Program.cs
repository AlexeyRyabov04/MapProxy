using MapProxy.Data;
using MapProxy.Middleware;
using MapProxy.Services.Implementations;
using MapProxy.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy",
        builder => builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreConnection")));

builder.Services.AddSingleton<IConnectionMultiplexer>(s => ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisConnection")));

builder.Services.AddScoped<IAccessRuleService, AccessRuleService>();
builder.Services.AddScoped<IRedisService, RedisService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("MyPolicy");

app.Map("/arcservertest/rest/services", proxyApp =>
{
    proxyApp.UseMiddleware<ProxyMiddleware>();
});


//app.UseHttpsRedirection();


app.MapControllers();

app.Run();
