using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Data;
using Avalonia.Media;
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
        this.WhenValueChanged(x => x.RaffleInProgress).Subscribe(delegate { InitializeParticipantSource(); });
    }

    private void InitializeParticipantSource()
    {
        ParticipantSource = new FlatTreeDataGridSource<Participant>(AllParticipants)
        {
            Columns =
            {
                new CheckBoxColumn<Participant>("DEL", participant => false,
                    (participant, b) =>
                    {
                        if (b)
                        {
                            _ = RaffleData.TryRemoveParticipant(participant);
                        }
                    }),
                new TextColumn<Participant, string>
                    ("Name", x => x.Name),
                new TextColumn<Participant, int>
                    ("Consecutive Lost", x => x.ConsecutiveLost),
                new CheckBoxColumn<Participant>("In Current Raffle?", participant => participant.Participating,
                    (participant, b) =>
                    {
                        if (b)
                        {
                            if (!participant.Participating)
                            {
                                CurrentParticipants.Add(participant);
                            }
                        }
                        else
                        {
                            if (participant.Participating)
                            {
                                CurrentParticipants.Remove(participant);
                            }
                        }
                    }),
            }
        };
    }

    public void BeginRaffle()
    {
        if (raffleInProgress) return;
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
            "Are you sure? This will decide the winner and cannot be undone.", "Let's go!", "Not yet.", Brushes.Red,
            Brushes.Yellow, new Thickness(5));
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