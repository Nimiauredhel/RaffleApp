﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Notifications;
using DynamicData.Binding;
using RaffleApp.Models;
using ReactiveUI;
using Notification = Avalonia.Controls.Notifications.Notification;

namespace RaffleApp.ViewModels;

public class MainViewModel : ViewModelBase
{
#pragma warning disable CA1822 // Mark members as static
#pragma warning restore CA1822 // Mark members as static

    public ObservableCollection<Participant> AllParticipants => Data.AllParticipants;
    public ObservableCollection<Participant> CurrentParticipants => Data.CurrentParticipants;
    public ObservableCollection<Participant> RaffleEntries => raffleEntries;
    public FlatTreeDataGridSource<Participant> ParticipantSource { get; private set; }

    public bool RaffleInProgress
    {
        get => raffleInProgress;
        set => this.RaiseAndSetIfChanged(ref raffleInProgress, value);
    }

    private bool raffleInProgress = false;

    private ObservableCollection<Participant> raffleEntries = new ObservableCollection<Participant>();

    public MainViewModel()
    {
        this.WhenPropertyChanged(x => x.RaffleInProgress).Subscribe(delegate
        {
            ParticipantSource = new FlatTreeDataGridSource<Participant>(AllParticipants)
            {
                Columns =
                {
                    new TextColumn<Participant, string>
                        ("Name", x => x.Name),
                    new TextColumn<Participant, int>
                        ("Consecutive Lost", x => x.ConsecutiveLost)
                },
            };
        });

        Data.TryAddParticipant("Gary");
        Console.WriteLine("DSFSDF");
    }

    public async Task DoRaffle()
    {
        Console.WriteLine("Doing the raffle procedure (ViewModel).");
        App app = (Application.Current as App);

        app.RaffleView.CurrentParticipantList.ItemsSource = CurrentParticipants;
        RaffleInProgress = true;

        if (app == null || CurrentParticipants.Count == 0)
        {
            Notification not = new Notification("Whoa!", "No participants!", NotificationType.Warning,
                TimeSpan.FromSeconds(3));
            RaffleInProgress = false;
            return;
        }
        else if (CurrentParticipants.Count == 1)
        {
            RaffleEntries[0].OnWon();
            RaffleEntries.Clear();
            await Task.Delay(500);
            RaffleInProgress = false;
            return;
        }

        RaffleEntries.Clear();
        Random random = new Random(DateTime.Now.Ticks.GetHashCode() + DateTime.Now.Nanosecond);

        for (int i = 0; i < 4; i++)
        {
            await Task.Delay(500);
            CurrentParticipants.Shuffle(random);
        }

        await Task.Delay(500);

        for (int i = CurrentParticipants.Count - 1; i >= 0; i--)
        {
            Participant participant = CurrentParticipants[i];
            int entryCount = 1 + participant.ConsecutiveLost;

            for (int j = 0; j < entryCount; j++)
            {
                await Task.Delay(500);
                RaffleEntries.Add(participant);
            }

            CurrentParticipants.RemoveAt(i);
        }

        app.RaffleView.CurrentParticipantList.ItemsSource = RaffleEntries;

        for (int i = 0; i < 4; i++)
        {
            await Task.Delay(500);
            RaffleEntries.Shuffle(random);
        }

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

        await Task.Delay(1000);

        RaffleEntries[0].OnWon();
        RaffleEntries.Clear();
        await Task.Delay(1000);
        RaffleInProgress = false;
    }
}