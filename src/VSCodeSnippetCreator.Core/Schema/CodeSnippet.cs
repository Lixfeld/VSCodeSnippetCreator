using System;
using System.Xml.Serialization;
using VSCodeSnippetCreator.Core.Helpers;
using static VSCodeSnippetCreator.Core.Constants;

namespace VSCodeSnippetCreator.Core.Schema
{
    [Serializable]
    [XmlRoot(Namespace = Namespace.VisualStudioCodeSnippet)]
    public class CodeSnippet
    {
        [XmlAttribute]
        public string Format { get; set; } = "1.0.0";
        public Header Header { get; set; } = new Header();
        public Snippet Snippet { get; set; } = new Snippet();

        public static CodeSnippet Deserialize(string xmlCodeSnippet) => SerializeHelper.Deserialize<CodeSnippet>(xmlCodeSnippet);

        public string Serialize() => SerializeHelper.Serialize(this);
    }
}
