using Microsoft.EntityFrameworkCore;
using CDC_Diabetes.Data;
using CDC_Diabetes.Models;
using CDC_Diabetes.Web.Components;
using CDC_Diabetes.Data.Services;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURATION CHECK ---
// This ensures the app actually finds your connection string
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection");

if (string.IsNullOrEmpty(connectionString))
{
    // This will print to your terminal if appsettings.json is missing or incorrectly formatted
    Console.WriteLine("CRITICAL ERROR: Connection string 'PostgresConnection' not found!");
    throw new InvalidOperationException("Connection string 'PostgresConnection' not found in appsettings.json.");
}

// --- 2. SERVICE REGISTRATION ---
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// PostgreSQL Connection (This is your AddDbContext)
builder.Services.AddDbContext<HealthcareDbContext>(options =>
    options.UseNpgsql(connectionString));

// ADD THIS LINE HERE: Register our Intelligence Layer
// This tells .NET: "Whenever a page asks for IHealthRiskService, give them HealthRiskService"
builder.Services.AddScoped<IHealthRiskService, HealthRiskService>();

var app = builder.Build();

// --- 3. MIDDLEWARE PIPELINE ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Log success to terminal on start
Console.WriteLine(">>> Application Started Successfully");
Console.WriteLine($">>> Connected to database: {connectionString.Split(';')[1]}");

app.Run();