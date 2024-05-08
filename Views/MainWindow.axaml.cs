using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using RaffleApp.Models;

namespace RaffleApp.Views;
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void RaffleButton_OnClick(object? sender, RoutedEventArgs e)
    {
        _ = (Application.Current as App)?.DoRaffle();
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