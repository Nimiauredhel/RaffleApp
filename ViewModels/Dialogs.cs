using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;
using AvaloniaDialogs.Views;

namespace RaffleApp.ViewModels;

public static class Dialogs
{
    public static async Task<Optional<bool>> ConfirmationDialog(string message, string positive, string negative,
        IBrush bgBrush, IBrush? borderBrush, Thickness borderThickness, IBrush foreground, int fontSize)
    {
        TwofoldDialog dialog = new()
        {
            Message = message,
            PositiveText = positive,
            NegativeText = negative,
            Background = bgBrush,
            BorderBrush = borderBrush,
            BorderThickness = borderThickness,
            Width = 400,
            Height = 200,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalButtonAlignment = HorizontalAlignment.Center,
            FontSize = fontSize, Foreground = foreground
        };
        
        return await dialog.ShowAsync();
    }
    
    public static async Task<Optional<EventArgs>> InfoDialog(string message, string buttonMessage,
        IBrush bgBrush, IBrush? borderBrush, Thickness borderThickness, IBrush foreground, int fontSize)
    {
        SingleActionDialog dialog = new()
        {
            Message = message,
            ButtonText = buttonMessage,
            Background = bgBrush,
            BorderBrush = borderBrush,
            BorderThickness = borderThickness,
            Width = 400,
            Height = 200,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalButtonAlignment = HorizontalAlignment.Center,
            FontSize = fontSize, Foreground = foreground
        };
        
        return await dialog.ShowAsync();
    }
}