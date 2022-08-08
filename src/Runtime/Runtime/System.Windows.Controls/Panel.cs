
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Markup;
using CSHTML5.Internal;
using OpenSilver.Internal.Controls;

#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Provides a base class for all Panel elements. Use Panel elements to position
    /// and arrange child objects in a UI page.
    /// </summary>
    [ContentProperty("Children")]
    public abstract partial class Panel : FrameworkElement
    {
        private UIElementCollection _uiElementCollection;
        private ItemContainerGenerator _itemContainerGenerator;

        /// <summary> 
        /// Returns enumerator to logical children.
        /// </summary>
        /*protected*/
        internal override IEnumerator LogicalChildren
        {
            get
            {
                if (this._uiElementCollection == null || this._uiElementCollection.Count == 0 || this.IsItemsHost)
                {
                    // empty panel or a panel being used as the items
                    // host has *no* logical children; give empty enumerator
                    return EmptyEnumerator.Instance;
                }

                // otherwise, its logical children is its visual children
                return this.Children.GetEnumerator();
            }
        }

        /// <summary>
        /// Gets the Visual children count.
        /// </summary>
        internal override int VisualChildrenCount
        {
            get
            {
                if (_uiElementCollection == null)
                {
                    return 0;
                }
                else
                {
                    return _uiElementCollection.Count;
                }
            }
        }

        /// <summary>
        /// Gets the Visual child at the specified index.
        /// </summary>
        internal override UIElement GetVisualChild(int index)
        {
            if (_uiElementCollection == null)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return _uiElementCollection[index];
        }

        /// <summary>
        /// The generator associated with this panel.
        /// </summary>
        internal IItemContainerGenerator Generator
        {
            get
            {
                return _itemContainerGenerator;
            }
        }

        internal bool HasChildren
        {
            get
            {
                return this._uiElementCollection != null &&
                       this._uiElementCollection.Count > 0;
            }
        }

        protected virtual UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent)
        {
            return new UIElementCollection(this, logicalParent);
        }

        internal override bool EnablePointerEventsCore
        {
            get
            {
                // We only check the Background property even if BorderBrush not null
                // and BorderThickness > 0 is a sufficient condition to enable pointer
                // events on the borders of the control.
                // There is no way right now to differentiate the Background and BorderBrush
                // as they are both defined on the same DOM element.
                return this.Background != null;
            }
        }

        private int _progressiveRenderingChunkSize;

        /// <summary>
        /// Gets or sets local value of chunk size to render progressively in a batch.
        /// Setting this option can improve performance.
        /// Value lower than 0 means progressive rendering is disabled.
        /// Value of 0 means it uses the size defined at the application level, see
        /// <see cref="Settings.ProgressiveRenderingChunkSize"/>.
        /// Default value is 0.
        /// </summary>
        /// <remarks>
        /// A value close to 1 can break UI in some cases.
        /// </remarks>
        public int ProgressiveRenderingChunkSize
        {
            get => _progressiveRenderingChunkSize == 0 ? GlobalProgressiveRenderingChunkSize : _progressiveRenderingChunkSize;
            set => _progressiveRenderingChunkSize = value;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use ProgressiveRenderingChunkSize instead.")]
        public bool EnableProgressiveRendering
        {
            get => ProgressiveRenderingChunkSize > 0;
            set => ProgressiveRenderingChunkSize = 1;
        }

        internal static int GlobalProgressiveRenderingChunkSize;

        private void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    this.OnChildrenReset();
                    break;
                case NotifyCollectionChangedAction.Add:
                    Debug.Assert(e.NewItems.Count == 1);
                    this.OnChildrenAdded((UIElement)e.NewItems[0], e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    Debug.Assert(e.OldItems.Count == 1);
                    this.OnChildrenRemoved((UIElement)e.OldItems[0], e.OldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    Debug.Assert(e.OldItems.Count == 1 && e.NewItems.Count == 1);
                    this.OnChildrenReplaced((UIElement)e.OldItems[0], (UIElement)e.NewItems[0], e.OldStartingIndex);
                    break;
                default:
                    throw new NotSupportedException(string.Format("Unexpected collection change action '{0}'.", e.Action));
            }
        }

#region Children Management

        internal virtual void OnChildrenReset()
        {
            if (this.INTERNAL_VisualChildrenInformation != null)
            {
                foreach (var childInfo in this.INTERNAL_VisualChildrenInformation.Select(kp => kp.Value).ToArray())
                {
                    INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(childInfo.INTERNAL_UIElement, this);
                }
            }

            if (!this.HasChildren)
            {
                return;
            }

            int chunkSize = ProgressiveRenderingChunkSize;
            var enableProgressiveRendering = chunkSize > 0 && Children.Count > chunkSize;
            if (enableProgressiveRendering)
            {
                this.ProgressivelyAttachChildren(this.Children);
            }
            else
            {
                for (int i = 0; i < this.Children.Count; ++i)
                {
                    INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(this.Children[i], this, i);
                }
            }
        }

        internal virtual void OnChildrenAdded(UIElement newChild, int index)
        {
            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(newChild, this, index);
        }

        internal virtual void OnChildrenRemoved(UIElement oldChild, int index)
        {
            INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(oldChild, this);
        }

        internal virtual void OnChildrenReplaced(UIElement oldChild, UIElement newChild, int index)
        {
            if (oldChild == newChild)
            {
                return;
            }

            INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(oldChild, this);

            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(newChild, this, index);
        }

#endregion Children Management

        /// <summary>
        /// Gets or sets a Brush that is used to fill the panel.
        /// </summary>
        public Brush Background
        {
            get { return (Brush)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="Panel.Background"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register(
                nameof(Background), 
                typeof(Brush), 
                typeof(Panel), 
                new PropertyMetadata((object)null)
                {
                    GetCSSEquivalent = (instance) =>
                    {
                        return new CSSEquivalent()
                        {
                            Name = new List<string> { "background", "backgroundColor", "backgroundColorAlpha" },
                        };
                    },
                    MethodToUpdateDom = (d, e) =>
                    {
                        var panel = (Panel)d;
                        if (e is ImageBrush imageBrush)
                        {
                            SetImageBrushRelatedBackgroundProperties(panel, imageBrush);
                        }
                        UIElement.SetPointerEvents(panel);
                    },
                    CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
                });

        /// <summary>
        /// If the Background property of some controls (Panel, Border) is ImageBrush, 
        /// to visualize similar as SilverLight for varisous Stretch values we need to set
        /// HTML tag's background-size, background-repeat and background-position values.
        /// </summary>
        /// <param name="element">The UIElement to which we are setting background image</param>
        /// <param name="imageBrush">The ImageBrush</param>
        internal static void SetImageBrushRelatedBackgroundProperties(UIElement element, ImageBrush imageBrush)
        {
            string cssSize = "auto";
            switch (imageBrush.Stretch)
            {
                case Stretch.Fill: cssSize = "100% 100%"; break;
                case Stretch.Uniform: cssSize = "contain"; break;
                case Stretch.UniformToFill: cssSize = "cover"; break;
            }

            string domUid = ((INTERNAL_HtmlDomElementReference)element.INTERNAL_OuterDomElement).UniqueIdentifier;
            string backProperties = $"e.style.backgroundSize = \"{cssSize}\";" +
                "e.style.backgroundRepeat = \"no-repeat\";" +
                "e.style.backgroundPosition = \"center center\";";

            string javaScriptCodeToExecute = $"var e = document.getElementById(\"{domUid}\");if (e) {{ {backProperties} }};";
            INTERNAL_SimulatorExecuteJavaScript.ExecuteJavaScriptAsync(javaScriptCodeToExecute);
        }

        /// <summary>
        /// Gets the collection of child elements of the panel.
        /// </summary>
        public UIElementCollection Children
        {
            get
            {
                if (IsItemsHost)
                {
                    EnsureGenerator();
                }
                else
                {
                    if (_uiElementCollection == null)
                    {
                        // First access on a regular panel
                        EnsureEmptyChildren(/* logicalParent = */ this);
                    }
                }

                return _uiElementCollection;
            }
        }

        private void ConnectToGenerator()
        {
            Debug.Assert(_itemContainerGenerator == null, "Attempted to connect to a generator when Panel._itemContainerGenerator is non-null.");

            ItemsControl itemsOwner = ItemsControl.GetItemsOwner(this);
            if (itemsOwner == null)
            {
                // This can happen if IsItemsHost=true, but the panel is not nested in an ItemsControl
                throw new InvalidOperationException("A panel with IsItemsHost=\"true\" is not nested in an ItemsControl. Panel must be nested in ItemsControl to get and show items.");
            }

            IItemContainerGenerator itemsOwnerGenerator = itemsOwner.ItemContainerGenerator;
            if (itemsOwnerGenerator != null)
            {
                _itemContainerGenerator = itemsOwnerGenerator.GetItemContainerGeneratorForPanel(this);
                if (_itemContainerGenerator != null)
                {
                    _itemContainerGenerator.ItemsChanged += new ItemsChangedEventHandler(OnItemsChanged);
                    ((IItemContainerGenerator)_itemContainerGenerator).RemoveAll();
                }
            }
        }

        private void EnsureEmptyChildren(FrameworkElement logicalParent)
        {
            if ((_uiElementCollection == null) || (_uiElementCollection.LogicalParent != logicalParent))
            {
                if (_uiElementCollection != null)
                {
                    _uiElementCollection.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnChildrenCollectionChanged);
                }

                _uiElementCollection = CreateUIElementCollection(logicalParent);
                
                if (_uiElementCollection != null && IsLoaded)
                {
                    _uiElementCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(OnChildrenCollectionChanged);
                }
            }
            else
            {
                ClearChildren();
            }
        }

        internal void EnsureGenerator()
        {
            Debug.Assert(IsItemsHost, "Should be invoked only on an ItemsHost panel");

            if (_itemContainerGenerator == null)
            {
                // First access on an items presenter panel
                ConnectToGenerator();

                // Children of this panel should not have their logical parent reset
                EnsureEmptyChildren(/* logicalParent = */ null);

                GenerateChildren();
            }
        }

        private void ClearChildren()
        {
            if (_itemContainerGenerator != null)
            {
                ((IItemContainerGenerator)_itemContainerGenerator).RemoveAll();
            }

            if ((_uiElementCollection != null) && (_uiElementCollection.Count > 0))
            {
                _uiElementCollection.Clear();
                OnClearChildrenInternal();
            }
        }

        internal virtual void OnClearChildrenInternal()
        {
        }

        internal virtual void GenerateChildren()
        {
            // This method is typically called during layout, which suspends the dispatcher.
            // Firing an assert causes an exception "Dispatcher processing has been suspended, but messages are still being processed."
            // Besides, the asserted condition can actually arise in practice, and the
            // code responds harmlessly.
            //Debug.Assert(_itemContainerGenerator != null, "Encountered a null _itemContainerGenerator while being asked to generate children.");

            IItemContainerGenerator generator = (IItemContainerGenerator)_itemContainerGenerator;
            if (generator != null)
            {
                using (generator.StartAt(new GeneratorPosition(-1, 0), GeneratorDirection.Forward, true))
                {
                    UIElement child;
                    bool isNewlyRealized;
                    while ((child = generator.GenerateNext(out isNewlyRealized) as UIElement) != null)
                    {
                        _uiElementCollection.Add(child);
                        generator.PrepareItemContainer(child);
                    }
                }
            }
        }

        private void OnItemsChanged(object sender, ItemsChangedEventArgs args)
        {
            //if (VerifyBoundState())
            //{
            Debug.Assert(_itemContainerGenerator != null, "Encountered a null _itemContainerGenerator while receiving an ItemsChanged from a generator.");

            bool affectsLayout = OnItemsChangedInternal(sender, args);

            if (affectsLayout)
            {
                // todo
                InvalidateMeasure();
            }
            //}
        }

        // This method returns a bool to indicate if or not the panel layout is affected by this collection change
        internal virtual bool OnItemsChangedInternal(object sender, ItemsChangedEventArgs args)
        {
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddChildren(args.Position, args.ItemCount);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveChildren(args.Position, args.ItemUICount);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    ReplaceChildren(args.Position, args.ItemCount, args.ItemUICount);
                    break;
                case NotifyCollectionChangedAction.Move:
                    MoveChildren(args.OldPosition, args.Position, args.ItemUICount);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    ResetChildren();
                    break;
            }

            return true;
        }

        private void AddChildren(GeneratorPosition pos, int itemCount)
        {
            Debug.Assert(_itemContainerGenerator != null, "Encountered a null _itemContainerGenerator while receiving an Add action from a generator.");

            IItemContainerGenerator generator = (IItemContainerGenerator)_itemContainerGenerator;
            using (generator.StartAt(pos, GeneratorDirection.Forward, true))
            {
                for (int i = 0; i < itemCount; i++)
                {
                    bool isNewlyRealized;
                    UIElement e = generator.GenerateNext(out isNewlyRealized) as UIElement;
                    if (e != null)
                    {
                        _uiElementCollection.Insert(pos.Index + 1 + i, e);
                        generator.PrepareItemContainer(e);
                    }
                    // check this
                    //else
                    //{
                    //    _itemContainerGenerator.Verify();
                    //}
                }
            }
        }

        private void RemoveChildren(GeneratorPosition pos, int containerCount)
        {
            // If anything is wrong, I think these collections should do parameter checking
            for (int i = pos.Index; i < pos.Index + containerCount; i++)
            {
                _uiElementCollection.RemoveAt(i);
            }
        }

        private void ReplaceChildren(GeneratorPosition pos, int itemCount, int containerCount)
        {
            Debug.Assert(itemCount == containerCount, "Panel expects Replace to affect only realized containers");
            Debug.Assert(_itemContainerGenerator != null, "Encountered a null _itemContainerGenerator while receiving an Replace action from a generator.");

            IItemContainerGenerator generator = (IItemContainerGenerator)_itemContainerGenerator;
            using (generator.StartAt(pos, GeneratorDirection.Forward, true))
            {
                for (int i = 0; i < itemCount; i++)
                {
                    bool isNewlyRealized;
                    UIElement e = generator.GenerateNext(out isNewlyRealized) as UIElement;

                    Debug.Assert(e != null && !isNewlyRealized, "Panel expects Replace to affect only realized containers");
                    if (e != null && !isNewlyRealized)
                    {
                        _uiElementCollection[pos.Index + i] = e;
                        generator.PrepareItemContainer(e);
                    }
                    // check this
                    //else
                    //{
                    //    _itemContainerGenerator.Verify();
                    //}
                }
            }
        }

        private void MoveChildren(GeneratorPosition fromPos, GeneratorPosition toPos, int containerCount)
        {
            if (fromPos == toPos)
                return;

            Debug.Assert(_itemContainerGenerator != null, "Encountered a null _itemContainerGenerator while receiving an Move action from a generator.");

            IItemContainerGenerator generator = (IItemContainerGenerator)_itemContainerGenerator;
            int toIndex = generator.IndexFromGeneratorPosition(toPos);

            UIElement[] elements = new UIElement[containerCount];

            for (int i = 0; i < containerCount; i++)
            {
                elements[i] = _uiElementCollection[fromPos.Index + i];
            }

            for (int i = 0; i < containerCount; i++)
            {
                _uiElementCollection.RemoveAt(fromPos.Index + i);
            }

            for (int i = 0; i < containerCount; i++)
            {
                _uiElementCollection.Insert(toIndex + i, elements[i]);
            }
        }

        private void ResetChildren()
        {
            EnsureEmptyChildren(null);
            GenerateChildren();
        }

        internal UIElementCollection InternalChildren
        {
            get
            {
                return _uiElementCollection;
            }
        }


        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();

            if (this._uiElementCollection != null)
            {
                this._uiElementCollection.CollectionChanged -= new NotifyCollectionChangedEventHandler(OnChildrenCollectionChanged);
                this._uiElementCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(OnChildrenCollectionChanged);
            }

            this.OnChildrenReset();
        }

        private async void ProgressivelyAttachChildren(IList<UIElement> newChildren)
        {
            int chunkSize = ProgressiveRenderingChunkSize;
            int from = 0;
            int to = (chunkSize * 2 > newChildren.Count) ? newChildren.Count : chunkSize; // do not process less number of items than chunk size
            
            while (true)
            {
                await Task.Delay(1);
                if (!INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
                {
                    //this can happen if the Panel is detached during the delay.
                    break;
                }

                for (int i = from; i < to; ++i)
                {
                    INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(newChildren[i], this, i);
                }

                int remaining = newChildren.Count - to;
                if (remaining == 0)
                {
                    break;
                }
                
                from = to;
                to += (chunkSize * 2 > remaining) ? remaining : chunkSize;
            }
        }

        /// <summary>
        /// Retrieves the named element in the instantiated ControlTemplate visual tree.
        /// </summary>
        /// <param name="childName">The name of the element to find.</param>
        /// <returns>
        /// The named element from the template, if the element is found. Can return
        /// null if no element with name childName was found in the template.
        /// </returns>
        protected internal new DependencyObject GetTemplateChild(string childName)
        {
            return base.GetTemplateChild(childName);
        }

        /// <summary>
        /// Identifies the <see cref="Panel.IsItemsHost"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsItemsHostProperty =
            DependencyProperty.Register(
                nameof(IsItemsHost),
                typeof(bool),
                typeof(Panel),
                new PropertyMetadata(false, OnIsItemsHostChanged));

        /// <summary>
        /// Gets a value that indicates whether this <see cref="Panel"/> is a container
        /// for UI items that are generated by an <see cref="ItemsControl"/>.
        /// </summary>
        public bool IsItemsHost
        {
            get { return (bool)this.GetValue(IsItemsHostProperty); }
            internal set { this.SetValue(IsItemsHostProperty, value); }
        }

        private static void OnIsItemsHostChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Panel panel = (Panel)d;
            ItemsControl itemsControl = ItemsControl.GetItemsOwner(panel);
            if (itemsControl != null)
            {
                itemsControl.ItemsHost = panel;
            }
        }
    }
}
