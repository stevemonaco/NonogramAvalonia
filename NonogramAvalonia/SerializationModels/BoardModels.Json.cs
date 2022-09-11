using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonogramAvalonia.SerializationModels.Json;

public record NonogramModel(string Name, int Rows, int Columns, List<List<int>> RowConstraints, List<List<int>> ColumnConstraints);