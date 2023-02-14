using RemindMeApp.Server.Authentication;
using RemindMeApp.Server.Data;
using RemindMeApp.Server.Extensions;
using RemindMeApp.Server.Reminders;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilog();

string connectionString = builder.Configuration.GetConnectionString("RemindMeDbContext") ??
                          throw new InvalidOperationException("Connection string 'RemindMeDbContext' not found.");
builder.Services.AddSqlite<RemindMeDbContext>(connectionString);

builder.Services.AddAuthenticationAndAuthorization();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}

var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthenticationAndAuthorization();

// Configure the APIs
app.MapReminders();

// Needed for auth
app.MapRazorPages();
app.MapControllers();

// Load the index.html from the wasm client
app.MapFallbackToFile("index.html");

await app.InitializeDatabaseAsync();

app.Run();
