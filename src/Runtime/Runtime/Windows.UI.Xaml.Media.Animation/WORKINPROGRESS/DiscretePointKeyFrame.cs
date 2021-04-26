﻿#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    /// <summary>
    /// Animates from the <see cref="Point"/> value of the previous key frame to its
    /// own <see cref="PointKeyFrame.Value"/> using discrete frames.
    /// </summary>
    [OpenSilver.NotImplemented]
    public sealed class DiscretePointKeyFrame : PointKeyFrame
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscretePointKeyFrame"/> class.
        /// </summary>
        [OpenSilver.NotImplemented]
        public DiscretePointKeyFrame()
        {

        }
    }
}
#endif
