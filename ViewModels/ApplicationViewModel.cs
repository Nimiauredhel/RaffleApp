using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Notification;
using Avalonia.Threading;
using DynamicData.Binding;
using RaffleApp.Models;
using ReactiveUI;

namespace RaffleApp.ViewModels;

public class ApplicationViewModel : ViewModelBase
{
#pragma warning disable CA1822 // Mark members as static
#pragma warning restore CA1822 // Mark members as static

    public ObservableCollection<Participant> AllParticipants => RaffleData.AllParticipants;
    public ObservableCollection<Participant> CurrentParticipants => RaffleData.CurrentParticipants;
    public ObservableCollection<Participant> RaffleEntries => raffleEntries;
    public FlatTreeDataGridSource<Participant> ParticipantSource { get; private set; }
    public INotificationMessageManager NotificationManager { get; } = new NotificationMessageManager();

    public bool RaffleInProgress
    {
        get => raffleInProgress;
        set => this.RaiseAndSetIfChanged(ref raffleInProgress, value);
    }

    public string RaffleStateText
    {
        get => raffleStateText;
        set => this.RaiseAndSetIfChanged(ref raffleStateText, value);
    }

    private bool raffleInProgress = false;
    private App app => Application.Current as App;
    private ObservableCollection<Participant> raffleEntries = new ObservableCollection<Participant>();
    private string raffleStateText;

    public ApplicationViewModel()
    {
        InitializeParticipantSource();
        this.WhenPropertyChanged(x => x.RaffleInProgress).Subscribe(delegate { InitializeParticipantSource(); });
    }

    public void NoParticipantsNotification()
    {
        this.NotificationManager
            .CreateMessage()
            .Accent("#1751C3")
            .Background("#333")
            .HasBadge("Warn")
            .HasMessage("No Participants Entered!")
            .Dismiss().WithDelay(TimeSpan.FromSeconds(5))
            .Queue();
    }

    private void InitializeParticipantSource()
    {
        ParticipantSource = new FlatTreeDataGridSource<Participant>(AllParticipants)
        {
            Columns =
            {
                new TextColumn<Participant, string>
                    ("Name", x => x.Name),
                new TextColumn<Participant, int>
                    ("Consecutive Lost", x => x.ConsecutiveLost),
                new CheckBoxColumn<Participant>("Remove", participant => true, (participant, b) => RaffleData.TryRemoveParticipant(participant))
            }
        };
    }

    public void BeginRaffle()
    {
        if (raffleInProgress) return;
        Console.WriteLine("Beginning Raffle...");

        if (CurrentParticipants.Count == 0)
        {
            Console.WriteLine("No Participants Entered!");
            Dispatcher.UIThread.Post(() => NoParticipantsNotification());
            return;
        }

        if (CurrentParticipants.Count == 1)
        {
            CurrentParticipants[0].OnWon();
            return;
        }

        RaffleInProgress = true;
        _ = DoRaffle();
    }

    private async Task DoRaffle()
    {
        Console.WriteLine("Doing the raffle procedure.");
        RaffleStateText = "Beginning Raffle!";
        app.RaffleView.CurrentParticipantList.ItemsSource = CurrentParticipants;
        RaffleEntries.Clear();
        Random random = new Random(DateTime.Now.Ticks.GetHashCode() + DateTime.Now.Nanosecond);
        
        await Task.Delay(1000);
        RaffleStateText = "Shuffling Participants!";

        for (int i = 0; i < CurrentParticipants.Count; i++)
        {
            await Task.Delay(500 / (i + 1));
            CurrentParticipants.Shuffle(random);
        }

        await Task.Delay(1000);
        RaffleStateText = "Dispensing Tickets!";

        for (int i = CurrentParticipants.Count - 1; i >= 0; i--)
        {
            Participant participant = CurrentParticipants[i];
            int entryCount = 1 + participant.ConsecutiveLost;

            for (int j = 0; j < entryCount; j++)
            {
                await Task.Delay(500 / (j + 1));
                RaffleEntries.Add(participant);
            }

            CurrentParticipants.RemoveAt(i);
            await Task.Delay(500 / (i + 1));
        }

        await Task.Delay(1000);
        RaffleStateText = "Shuffling Tickets!";
        app.RaffleView.CurrentParticipantList.ItemsSource = RaffleEntries;

        for (int i = 0; i < RaffleEntries.Count; i++)
        {
            await Task.Delay(500 / (i + 1));
            RaffleEntries.Shuffle(random);
        }

        await Task.Delay(1000);
        RaffleStateText = "Eliminating Tickets!";
        
        while (RaffleEntries.Count > 1)
        {
            await Task.Delay(1500 / RaffleEntries.Count);
            int randomIndex = (int)(random.NextSingle() * RaffleEntries.Count);
            Participant removed = RaffleEntries[randomIndex];
            RaffleEntries.RemoveAt(randomIndex);

            if (!RaffleEntries.Contains(removed))
            {
                removed.OnLost();
            }
        }

        string winnerName = raffleEntries[0].Name;
        await Task.Delay(1000);
        RaffleEntries[0].OnWon();
        RaffleEntries.Clear();
        RaffleData.Save();
        RaffleStateText = $"{winnerName} is the winner!!!";
        await Task.Delay(2000);

        RaffleInProgress = false;
    }
}