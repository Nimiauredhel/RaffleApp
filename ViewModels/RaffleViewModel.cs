using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Data;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using RaffleApp.Models;

namespace RaffleApp.ViewModels;

public partial class RaffleViewModel : ViewModelBase
{
    private App app => Application.Current as App;
    
    [ObservableProperty][NotifyPropertyChangedFor(nameof(AllParticipants))]
    private ObservableCollection<Participant> allParticipants = RaffleData.AllParticipants;
    [ObservableProperty][NotifyPropertyChangedFor(nameof(CurrentParticipants))]
    private ObservableCollection<Participant> currentParticipants = RaffleData.CurrentParticipants;
    [ObservableProperty][NotifyPropertyChangedFor(nameof(RaffleEntries))]
    private ObservableCollection<Participant> raffleEntries = new ObservableCollection<Participant>();
    [ObservableProperty][NotifyPropertyChangedFor(nameof(RaffleStateText))]
    private string raffleStateText;
    [ObservableProperty][NotifyPropertyChangedFor(nameof(RaffleInProgress))]
    private bool raffleInProgress = false;

    public RaffleViewModel()
    {
    }

    public void BeginRaffle()
    {
        if (RaffleInProgress) return;
        Console.WriteLine("Requested raffle...");

        if (CurrentParticipants.Count == 0)
        {
            Console.WriteLine("No Participants Entered!");
            return;
        }

        if (CurrentParticipants.Count == 1)
        {
            Console.WriteLine("Only one participant, they win by default!");
            CurrentParticipants[0].OnWon();
            return;
        }

        _ = DoRaffle();
    }

    private async Task DoRaffle()
    {
        Optional<bool> confirmed = await Dialogs.ConfirmationDialog(
            "Are you sure?\nThis will decide the winner and cannot be undone.", "Let's go!", "Not yet.", Brushes.Red,
            Brushes.Yellow, new Thickness(5), Brushes.Black, 24);
        while (!confirmed.HasValue) await Task.Delay(10);

        if (confirmed.Value == false)
        {
            Console.WriteLine("Aborting raffle");
            return;
        }

        RaffleInProgress = true;
        Console.WriteLine("Doing the raffle procedure.");
        RaffleStateText = "Beginning Raffle!";
        app.RaffleView.CurrentParticipantList.ItemsSource = CurrentParticipants;
        RaffleEntries.Clear();
        Random random = new Random(DateTime.Now.Ticks.GetHashCode() + DateTime.Now.Nanosecond);

        await Task.Delay(500);
        RaffleStateText = "Shuffling Participants!";

        for (int i = 0; i < CurrentParticipants.Count; i++)
        {
            await Task.Delay(250 / (i + 1));
            CurrentParticipants.Shuffle(random);
        }

        await Task.Delay(500);
        RaffleStateText = "Dispensing Tickets!";

        for (int i = CurrentParticipants.Count - 1; i >= 0; i--)
        {
            Participant participant = CurrentParticipants[i];
            int entryCount = 1 + participant.ConsecutiveLost;

            for (int j = 0; j < entryCount; j++)
            {
                await Task.Delay(250 / (j + 1));
                RaffleEntries.Add(participant);
            }

            CurrentParticipants.RemoveAt(i);
            await Task.Delay(250 / (i + 1));
        }

        await Task.Delay(500);
        RaffleStateText = "Shuffling Tickets!";
        app.RaffleView.CurrentParticipantList.ItemsSource = RaffleEntries;

        for (int i = 0; i < RaffleEntries.Count; i++)
        {
            await Task.Delay(250 / (i + 1));
            RaffleEntries.Shuffle(random);
        }

        await Task.Delay(500);
        RaffleStateText = "Eliminating Tickets!";

        while (RaffleEntries.Count > 1)
        {
            await Task.Delay(1000 / RaffleEntries.Count);
            int randomIndex = (int)(random.NextSingle() * RaffleEntries.Count);
            Participant removed = RaffleEntries[randomIndex];
            RaffleEntries.RemoveAt(randomIndex);

            if (!RaffleEntries.Contains(removed))
            {
                removed.OnLost();
            }
        }

        string winnerName = RaffleEntries[0].Name;
        await Task.Delay(500);
        RaffleEntries[0].OnWon();
        RaffleEntries.Clear();
        RaffleData.Save();
        RaffleStateText = $"{winnerName} is the winner!";
        await Task.Delay(500);
        
        await Dialogs.InfoDialog(
                    RaffleStateText, "Congrats!", Brushes.Orchid,
                    Brushes.Plum, new Thickness(5), Brushes.Gold, 32);

        RaffleInProgress = false;
    }
}