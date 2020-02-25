#if WORKINPROGRESS
#if MIGRATION
namespace System.Windows.Threading
#else
namespace Windows.UI.Core
#endif
{
    public sealed partial class DispatcherOperation
    {
        internal DispatcherOperation()
        {

        }
    }
}
#endif
