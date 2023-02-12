using Serilog;
using Serilog.Exceptions;

namespace RemindMeApp.Server.Extensions;

public static class SerilogExtensions
{
    public static WebApplicationBuilder AddSerilog(
        this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, loggerConfiguration) =>
        {
            loggerConfiguration.ReadFrom.Configuration(context.Configuration)
                .Enrich.WithProperty("Application", builder.Environment.ApplicationName)
                .Enrich.FromLogContext()
                .Enrich.WithExceptionDetails();

            if (builder.Environment.IsDevelopment())
            {
                loggerConfiguration.WriteTo.Console();
            }
        });

        return builder;
    }
}
