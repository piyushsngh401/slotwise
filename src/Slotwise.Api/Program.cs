using Scalar.AspNetCore;
using Slotwise.Api;
using Slotwise.ServiceDefaults;
using Slotwise.SharedKernel.Modules;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddModules(ModuleCatalog.All);

builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapDefaultEndpoints();
app.MapServiceInfo();
app.MapModules();

app.Run();
