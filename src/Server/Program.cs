using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using RemindMeApp.Server;
using RemindMeApp.Server.Authentication;
using RemindMeApp.Server.Data;
using RemindMeApp.Server.Extensions;
using RemindMeApp.Server.Reminders;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.AddSerilog();

// Configure the database
string connectionString = builder.Configuration.GetConnectionString("RemindMeDbContext") ??
                          throw new InvalidOperationException("Connection string 'RemindMeDbContext' not found.");
builder.Services.AddSqlite<RemindMeDbContext>(connectionString);

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<RemindMeDbContext>();

builder.Services.AddIdentityServer()
    .AddConfigurationStore(options =>
        options.ConfigureDbContext = b => b.UseSqlite(connectionString))
    .AddOperationalStore(options =>
        options.ConfigureDbContext = b => b.UseSqlite(connectionString))
    .AddApiAuthorization<ApplicationUser, RemindMeDbContext>();

builder.Services.AddAuthentication()
    .AddIdentityServerJwt();
builder.Services.AddLocalApiAuthentication();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

if (builder.Environment.IsDevelopment())
{
    // Configure Open API
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.Configure<SwaggerGeneratorOptions>(options
        => options.InferSecuritySchemes = true);

    builder.Services.AddDatabaseDeveloperPageExceptionFilter();
}

var app = builder.Build();

// Add Serilog requests logging
app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseWebAssemblyDebugging();

    app.UseSwagger();
    app.UseSwaggerUI();
    app.Map("/", () => Results.Redirect("/swagger"));
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

app.UseIdentityServer();
app.UseAuthorization();

// Configure the APIs
app.MapReminders();
app.MapOpenIdConnectEndpoint();
app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

await app.InitializeDatabaseAsync();

app.Run();
