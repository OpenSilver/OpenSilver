#if MIGRATION
namespace System.Windows.Ink
{
    /// <summary>
    /// Represents a collection of <see cref="Stroke"/> objects.
    /// </summary>
    public sealed class StrokeCollection : PresentationFrameworkCollection<Stroke>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StrokeCollection"/> class.
        /// </summary>
        public StrokeCollection() : base(true)
        {
        }

        internal override void AddOverride(Stroke point)
        {
            this.AddInternal(point);
        }

        internal override void ClearOverride()
        {
            this.ClearInternal();
        }

        internal override void RemoveAtOverride(int index)
        {
            this.RemoveAtInternal(index);
        }

        internal override void InsertOverride(int index, Stroke point)
        {
            this.InsertInternal(index, point);
        }

        internal override Stroke GetItemOverride(int index)
        {
            return this.GetItemInternal(index);
        }

        internal override void SetItemOverride(int index, Stroke point)
        {
            this.SetItemInternal(index, point);
        }
    }
}
#endif