using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using NonogramAvalonia.Controls;
using System;
using System.Linq;

namespace NonogramAvalonia.Views;
public partial class BoardPlayView : BoardView, 
    IRecipient<GameStartedMessage>, IRecipient<GameWinMessage>, IRecipient<GameQuitMessage>
{
    private readonly DispatcherTimer _timer;
    private DateTime _timeStarted;

    public BoardPlayView()
    {
        InitializeComponent();

        _timer = new(TimeSpan.FromSeconds(1), DispatcherPriority.Normal, Timer_Tick);
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        Focus();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        if (e.Key == Key.S)
        {
            ViewModel.SolveBoard();
        }
        else if (e.Key == Key.T)
        {
            var index = (AvailableThemes.IndexOf(BoardTheme) + 1) % AvailableThemes.Count;
            BoardTheme = AvailableThemes[index];
        }
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        ViewModel.TimeElapsed = DateTime.Now - _timeStarted;
    }

    public void Receive(GameStartedMessage message)
    {
        _timeStarted = DateTime.Now;
        _timer.Start();
    }

    public void Receive(GameWinMessage message)
    {
        _timer.Stop();
        ViewModel.TimeElapsed = DateTime.Now - _timeStarted;

        foreach (var cellControl in board.GetLogicalChildren().Select(x => x.LogicalChildren.OfType<Cell>().First()))
        {
            cellControl.Classes.Add("win");
        }
    }

    public void Receive(GameQuitMessage message)
    {
        _timer.Stop();
        WeakReferenceMessenger.Default.UnregisterAll(this);
    }
}