using System;

namespace RaffleApp.Models;

public static class TwitchManager
{
    public static bool IsBotSwitchedOn
    {
        get => isBotSwitchedOn;

        set
        {
            if (isBotSwitchedOn != value)
            {
                isBotSwitchedOn = value;

                if (isBotSwitchedOn)
                {
                    if (twitchBot != null)
                    {
                        twitchBot.Shutdown();
                        twitchBot = null;
                    }

                    twitchBot = new TwitchBot(Username, Token, Keyword);
                }
                else if (twitchBot != null)
                {
                    twitchBot.Shutdown();
                    twitchBot = null;
                }
            }
        }
    }

    private static bool isBotSwitchedOn;
    public static string Username { set; private get; }
    public static string Token { set; private get; }
    public static string Keyword { set; private get; }
    private static TwitchBot? twitchBot = null;
}