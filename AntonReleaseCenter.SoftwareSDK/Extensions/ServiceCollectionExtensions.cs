using AntonReleaseCenter.SoftwareSDK.Model;
using AntonReleaseCenter.SoftwareSDK.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AntonReleaseCenter.SoftwareSDK.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSoftwareSDK(
        this IServiceCollection services,
        Configure configure)
    {
        services.AddSingleton(configure);
        services.AddTransient<UpdateCheckService>();
        services.AddSingleton<FileDownloadService>();
        return services;
    }
}
