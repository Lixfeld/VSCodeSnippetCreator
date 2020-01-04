using System.Windows;
using MahApps.Metro.Controls;
using ReactiveUI;
using VSCodeSnippetCreator.Core.ViewModels;

namespace VSCodeSnippetCreator
{
    public abstract class MainWindowBase : MetroWindow, IViewFor<MainWindowModel>
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(MainWindowModel), typeof(MainWindow));

        public MainWindowModel ViewModel
        {
            get => (MainWindowModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (MainWindowModel)value;
        }
    }
}
