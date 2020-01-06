using System.Windows;
using System.Windows.Controls;

namespace VSCodeSnippetCreator
{
    public class InputControl : ContentControl
    {
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty
            .Register(nameof(Text), typeof(string), typeof(InputControl), new PropertyMetadata(string.Empty));

        public InputControl()
        {
            //Disable TabNavigation
            IsTabStop = false;
        }
    }
}
