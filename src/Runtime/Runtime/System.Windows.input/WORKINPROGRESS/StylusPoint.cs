using System;

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
    [OpenSilver.NotImplemented]
    public struct StylusPoint : IEquatable<StylusPoint>
    {
        public bool Equals(StylusPoint other)
        {
            throw new NotImplementedException();
        }
    }
}
