using SQLite;

namespace RaffleApp.Models.Twitch;

public class TwitchSettings
{
    // temporary hack to make sure sqlite doesn't save a lot of unnecessary stuff we're not using
    [Unique] 
    public int id { get; set; } = 1;

    public string Username { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string Keyword { get; set; } = string.Empty;
    public string ChannelName { get; set; } = string.Empty;

    public bool ValidateNoneMissing()
    {
        return Username.Length > 0
            && Token.Length > 0
            && ChannelName.Length > 0;
    }
}