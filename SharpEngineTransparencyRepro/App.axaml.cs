using System;
using Ab4d.SharpEngine.Common;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace SharpEngineTransparencyRepro;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            if (e.ExceptionObject is SharpEngineException exception)
            {
                System.Diagnostics.Debug.WriteLine($"Unhandled SharpEngineException: {exception.Message}");
            }
        };

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
        }
        Ab4d.SharpEngine.Licensing.SetLicense(
           );
        base.OnFrameworkInitializationCompleted();
    }
}
