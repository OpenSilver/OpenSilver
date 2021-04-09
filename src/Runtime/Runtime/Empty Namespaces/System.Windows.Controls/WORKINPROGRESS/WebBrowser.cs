#if WORKINPROGRESS

using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public partial class WebBrowser
    {
        public string SaveToString()
        {
            return "";
        }

        public static DependencyProperty SourceProperty
        {
            get; set;
        }

        public System.Uri Source { get; set; }
    }
}
#endif