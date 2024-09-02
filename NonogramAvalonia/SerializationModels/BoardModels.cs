using Nonogram.Domain;
using System.Collections.Generic;
namespace NonogramAvalonia.SerializationModels;

public record NonogramModel(string Name, int RowCount, int ColumnCount, IList<LineConstraints> RowConstraints, IList<LineConstraints> ColumnConstraints);