using System.Windows;

namespace DotNetForHtml5.EmulatorWithoutJavascript
{
    public class ClipboardHandler
    {
        public void SetText(string text)
        {
            Clipboard.SetText(text);
        }
    }
}
