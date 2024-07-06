using System.Text.Json;
using NonogramAvalonia.SerializationModels.Json;
using Remora.Results;
using Nonogram.Domain;
using System.Linq;

namespace NonogramAvalonia.Services;
public class BoardService
{
    public Result<NonogramBoard> DeserializeBoard(string json)
    {
        var jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        var model = JsonSerializer.Deserialize<NonogramModel>(json, jsonOptions);

        if (model is null)
        {
            return Result<NonogramBoard>.FromError(new ArgumentInvalidError("JSON parsing error", "Could not parse the JSON content for an undetermined reason"));
        }

        if (model.Rows != model.RowConstraints.Count)
        {
            return Result<NonogramBoard>.FromError(new ArgumentInvalidError("Mismatched row dimensions", $"The 'rows' ({model.Rows}) did not match the number of constraints ({model.RowConstraints.Count})"));
        }

        if (model.Columns != model.ColumnConstraints.Count)
        {
            return Result<NonogramBoard>.FromError(new ArgumentInvalidError("Mismatched column dimensions", $"The 'columns' ({model.Columns}) did not match the number of constraints ({model.ColumnConstraints.Count})"));
        }

        var board = new NonogramBoard(model.RowConstraints, model.ColumnConstraints)
        {
            Name = model.Name
        };

        return Result<NonogramBoard>.FromSuccess(board);
    }

    public string SerializeBoard(NonogramBoard board)
    {
        var jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        var rowConstraints = board.PlayerRowConstraints.Select(x => x.Items).ToList();
        var columnConstraints = board.PlayerColumnConstraints.Select(x => x.Items).ToList();

        var model = new NonogramModel(board.Name ?? "", board.Rows, board.Columns, rowConstraints, columnConstraints);

        var json = JsonSerializer.Serialize(model, jsonOptions);

        return json;
    }
}
