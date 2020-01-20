using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using VSCodeSnippetCreator.Core.Helpers;
using VSCodeSnippetCreator.Core.Schema;
using static VSCodeSnippetCreator.Core.Constants;

namespace VSCodeSnippetCreator.Core.ViewModels
{
    public class MainWindowModel : ReactiveObject
    {
        private readonly SourceCache<LiteralViewModel, string> _sourceCache;
        private readonly ReadOnlyObservableCollection<LiteralViewModel> _literalViewModelsCollection;
        public ReadOnlyObservableCollection<LiteralViewModel> LiteralViewModelsCollection => _literalViewModelsCollection;

        private ObservableAsPropertyHelper<int> _cacheCount;
        public int CacheCount => _cacheCount.Value;

        [Reactive]
        public LiteralViewModel SelectedLiteral { get; set; }

        [Reactive]
        public string FileName { get; set; }

        [Reactive]
        public string Shortcut { get; set; }

        [Reactive]
        public string Title { get; set; }

        [Reactive]
        public string Description { get; set; }

        [Reactive]
        public string Author { get; set; }

        [Reactive]
        public string NamespaceText { get; set; }
        private readonly IEnumerable<string> _namespaces;

        public IEnumerable<string> LanguageCollection { get; set; } = EnumConvertHelper.GetEnumDescriptions<Language>();

        [Reactive]
        public Language SelectedLanguage { get; set; } = Language.CSharp;

        [Reactive]
        public string CodeText { get; set; }

        [Reactive]
        public int CodeTextPositionIndex { get; set; }

        [Reactive]
        public string SnippetText { get; set; }

        [Reactive]
        public string ExportFolder { get; set; }

        public ReactiveCommand<Unit, string> CreateCodeSnippet { get; }

        public ReactiveCommand<Unit, Unit> ReadCodeSnippets { get; }

        public ReactiveCommand<Unit, Unit> ExportCodeSnippets { get; }

        public ReactiveCommand<Unit, string> ChooseExportFolder { get; }

        public ReactiveCommand<Unit, Unit> AddEndKeyword { get; }

        public ReactiveCommand<Unit, Unit> AddSelectedKeyword { get; }

        public Interaction<string, string> ChooseFileInteraction { get; } = new Interaction<string, string>();
        public Interaction<Unit, string> ChooseFolderInteraction { get; } = new Interaction<Unit, string>();
        public Interaction<string, bool> ConfirmOverwritingInteraction { get; } = new Interaction<string, bool>();
        public Interaction<MessageBoxContent, Unit> ShowMessageInteraction { get; } = new Interaction<MessageBoxContent, Unit>();

        public MainWindowModel()
        {
            _sourceCache = new SourceCache<LiteralViewModel, string>(lvm => lvm.ID);
            _sourceCache.Connect()
                    .Bind(out _literalViewModelsCollection)
                    .DisposeMany()
                    .Subscribe();

            _cacheCount = _sourceCache.CountChanged
                .ToProperty(this, nameof(CacheCount));

            this.WhenAnyValue(x => x.CodeText)
                .Skip(1)
                .Where(code => code != null)
                .Throttle(TimeSpan.FromSeconds(0.6))
                .Select(t => RegexDelimiter(t))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Do(strCollection => UpdateList(strCollection))
                .Subscribe();

            this
                .WhenAnyValue(x => x.NamespaceText)
                .Skip(1)
                .Throttle(TimeSpan.FromSeconds(0.6))
                .Select(t => t.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
                .Select(RemoveImportKeywords)
                .BindTo(this, x => x._namespaces);

            this
                .WhenAnyValue(x => x.SelectedLanguage, x => x.CacheCount, (lang, count) => lang)
                .Select(l => l == Language.CSharp)
                .Do(ChangeFunctionForAllViewModels)
                .Subscribe();

            this
                .WhenAnyValue(x => x.Shortcut)
                .Select(x => x + Extension.Snippet)
                .BindTo(this, x => x.FileName);

            //CanExecute
            var canExecuteEnd = this
                .WhenAnyValue(x => x.CodeText)
                .Where(s => s != null)
                .Select(s => !s.Contains(Keyword.WithDelimiters.End));

            var canExecuteSelected = this
                .WhenAnyValue(x => x.CodeText)
                .Where(s => s != null)
                .Select(s => !s.Contains(Keyword.WithDelimiters.Selected));

            var canExecuteSnippet = this
                .WhenAnyValue(x => x.Shortcut, x => x.Title, (shortcut, title) =>
                    !string.IsNullOrWhiteSpace(shortcut) && !string.IsNullOrWhiteSpace(title));

            //Commands
            AddEndKeyword = ReactiveCommand.Create(() =>
            {
                CodeText = CodeText.Insert(CodeTextPositionIndex, Keyword.WithDelimiters.End);
            }, canExecuteEnd);

            AddSelectedKeyword = ReactiveCommand.Create(() =>
            {
                CodeText = CodeText.Insert(CodeTextPositionIndex, Keyword.WithDelimiters.Selected);
            }, canExecuteSelected);

            ChooseExportFolder = ReactiveCommand.CreateFromObservable(() => ChooseFolderInteraction.Handle(Unit.Default));
            ChooseExportFolder.Log(this, "CHOOSE").Where(f => f != null).Subscribe(f => ExportFolder = f);

            ReadCodeSnippets = ReactiveCommand.CreateFromTask(ReadCodeSnippet);
            CreateCodeSnippet = ReactiveCommand.Create(() => SnippetText = CreateCodeSnippets(), canExecuteSnippet);
            ExportCodeSnippets = ReactiveCommand.CreateFromTask(ExportCodeSnippet, canExecuteSnippet);
        }

        private void ChangeFunctionForAllViewModels(bool hasFunction)
        {
            foreach (LiteralViewModel lvm in LiteralViewModelsCollection)
            {
                lvm.HasFunction = hasFunction;
            }
        }

        private IEnumerable<string> RegexDelimiter(string text)
        {
            string escapedDelimiter = Regex.Escape("$");
            string pattern = escapedDelimiter + @"(.*?)" + escapedDelimiter;
            MatchCollection matches = Regex.Matches(text, pattern);

            return matches.Cast<Match>().Select(m => m.Groups[1].Value);
        }

        private IEnumerable<string> RemoveImportKeywords(IEnumerable<string> namespaces)
        {
            return namespaces.Select(ns => ns
                //C#
                .Trim(' ', ';')
                .Replace("using ", string.Empty)
                //VB.NET
                .Replace("Imports ", string.Empty));
        }

        private void UpdateList(IEnumerable<string> strCollection)
        {
            IEnumerable<string> ids = strCollection.Distinct();
            IEnumerable<string> keys = LiteralViewModelsCollection
                .Select(lvm => lvm.ID)
                .Where(id => ids.Contains(id) == false);

            _sourceCache.Edit(innerlist =>
            {
                _sourceCache.RemoveKeys(keys);

                foreach (string id in ids)
                {
                    //Ignore whitespace and constants
                    if (!string.IsNullOrWhiteSpace(id) && id != Keyword.End && id != Keyword.Selected)
                    {
                        LiteralViewModel literalViewModel = LiteralViewModelsCollection.Where(l => l.ID == id).FirstOrDefault();
                        if (literalViewModel == null)
                        {
                            _sourceCache.AddOrUpdate(new LiteralViewModel(id));
                        }
                    }
                }
            });
        }

        private async Task ReadCodeSnippet()
        {
            try
            {
                string codeSnippetPath = await ChooseFileInteraction.Handle(ExportFolder);
                if (codeSnippetPath != null)
                {
                    string fileContent = File.ReadAllText(codeSnippetPath);
                    //Deserialize Format 1.0.0
                    CodeSnippets codeSnippets = CodeSnippets.Deserialize(fileContent);

                    if (codeSnippets == null)
                    {
                        //Deserialize Format 1.1.0
                        CodeSnippet codeSnippet = CodeSnippet.Deserialize(fileContent);
                        if (codeSnippet != null)
                        {
                            codeSnippets = new CodeSnippets() { CodeSnippet = codeSnippet };
                        }
                    }

                    if (codeSnippets.CodeSnippet.Header.SnippetTypes?.Contains(SnippetType.Refactoring) == true)
                    {
                        var content = new MessageBoxContent("Warning", "'Refactoring' cannot be used in custom code snippets!");
                        await ShowMessageInteraction.Handle(content);
                    }

                    Header header = codeSnippets.CodeSnippet.Header;
                    Author = header.Author;
                    Description = header.Description;
                    Shortcut = header.Shortcut;
                    Title = header.Title;

                    Snippet snippet = codeSnippets.CodeSnippet.Snippet;
                    IEnumerable<string> ns = snippet.Imports.Select(i => i.Namespace);
                    NamespaceText = string.Join(Environment.NewLine, ns);

                    CodeText = snippet.Code.Data;
                    SelectedLanguage = snippet.Code.LanguageEnum;

                    IEnumerable<LiteralViewModel> literalViewModels = snippet.Declarations
                        .Where(l => !string.IsNullOrWhiteSpace(l.ID))
                        .Select(l => new LiteralViewModel(l));

                    _sourceCache.Clear();
                    _sourceCache.Edit(innerlist =>
                    {
                        foreach (LiteralViewModel literalVm in literalViewModels)
                        {
                            _sourceCache.AddOrUpdate(literalVm);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: Read code snippets. " + ex.Message);
                var content = new MessageBoxContent("Error", "Could not read complete code snippet successfully!");
                await ShowMessageInteraction.Handle(content);
                return;
            }
        }

        private string CreateCodeSnippets()
        {
            CodeSnippets codeSnippets = new CodeSnippets();
            Header header = codeSnippets.CodeSnippet.Header;

            header.Author = Author;
            header.Description = Description;
            header.Shortcut = Shortcut;
            header.Title = Title;
            header.SnippetTypes.Add(SnippetType.Expansion);
            if (CodeText.Contains(Keyword.WithDelimiters.Selected))
            {
                header.SnippetTypes.Add(SnippetType.SurroundsWith);
            }

            Snippet snippet = codeSnippets.CodeSnippet.Snippet;
            IEnumerable<Import> imports = _namespaces.Select(ns => new Import() { Namespace = ns });
            snippet.Imports.AddRange(imports);
            IEnumerable<Literal> literals = LiteralViewModelsCollection.Select(l => new Literal(l));
            snippet.Declarations.AddRange(literals);

            snippet.Code.Data = CodeText;
            snippet.Code.LanguageEnum = SelectedLanguage;

            return codeSnippets.Serialize();
        }

        private async Task ExportCodeSnippet()
        {
            try
            {
                if (Directory.Exists(ExportFolder) == false)
                {
                    string folderPath = await ChooseFolderInteraction.Handle(Unit.Default);
                    if (Directory.Exists(folderPath))
                    {
                        ExportFolder = folderPath;
                    }
                    else
                    {
                        return;
                    }
                }

                SnippetText = CreateCodeSnippets();
                string filePath = Path.Combine(ExportFolder, FileName);
                if (File.Exists(filePath))
                {
                    string warningMessage = $"The file '{FileName}' already exists.{Environment.NewLine}Do you want to overwrite it?";
                    if (await ConfirmOverwritingInteraction.Handle(warningMessage) == false)
                        return;
                }
                File.WriteAllText(filePath, SnippetText);

                var successContent = new MessageBoxContent("Info", $"The file {FileName} was saved successfully.");
                await ShowMessageInteraction.Handle(successContent);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: Export code snippets. " + ex.Message);
                var errorContent = new MessageBoxContent(
                    "Error", $"The file {FileName} could NOT be saved!" + Environment.NewLine + @"Possible solution: ""Run as administrator""");
                await ShowMessageInteraction.Handle(errorContent);
            }
            return;
        }
    }
}