#if WORKINPROGRESS

#if !MIGRATION
using System;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.Foundation
#endif
{
	public static class RectExtensions
    {
        public static bool IsClose(this Rect @this, Rect rect)
        {
            return @this.Location.IsClose(rect.Location) && @this.Size.IsClose(rect.Size);
        }
    }
}
#endif