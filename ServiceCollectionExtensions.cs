using Bikiran.Payment.Bkash.Configuration;
using Bikiran.Payment.Bkash.Exceptions;
using Bikiran.Payment.Bkash.Services;
using Bikiran.Payment.Bkash.HealthChecks;
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

    /// <summary>
    /// Adds bKash health check to the health check builder
    /// </summary>
    /// <param name="builder">Health check builder</param>
    /// <param name="name">Name of the health check (default: bkash)</param>
    /// <param name="tags">Tags for the health check</param>
    /// <returns>Health check builder for chaining</returns>
    public static IHealthChecksBuilder AddBkashHealthCheck(
        this IHealthChecksBuilder builder,
        string name = "bkash",
        params string[] tags)
    {
        return builder.AddCheck<BkashHealthCheck>(name, tags: tags);
    }

    private static void RegisterServices(IServiceCollection services)
    {
        // Register HttpClient for bKash API calls
        services.AddHttpClient<BkashTokenService>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<BkashOptions>>();
            client.BaseAddress = new Uri(options.Value.GetBaseUrl());
            client.Timeout = TimeSpan.FromSeconds(options.Value.TimeoutSeconds);
        });

        services.AddHttpClient<BkashPaymentService>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<BkashOptions>>();
            client.BaseAddress = new Uri(options.Value.GetBaseUrl());
            client.Timeout = TimeSpan.FromSeconds(options.Value.TimeoutSeconds);
        });

        // Register token service as singleton to maintain token cache across requests
        services.AddSingleton<IBkashTokenService, BkashTokenService>();
        
        // Register payment service as scoped
        services.AddScoped<IBkashPaymentService, BkashPaymentService>();
    }
}
