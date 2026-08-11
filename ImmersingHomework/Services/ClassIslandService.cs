using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ClassIsland.Shared.IPC;
using ClassIsland.Shared.IPC.Abstractions.Services;
using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;
using ImmersingHomework.Shared.Models;
using Serilog;

namespace ImmersingHomework.Services;

public class ClassIslandService
{
    public static ClassIslandService Instance { get; } = new ClassIslandService();

    private readonly ILogger _logger = Log.ForContext<ClassIslandService>();
    private IpcClient _client = new IpcClient();
    private bool _initialized;

    public bool Initialized => _initialized;

    public event EventHandler<string> HomeworkAssignmentReminder;
    
    private ClassIslandService()
    {
    }

    public void Initialize()
    {
        if (_initialized) return;

        _initialized = true;
        _client.JsonIpcProvider.AddNotifyHandler(IpcRoutedNotifyIds.OnBreakingTimeNotifyId, () =>
        {
            var lessonService = _client.Provider.CreateIpcProxy<IPublicLessonsService>(_client.PeerProxy);
            var profileService = _client.Provider.CreateIpcProxy<IPublicProfileService>(_client.PeerProxy);
            
            var classes = lessonService.CurrentClassPlan.Classes;
            
            // TODO 实现获取上一节课
        });
        Connect();
    }

    public List<string> GetSubjects()
    {
        if (!_initialized) return new List<string>();
        
        var profileService = _client.Provider.CreateIpcProxy<IPublicProfileService>(_client.PeerProxy);
        return profileService.Profile.Subjects.Select(s => s.Value.Name)
            .Distinct().ToList();
    }

    private async void Connect()
    {
        try
        {
            await _client.Connect();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "连接 ClassIsland IPC 服务失败");
        }
    }
}