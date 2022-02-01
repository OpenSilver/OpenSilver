#if MIGRATION
namespace System.Windows.Ink
{
    /// <summary>
    /// Collection of strokes objects which can be operated on in aggregate.
    /// </summary>
    [OpenSilver.NotImplemented]
    public sealed class StrokeCollection : PresentationFrameworkCollection<Stroke>
    {
        [OpenSilver.NotImplemented]
        public StrokeCollection() : base(false)
        {
        }

        internal override void AddOverride(Stroke value)
        {
            throw new NotImplementedException();
        }

        internal override void ClearOverride()
        {
            throw new NotImplementedException();
        }

        internal override Stroke GetItemOverride(int index)
        {
            throw new NotImplementedException();
        }

        internal override void InsertOverride(int index, Stroke value)
        {
            throw new NotImplementedException();
        }

        internal override void RemoveAtOverride(int index)
        {
            throw new NotImplementedException();
        }

        internal override void SetItemOverride(int index, Stroke value)
        {
            throw new NotImplementedException();
        }
    }
}
#endif