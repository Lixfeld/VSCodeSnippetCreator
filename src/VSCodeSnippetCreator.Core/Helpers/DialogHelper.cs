using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace VSCodeSnippetCreator.Core.Helpers
{
    public static class DialogHelper
    {
        public static string GetFolderPath()
        {
            CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog() { IsFolderPicker = true };
            if (commonOpenFileDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                return commonOpenFileDialog.FileName;
            }
            return null;
        }

        public static string GetFilePath(string initialDirectory = null, string filter = null)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = initialDirectory,
                Filter = filter
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }
            return null;
        }
    }
}
