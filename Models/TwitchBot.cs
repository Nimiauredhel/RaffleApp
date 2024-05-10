using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace RaffleApp.Models;

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
    private TcpClient tcpClient;
    private StreamReader streamReader;
    private StreamWriter streamWriter;
    private PriorityQueue<string, int> messageQueue = new PriorityQueue<string, int>(8);

    public TwitchBot(string name, string token, string keyword)
    {
        username = name;
        oAuthToken = token;
        keyWord = keyword;

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
        await streamWriter.WriteLineAsync(
            "PRIVMSG #twitchchannelname :I'm a harmless bot joining this chat for testing, please do not judge");
        _ = BotListenLoop();

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
                }
            }
            else
            {
                Console.WriteLine($"Bot {username} not signed in.");
                SignedIn = await TrySignIn();
            }

            await Task.Delay(10);
        }
    }

    private async Task BotListenLoop()
    {
        while (!shutDown)
        {
            string? line = await streamReader.ReadLineAsync();
            if (line == null) continue;

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
        await streamWriter.WriteLineAsync($"JOIN #pikat");
        return true;
    }

    private void HandleMessage(string messageBody, int messageType)
    {
        //TODO: actual message handling
        Console.WriteLine(messageBody);
    }
}