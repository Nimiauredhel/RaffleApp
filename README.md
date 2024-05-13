# Raffle App
### What is it?
A simple desktop app for managing raffles with a so called "pity" mechanic (extra tickets for past losses), featuring Twitch integration for running a bot and letting chatters sign themselves up.
Uses AvaloniaUI over .NET C#, and SQLite because why the hell not.
My first time writing a Twitch bot and using Avalonia so it might not be top notch software, but it definitely works!
## How do I use it?
Add participants to the current raffle, and push the big "Start Raffle!" button to watch as the app does some shuffling and elimination to select a winner.
Each participant gets one ticket, plus as many tickets as the number of their "consecutive losses".
When a raffle is over, the winner has their "consecutive losses" zeroed, while every other participant has it incremented by one.
This mechanic is intended for situations where you'd like to favor people who keep showing up to raffles but haven't had a win.
## Twitch integration?
This app also doubles as a Twitch bot, which allows chatters to join an open raffle via a chat keyword.
You only need to have a spare Twitch account to use as a bot.
The "Twitch Settings" panel has all the fields you need to fill out for this to work:
* The bot's username
* The bot's OAuth token (it's easy, google it)
* A keyword chatters can use to join the raffle (preceded by "!" in the chat, but not here - the app handles that part for you)
* The name of the channel on which the bot should listen for the keyword

The Twitch-Bot On/Off switch instantly starts and terminates the bot's listening activity, respectively.
It may take the bot a moment to log-in. There's not a lot of feedback at the moment for what the bot is doing and whether it encountered any issues, but that's coming very soon.
