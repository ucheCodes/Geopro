using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using PenPro.Data;
using PenPro.Database;
using PenPro.Store;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddOxyPlotBlazor();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddScoped<DatabaseService>();
builder.Services.AddScoped<IStore,Store>();
builder.Services.AddScoped<IFunctions,Functions>();
builder.Services.AddScoped<State>();
builder.Services.AddScoped<WriteToFile>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
