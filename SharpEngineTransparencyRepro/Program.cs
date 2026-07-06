using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Vulkan;

namespace SharpEngineTransparencyRepro;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
#if VULKAN_BACKEND
            .With(new Win32PlatformOptions
            {
                RenderingMode = [Win32RenderingMode.Vulkan]
            })
            .With(new VulkanOptions
            {
                VulkanDeviceCreationOptions = new VulkanDeviceCreationOptions
                {
                    PreferDiscreteGpu = true
                }
            })
#endif
            .LogToTrace();
    }
}
