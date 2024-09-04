using Avalonia.Controls;
using Avalonia.Interactivity;
using NonogramAvalonia.ViewModels;

namespace NonogramAvalonia.Views;
public sealed partial class MenuView : UserControl
{
    public MenuViewModel ViewModel => (MenuViewModel)DataContext!;

    public MenuView()
    {
        InitializeComponent();
    }

    private void Random5by5_Click(object? sender, RoutedEventArgs e) =>
        ViewModel.RandomPlay(5, 5);

    private void Random10by10_Click(object? sender, RoutedEventArgs e) =>
        ViewModel.RandomPlay(10, 10);

    private void Random15by15_Click(object? sender, RoutedEventArgs e) =>
        ViewModel.RandomPlay(15, 15);

    private void Random20by20_Click(object? sender, RoutedEventArgs e) =>
        ViewModel.RandomPlay(20, 20);

    private void Random25by25_Click(object? sender, RoutedEventArgs e) =>
        ViewModel.RandomPlay(25, 25);
}