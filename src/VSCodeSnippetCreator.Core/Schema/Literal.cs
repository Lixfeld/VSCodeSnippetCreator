using System;
using System.Xml.Serialization;
using VSCodeSnippetCreator.Core.ViewModels;
using FunctionEnum = VSCodeSnippetCreator.Core.Function;

namespace VSCodeSnippetCreator.Core.Schema
{
    [Serializable]
    public class Literal
    {
        [XmlAttribute]
        public bool Editable { get; set; } = true;
        public string ID { get; set; }
        public string Default { get; set; }
        public string ToolTip { get; set; }
        public string Function { get; set; }

        private Literal()
        {
            //XmlSerializer 
        }

        public Literal(LiteralViewModel literalViewModel)
        {
            Editable = literalViewModel.IsEditable;
            ID = literalViewModel.ID;
            Default = literalViewModel.Default;
            ToolTip = literalViewModel.ToolTip;
            if (literalViewModel.HasFunction && Editable == false)
            {
                Function = $"{literalViewModel.Function}({literalViewModel.TypeName})";
            }
        }

        public bool ShouldSerializeToolTip() => !string.IsNullOrWhiteSpace(ToolTip);
        public bool ShouldSerializeFunction() => !string.IsNullOrWhiteSpace(Function);
        public bool ShouldSerializeDefault()
        {
            if (Function != null)
            {
                return !Function.StartsWith(nameof(FunctionEnum.SimpleTypeName));
            }
            return true;
        }
    }
}