using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using RemindMeApp.Server;
using RemindMeApp.Server.Extensions;
using RemindMeApp.Server.Reminders;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add Serilog
builder.AddSerilog();

// Configure the database
var connectionString = builder.Configuration.GetConnectionString("RemindMeDbContext") ?? "Data Source=.db/Reminders.db";
builder.Services.AddSqlite<RemindMeDbContext>(connectionString);

// Configure Open API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<SwaggerGeneratorOptions>(options
    => options.InferSecuritySchemes = true);

// Configure rate limiting
builder.Services.AddRateLimiting();

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Add Serilog requests logging
app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseRateLimiter();

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.Map("/", () => Results.Redirect("/swagger"));

// Configure the APIs
app.MapReminders();

app.MapRazorPages();

app.MapFallbackToFile("index.html");

app.Run();
