using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using RaffleApp.Models;

namespace RaffleApp.Views;

public partial class TwitchView : UserControl
{
    public TwitchView()
    {
        InitializeComponent();
    }

    public void SetBotIsOn(object? sender, RoutedEventArgs e)
    {
        TwitchManager.IsBotSwitchedOn = BotSwitch.IsChecked.Value;
    }
    
    public void SetUsername(object? sender, RoutedEventArgs e)
    {
        TwitchManager.Username = UsernameBox.Text;
    }
    
    public void SetPassword(object? sender, RoutedEventArgs e)
    {
        TwitchManager.Token = PasswordBox.Text;
    }
    
    public void SetKeyword(object? sender, RoutedEventArgs e)
    {
        TwitchManager.Keyword = KeywordBox.Text;
    }

    private void jk(object? sender, TextChangedEventArgs e)
    {
        throw new System.NotImplementedException();
    }
}