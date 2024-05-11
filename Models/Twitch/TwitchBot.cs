using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace RaffleApp.Models.Twitch;

public class TwitchBot
{
    private static class TwitchConstants
    {
        public const int TWITCH_PORT = 6667;
        public const string TWITCH_IP = "irc.chat.twitch.tv";
        public const string PASS_FORMAT = "PASS {0}";
        public const string USER_FORMAT = "NICK {0}";
        public const string PING = "PING";
        public const string PONG = "PONG :tmi.twitch.tv";
        public const string PRIVMSG = "PRIVMSG";
    }

    public bool SignedIn { get; private set; }

    private bool shutDown = false;
    private readonly string username;
    private readonly string oAuthToken;
    private readonly string keyWord;
    private readonly string channelName;
    private TcpClient tcpClient;
    private StreamReader streamReader;
    private StreamWriter streamWriter;
    private PriorityQueue<string, int> messageQueue = new PriorityQueue<string, int>(8);

    public TwitchBot(TwitchSettings settings)
    {
        username = settings.Username;
        oAuthToken = settings.Token;
        keyWord = settings.Keyword;
        channelName = settings.ChannelName;

        //TODO: implement better task handling & cancellation, this is hack
        _ = BotRoutine();
    }

    public void Shutdown()
    {
        shutDown = true;
    }

    private async Task BotRoutine()
    {
        Console.WriteLine($"Bot {username} establishing connection...");
        tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(TwitchConstants.TWITCH_IP, TwitchConstants.TWITCH_PORT);
        streamReader = new StreamReader(tcpClient.GetStream());
        streamWriter = new StreamWriter(tcpClient.GetStream()) { NewLine = "\r\n", AutoFlush = true };

        SignedIn = await TrySignIn();
        // disabling this until I'm more confident about everything
        //await streamWriter.WriteLineAsync($"PRIVMSG #{channelName} :I'm a harmless bot joining this chat for testing, please do not judge");
        Task listenLoop = BotListenLoop();

        while (!shutDown)
        {
            if (SignedIn)
            {
                string element;
                int priority;

                while (messageQueue.Count > 0)
                {
                    if (messageQueue.TryDequeue(out element, out priority))
                    {
                        HandleMessage(element, priority);
                    }

                    if (messageQueue.Count == 0)
                    {
                        await Task.Delay(10);
                    }
                }
            }
            else
            {
                await Task.Delay(10);
                Console.WriteLine($"Bot {username} not signed in.");
                SignedIn = await TrySignIn();
            }

            await Task.Delay(10);
        }

        Console.WriteLine($"Bot {username} shutting down...");

        while (listenLoop.Status == TaskStatus.Running)
        {
            await Task.Delay(10);
        }

        tcpClient.Dispose();
        streamReader.Dispose();
        streamWriter.Dispose();
        Console.WriteLine($"Bot {username} shut down.");
    }

    private async Task BotListenLoop()
    {
        while (!shutDown)
        {
            string? line = await streamReader.ReadLineAsync();

            if (line == null)
            {
                await Task.Delay(10);
                continue;
            }

            if (line.StartsWith(TwitchConstants.PING))
            {
                messageQueue.Enqueue(TwitchConstants.PING, 0);
            }
            else
            {
                string[] split = line.Split(' ');

                if (split[1] == TwitchConstants.PRIVMSG)
                {
                    //Grab this name here
                    int exclamationPointPosition = split[0].IndexOf("!");
                    string senderName = split[0].Substring(1, exclamationPointPosition - 1);
                    //Skip the first character, in other words the first colon, then find the next colon
                    int secondColonPosition = line.IndexOf(':', 1); //the 1 here is what skips the first character
                    string message = line.Substring(secondColonPosition + 1); //Everything past the second colon
                    messageQueue.Enqueue($"{senderName}:{message}", 1);
                }
                else
                {
                    messageQueue.Enqueue(line, 2);
                }
            }
        }
    }

    private async Task<bool> TrySignIn()
    {
        Console.WriteLine($"Bot {username} signing in.");
        await streamWriter.WriteLineAsync(string.Format(TwitchConstants.PASS_FORMAT, oAuthToken));
        await streamWriter.WriteLineAsync(string.Format(TwitchConstants.USER_FORMAT, username));
        await streamWriter.WriteLineAsync($"JOIN #{channelName}");
        return true;
    }

    private void HandleMessage(string messageBody, int messageType)
    {
        //TODO: actual message handling
        switch (messageType)
        {
            case 0:
                _ = streamWriter.WriteLineAsync(TwitchConstants.PONG);
                break;
            case 1:
                Console.WriteLine(messageBody);
                // Check for keyword
                if (keyWord.Length > 0)
                {
                    string[] userAndMessage = messageBody.Split(':');
                    
                    if (userAndMessage.Length > 1 && userAndMessage[1].Contains($"!{keyWord}"))
                    {
                       RaffleData.TryAddParticipant(userAndMessage[0]); 
                    }
                }
                break;
            case 2:
                Console.WriteLine(messageBody);
                break;
            default:
                break;
        }
    }
}