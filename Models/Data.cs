using System;
using System.Collections.ObjectModel;
using System.Linq;
using DynamicData.Kernel;

namespace RaffleApp.Models;

public static class Data
{
    public static ObservableCollection<Participant> CurrentParticipants { get; } =
        new ObservableCollection<Participant>();

    public static ObservableCollection<Participant> AllParticipants { get; } =
        new ObservableCollection<Participant>();

    public static void TryAddParticipant(string name)
    {
        if (CurrentParticipants.Any(participant => participant.Name == name))
        {
            Console.WriteLine("Tried to add current participant who is already a current participant. Doing nothing.");
        }
        else
        {
            Optional<Participant> existing = AllParticipants.FirstOrOptional(participant => participant.Name == name);
            
            if (existing.HasValue)
            { 
                Console.WriteLine("Adding past participant to current participants.");
                CurrentParticipants.Add(existing.Value); 
            }
            else
            {
                Console.WriteLine("Adding new participant to pool.");
                Participant newParticipant = new Participant(name);
                AllParticipants.Add(newParticipant);
                CurrentParticipants.Add(newParticipant);
            }
        }
    }
}

public class Participant(string name)
{
    public string Name { get; private set; } = name;
    public int ConsecutiveLost { get; private set; } = 0;

    public void OnLost()
    {
        ConsecutiveLost++;
    }
    
    public void OnWon()
    {
        ConsecutiveLost = 0;
    }
}
