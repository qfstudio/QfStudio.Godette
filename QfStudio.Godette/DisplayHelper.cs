using Godot;

namespace QfStudio.Godette;

public static class DisplayHelper
{
    /// <summary>
    /// 参考实现 https://github.com/godotengine/godot/blob/f7fcb79c4399070be07caadfbe2304a9c94711fa/editor/settings/editor_settings.cpp#L1978
    /// </summary>
    /// <returns></returns>
    public static double GetAutoDisplayScale()
    {
        return DisplayServer.Singleton.ScreenGetDpi() / 96.0;
    }
}
