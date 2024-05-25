using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.ReactiveUI;
using RaffleApp.Models;
using RaffleApp.ViewModels.Twitch;

namespace RaffleApp;

internal static class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static async Task<int> Main(string[] args)
    {
        var builder = BuildAvaloniaApp();

        RaffleData.Initialize();

        if (!TwitchManager.Initialized)
        {
            TwitchManager.InitializeTwitchSettings();
        }
        
        return builder.StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    private static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}