#if WORKINPROGRESS

using System;

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    /// <summary>
    /// This class implements an easing function that backs up before going to the destination.
    /// </summary>
    public partial class BackEase : EasingFunctionBase
    {
        public double Amplitude { get; set; }
    }
}

#endif