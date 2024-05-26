using Avalonia.Controls;
using Avalonia.Interactivity;
using RaffleApp.Models;

namespace RaffleApp.Views;

public partial class SignupView : UserControl
{
    public SignupView()
    {
        InitializeComponent();
    }

    private void AddButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (AddBox.Text == null || AddBox.Text.Length <= 0) return;
        string[] names = AddBox.Text.Split('\n');

        for (int i = 0; i < names.Length; i++)
        {
            RaffleData.TryAddParticipant(names[i]);
        }
            
        AddBox.Text = string.Empty;
    }
}