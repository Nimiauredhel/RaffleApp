using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RaffleApp.Models;
using RaffleApp.ViewModels;

namespace RaffleApp.Views;

public partial class SignupView : UserControl
{
    public SignupView()
    {
        InitializeComponent();
    }

    private void RaffleButton_OnClick(object? sender, RoutedEventArgs e)
    {
        Task.Run((DataContext as MainViewModel).DoRaffle);
    }

    private void AddButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (AddBox.Text != null && AddBox.Text.Length > 0)
        {
            string[] names = AddBox.Text.Split('\n');

            for (int i = 0; i < names.Length; i++)
            {
                Data.TryAddParticipant(names[i]);
            }
        }
    }
}