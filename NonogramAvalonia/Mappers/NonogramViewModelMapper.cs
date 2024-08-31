using NonogramAvalonia.SerializationModels;
using NonogramAvalonia.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace NonogramAvalonia.Mappers;
public static class NonogramViewModelMapper
{
    public static NonogramModel ToModel(this NonogramViewModel viewModel)
    {
        var rowConstraints = viewModel.SolutionRowConstraints.Select(x => new List<int>(x)).ToList();
        var columnConstraints = viewModel.SolutionColumnConstraints.Select(x => new List<int>(x)).ToList();

        return new NonogramModel(viewModel.Name ?? "", viewModel.RowCount, viewModel.ColumnCount, rowConstraints, columnConstraints);
    }

    public static NonogramViewModel ToViewModel(this NonogramModel model)
    {
        return new NonogramViewModel(model.RowConstraints, model.ColumnConstraints)
        {
            Name = model.Name
        };
    }
}
