using System;

namespace VSCodeSnippetCreator.Core.Schema
{
    [Serializable]
    public class Import
    {
        public string Namespace { get; set; }
    }
}