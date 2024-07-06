using System.Text.Json;
using NonogramAvalonia.SerializationModels.Json;
using Remora.Results;
using Nonogram.Domain;

namespace NonogramAvalonia.Services;
public class BoardService
{
    public Result<NonogramBoard> LoadBoardFromJson(string json)
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
}
