#if WORKINPROGRESS

using System;

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
    public static partial class Cursors
    {
        public static Cursor Eraser { get; }
        public static Cursor Stylus { get; }
    }
}
#endif