using System;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace VSCodeSnippetCreator.Core.Schema
{
    [Serializable]
    public class Code
    {
        [XmlAttribute]
        public string Delimiter { get; set; } = "$";

        [XmlIgnore]
        public Language LanguageEnum { get; set; }

        [XmlAttribute]
        public string Language
        {
            get => LanguageEnum.ToString();
            set
            {
                if (Enum.TryParse(value, true, out Language lang))
                    LanguageEnum = lang;
            }
        }

        [XmlIgnore]
        public string Data { get; set; }

        [XmlText]
        public XmlNode[] CDataSection
        {
            get => new[] { new XmlDocument().CreateCDataSection(Data) };
            set => Data = value?.FirstOrDefault()?.Value;
        }

        public bool ShouldSerializeDelimiter() => !string.IsNullOrWhiteSpace(Delimiter) && Delimiter != "$";
        public bool ShouldSerializeLanguage() => LanguageEnum != Core.Language.None;
        public bool ShouldSerializeData() => !string.IsNullOrWhiteSpace(Data);
    }
}
