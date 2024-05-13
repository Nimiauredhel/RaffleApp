using System.Threading.Tasks;
using Avalonia;
using Avalonia.Data;
using Avalonia.Media;
using AvaloniaDialogs.Views;

namespace RaffleApp.ViewModels;

public static class Dialogs
{
    public static async Task<Optional<bool>> ConfirmationDialog(string message, string positive, string negative,
        IBrush bgBrush, IBrush? borderBrush, Thickness borderThickness)
    {
        TwofoldDialog dialog = new()
        {
            Message = message,
            PositiveText = positive,
            NegativeText = negative,
            Background = bgBrush,
            BorderBrush = borderBrush,
            BorderThickness = borderThickness,
        };
        
        return await dialog.ShowAsync();
    }
}