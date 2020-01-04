using System;
using System.Xml.Serialization;
using VSCodeSnippetCreator.Core.Helpers;
using static VSCodeSnippetCreator.Core.Constants;

namespace VSCodeSnippetCreator.Core.Schema
{
    [Serializable]
    [XmlRoot(Namespace = Namespace.VisualStudioCodeSnippet)]
    public class CodeSnippets
    {
        public CodeSnippet CodeSnippet { get; set; } = new CodeSnippet();

        public static CodeSnippets Deserialize(string xmlCodeSnippets) => SerializeHelper.Deserialize<CodeSnippets>(xmlCodeSnippets);

        public string Serialize() => SerializeHelper.Serialize(this);
    }
}
