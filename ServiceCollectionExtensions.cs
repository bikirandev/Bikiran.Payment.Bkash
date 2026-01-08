using Bikiran.Payment.Bkash.Configuration;
using Bikiran.Payment.Bkash.Exceptions;
using Bikiran.Payment.Bkash.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bikiran.Payment.Bkash;

/// <summary>
/// Extension methods for configuring bKash payment services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds bKash payment services to the service collection
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration containing bKash settings</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddBkashPayment(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        // Configure options from configuration
        services.Configure<BkashOptions>(configuration.GetSection(BkashOptions.SectionName));

        // Validate options on startup
        services.AddOptions<BkashOptions>()
            .Bind(configuration.GetSection(BkashOptions.SectionName))
            .ValidateOnStart();

        RegisterServices(services);

        return services;
    }

    /// <summary>
    /// Adds bKash payment services to the service collection with explicit configuration
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configureOptions">Action to configure bKash options</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddBkashPayment(
        this IServiceCollection services,
        Action<BkashOptions> configureOptions)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        if (configureOptions == null)
            throw new ArgumentNullException(nameof(configureOptions));

        services.Configure(configureOptions);

        // Validate options
        var options = new BkashOptions();
        configureOptions(options);
        options.Validate();

        RegisterServices(services);

        return services;
    }

    private static void RegisterServices(IServiceCollection services)
    {
        // Register HttpClient for bKash API calls
        services.AddHttpClient<IBkashTokenService, BkashTokenService>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<BkashOptions>>();
            client.BaseAddress = new Uri(options.Value.GetBaseUrl());
            client.Timeout = TimeSpan.FromSeconds(options.Value.TimeoutSeconds);
        });

        services.AddHttpClient<IBkashPaymentService, BkashPaymentService>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<BkashOptions>>();
            client.BaseAddress = new Uri(options.Value.GetBaseUrl());
            client.Timeout = TimeSpan.FromSeconds(options.Value.TimeoutSeconds);
        });

        // Register services as scoped (can be changed to singleton if needed)
        services.AddScoped<IBkashTokenService, BkashTokenService>();
        services.AddScoped<IBkashPaymentService, BkashPaymentService>();
    }
}
