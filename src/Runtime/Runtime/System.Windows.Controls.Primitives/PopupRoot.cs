

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
using CSHTML5.Internal;

#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    internal partial class PopupRoot : FrameworkElement
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

        internal PopupRoot(string uniqueIdentifier, Window parentWindow)
        {
            INTERNAL_UniqueIndentifier = uniqueIdentifier;
            INTERNAL_ParentWindow = parentWindow;

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
