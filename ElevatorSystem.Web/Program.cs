using DomainLogger = ElevatorSystem.Domain.Interfaces.ILogger;
using ElevatorSystem.Domain.Entities;
using ElevatorSystem.Domain.ValueObjects;
using ElevatorSystem.Domain.Interfaces;
using ElevatorSystem.Infrastructure.Logging;
using ElevatorSystem.Infrastructure.Services;
using ElevatorSystem.Infrastructure.Time;
using ElevatorSystem.Application.UseCases;
using ElevatorSystem.Web.Components;
using ILogger = Microsoft.Extensions.Logging.ILogger; // App.razor principal

var builder = WebApplication.CreateBuilder(args);

// ===== Blazor default stuff =====
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ===== Elevator dependencies =====
builder.Services.AddSingleton<DomainLogger, ConsoleLogger>();
builder.Services.AddSingleton<ITimeProvider, SystemTimeProvider>();
builder.Services.AddSingleton<IRequestQueue, FifoRequestQueue>();

builder.Services.AddSingleton<Elevator>(_ => new Elevator(1, new Floor(1)));
builder.Services.AddSingleton<IElevatorController, ElevatorController>();
builder.Services.AddSingleton<RequestElevatorUseCase>();

var app = builder.Build();

// ===== ASP.NET pipeline (do template) =====
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

// Map Blazor root component
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// ===== start controller when app starts =====
var controller = app.Services.GetRequiredService<IElevatorController>();
(controller as ElevatorController)?.Start();

// ===== keep server running =====
app.Run();