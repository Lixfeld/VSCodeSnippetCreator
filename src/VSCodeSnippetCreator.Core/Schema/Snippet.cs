using System;
using System.Collections.Generic;

namespace VSCodeSnippetCreator.Core.Schema
{
    [Serializable]
    public class Snippet
    {
        public List<Literal> Declarations { get; set; } = new List<Literal>();

        public List<Import> Imports { get; set; } = new List<Import>();

        public Code Code { get; set; } = new Code();

        public bool ShouldSerializeImports() => Imports?.Count > 0;
        public bool ShouldSerializeDeclarations() => Declarations?.Count > 0;
    }
}