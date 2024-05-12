using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using RaffleApp.ViewModels.Twitch;

namespace RaffleApp.Views;

public partial class TwitchView : UserControl
{
    private BlurEffect credsBlur = new BlurEffect();
    
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
        credsBlur.Radius = 30.0;
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
        CredentialsPanel.IsEnabled = value;
        CredentialsPanel.Effect = value ? null : credsBlur ;
        IsPaneOpen = value;
    }
}