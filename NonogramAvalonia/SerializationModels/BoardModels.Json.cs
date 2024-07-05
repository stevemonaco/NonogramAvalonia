using System.Collections.Generic;
namespace NonogramAvalonia.SerializationModels.Json;

public record NonogramModel(string Name, int Rows, int Columns, List<List<int>> RowConstraints, List<List<int>> ColumnConstraints);