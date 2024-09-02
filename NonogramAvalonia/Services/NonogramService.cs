using System.Text.Json;
using Nonogram.Domain;
using NonogramAvalonia.SerializationModels;
using Remora.Results;

namespace NonogramAvalonia.Services;
public class NonogramService
{
    public Result<NonogramModel> DeserializeNonogram(string json)
    {
        var jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        var model = JsonSerializer.Deserialize<NonogramModel>(json, jsonOptions);

        if (model is null)
        {
            return Result<NonogramModel>.FromError(new ArgumentInvalidError("JSON parsing error", "Could not parse the JSON content for an undetermined reason"));
        }

        if (model.RowCount != model.RowConstraints.Count)
        {
            return Result<NonogramModel>.FromError(new ArgumentInvalidError("Mismatched row dimensions", $"The 'rows' ({model.RowCount}) did not match the number of constraints ({model.RowConstraints.Count})"));
        }

        if (model.ColumnCount != model.ColumnConstraints.Count)
        {
            return Result<NonogramModel>.FromError(new ArgumentInvalidError("Mismatched column dimensions", $"The 'columns' ({model.ColumnCount}) did not match the number of constraints ({model.ColumnConstraints.Count})"));
        }

        return Result<NonogramModel>.FromSuccess(model);
    }

    public string SerializeNonogram(NonogramModel model)
    {
        var jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        return JsonSerializer.Serialize(model, jsonOptions);
    }

    public NonogramModel CreateRandomNonogram(int rows, int columns)
    {
        var generator = new NonogramGenerator(rows, columns);
        var puzzle = generator.CreateRandom();

        return new NonogramModel("", rows, columns, puzzle.RowConstraints, puzzle.ColumnConstraints);
    }
}
