using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Rendering;
using Avalonia.Threading;
using System;
using zombieturkey.ViewModels;

namespace zombieturkey.Views;

public class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent(); 
        var rt = AvaloniaLocator.CurrentMutable.GetService<IRenderTimer>();
        rt.Tick += Rt_Tick;
    }
      

    private void Rt_Tick(TimeSpan obj)
    {
        Dispatcher.UIThread.Post(() =>
        { if (DataContext is MainViewModel vm)
                {
                    vm.Tick();
                } 
            }
            , DispatcherPriority.Background);

    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    void MouseMoved(object sender, PointerEventArgs args)
    {
        if (DataContext is MainViewModel vm)
        {
            vm.SetPointer(args.GetPosition(this.FindControl<ItemsControl>("canvas")));
        }
    }
}