using System.ComponentModel;

namespace VSCodeSnippetCreator.Core
{
    public enum SnippetType
    {
        Expansion,
        SurroundsWith,
        //Can not be used in custom code snippets
        Refactoring
    }

    public enum Function
    {
        ClassName,
        SimpleTypeName,
        //Not supported
        GenerateSwitchCases
    }

    public enum Language
    {
        None,
        [Description("C#")]
        CSharp,
        [Description("C++")]
        CPP,
        HTML,
        JavaScript,
        SQL,
        TypeScript,
        XAML,
        XML,
        [Description("Visual Basic")]
        VB,
    }
}
