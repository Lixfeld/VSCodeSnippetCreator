using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using ReactiveUI;
using VSCodeSnippetCreator.Core;
using VSCodeSnippetCreator.Core.Helpers;
using VSCodeSnippetCreator.Core.ViewModels;
using static VSCodeSnippetCreator.Core.Constants;
using Extension = VSCodeSnippetCreator.Core.Constants.Extension;

namespace VSCodeSnippetCreator
{
    public partial class MainWindow : MainWindowBase
    {
        private bool _isDarkModeActive;

        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MainWindowModel();

            ReadSettings();
            LoadTheme();

            TextBoxHelper.SetWatermark(namespacesTextBox, $"System.Linq{Environment.NewLine}static System.Math");

            this.WhenActivated(d =>
            {
                //Bind ComboBoxes
                d(this.OneWayBind(ViewModel, vm => vm.LiteralViewModelsCollection, v => v.literalComboBox.ItemsSource));
                d(this.Bind(ViewModel, vm => vm.SelectedLiteral, v => v.literalComboBox.SelectedItem));

                d(this.OneWayBind(ViewModel, vm => vm.LanguageCollection, v => v.languageComboBox.ItemsSource));
                d(this.Bind(ViewModel, vm => vm.SelectedLanguage, v => v.languageComboBox.SelectedItem,
                    vmToViewConverter: EnumConvertHelper.ConvertEnumDescriptionToString,
                    viewToVmConverter: EnumConvertHelper.ConvertStringToEnumValue<Language>));

                //Bind ViewModel
                d(this.Bind(ViewModel, vm => vm.SelectedLiteral, v => v.literalView.ViewModel));

                //Bind TextBoxes
                d(this.Bind(ViewModel, vm => vm.Shortcut, v => v.shortcutTextBox.Text));
                d(this.Bind(ViewModel, vm => vm.Title, v => v.titleTextBox.Text));
                d(this.Bind(ViewModel, vm => vm.Description, v => v.descriptionTextBox.Text));
                d(this.Bind(ViewModel, vm => vm.Author, v => v.authorTextBox.Text));
                d(this.Bind(ViewModel, vm => vm.NamespaceText, v => v.namespacesTextBox.Text));
                d(this.Bind(ViewModel, vm => vm.CodeText, v => v.codeTextBox.Text));
                d(this.Bind(ViewModel, vm => vm.SnippetText, v => v.snippetTextBox.Text));
                d(this.OneWayBind(ViewModel, vm => vm.FileName, v => v.filenameTextBlock.Text));
                d(this.OneWayBind(ViewModel, vm => vm.ExportFolder, v => v.exportFolder.Text));

                //Commands
                d(this.BindCommand(ViewModel, vm => vm.AddEndKeyword, v => v.addEndKeywordButton));
                d(this.BindCommand(ViewModel, vm => vm.AddSelectedKeyword, v => v.addSelectedKeywordButton));
                d(this.BindCommand(ViewModel, vm => vm.ChooseExportFolder, v => v.selectExportPathButton));
                d(this.BindCommand(ViewModel, vm => vm.ReadCodeSnippets, v => v.openCodeSnippetButton));
                d(this.BindCommand(ViewModel, vm => vm.CreateCodeSnippet, v => v.createSnippetButton));
                d(this.BindCommand(ViewModel, vm => vm.ExportCodeSnippets, v => v.exportSnippetButton));

                //Interactions
                d(this.ViewModel.ChooseFolderInteraction.RegisterHandler(
                    interaction => interaction.SetOutput(DialogHelper.GetFolderPath())));

                d(this.ViewModel.ChooseFileInteraction.RegisterHandler(interaction =>
                {
                    string filter = Extension.CodeSnippetFilter;
                    interaction.SetOutput(DialogHelper.GetFilePath(interaction.Input, filter));
                }));

                d(this.ViewModel.ShowMessageInteraction.RegisterHandler(async interaction =>
                {
                    await this.ShowMessageAsync(interaction.Input.Title, interaction.Input.Message, MessageDialogStyle.Affirmative);
                    interaction.SetOutput(Unit.Default);
                }));

                d(this.ViewModel.ConfirmOverwritingInteraction.RegisterHandler(async interaction =>
                {
                    MessageDialogResult dialogResult = await this.ShowMessageAsync("Warning", interaction.Input, MessageDialogStyle.AffirmativeAndNegative);
                    interaction.SetOutput(dialogResult == MessageDialogResult.Affirmative);
                }));

                //WhenAny
                d(this
                    .WhenAnyValue(x => x.ViewModel.CacheCount)
                    .Where(count => count >= 1)
                    .Do(_ => literalStackPanel.IsEnabled = true)
                    .Subscribe(_ => literalComboBox.SelectedIndex = 0));

                d(this
                    .WhenAnyValue(x => x.ViewModel.CacheCount)
                    .Where(count => count <= 0)
                    .Subscribe(_ => literalStackPanel.IsEnabled = false));

                //Events
                d(changeThemeButton.Events()
                    .Click
                    .Do(_ =>
                    {
                        _isDarkModeActive = !_isDarkModeActive;
                        LoadTheme();
                    })
                    .Subscribe());

                //SelectionStart is no DependencyProperty (No binding)
                codeTextBox.Events()
                    .SelectionChanged
                    .Select(_ => codeTextBox.SelectionStart)
                    .BindTo(this, x => x.ViewModel.CodeTextPositionIndex);

                d(this.Events()
                    .Closing
                    .Do(_ => SaveSettings())
                    .Subscribe());
            });
        }

        private void ReadSettings()
        {
            ViewModel.ExportFolder = ConfigurationHelper.ReadSetting(AppConfigKey.ExportFolderPath);
            bool.TryParse(ConfigurationHelper.ReadSetting(AppConfigKey.IsDarkModeActive), out _isDarkModeActive);
        }

        private void SaveSettings()
        {
            ConfigurationHelper.AddOrUpdateAppSettings(AppConfigKey.ExportFolderPath, ViewModel.ExportFolder);
            ConfigurationHelper.AddOrUpdateAppSettings(AppConfigKey.IsDarkModeActive, _isDarkModeActive.ToString());
        }

        private void LoadTheme()
        {
            if (_isDarkModeActive == false)
            {
                ThemeManager.ChangeAppTheme(Application.Current, Constants.Theme.BaseLight);
            }
            else
            {
                ThemeManager.ChangeAppTheme(Application.Current, Constants.Theme.BaseDark);
            }
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(snippetTextBox.Text);
            e.Handled = true;
        }
    }
}
