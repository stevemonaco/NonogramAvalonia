using System.Text.Json;
using NonogramAvalonia.SerializationModels.Json;
using Remora.Results;
using NonogramAvalonia.ViewModels;
using System.Linq;
using System.Collections.Generic;

namespace NonogramAvalonia.Services;
public class BoardService
{
    public Result<NonogramViewModel> DeserializeBoard(string json)
    {
        var jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        var model = JsonSerializer.Deserialize<NonogramModel>(json, jsonOptions);

        if (model is null)
        {
            return Result<NonogramViewModel>.FromError(new ArgumentInvalidError("JSON parsing error", "Could not parse the JSON content for an undetermined reason"));
        }

        if (model.Rows != model.RowConstraints.Count)
        {
            return Result<NonogramViewModel>.FromError(new ArgumentInvalidError("Mismatched row dimensions", $"The 'rows' ({model.Rows}) did not match the number of constraints ({model.RowConstraints.Count})"));
        }

        if (model.Columns != model.ColumnConstraints.Count)
        {
            return Result<NonogramViewModel>.FromError(new ArgumentInvalidError("Mismatched column dimensions", $"The 'columns' ({model.Columns}) did not match the number of constraints ({model.ColumnConstraints.Count})"));
        }

        var board = new NonogramViewModel(model.RowConstraints, model.ColumnConstraints)
        {
            Name = model.Name
        };

        return Result<NonogramViewModel>.FromSuccess(board);
    }

    public string SerializeBoard(NonogramViewModel board)
    {
        var jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        var rowConstraints = board.PlayerRowConstraints.Select(x => new List<int>(x)).ToList();
        var columnConstraints = board.PlayerColumnConstraints.Select(x => new List<int>(x)).ToList();

        var model = new NonogramModel(board.Name ?? "", board.RowCount, board.ColumnCount, rowConstraints, columnConstraints);

        var json = JsonSerializer.Serialize(model, jsonOptions);

        return json;
    }
}
