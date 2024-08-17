using Avalonia.Interactivity;

namespace NonogramAvalonia.Views;
public partial class BoardEditorView : BoardView
{
    public BoardEditorView ()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        Focus();
    }
}