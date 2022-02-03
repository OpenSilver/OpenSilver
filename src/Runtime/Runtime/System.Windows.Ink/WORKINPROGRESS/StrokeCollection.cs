#if MIGRATION
namespace System.Windows.Ink
{
    /// <summary>
    /// Represents a collection of <see cref="Stroke"/> objects.
    /// </summary>
    [OpenSilver.NotImplemented]
    public sealed class StrokeCollection : PresentationFrameworkCollection<Stroke>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StrokeCollection"/> class.
        /// </summary>
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