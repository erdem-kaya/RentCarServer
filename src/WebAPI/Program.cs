using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.RateLimiting;
using RentCarServer.Application;
using RentCarServer.Infrastructure;
using Scalar.AspNetCore;
using System.Threading.RateLimiting;
using WebAPI;
using WebAPI.Modules;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddRateLimiter(configureOptions =>
{
    configureOptions.AddFixedWindowLimiter("fixed", options =>
    {
        options.PermitLimit = 100;
        options.QueueLimit = 100;
        options.Window = TimeSpan.FromSeconds(1);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    configureOptions.AddFixedWindowLimiter("login-fixed", options =>
    {
        options.PermitLimit = 5;
        options.QueueLimit = 1;
        options.Window = TimeSpan.FromMinutes(1);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

builder.Services.AddControllers()
    .AddOData(options =>
    {
        options.Select()
        .Filter()
        .Count()
        .Expand()
        .OrderBy()
        .SetMaxTop(null);
    });

builder.Services.AddCors();
builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<ExceptionHandler>().AddProblemDetails();
builder.Services.AddResponseCompression(option => { option.EnableForHttps = true; });

var app = builder.Build();
app.MapOpenApi();
app.MapScalarApiReference();
app.UseHttpsRedirection();
app.UseCors(policy => policy
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod()
    .SetPreflightMaxAge(TimeSpan.FromMinutes(10)));
app.UseResponseCompression();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.UseExceptionHandler();
app.MapControllers().RequireRateLimiting("fixed").RequireAuthorization();
app.MapAuthEndpoint();
app.MapGet("/", () => "hello world").RequireAuthorization();
//await app.CreateFirstUser();
app.Run();
