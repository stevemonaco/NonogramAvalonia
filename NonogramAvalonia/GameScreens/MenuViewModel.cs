﻿using Avalonia.Controls.Documents;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NonogramAvalonia.Services;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NonogramAvalonia.ViewModels;
public partial class MenuViewModel : ObservableRecipient
{
    [ObservableProperty] ObservableCollection<BoardViewModel> _availableBoards = [];

    private readonly BoardService _boardService;
    private string _boardLocation = @"_boards";

    public MenuViewModel(BoardService boardService)
    {
        _boardService = boardService;
    }

    public async Task InitializeAsync()
    {
        foreach (var file in Directory.GetFiles(_boardLocation).Where(x => x.EndsWith(".json")))
        {
            var content = await File.ReadAllTextAsync(file);
            var board = _boardService.DeserializeBoard(content);

            if (board.IsSuccess)
            {
                var vm = new BoardViewModel(board.Entity);
                AvailableBoards.Add(vm);
            }
        }
    }

    [RelayCommand]
    public void Play(BoardViewModel board)
    {
        WeakReferenceMessenger.Default.Send(new NavigateToPlayMessage(board.Board));
    }

    [RelayCommand]
    public void CreateBoard()
    {
        WeakReferenceMessenger.Default.Send(new NavigateToCreateMessage());
    }
}
