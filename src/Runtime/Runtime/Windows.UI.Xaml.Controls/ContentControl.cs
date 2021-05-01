

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

        }

        #endregion Constructor

        #region Public Properties

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
                "Content",
                typeof(object),
                typeof(ContentControl),
                new PropertyMetadata(null, OnContentChanged));

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
                "ContentTemplate",
                typeof(DataTemplate),
                typeof(ContentControl),
                new PropertyMetadata(null, OnContentTemplateChanged));

        #endregion Public Properties

        #region Public Methods

        protected virtual void OnContentChanged(object oldContent, object newContent)
        {
            // Remove the old content child
            this.RemoveLogicalChild(oldContent);

            this.OnContentChangedInternal(oldContent, newContent);

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

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();

            if (this.HasTemplate)
            {
                return;
            }
            this.ApplyContentTemplate(this.ContentTemplate);
        }

        #endregion Public Methods

        #region Internal API

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

        private UIElement Child
        {
            get;
            set;
        }

        // Note: This is virtual so it can be overriden by ContentPresenter.
        // Remove virtual once ContentPresenter has a proper implementation.
        internal virtual bool ContentIsNotLogical
        {
            get;
            set;
        }

        internal void OnContentChangedInternal(object oldContent, object newContent)
        {
            if (!this.IsLoaded) // change will be handle when attached to Visual Tree in INTERNAL_OnAttachedToVisualTree().
            {
                return;
            }
            if (this.HasTemplate)
            {
                return;
            }
            if (this.ContentTemplate != null)
            {
                // note: At this point, we don't need to regenerate the DataTemplate because it is either
                // handle in OnContentTemplatePropertyChanged() or INTERNAL_OnAttachedToVisualTree().
                if (this.Child != null) // else it means that the ContentTemplate is empty, and there is nothing to do.
                {
                    ((FrameworkElement)this.Child).DataContext = newContent;
                }
            }
            else
            {
                this.ChangeChild(newContent);
            }
        }

        private void ApplyContentTemplate(DataTemplate template)
        {
            object newChild;
            if (template == null)
            {
                newChild = this.Content;
            }
            else
            {
                FrameworkElement generatedContent = template.INTERNAL_InstantiateFrameworkTemplate();
                if (generatedContent != null)
                {
                    generatedContent.DataContext = this.Content;
                }
                newChild = generatedContent;
            }
            this.ChangeChild(newChild);
        }

        private void ChangeChild(object newChild)
        {
            this.DetachChild(); // Detach current child.

            UIElement newChildAsUIElement = newChild as UIElement;
            if(newChildAsUIElement != null)
            {
                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(newChildAsUIElement, this);
                this.Child = newChildAsUIElement;
            }
            else
            {
                string contentAsString = newChild == null ? string.Empty : newChild.ToString();
                INTERNAL_HtmlDomManager.SetContentString(this, contentAsString, removeTextWrapping: true);
                this.Child = this; // In the case where the child is not an UIElement, we consider the child to be this Control because we don't add a child (we directly set the content of this element).
            }
        }

        private void DetachChild()
        {
            if (object.ReferenceEquals(this, this.Child)) // if ContentTemplate is null and Content is not an UIElement
            {
                INTERNAL_HtmlDomManager.SetContentString(this, string.Empty, removeTextWrapping: true);
            }
            else
            {
                INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(this.Child, this);
            }
            this.Child = null;
        }

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //we may want to throw the event here instead of in OnContentChanged since we throw the event every time and OnContentChanged can be overriden.
            ((ContentControl)d).OnContentChanged(e.OldValue, e.NewValue);
            //else, it should be directly handled by a Binding in the template.
        }

        private static void OnContentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContentControl contentControl = (ContentControl)d;
            if (!contentControl.IsLoaded) // change will be handle when attached to Visual Tree in INTERNAL_OnAttachedToVisualTree().
            {
                return;
            }
            if (contentControl.HasTemplate)
            {
                return;
            }
            contentControl.ApplyContentTemplate((DataTemplate)e.NewValue);
        }

        #endregion Internal API
    }
}
