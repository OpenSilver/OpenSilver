#if WORKINPROGRESS

using System;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    public partial class Window
    {
        public string Title { get; set; }
        public bool Install()
        {
            return true;
        }
    }
}
#endif