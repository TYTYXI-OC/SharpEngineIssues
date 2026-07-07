using Avalonia;

namespace SharpEngineDockCrash;

internal class Program
{
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .With(new Win32PlatformOptions
            {
                // If Use AngleEgl, it will not crash
                RenderingMode = [Win32RenderingMode.Vulkan]
            })
            .LogToTrace();
}
