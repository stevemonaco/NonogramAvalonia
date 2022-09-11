using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NonogramAvalonia.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonogramAvalonia.ViewModels;
internal partial class ShellViewModel : ViewModelBase
{
	[ObservableProperty] private BoardViewModel _activeBoard;

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
		}
	}

	[RelayCommand]
	public async Task OpenBoard()
	{
		
	}
}
