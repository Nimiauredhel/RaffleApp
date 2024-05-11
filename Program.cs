using Avalonia;
using Avalonia.ReactiveUI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Extensions.Controls;
using RaffleApp.Models;
using RaffleApp.Models.Twitch;
using RaffleApp.ViewModels.Twitch;

namespace RaffleApp;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static async Task<int> Main(string[] args)
    {
        var builder = BuildAvaloniaApp();

        if (args.Contains("--drm"))
        {
            SilenceConsole();
            return builder.StartLinuxDrm(args, null, false);
        }

        RaffleData.Initialize();

        if (!TwitchManager.Initialized)
        {
            TwitchManager.InitializeTwitchSettings();
        }
        
        return builder.StartWithClassicDesktopLifetime(args);
    }

    private static void SilenceConsole()
    {
        new Thread(() =>
            {
                Console.CursorVisible = false;
                while (true)
                    Console.ReadKey(true);
            })
            { IsBackground = true }.Start();
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .UseDoveExtensions()
            .LogToTrace()
            .UseReactiveUI();
}