using System.Windows.Markup;

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    [ContentProperty("KeyFrames")]  
    [OpenSilver.NotImplemented]
    public sealed class PointAnimationUsingKeyFrames : Timeline
    {
        private PointKeyFrameCollection _keyFrames;

        /// <summary>
        /// Initializes a new instance of the <see cref="PointAnimationUsingKeyFrames"/>
        /// class.
        /// </summary>
        [OpenSilver.NotImplemented]
        public PointAnimationUsingKeyFrames()
        {

        }
        
        /// <summary>
        /// Gets the collection of <see cref="PointKeyFrame"/> objects that
        /// define the animation.
        /// </summary>
        /// <returns>
        /// The collection of <see cref="PointKeyFrame"/> objects that define
        /// the animation. The default is an empty collection.
        /// </returns>
        [OpenSilver.NotImplemented]
        public PointKeyFrameCollection KeyFrames => _keyFrames ??= new PointKeyFrameCollection();
    }
}
