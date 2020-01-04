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

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty = DependencyProperty
            .Register(nameof(Text), typeof(string), typeof(InputControl), new PropertyMetadata(string.Empty));
    }
}
