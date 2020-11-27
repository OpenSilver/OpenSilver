

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
using System.Windows.Markup;

#if MIGRATION
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a control with a single piece of content. Controls such as Button,
    /// CheckBox, and ScrollViewer directly or indirectly inherit from this class.
    /// </summary>
    [ContentProperty("Content")]
    public partial class ContentControl : Control
    {
#region Constructor

        public ContentControl()
        {
            this.DefaultStyleKey = typeof(ContentControl);
        }

#endregion Constructor

#region Dependency Properties

        /// <summary>
        /// Gets or sets the content of a ContentControl.
        /// </summary>
        public object Content
        {
            get { return this.GetValue(ContentProperty); }
            set { this.SetValue(ContentProperty, value); }
        }

        /// <summary>
        /// Identifies the Content dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                nameof(Content),
                typeof(object),
                typeof(ContentControl),
                new PropertyMetadata(null, OnContentChanged));

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ContentControl)d).OnContentChanged(e.OldValue, e.NewValue);
        }

        /// <summary>
        /// Gets or sets the data template that is used to display the content of the
        /// ContentControl.
        /// </summary>
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)this.GetValue(ContentTemplateProperty); }
            set { this.SetValue(ContentTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the ContentTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register(
                nameof(ContentTemplate),
                typeof(DataTemplate),
                typeof(ContentControl),
                new PropertyMetadata((object)null));

#endregion Dependency Properties

#region Protected Methods

        protected virtual void OnContentChanged(object oldContent, object newContent)
        {
            // Remove the old content child
            this.RemoveLogicalChild(oldContent);

            if (this.ContentIsNotLogical)
            {
                return;
            }

            // We want to update the logical parent only if we don't have one already.
            FrameworkElement fe = newContent as FrameworkElement;
            if (fe != null)
            {
                DependencyObject logicalParent = fe.Parent;
                if (logicalParent != null)
                {
                    return;
                }
            }

            this.AddLogicalChild(newContent);
        }

#endregion Protected Methods

#region Internal Properties

        /// <summary>
        ///    Indicates whether Content should be a logical child or not.
        /// </summary>
        internal bool ContentIsNotLogical
        {
            get;
            set;
        }

        /// <summary>
        /// Returns enumerator to logical children
        /// </summary>
        /*protected*/ internal override IEnumerator LogicalChildren
        {
            get
            {
                object content = Content;
                
                if (ContentIsNotLogical || content == null)
                {
                    return EmptyEnumerator.Instance;
                }

                // If the current ContentControl is in a Template.VisualTree and is meant to host
                // the content for the container then that content shows up as the logical child
                // for the container and not for the current ContentControl.
                FrameworkElement fe = content as FrameworkElement;
                if (fe != null)
                {
                    DependencyObject logicalParent = fe.Parent;
                    if (logicalParent != null && logicalParent != this)
                    {
                        return EmptyEnumerator.Instance;
                    }
                }

                return new ContentModelTreeEnumerator(this, content);
            }
        }

#endregion Internal Properties

#region Internal Methods

        /// <summary>
        /// Prepare to display the item.
        /// </summary>
        internal void PrepareContentControl(object item, DataTemplate template)
        {
            if (item != this)
            {
                // don't treat Content as a logical child
                this.ContentIsNotLogical = true;

                this.ContentTemplate = template;
                this.Content = item;
            }
            else
            {
                this.ContentIsNotLogical = false;
            }
        }

        internal void ClearContentControl(object item)
        {
            if (this != item)
            {
                this.ClearValue(ContentProperty);
            }
        }

#endregion Internal Methods

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();

            if (!this.HasTemplate && this.TemplateChild == null)
            {
                // If we have no template we have to create a ContentPresenter
                // manually and attach it to this control.
                // This can happen for instance if a class derive from
                // ContentControl and specify a DefaultStyleKey and the associated
                // default style does not contain a Setter for the Template
                // property, or if we are not able to find a style for the
                // given key.
                // We need to set this ContentPresenter so that if we move from 
                // no template to a template, the "manually generated" template
                // will be detached as expected.
                // Note: this is a Silverlight specific behavior.
                // In WPF the content of the ContentControl would simply not be
                // displayed in this scenario.
                ContentPresenter presenter = new ContentPresenter();
                BindingOperations.SetBinding(presenter, ContentPresenter.ContentTemplateProperty, new Binding("ContentTemplate") { Source = this });
                BindingOperations.SetBinding(presenter, ContentPresenter.ContentProperty, new Binding("Content") { Source = this });
                this.TemplateChild = presenter;
            }
        }
    }
}
