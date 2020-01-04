using System.Linq;
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

            typeNameInputControl.Visibility = Visibility.Collapsed;
            functionInputControl.Visibility = Visibility.Collapsed;

            this.WhenActivated(d =>
            {
                //Bind CheckBoxes
                d(this.Bind(ViewModel, vm => vm.IsEditable, v => v.isEditableCheckBox.IsChecked));

                //Bind TextBoxes
                d(this.Bind(ViewModel, vm => vm.Default, v => v.literalDefaultTextBox.Text));
                d(this.Bind(ViewModel, vm => vm.ToolTip, v => v.literalToolTipTextBox.Text));
                d(this.Bind(ViewModel, vm => vm.TypeName, v => v.typeNameTextBox.Text));


                //Bind ComboBoxes
                d(this.OneWayBind(ViewModel, vm => vm.FunctionCollection, v => v.literalFunctionComboBox.ItemsSource));
                d(this.Bind(ViewModel, vm => vm.Function, v => v.literalFunctionComboBox.SelectedItem,
                    vmToViewConverter: EnumConvertHelper.ConvertEnumDescriptionToString,
                    viewToVmConverter: EnumConvertHelper.ConvertStringToEnumValue<Function>));

                //BindTo Visibility
                d(this
                    .WhenAnyValue(x => x.ViewModel.IsEditable)
                    .BindTo(this, x => x.toolTipInputControl.Visibility));

                d(this
                    .WhenAnyValue(x => x.ViewModel.Function)
                    .Select(f => f != Function.SimpleTypeName)
                    .BindTo(this, x => x.defaultInputControl.Visibility));

                d(this
                    .WhenAnyValue(x => x.ViewModel.IsEditable, x => x.ViewModel.HasFunction, (edit, func) => !edit && func)
                    .BindTo(this, x => x.functionInputControl.Visibility));

                d(this
                    .WhenAnyValue(x => x.ViewModel.Function)
                    .Select(f => f == Function.SimpleTypeName)
                    .BindTo(this, x => x.typeNameInputControl.Visibility));
            });
        }
    }
}
