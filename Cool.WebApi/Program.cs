using Cool.WebApi.Extensions;
using Jarvis.WebApi;
using Jarvis.WebApi.Auth;
using Jarvis.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMonitoring();
builder.Services.AddCoreDefault(builder.Configuration);

builder.Services
    .AddHttpConnectionStringResolver()
    .AddMasterContext()
    .AddAppContext()
    .AddServices()
    .AddMultiTenancy();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCoreSwagger();

app.UseHttpsRedirection();

app.UseCoreAuth();

app.UseMiddleware<ApiResponseWrapperMiddleware>();

app.MapControllers();

await app.RunAsync();
