using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Nonogram.Domain;
using NonogramAvalonia.Services;
using System.IO;
using System.Threading.Tasks;

namespace NonogramAvalonia.ViewModels;
public partial class CreateBoardViewModel : BoardViewModel
{
    private readonly BoardService _boardService;
    private readonly IFileSelectService _fileSelectService;

    public CreateBoardViewModel(NonogramBoard board, BoardService boardService, IFileSelectService fileSelectService) : base(board)
    {
        _boardService = boardService;
        _fileSelectService = fileSelectService;
    }

    [RelayCommand]
    public async Task SaveBoard()
    {
        var fileName = await _fileSelectService.RequestSaveBoardFileNameAsync();

        if (fileName is null)
            return;

        Board.Name = Path.GetFileNameWithoutExtension(fileName);
        Board.BuildConstraints();
        var json = _boardService.SerializeBoard(Board);
        await File.WriteAllTextAsync(fileName, json);
    }
}
