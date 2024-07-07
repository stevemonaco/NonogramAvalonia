using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using CommunityToolkit.Mvvm.ComponentModel;
using NonogramAvalonia.ViewModels;
using NonogramAvalonia.Views;

namespace NonogramAvalonia;
public class ViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null)
            return new TextBlock { Text = "Null view" };

        if (data is BoardViewModel { Mode: BoardMode.Play })
            return new BoardPlayView();
        else if (data is BoardViewModel { Mode: BoardMode.Editor })
            return new BoardEditorView();

        var name = data.GetType().FullName!.Replace("ViewModel", "View");
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }
        else
        {
            return new TextBlock { Text = "Not Found: " + name };
        }
    }

    public bool Match(object? data)
    {
        return data is ObservableObject;
    }
}
