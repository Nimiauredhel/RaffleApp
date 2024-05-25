using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;
using DynamicData.Kernel;
using RaffleApp.ViewModels;
using SQLite;

namespace RaffleApp.Models;

public static class RaffleData
{
    public static ObservableCollection<Participant> CurrentParticipants { get; } =
        new ObservableCollection<Participant>();

    public static ObservableCollection<Participant> AllParticipants { get; private set; } =
        new ObservableCollection<Participant>();

    private static readonly SQLiteConnection raffleDb =
        new SQLiteConnection(Path.Combine(AppContext.BaseDirectory, "raffle.sqlite"));

    public static void Initialize()
    {
        raffleDb.CreateTable<Participant>();
        AllParticipants = new ObservableCollection<Participant>(raffleDb.Table<Participant>().ToList());
    }

    public static void Save()
    {
        raffleDb.UpdateAll(AllParticipants);
    }

    public static void TryAddParticipant(string name)
    {
        if (CurrentParticipants.Any(participant => participant.Name == name))
        {
            Console.WriteLine("Tried to add participant who is already a current participant. Doing nothing.");
        }
        else
        {
            Optional<Participant> existing = AllParticipants.FirstOrOptional(participant => participant.Name == name);

            if (existing.HasValue)
            {
                Console.WriteLine("Adding past participant to current participants.");
                CurrentParticipants.Add(existing.Value);
                raffleDb.InsertOrReplace(existing.Value);
            }
            else
            {
                Console.WriteLine("Adding new participant to pool.");
                Participant newParticipant = new Participant(name);
                AllParticipants.Add(newParticipant);
                raffleDb.InsertOrReplace(newParticipant);
                CurrentParticipants.Add(newParticipant);
            }
        }
    }

    public static async Task TryRemoveParticipant(Participant toRemove)
    {
        Avalonia.Data.Optional<bool> confirmed = await Dialogs.ConfirmationDialog("Are you sure?\nThis will delete the participant from the database and cannot be undone.", "Yes.", "No.", 
            Brushes.Red, Brushes.Yellow, new Thickness(5), Brushes.Black, 24);
        while (!confirmed.HasValue) await Task.Delay(10);
        
        if (confirmed.Value == false)
        {
           Console.WriteLine("Cancelled removing participant.");
           return;
        }
        
        if (CurrentParticipants.Contains(toRemove)) CurrentParticipants.Remove(toRemove);
        if (AllParticipants.Contains(toRemove)) AllParticipants.Remove(toRemove);
        
        try
        {
            TableQuery<Participant> query = raffleDb.Table<Participant>();
            query.Delete(participant => participant.Name == toRemove.Name);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public static void OnExit()
    {
        raffleDb.Close();
    }
}

public class Participant
{
    [Unique, PrimaryKey] public string Name { get; set; }
    public int ConsecutiveLost { get; set; }

    public bool Participating => RaffleData.CurrentParticipants.Contains(this);

    public Participant(string name)
    {
        Name = name;
        ConsecutiveLost = 0;
    }

    public Participant()
    {
        Name = string.Empty;
        ConsecutiveLost = 0;
    }

    public void OnLost()
    {
        ConsecutiveLost++;
    }

    public void OnWon()
    {
        ConsecutiveLost = 0;
    }
}