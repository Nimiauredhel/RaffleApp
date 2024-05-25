using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using RaffleApp.Models;
using RaffleApp.ViewModels;
using RaffleApp.ViewModels.Twitch;
using RaffleApp.Views;

namespace RaffleApp;

public partial class App : Application
{
    public MainView MainView => mainView;
    public SignupView SignupView => signupView;
    public RaffleView RaffleView => raffleView;
    
    private MainView mainView;
    private SignupView signupView;
    private RaffleView raffleView;
    private RaffleViewModel _raffleViewModel = new RaffleViewModel();

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        signupView = new SignupView();
        raffleView = new RaffleView();
        
        mainView = new MainView()
        {
             DataContext = _raffleViewModel, SignupView = signupView, RaffleView = raffleView
        };

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            desktopLifetime.MainWindow = new MainWindow()
            {
                DataContext = _raffleViewModel, MainView = mainView
            };

            desktopLifetime.Exit += OnExit;
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            singleView.MainView = new MainSingleView()
            {
                DataContext = _raffleViewModel, MainView = mainView
            };
        }
        
        base.OnFrameworkInitializationCompleted();
    }

    private void OnExit(object sender, ControlledApplicationLifetimeExitEventArgs args)
    {
        TwitchViewModel.OnExit();
        RaffleData.OnExit();
    }
}