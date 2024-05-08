using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using RaffleApp.ViewModels;
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
    private MainViewModel mainViewModel = new MainViewModel();
    

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
            DataContext = mainViewModel, SignupView = signupView, RaffleView = raffleView
        };

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            desktopLifetime.MainWindow = new MainWindow()
            {
                DataContext = mainViewModel, MainView = mainView
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            singleView.MainView = new MainSingleView()
            {
                DataContext = mainViewModel, MainView = mainView
            };
        }
        
        base.OnFrameworkInitializationCompleted();
    }
}