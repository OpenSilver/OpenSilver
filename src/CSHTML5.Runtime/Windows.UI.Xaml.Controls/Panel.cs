
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
using System.Windows.Controls;
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
    public abstract class Panel : FrameworkElement
    {
        UIElementCollection _children;

        void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
#if PERFSTAT
            var t0 = Performance.now();
#endif

            UIElementCollection oldItems = null;
            UIElementCollection newItems = null;
            var list = e.NewItems;
            if (list != null)
            {
                newItems = new UIElementCollection();
                foreach (UIElement uiElement in list)
                {
                    newItems.Add(uiElement);
                }
            }

            list = e.OldItems;
            if (list != null)
            {
                oldItems = new UIElementCollection();
                foreach (UIElement uiElement in list)
                {
                    oldItems.Add(uiElement);
                }
            }

#if PERFSTAT
            Performance.Counter("Panel.Children_CollectionChanged without manage children", t0);
#endif

            //we put "this" below because the sender is directly the colection and we need to have an access to the control
            ManageChildrenChanged(this, oldItems, newItems);
        }

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
            DependencyProperty.Register("Background", typeof(Brush), typeof(Panel), new PropertyMetadata(null)
            {
                GetCSSEquivalent = (instance) =>
                {
                    return new CSSEquivalent()
                    {
                        Name = new List<string> { "background", "backgroundColor", "backgroundColorAlpha" },
                    };
                }
            }
            );

        /// <summary>
        /// Gets the collection of child elements of the panel.
        /// </summary>
        public UIElementCollection Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new UIElementCollection();

                    if (this._isLoaded)
                        _children.CollectionChanged += Children_CollectionChanged;
                }

                return _children;
            }
        }


        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            if (_children != null)
            {
                _children.CollectionChanged -= Children_CollectionChanged;
                _children.CollectionChanged += Children_CollectionChanged;
            }

            ManageChildrenChanged(this, _children, _children);
        }

        protected internal override void INTERNAL_OnDetachedFromVisualTree()
        {
            if (_children != null)
                _children.CollectionChanged -= Children_CollectionChanged;
        }

        static void ManageChildrenChanged(DependencyObject d, UIElementCollection oldChildren, UIElementCollection newChildren)
        {
#if PERFSTAT
            var t1 = Performance.now();
#endif

            Panel parent = (Panel)d;
            if (parent is DockPanel)
            {
                ((DockPanel)parent).ManageChildrenChanged(oldChildren, newChildren);
            }
            else
            {
                bool isCSSGrid = Grid_InternalHelpers.isCSSGridSupported();
                if (!isCSSGrid && parent is Grid)
                {
                    ((Grid)parent).ManageChildrenChanged(oldChildren, newChildren);
                }
                else
                {
                    if (oldChildren != null)
                    {
                        //// Put the list in a HashSet for performant lookup:
                        //HashSet<UIElement> newChidrenHashSet = new HashSet<UIElement>();
                        //if (newChildren != null)
                        //{
                        //    foreach (UIElement child in newChildren)
                        //        newChidrenHashSet.Add(child);
                        //}
                        //// Detach old children only if they are not in the "newChildren" collection:
                        //foreach (UIElement child in oldChildren)
                        //{
                        //    if (newChildren == null || !newChidrenHashSet.Contains(child)) //todo: verify that in the produced JavaScript, "newChidrenHashSet.Contains" has still a O(1) complexity.
                        //    {
                        //        INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(child, parent);
                        //    }
                        //}

                        //todo: use HashSet version.

                        // Detach old children only if they are not in the "newChildren" collection:
                        foreach (UIElement child in oldChildren) //note: there is no setter for Children so the user cannot change the order of the elements in one step --> we cannot have the same children in another order (which would keep the former order with the way it is handled now) --> no problem here
                        {
#if PERFSTAT
                    var t2 = Performance.now();
#endif
                            if (newChildren == null || !newChildren.Contains(child))
                            {
#if PERFSTAT
                        Performance.Counter("Panel.ManageChildrenChanged 'Contains'", t2);
#endif
                                INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(child, parent);
                            }
                            else
                            {
#if PERFSTAT
                        Performance.Counter("Panel.ManageChildrenChanged 'Contains'", t2);
#endif
                            }
                        }
                    }
                    if (newChildren != null)
                    {
                        foreach (UIElement child in newChildren)
                        {
                            // Note: we do this for all items (regardless of whether they are in the oldChildren collection or not) to make it work when the item is first added to the Visual Tree (at that moment, all the properties are refreshed by calling their "Changed" method).
                            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(child, parent);
                        }
                    }

                    if (parent is Grid)
                    {
                        ((Grid)parent).LocallyManageChildrenChanged();
                    }
                }
            }
        }


        //internal override void INTERNAL_Render()
        //{
        //    base.INTERNAL_Render();

        //    if (Background is SolidColorBrush)
        //    {
        //        INTERNAL_HtmlDomManager.GetFrameworkElementOuterStyleForModification(this).backgroundColor = ((SolidColorBrush)Background).Color.INTERNAL_ToHtmlString();
        //    }
        //}
    }
}
