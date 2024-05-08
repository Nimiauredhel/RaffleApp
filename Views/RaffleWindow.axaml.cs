using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RaffleApp.Models;
using RaffleApp.ViewModels;

namespace RaffleApp.Views;

public partial class RaffleWindow : Window
{
    public RaffleWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
    }
}