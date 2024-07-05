using System;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NonogramAvalonia.Services;

namespace NonogramAvalonia.ViewModels;
public partial class ShellViewModel : ViewModelBase
{
	[ObservableProperty] private BoardViewModel? _activeBoard;
	[ObservableProperty] private TimeSpan? _timeElapsed;

	private readonly BoardService _boardService;
	private readonly IFileSelectService _fileSelectService;

	public ShellViewModel(BoardService boardService, IFileSelectService fileSelectService)
	{
		_boardService = boardService;
		_fileSelectService = fileSelectService;
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
            ActiveBoard = boardResult.Entity;
            TimeElapsed = TimeSpan.Zero;
            Messenger.Send(new GameStartedMessage());
        }
    }

	public bool RequestApplicationExit()
	{
		return true;
	}
}
