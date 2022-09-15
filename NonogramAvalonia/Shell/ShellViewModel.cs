using System;
using System.IO;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NonogramAvalonia.Messages;
using NonogramAvalonia.Services;

namespace NonogramAvalonia.ViewModels;
internal partial class ShellViewModel : ViewModelBase
{
	[ObservableProperty] private BoardViewModel _activeBoard;
	[ObservableProperty] private TimeSpan? _timeElapsed;

	private readonly BoardService _boardService;
	private readonly IFileSelectService _fileSelectService;

	public ShellViewModel(BoardService boardService, IFileSelectService fileSelectService)
	{
		_boardService = boardService;
		_fileSelectService = fileSelectService;

		string json = File.ReadAllText(@"_boards\PicrossDS 1-B.json");
		var result = _boardService.LoadBoardFromJson(json);

		if (result.IsSuccess)
		{
			_activeBoard = result.Entity;
			Messenger.Send(new GameStartedMessage());
		}
	}

	[RelayCommand]
	public async Task OpenBoard()
	{
		var dialogResult = await _fileSelectService.GetBoardFileNameByUserAsync();

		if (dialogResult is Uri filePath)
		{
			string json = await File.ReadAllTextAsync(filePath.LocalPath);
			var boardResult = _boardService.LoadBoardFromJson(json);

			if (boardResult.IsSuccess)
			{
				ActiveBoard = boardResult.Entity;
				_timeElapsed = TimeSpan.Zero;
				Messenger.Send(new GameStartedMessage());
			}
		}
	}

	public bool RequestApplicationExit()
	{
		return true;
	}
}
