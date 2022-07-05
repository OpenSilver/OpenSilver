namespace System.Windows.Controls.DataVisualization.Charting.Primitives
{
    /// <summary>
    /// Subclasses ListBox to provide an easy way for a consumer of
    /// ListBox to hook into the four standard ListBox *Container*
    /// overrides.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class DelegatingListBox : ListBox
    {
        /// <summary>
        /// Gets or sets a function to call when the
        /// IsItemItsOwnContainerOverride method executes.
        /// </summary>
        public Func<object, bool> IsItemItsOwnContainer { get; set; }

        /// <summary>
        /// Gets or sets a function to call when the
        /// GetContainerForItem method executes.
        /// </summary>
        public Func<DependencyObject> GetContainerForItem { get; set; }

        /// <summary>
        /// Gets or sets an action to call when the
        /// PrepareContainerForItem method executes.
        /// </summary>
        public Action<DependencyObject, object> PrepareContainerForItem { get; set; }

        /// <summary>
        /// Gets or sets an action to call when the
        /// ClearContainerForItem method executes.
        /// </summary>
        public Action<DependencyObject, object> ClearContainerForItem { get; set; }

        /// <summary>
        /// Initializes a new instance of the DelegatingListBox class.
        /// </summary>
        public DelegatingListBox()
        {
            this.DefaultStyleKey = (object)typeof(DelegatingListBox);
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the item is (or is eligible to be) its own container; otherwise, false.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return this.IsItemItsOwnContainer != null ? this.IsItemItsOwnContainer(item) : base.IsItemItsOwnContainerOverride(item);
        }

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return this.GetContainerForItem != null ? this.GetContainerForItem() : base.GetContainerForItemOverride();
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">The element used to display the specified item.</param>
        /// <param name="item">The item to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            if (null == this.PrepareContainerForItem)
                return;
            this.PrepareContainerForItem(element, item);
        }

        /// <summary>
        /// Undoes the effects of the PrepareContainerForItemOverride method.
        /// </summary>
        /// <param name="element">The container element.</param>
        /// <param name="item">The item to display.</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            base.ClearContainerForItemOverride(element, item);
            if (null == this.ClearContainerForItem)
                return;
            this.ClearContainerForItem(element, item);
        }
    }
}
