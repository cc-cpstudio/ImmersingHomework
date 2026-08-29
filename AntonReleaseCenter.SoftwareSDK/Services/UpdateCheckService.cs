using System.Net;
using System.Text.Json;
using AntonReleaseCenter.Core.DTOs;
using AntonReleaseCenter.Core.Models;
using AntonReleaseCenter.SoftwareSDK.Model;

namespace AntonReleaseCenter.SoftwareSDK.Services;

public sealed class UpdateCheckService(HttpClient httpClient, Configure configure)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly Configure _configure = configure;

    public async Task<CheckUpdateResponse?> CheckUpdateAsync(
        string currentVersion,
        int channelCode = 0,
        PlatformEnum? platform = null,
        string? deviceId = null,
        CancellationToken ct = default)
    {
        var effectiveChannelCode = channelCode != 0 ? channelCode : _configure.ChannelCode;
        var effectivePlatform = platform ?? _configure.Platform;

        var queryParameters = new List<string>
        {
            $"appKey={WebUtility.UrlEncode(_configure.SoftwareKey)}",
            $"channelCode={effectiveChannelCode}",
            $"platform={effectivePlatform}",
            $"currentVersion={WebUtility.UrlEncode(currentVersion)}"
        };

        if (!string.IsNullOrEmpty(deviceId))
            queryParameters.Add($"deviceId={WebUtility.UrlEncode(deviceId)}");

        var url = $"{_configure.Url}/api/public/check-update?{string.Join("&", queryParameters)}";

        var result = await _httpClient.GetAsync(url, ct);
        if (result.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (result.StatusCode == HttpStatusCode.Unauthorized)
            throw new UpdateCheckException("认证失败：请检查 SoftwareKey 是否正确");

        if (result.StatusCode == HttpStatusCode.Forbidden)
            throw new UpdateCheckException("权限不足：当前软件无权访问此渠道");

        if (result.StatusCode == HttpStatusCode.BadRequest)
            throw new UpdateCheckException("请求参数错误：请检查 channelCode 和 platform 是否有效");

        if ((int)result.StatusCode >= 500)
            throw new UpdateCheckException($"服务器错误：{(int)result.StatusCode}");

        result.EnsureSuccessStatusCode();

        var json = await result.Content.ReadAsStringAsync(ct);
        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        return JsonSerializer.Deserialize<CheckUpdateResponse>(json, options);
    }
}

public class UpdateCheckException : Exception
{
    public UpdateCheckException(string message) : base(message) { }
    public UpdateCheckException(string message, Exception innerException)
        : base(message, innerException) { }
}
