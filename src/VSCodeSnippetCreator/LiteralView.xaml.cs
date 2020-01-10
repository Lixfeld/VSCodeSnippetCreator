using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using ReactiveUI;
using VSCodeSnippetCreator.Core;
using VSCodeSnippetCreator.Core.Helpers;
using VSCodeSnippetCreator.Core.ViewModels;

namespace VSCodeSnippetCreator
{
    public abstract class LiteralViewBase : ReactiveUserControl<LiteralViewModel> { }

    public partial class LiteralView
    {
        public LiteralView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                SerialDisposable serialDisposable = new SerialDisposable();
                d(this
                    .WhenAnyValue(x => x.ViewModel)
                    .Do(vm =>
                    {
                        serialDisposable.Disposable = Disposable.Empty;
                        if (vm == null)
                        {
                            //Reset Controls
                            typeNameInputControl.Visibility = Visibility.Collapsed;
                            functionInputControl.Visibility = Visibility.Collapsed;

                            isEditableCheckBox.IsChecked = true;
                            literalDefaultTextBox.Text = string.Empty;
                            literalToolTipTextBox.Text = string.Empty;
                            defaultInputControl.Visibility = Visibility.Visible;
                            toolTipInputControl.Visibility = Visibility.Visible;
                        }
                    })
                    .Select(_ => new CompositeDisposable()
                    {
                        //Bind CheckBoxes
                        this.Bind(ViewModel, vm => vm.IsEditable, v => v.isEditableCheckBox.IsChecked),

                        //Bind TextBoxes
                        this.Bind(ViewModel, vm => vm.Default, v => v.literalDefaultTextBox.Text),
                        this.Bind(ViewModel, vm => vm.ToolTip, v => v.literalToolTipTextBox.Text),
                        this.Bind(ViewModel, vm => vm.TypeName, v => v.typeNameTextBox.Text),

                        //Bind ComboBoxes
                        this.OneWayBind(ViewModel, vm => vm.FunctionCollection, v => v.literalFunctionComboBox.ItemsSource),
                        this.Bind(ViewModel, vm => vm.Function, v => v.literalFunctionComboBox.SelectedItem,
                            vmToViewConverter: EnumConvertHelper.ConvertEnumDescriptionToString,
                            viewToVmConverter: EnumConvertHelper.ConvertStringToEnumValue<Function>),

                        //BindTo Visibility
                        this.WhenAnyValue(x => x.ViewModel.IsEditable)
                            .BindTo(this, x => x.toolTipInputControl.Visibility),

                        this.WhenAnyValue(x => x.ViewModel.Function)
                            .Select(f => f != Function.SimpleTypeName)
                            .BindTo(this, x => x.defaultInputControl.Visibility),

                        this.WhenAnyValue(x => x.ViewModel.IsEditable, x => x.ViewModel.HasFunction, (edit, func) => !edit && func)
                            .BindTo(this, x => x.functionInputControl.Visibility),

                        this.WhenAnyValue(x => x.ViewModel.Function)
                            .Select(f => f == Function.SimpleTypeName)
                            .BindTo(this, x => x.typeNameInputControl.Visibility)
                    })
                    .Subscribe(disposable => serialDisposable.Disposable = disposable));

                d(serialDisposable);
            });
        }
    }
}
