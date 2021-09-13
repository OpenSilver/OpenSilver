#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    [OpenSilver.NotImplemented]
    public class PointKeyFrameCollection : PresentationFrameworkCollection<PointKeyFrame>
    {
        internal override void AddOverride(PointKeyFrame value)
        {
            this.AddDependencyObjectInternal(value);
        }

        internal override void ClearOverride()
        {
            this.ClearDependencyObjectInternal();
        }

        internal override PointKeyFrame GetItemOverride(int index)
        {
            return this.GetItemInternal(index);
        }

        internal override void InsertOverride(int index, PointKeyFrame value)
        {
            this.InsertDependencyObjectInternal(index, value);
        }

        internal override void RemoveAtOverride(int index)
        {
            this.RemoveAtDependencyObjectInternal(index);
        }

        internal override bool RemoveOverride(PointKeyFrame value)
        {
            return this.RemoveDependencyObjectInternal(value);
        }

        internal override void SetItemOverride(int index, PointKeyFrame value)
        {
            this.SetItemDependencyObjectInternal(index, value);
        }
    }
}
