using System.Collections.Generic;
namespace NonogramAvalonia.SerializationModels;

public record NonogramModel(string Name, int RowCount, int ColumnCount, List<List<int>> RowConstraints, List<List<int>> ColumnConstraints);