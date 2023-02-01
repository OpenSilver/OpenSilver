

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
using System.Collections.Generic;
using CSHTML5.Internal;
using DotNetForHtml5.Core;

#if MIGRATION
using System.Windows.Input;
#else
using Windows.Foundation;
using Windows.UI.Xaml.Input;
#endif

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    internal sealed class PopupRoot : FrameworkElement
    {
        /// <summary>
        /// Returns the Visual children count.
        /// </summary>
        internal override int VisualChildrenCount
        {
            get
            {
                if (Content == null)
                {
                    return 0;
                }

                return 1;
            }
        }

        /// <summary>
        /// Returns the child at the specified index.
        /// </summary>
        internal override UIElement GetVisualChild(int index)
        {
            if (Content == null || index != 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return Content;
        }

        internal string INTERNAL_UniqueIndentifier { get; set; }

        internal Popup INTERNAL_LinkedPopup { get; set; }

        internal PopupRoot(string uniqueIdentifier, Window parentWindow, Popup popup)
        {
            INTERNAL_UniqueIndentifier = uniqueIdentifier;
            INTERNAL_ParentWindow = parentWindow;
            INTERNAL_LinkedPopup = popup;

            // Make sure that after the Loaded event of the PopupRoot, the parent Popup also raises the Loaded event:
            this.Loaded += PopupRoot_Loaded;
        }

        private void PopupRoot_Loaded(object sender, RoutedEventArgs e)
        {
            // Make sure that the Loaded event of the Popup is raised (this is useful if the <Popup> control is never added to the visual tree, such as for tooltips).
            if (this.INTERNAL_LinkedPopup != null
                && !this.INTERNAL_LinkedPopup.IsConnectedToLiveTree) // We check that the <Popup> has no visual parent. In fact, if it had a visual parent, it means that it is in the visual tree (for example if the <Popup> was declared in XAML), and therefore the Loaded event has already been called once.
            {
                this.INTERNAL_LinkedPopup.RaiseLoadedEvent();
                this.INTERNAL_LinkedPopup.InvalidateMeasure();
            }
            if (INTERNAL_ParentWindow != null && this.IsCustomLayoutRoot)
            {
                Rect windowBounds = INTERNAL_ParentWindow.Bounds;
                this.Measure(new Size(windowBounds.Width, windowBounds.Height));
                this.Arrange(windowBounds);
            }
        }

        /// <summary>
        /// Gets or sets the visual root of a popup
        /// </summary>
        public UIElement Content
        {
            get { return (UIElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        /// <summary>
        /// Identifies the PopupRoot.Content dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(UIElement), typeof(PopupRoot), new PropertyMetadata(null, Content_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });


        static void Content_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement parent = (UIElement)d;
            UIElement oldChild = (UIElement)e.OldValue;
            UIElement newChild = (UIElement)e.NewValue;

            INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(oldChild, parent);
            parent.RemoveVisualChild(oldChild);
            parent.AddVisualChild(newChild);
            INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(newChild, parent);
        }

#if MIGRATION
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
#else
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
#endif
        {
#if MIGRATION
            base.OnMouseLeftButtonDown(e);
#else
            base.OnPointerPressed(e);
#endif

            // Note: If a popup has StayOpen=True, the value of "StayOpen" of its parents is ignored.
            // In other words, the parents of a popup that has StayOpen=True will always stay open
            // regardless of the value of their "StayOpen" property.

            HashSet<Popup> listOfPopupThatMustBeClosed = new HashSet<Popup>();
            List<PopupRoot> popupRootList = new List<PopupRoot>();

            foreach (object obj in INTERNAL_PopupsManager.GetAllRootUIElements())
            {
                if (obj is PopupRoot)
                {
                    PopupRoot root = (PopupRoot)obj;
                    popupRootList.Add(root);

                    if (root.INTERNAL_LinkedPopup != null)
                        listOfPopupThatMustBeClosed.Add(root.INTERNAL_LinkedPopup);
                }
            }

            // We determine which popup needs to stay open after this click
            foreach (PopupRoot popupRoot in popupRootList)
            {
                if (popupRoot.INTERNAL_LinkedPopup != null)
                {
                    // We must prevent all the parents of a popup to be closed when:
                    // - this popup is set to StayOpen
                    // - or the click happend in this popup

                    Popup popup = popupRoot.INTERNAL_LinkedPopup;

                    if (popup.StayOpen)
                    {
                        do
                        {
                            if (!listOfPopupThatMustBeClosed.Contains(popup))
                                break;

                            listOfPopupThatMustBeClosed.Remove(popup);

                            popup = popup.ParentPopup;

                        } while (popup != null);
                    }
                }
            }

            foreach (Popup popup in listOfPopupThatMustBeClosed)
            {
                var args = new OutsideClickEventArgs();
                popup.OnOutsideClick(args);
                if (!args.Handled)
                {
                    popup.CloseFromAnOutsideClick();
                }
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.Content == null || this.INTERNAL_ParentWindow == null)
                return availableSize;

            Rect windowBounds = INTERNAL_ParentWindow.Bounds;
            availableSize = new Size(windowBounds.Width, windowBounds.Height);
            this.Content.Measure(availableSize);
            return availableSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.Content == null)
                return finalSize;

            Rect windowBounds = INTERNAL_ParentWindow.Bounds;
            finalSize = new Size(windowBounds.Width, windowBounds.Height);
            this.Content.Arrange(new Rect(finalSize));
            return finalSize;
        }
    }
}
