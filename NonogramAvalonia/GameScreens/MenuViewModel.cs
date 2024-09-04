using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using NonogramAvalonia.Factory;
using NonogramAvalonia.Mappers;
using NonogramAvalonia.SerializationModels;
using NonogramAvalonia.Services;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NonogramAvalonia.ViewModels;
public partial class MenuViewModel : ObservableRecipient
{
    [ObservableProperty] ObservableCollection<NonogramModel> _availableBoards = [];

    private readonly NonogramService _nonogramService;
    private readonly BoardViewModelFactory _boardViewModelFactory;
    private string _boardLocation = @"_boards";

    public MenuViewModel(NonogramService nonogramService, BoardViewModelFactory boardViewModelFactory)
    {
        _nonogramService = nonogramService;
        _boardViewModelFactory = boardViewModelFactory;
    }

    public async Task InitializeAsync()
    {
        foreach (var file in Directory.GetFiles(_boardLocation).Where(x => x.EndsWith(".json")))
        {
            var content = await File.ReadAllTextAsync(file);
            var result = _nonogramService.DeserializeNonogram(content);

            if (result.IsSuccess)
            {
                AvailableBoards.Add(result.Entity);
            }
        }
    }

    [RelayCommand]
    public void Play(NonogramModel model)
    {
        var vm = model.ToViewModel();
        WeakReferenceMessenger.Default.Send(new NavigateToPlayMessage(vm));
    }

    [RelayCommand]
    public void CreateBoard()
    {
        WeakReferenceMessenger.Default.Send(new NavigateToCreateMessage(10, 10));
    }

    public void RandomPlay(int rows, int columns)
    {
        var model = _nonogramService.CreateRandomNonogram(rows, columns);
        var vm = model.ToViewModel();
        WeakReferenceMessenger.Default.Send(new NavigateToPlayMessage(vm));
    }
}
