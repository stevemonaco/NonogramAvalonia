using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NonogramAvalonia.Factory;
using NonogramAvalonia.Mappers;
using NonogramAvalonia.Services;

namespace NonogramAvalonia.ViewModels;
public sealed partial class ShellViewModel : ObservableRecipient,
	IRecipient<NavigateToPlayMessage>, IRecipient<NavigateToMenuMessage>, IRecipient<NavigateToEditorMessage>
{
	[ObservableProperty] private ObservableObject? _activeScreen;

	private readonly MenuViewModel _menuViewModel;
    private readonly NonogramService _nonogramService;
    private readonly IFileSelectService _fileSelectService;
    private readonly BoardViewModelFactory _boardViewModelFactory;

    public ShellViewModel(MenuViewModel menuViewModel, NonogramService nonogramService, IFileSelectService fileSelectService, BoardViewModelFactory boardViewModelFactory)
	{
		_menuViewModel = menuViewModel;
        _nonogramService = nonogramService;
        _fileSelectService = fileSelectService;
        _boardViewModelFactory = boardViewModelFactory;

        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public async Task InitializeAsync()
	{
		await _menuViewModel.InitializeAsync();
		ActiveScreen = _menuViewModel;
	}

    [RelayCommand]
	public async Task RequestBoard()
	{
		var dialogResult = await _fileSelectService.RequestOpenBoardFileNameAsync();

		if (dialogResult is not string path)
			return;

		await LoadBoardAsync(path);
	}

	public async Task LoadBoardAsync(string path)
	{
        string json = await File.ReadAllTextAsync(path);
        var result = _nonogramService.DeserializeNonogram(json);

        if (result.IsSuccess)
        {
			PlayBoard(result.Entity.ToViewModel());
        }
    }

    public void PlayBoard(NonogramViewModel board)
    {
        ActiveScreen = _boardViewModelFactory.CreatePlay(board);
        Messenger.Send(new GameStartedMessage());
    }

    public bool RequestApplicationExit()
	{
		return true;
	}

    public void Receive(NavigateToPlayMessage message)
    {
		PlayBoard(message.Board);
    }

    public void Receive(NavigateToMenuMessage message)
    {
        ActiveScreen = _menuViewModel;
    }

    public void Receive(NavigateToEditorMessage message)
    {
        var board = new NonogramViewModel(message.Rows, message.Columns);
        ActiveScreen = _boardViewModelFactory.CreateEditor(board);
    }
}
