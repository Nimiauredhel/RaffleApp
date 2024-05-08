using System.Net.Mime;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using RaffleApp.ViewModels;
using RaffleApp.Views;

namespace RaffleApp;

public partial class App : Application
{
    public RaffleView RaffleView => raffleView;

    private MainViewModel mainViewModel = new MainViewModel();
    private MainView mainView;
    private RaffleView? raffleView;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    { 
        InitializeAllViews();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            desktopLifetime.MainWindow = new MainWindow();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView) 
        {
            singleView.MainView = new MainSingleView();
        }
        
        base.OnFrameworkInitializationCompleted();
        ShowMainView();
    }

    public async Task DoRaffle()
    {
        ShowRaffleView();
        await mainViewModel.DoRaffle();
        ShowMainView();
    }

    private void ShowMainView()
    {
        if (raffleView != null)
        {
            raffleView.IsVisible = false;
        }

        mainView.IsVisible = true;
        mainView.Focus();
    }

    private void ShowRaffleView()
    {
        if (raffleView == null) return;
        mainView.IsVisible = false;
        raffleView.IsVisible = true;
        raffleView.Focus();
    }

    private void InitializeAllViews()
    {
        mainView = new MainView()
        {
            DataContext = mainViewModel
        };
        
        raffleView = new RaffleView(mainViewModel)
        {
            DataContext = mainViewModel
        };
    }
}