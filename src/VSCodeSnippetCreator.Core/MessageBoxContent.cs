namespace VSCodeSnippetCreator.Core
{
    public class MessageBoxContent
    {
        public string Title { get; }
        public string Message { get; }

        public MessageBoxContent(string title, string message)
        {
            Title = title;
            Message = message;
        }
    }
}
