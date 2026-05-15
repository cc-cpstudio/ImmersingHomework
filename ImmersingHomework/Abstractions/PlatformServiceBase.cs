using Avalonia.Controls;

namespace ImmersingHomework.Abstractions;

public abstract class PlatformServiceBase
{
    public abstract void SetTopmost(Window window, bool enable =  true);
    public abstract void DisableFocus(Window window);
    public abstract void HideFromTaskbar(Window window);
    public abstract void HideFromAltTab(Window window);
}