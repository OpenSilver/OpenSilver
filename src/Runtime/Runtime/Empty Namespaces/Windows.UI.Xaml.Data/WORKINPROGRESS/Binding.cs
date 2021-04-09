#if WORKINPROGRESS

using System;

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    public partial class Binding : BindingBase
    {
        public static bool IsDebuggingEnabled;
    }
}
#endif