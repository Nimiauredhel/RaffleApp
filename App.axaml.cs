using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using RaffleApp.ViewModels;
using RaffleApp.Views;

namespace RaffleApp;

public partial class App : Application
{
    public MainWindow MainWindow => mainWindow;
    public RaffleWindow RaffleWindow => raffleWindow;

    private MainWindowViewModel mainViewModel = new MainWindowViewModel();
    private MainWindow mainWindow;
    private RaffleWindow raffleWindow;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    { 
        InitializeAllWindows(); 
        ShowMainWindow();
        base.OnFrameworkInitializationCompleted();
    }

    public async Task DoRaffle()
    {
        ShowRaffleWindow();
        await mainViewModel.DoRaffle();
        ShowMainWindow();
    }
    
    public void ShowMainWindow()
    {
        raffleWindow.Hide();
        mainWindow.Effect = null;
        mainWindow.Focusable = true;
        mainWindow.Show();
        mainWindow.Focus();
    }
    
    public void ShowRaffleWindow()
    {
        mainWindow.Effect = new BlurEffect();
        mainWindow.Focusable = false;
        raffleWindow.Show();
        raffleWindow.Focus();
    }

    private void InitializeAllWindows()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            mainWindow = new MainWindow()
            {
                DataContext = mainViewModel
            };
            
            raffleWindow = new RaffleWindow(mainViewModel)
            {
                DataContext = mainViewModel, Effect = new DropShadowEffect()
            };
        }
        
    }
}