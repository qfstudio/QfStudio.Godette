namespace QfStudio.Godette.Tests.Operators;

internal static class FrameTestHelper
{
    public static void AdvanceFrame(this GodotFrameScheduler scheduler, int count = 1, double delta = 1.0 / 60)
    {
        for (var i = 0; i < count; i++)
            scheduler.NotifyProcess(delta);
    }
}
