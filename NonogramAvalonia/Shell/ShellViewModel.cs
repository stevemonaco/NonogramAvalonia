using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Nonogram.Domain;
using NonogramAvalonia.Services;

namespace NonogramAvalonia.ViewModels;
public partial class ShellViewModel : ViewModelBase, IRecipient<NavigateToPlayMessage>, IRecipient<NavigateToMenuMessage>
{
	[ObservableProperty] private ObservableObject? _activeScreen;

	private readonly MenuViewModel _selectBoardViewModel;

	private readonly BoardService _boardService;
	private readonly IFileSelectService _fileSelectService;

	public ShellViewModel(BoardService boardService, IFileSelectService fileSelectService)
	{
		_boardService = boardService;
		_fileSelectService = fileSelectService;

		_selectBoardViewModel = new(boardService);

        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public async Task InitializeAsync()
	{
		await _selectBoardViewModel.InitializeAsync();
		ActiveScreen = _selectBoardViewModel;
	}


    [RelayCommand]
	public async Task RequestBoard()
	{
		var dialogResult = await _fileSelectService.RequestBoardFileNameAsync();

		if (dialogResult is not string path)
			return;

		await LoadBoardAsync(path);
	}

	public async Task LoadBoardAsync(string path)
	{
        string json = await File.ReadAllTextAsync(path);
        var boardResult = _boardService.LoadBoardFromJson(json);

        if (boardResult.IsSuccess)
        {
			PlayBoard(boardResult.Entity);
            ActiveScreen = new PlayBoardViewModel(boardResult.Entity);
            Messenger.Send(new GameStartedMessage());
        }
    }

    public void PlayBoard(NonogramBoard board)
    {
        ActiveScreen = new PlayBoardViewModel(board);
        Messenger.Send(new GameStartedMessage());
    }

    public bool RequestApplicationExit()
	{
		return true;
	}

    public void Receive(NavigateToPlayMessage message)
    {
		message.Board.ResetCellStates();
		PlayBoard(message.Board);
    }

    public void Receive(NavigateToMenuMessage message)
    {
        ActiveScreen = _selectBoardViewModel;
    }
}
