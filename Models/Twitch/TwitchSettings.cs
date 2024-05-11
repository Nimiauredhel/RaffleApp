using SQLite;

namespace RaffleApp.Models.Twitch;

public class TwitchSettings
{
    // temporary hack to make sure sqlite doesn't save a lot of unnecessary stuff we're not using
    [Unique] 
    public int id { get; set; }
    public string Username { get; set; }
    public string Token { get; set; }
    public string Keyword { get; set; }
    public string ChannelName { get; set; }

    public TwitchSettings(string username, string token, string keyword, string channelName)
    {
        id = 0;
        Username = username;
        Token = token;
        Keyword = keyword;
        ChannelName = channelName;
    }
    
    public TwitchSettings()
    {
        id = 0;
        Username = string.Empty;
        Token = string.Empty;
        Keyword = string.Empty;
        ChannelName = string.Empty;
    }

    public bool ValidateNoneMissing()
    {
        return Username.Length > 0
            && Token.Length > 0
            && ChannelName.Length > 0;
    }
}