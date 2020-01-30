
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class ContentControl : Control
    {
        #region Constructor
        public ContentControl()
        {

        }
        #endregion

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
        public static readonly DependencyProperty ContentProperty = DependencyProperty.Register("Content", 
                                                                                                typeof(object), 
                                                                                                typeof(ContentControl), 
                                                                                                new PropertyMetadata(null, OnContentPropertyChanged) 
                                                                                                { 
                                                                                                    CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never 
                                                                                                });

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
        public static readonly DependencyProperty ContentTemplateProperty = DependencyProperty.Register("ContentTemplate", 
                                                                                                        typeof(DataTemplate), 
                                                                                                        typeof(ContentControl), 
                                                                                                        new PropertyMetadata(null, OnContentTemplatePropertyChanged) 
                                                                                                        { 
                                                                                                            CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never 
                                                                                                        });
        #endregion

        #region Public Methods
        protected virtual void OnContentChanged(object oldContent, object newContent)
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
                // note: At this point, we don't need to regenerate the DataTemplate because it is either handle in OnContentTemplatePropertyChanged() or INTERNAL_OnAttachedToVisualTree().
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

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            if (this.HasTemplate)
            {
                return;
            }
            this.ApplyContentTemplate(this.ContentTemplate);
        }
        #endregion

        #region Internal API
        private UIElement Child { get; set; }

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

        private static void OnContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //we may want to throw the event here instead of in OnContentChanged since we throw the event every time and OnContentChanged can be overriden.
            ((ContentControl)d).OnContentChanged(e.OldValue, e.NewValue);
            //else, it should be directly handled by a Binding in the template.
        }

        private static void OnContentTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
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
        #endregion
    }
}
