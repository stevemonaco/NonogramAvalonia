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

    private readonly SerializationService _boardService;
    private readonly BoardViewModelFactory _boardViewModelFactory;
    private string _boardLocation = @"_boards";

    public MenuViewModel(SerializationService boardService, BoardViewModelFactory boardViewModelFactory)
    {
        _boardService = boardService;
        _boardViewModelFactory = boardViewModelFactory;
    }

    public async Task InitializeAsync()
    {
        foreach (var file in Directory.GetFiles(_boardLocation).Where(x => x.EndsWith(".json")))
        {
            var content = await File.ReadAllTextAsync(file);
            var result = _boardService.DeserializeNonogram(content);

            if (result.IsSuccess)
            {
                AvailableBoards.Add(result.Entity);
                //var vm = _boardViewModelFactory.CreateEditor(result.Entity.ToViewModel());
                //AvailableBoards.Add(vm);
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
        WeakReferenceMessenger.Default.Send(new NavigateToCreateMessage(25, 25));
    }
}
