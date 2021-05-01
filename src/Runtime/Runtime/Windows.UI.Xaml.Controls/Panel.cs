

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


using CSHTML5.Internal;
using OpenSilver.Internal.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Markup;

#if MIGRATION
using System.Windows.Media;
#else
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
        /// <summary> 
        /// Returns enumerator to logical children.
        /// </summary>
        /*protected*/ internal override IEnumerator LogicalChildren
        {
            get
            {
                if (this._children == null || this._children.Count == 0 || this.IsItemsHost)
                {
                    // empty panel or a panel being used as the items
                    // host has *no* logical children; give empty enumerator
                    return EmptyEnumerator.Instance;
                }

                // otherwise, its logical children is its visual children
                return this.Children.GetEnumerator();
            }
        }

        internal bool HasChildren
        {
            get
            {
                return this._children != null &&
                       this._children.Count > 0;
            }
        }

        protected virtual UIElementCollection CreateUIElementCollection(FrameworkElement logicalParent)
        {
            return new UIElementCollection(this, logicalParent);
        }

#if REVAMPPOINTEREVENTS
        internal override bool INTERNAL_ManageFrameworkElementPointerEventsAvailability()
        {
            // We only check the Background property even if BorderBrush not null + BorderThickness > 0 is a sufficient condition to enable pointer events on the borders of the control.
            // There is no way right now to differentiate the Background and BorderBrush as they are both defined on the same DOM element.
            return Background != null;
        }
#endif

        private UIElementCollection _children;

        private bool _enableProgressiveRendering;
        public bool EnableProgressiveRendering
        {
            get { return this._enableProgressiveRendering || INTERNAL_ApplicationWideEnableProgressiveRendering; }
            set { this._enableProgressiveRendering = value; }
        }
        internal static bool INTERNAL_ApplicationWideEnableProgressiveRendering;

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

            if (this._enableProgressiveRendering || this.INTERNAL_EnableProgressiveLoading)
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
        /// Identifies the Background dependency property.
        /// </summary>
        public static readonly DependencyProperty BackgroundProperty =
            DependencyProperty.Register("Background", typeof(Brush), typeof(Panel), new PropertyMetadata(null
#if REVAMPPOINTEREVENTS
                , Background_Changed
#endif
                )
            {
                GetCSSEquivalent = (instance) =>
                {
                    return new CSSEquivalent()
                    {
                        Name = new List<string> { "background", "backgroundColor", "backgroundColorAlpha" },
                    };
                },
                CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet
            }
            );

#if REVAMPPOINTEREVENTS
        private static void Background_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = (UIElement)d;
            INTERNAL_UpdateCssPointerEvents(element);
        }
#endif

        /// <summary>
        /// Gets the collection of child elements of the panel.
        /// </summary>
        public UIElementCollection Children
        {
            get
            {
                if (_children == null)
                {
                    _children = CreateUIElementCollection(IsItemsHost ? null : this);

                    if (this._isLoaded)
                        _children.CollectionChanged += OnChildrenCollectionChanged;
                }

                return _children;
            }
        }

        internal UIElementCollection InternalChildren
        {
            get
            {
                return _children;
            }
        }


        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();

            if (_children != null)
            {
                _children.CollectionChanged -= OnChildrenCollectionChanged;
                _children.CollectionChanged += OnChildrenCollectionChanged;
            }

            this.OnChildrenReset();
        }

        protected internal override void INTERNAL_OnDetachedFromVisualTree()
        {
            base.INTERNAL_OnDetachedFromVisualTree();

            if (_children != null)
                _children.CollectionChanged -= OnChildrenCollectionChanged;
        }

        private async void ProgressivelyAttachChildren(IList<UIElement> newChildren)
        {
            for (int i = 0; i < newChildren.Count; ++i)
            {
                await Task.Delay(1);
                if (!INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
                {
                    //this can happen if the Panel is detached during the delay.
                    break;
                }
                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(newChildren[i], this);
                INTERNAL_OnChildProgressivelyLoaded();
            }
        }

        protected virtual void INTERNAL_OnChildProgressivelyLoaded()
        {
        }


        /// <summary>
        /// Retrieves the named element in the instantiated ControlTemplate visual tree.
        /// </summary>
        /// <param name="childName">The name of the element to find.</param>
        /// <returns>
        /// The named element from the template, if the element is found. Can return
        /// null if no element with name childName was found in the template.
        /// </returns>
        protected internal DependencyObject GetTemplateChild(string childName)
        {
            return (DependencyObject)this.TryFindTemplateChildFromName(childName);
        }
#region ---------- INameScope implementation ----------
        //note: copy from UserControl
        Dictionary<string, object> _nameScopeDictionary = new Dictionary<string, object>();

        /// <summary>
        /// Finds the UIElement with the specified name. Returns null if not found.
        /// </summary>
        /// <param name="name">The name to look for.</param>
        /// <returns>The object with the specified name if any; otherwise null.</returns>
        private object TryFindTemplateChildFromName(string name)
        {
            //todo: see if this fits to the behaviour it should have.
            if (_nameScopeDictionary.ContainsKey(name))
                return _nameScopeDictionary[name];
            else
                return null;
        }

        public void RegisterName(string name, object scopedElement)
        {
            if (_nameScopeDictionary.ContainsKey(name) && _nameScopeDictionary[name] != scopedElement)
                throw new ArgumentException(string.Format("Cannot register duplicate name '{0}' in this scope.", name));

            _nameScopeDictionary[name] = scopedElement;
        }

        public void UnregisterName(string name)
        {
            if (!_nameScopeDictionary.ContainsKey(name))
                throw new ArgumentException(string.Format("Name '{0}' was not found.", name));

            _nameScopeDictionary.Remove(name);
        }

        void ClearRegisteredNames()
        {
            _nameScopeDictionary.Clear();
        }


        #endregion

        /// <summary>
        /// Identifies the <see cref="Panel.IsItemsHost"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsItemsHostProperty =
            DependencyProperty.Register(
                nameof(IsItemsHost),
                typeof(bool),
                typeof(Panel),
                new PropertyMetadata(false));

        /// <summary>
        /// Gets a value that indicates whether this <see cref="Panel"/> is a container
        /// for UI items that are generated by an <see cref="ItemsControl"/>.
        /// </summary>
        public bool IsItemsHost
        {
            get { return (bool)this.GetValue(IsItemsHostProperty); }
            internal set { this.SetValue(IsItemsHostProperty, value); }
        }
    }
}
