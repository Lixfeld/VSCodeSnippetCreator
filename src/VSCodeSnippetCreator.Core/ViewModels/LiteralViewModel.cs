using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using VSCodeSnippetCreator.Core.Schema;

namespace VSCodeSnippetCreator.Core.ViewModels
{
    public class LiteralViewModel : ReactiveObject
    {
        public IEnumerable<string> FunctionCollection { get; set; } =
            new List<string>() { nameof(Function.ClassName), nameof(Function.SimpleTypeName) };

        [Reactive]
        public bool IsEditable { get; set; } = true;

        [Reactive]
        public string ID { get; set; }

        [Reactive]
        public string Default { get; set; }

        [Reactive]
        public string ToolTip { get; set; }

        [Reactive]
        public bool HasFunction { get; set; }

        [Reactive]
        public Function Function { get; set; }

        [Reactive]
        public string TypeName { get; set; }

        private LiteralViewModel()
        {
            //XmlSerializer 
        }

        public LiteralViewModel(string id)
        {
            ID = id;
        }

        public LiteralViewModel(Literal literal)
        {
            IsEditable = literal.Editable;
            ID = literal.ID;
            Default = literal.Default;
            ToolTip = literal.ToolTip;
            Function = GetFunctionAndTypeName(literal.Function, out string typeName);
            TypeName = typeName;
        }

        private Function GetFunctionAndTypeName(string functionAndTypeName, out string typeName)
        {
            typeName = null;
            Function function = Function.ClassName;

            functionAndTypeName = functionAndTypeName?.Replace("\n", string.Empty).Replace("\r", string.Empty);
            if (string.IsNullOrWhiteSpace(functionAndTypeName) == false)
            {
                Match match = Regex.Match(functionAndTypeName, @"(\w+)\((.*?)\)");
                if (match.Success)
                {
                    typeName = match.Groups[2].Value;
                    string functionString = match.Groups[1].Value;
                    Enum.TryParse(functionString, out function);
                }
            }
            return function;
        }
    }
}
