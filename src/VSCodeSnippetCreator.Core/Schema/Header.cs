using System;
using System.Collections.Generic;

namespace VSCodeSnippetCreator.Core.Schema
{
    [Serializable]
    public class Header
    {
        public string Title { get; set; }
        public string Shortcut { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public List<SnippetType> SnippetTypes { get; set; } = new List<SnippetType>();

        public bool ShouldSerializeDescription() => !string.IsNullOrWhiteSpace(Description);
        public bool ShouldSerializeAuthor() => !string.IsNullOrWhiteSpace(Author);
    }
}