using NonogramAvalonia.ViewModels;
using NonogramAvalonia.SerializationModels.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Remora.Results;
using Nonogram.Domain;

namespace NonogramAvalonia.Services;
internal class BoardService
{
    public Result<BoardViewModel> LoadBoardFromJson(string json)
    {
        var jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        var model = JsonSerializer.Deserialize<NonogramModel>(json, jsonOptions);

        if (model is null)
        {
            return Result<BoardViewModel>.FromError(new ArgumentInvalidError("JSON parsing error", "Could not parse the JSON content for an undetermined reason"));
        }

        if (model.Rows != model.RowConstraints.Count)
        {
            return Result<BoardViewModel>.FromError(new ArgumentInvalidError("Mismatched row dimensions", $"The 'rows' ({model.Rows}) did not match the number of constraints ({model.RowConstraints.Count})"));
        }

        if (model.Columns != model.ColumnConstraints.Count)
        {
            return Result<BoardViewModel>.FromError(new ArgumentInvalidError("Mismatched column dimensions", $"The 'columns' ({model.Columns}) did not match the number of constraints ({model.ColumnConstraints.Count})"));
        }

        var board = new NonogramBoard(model.RowConstraints, model.ColumnConstraints);
        var vm = new BoardViewModel(board)
        {
            PuzzleName = model.Name
        };

        return Result<BoardViewModel>.FromSuccess(vm);
    }
}
