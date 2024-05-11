using System;
using System.IO;
using System.Linq;
using RaffleApp.Models.Twitch;
using SQLite;

namespace RaffleApp.ViewModels.Twitch;

public static class TwitchManager
{
    public static TwitchSettings CurrentTwitchSettings { get; private set; }
    public static bool Initialized { get; private set; }
    public static bool IsBotSwitchedOn { get; private set; } = false;
    
    private static TwitchBot? twitchBot = null;
    private static readonly SQLiteConnection db =
        new SQLiteConnection(Path.Combine(AppContext.BaseDirectory, "twitch.sqlite"));

    public static void SetBotSwitchedOn(bool value)
    {
        if (IsBotSwitchedOn != value)
        {
            IsBotSwitchedOn = value;

            if (IsBotSwitchedOn)
            {
                if (twitchBot != null)
                {
                    twitchBot.Shutdown();
                    twitchBot = null;
                }

                twitchBot = new TwitchBot(CurrentTwitchSettings);
            }
            else if (twitchBot != null)
            {
                twitchBot.Shutdown();
                twitchBot = null;
            }
        }
    }

    public static void SaveTwitchSettings()
    {
        db.InsertOrReplace(CurrentTwitchSettings);
    }

    public static void InitializeTwitchSettings()
    {
        if (Initialized)
        {
            Console.WriteLine(
                "Attempted to initialize Twitch Settings twice.");
            return;
        }

        Initialized = true;
        db.CreateTable<TwitchSettings>();

        try
        {
            TableQuery<TwitchSettings> query = db.Table<TwitchSettings>();
            CurrentTwitchSettings = query.FirstOrDefault(new TwitchSettings());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            CurrentTwitchSettings = new TwitchSettings();
        }
    }
}