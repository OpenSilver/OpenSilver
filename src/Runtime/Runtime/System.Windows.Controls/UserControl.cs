
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
using System.Windows.Markup;
using OpenSilver.Internal.Controls;
using OpenSilver.Internal.Xaml.Context;

#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Markup;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Provides the base class for defining a new control that encapsulates related
    /// existing controls and provides its own logic.
    /// </summary>
    [ContentProperty("Content")]
    public partial class UserControl : Control
    {
        /// <summary> 
        /// Returns enumerator to logical children.
        /// </summary>
        /*protected*/ internal override IEnumerator LogicalChildren
        {
            get
            {
                if (this.Content == null)
                {
                    return EmptyEnumerator.Instance;
                }

                // otherwise, its logical children is its visual children
                return new SingleChildEnumerator(this.Content);
            }
        }

#region Constructors

        public UserControl()
        {
            IsTabStop = false; //we want to avoid stopping on this element's div when pressing tab.
        }

#endregion Constructors

        /// <summary>
        /// Gets or sets the content that is contained within a user control.
        /// </summary>
        public UIElement Content
        {
            get { return (UIElement)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Identifies the Content dependency property
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                nameof(Content),
                typeof(UIElement),
                typeof(UserControl),
                new PropertyMetadata(null, OnContentChanged));

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UserControl uc = (UserControl)d;

            uc.TemplateChild = null;
            uc.RemoveLogicalChild(e.OldValue);
            uc.AddLogicalChild(e.NewValue);

            if (uc.IsConnectedToLiveTree)
            {
                uc.InvalidateMeasureInternal();
            }
        }

        /// <summary>
        /// Gets the element that should be used as the StateGroupRoot for VisualStateMangager.GoToState calls
        /// </summary>
        internal override FrameworkElement StateGroupsRoot
        {
            get
            {
                return Content as FrameworkElement;
            }
        }

        internal override FrameworkTemplate TemplateCache
        {
            get { return DefaultTemplate; }
            set { }
        }

        internal override FrameworkTemplate TemplateInternal
        {
            get { return DefaultTemplate; }
        }

        private static UseContentTemplate DefaultTemplate { get; } = new UseContentTemplate();

        private class UseContentTemplate : FrameworkTemplate
        {
            public UseContentTemplate()
            {
                Seal();
            }

            internal override bool BuildVisualTree(FrameworkElement container)
            {
                container.TemplateChild = ((UserControl)container).Content as FrameworkElement;
                return false;
            }
        }
    }
}
