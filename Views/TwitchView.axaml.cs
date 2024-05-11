using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using RaffleApp.Models.Twitch;
using RaffleApp.ViewModels.Twitch;

namespace RaffleApp.Views;

public partial class TwitchView : UserControl
{
    private bool IsPaneOpen
    {
        get => CredentialsPanel.IsEnabled;
        set => CredentialsPanel.IsEnabled = value;
    }

    private bool IsBotSwitchedOn
    {
        get => BotSwitch.IsChecked != null && BotSwitch.IsChecked.Value;
    }

    public TwitchView()
    {
        InitializeComponent();
    }

    public void SetBotIsOn(object? sender, RoutedEventArgs e)
    {
        if (IsBotSwitchedOn)
        {
            if (IsPaneOpen && TwitchManager.CurrentTwitchSettings.ValidateNoneMissing())
            {
                TwitchManager.SaveTwitchSettings();
            }
            else
            {
                SetPaneOpen(true);
                return;
            }
        }

        SetPaneOpen(!IsBotSwitchedOn);
        TwitchManager.SetBotSwitchedOn(IsBotSwitchedOn);
    }

    private void SetPaneOpen(bool value)
    {
        CredentialsPanel.IsVisible = value;
        IsPaneOpen = value;
    }
}