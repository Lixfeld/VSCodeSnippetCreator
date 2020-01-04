namespace VSCodeSnippetCreator.Core
{
    public static class Constants
    {
        public static class AppConfigKey
        {
            public const string ExportFolderPath = "ExportFolderPath";
            public const string IsDarkModeActive = "IsDarkModeActive";
        }

        public static class Theme
        {
            public const string BaseLight = "BaseLight";
            public const string BaseDark = "BaseDark";
        }

        public static class Namespace
        {
            public const string VisualStudioCodeSnippet = "http://schemas.microsoft.com/VisualStudio/2005/CodeSnippet";
        }

        public static class Keyword
        {
            public const string Selected = "selected";
            public const string End = "end";

            public static class WithDelimiters
            {
                public const string Selected = "$selected$";
                public const string End = "$end$";
            }
        }

        public static class Extension
        {
            public const string Snippet = ".snippet";
            public const string CodeSnippetFilter = "Code snippets (*.snippet)|*.snippet";
        }
    }
}
