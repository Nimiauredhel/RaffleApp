using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using DynamicData.Kernel;
using SQLite;

namespace RaffleApp.Models;

public static class RaffleData
{
    public static ObservableCollection<Participant> CurrentParticipants { get; } =
        new ObservableCollection<Participant>();
    public static ObservableCollection<Participant> AllParticipants { get; private set; } =
        new ObservableCollection<Participant>();
    private static readonly SQLiteConnection db = new SQLiteConnection(Path.Combine(AppContext.BaseDirectory, "raffle.sqlite"));

    public static void Initialize()
    {
        db.CreateTable<Participant>();
        AllParticipants = new ObservableCollection<Participant>(db.Table<Participant>().ToList());
    }

    public static void Save()
    {
        db.UpdateAll(AllParticipants);
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
                db.InsertOrReplace(existing.Value);
            }
            else
            {
                Console.WriteLine("Adding new participant to pool.");
                Participant newParticipant = new Participant(name);
                AllParticipants.Add(newParticipant);
                db.InsertOrReplace(newParticipant);
                CurrentParticipants.Add(newParticipant);
            }
        }
    }
}

public class Participant
{
    [Unique, PrimaryKey]
    public string Name { get; set; }
    public int ConsecutiveLost { get; set; }

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
