using Avalonia.Interactivity;

namespace NonogramAvalonia.Views;
public sealed partial class BoardEditorView : BoardView
{
    public BoardEditorView ()
    {
        InitializeComponent();
    }

    private void Resize_Click(object? sender, RoutedEventArgs e)
    {
        if (rowCountControl.Value is null || columnCountControl.Value is null)
            return;

        var rows = (int)rowCountControl.Value;
        var columns = (int)columnCountControl.Value;

        if (rows < 1 || columns < 1)
            return;

        ViewModel.Nonogram.Resize(rows, columns);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        Focus();
    }
}