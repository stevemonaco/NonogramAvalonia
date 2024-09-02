using NonogramAvalonia.SerializationModels;
using NonogramAvalonia.ViewModels;
using System.Linq;

namespace NonogramAvalonia.Mappers;
public static class NonogramViewModelMapper
{
    public static NonogramModel ToModel(this NonogramViewModel viewModel)
    {
        return new NonogramModel(viewModel.Name ?? "", 
            viewModel.RowCount, 
            viewModel.ColumnCount, 
            viewModel.SolutionRowConstraints.ToList(), 
            viewModel.SolutionColumnConstraints.ToList());
    }

    public static NonogramViewModel ToViewModel(this NonogramModel model)
    {
        return new NonogramViewModel(model.RowConstraints, model.ColumnConstraints)
        {
            Name = model.Name
        };
    }
}
