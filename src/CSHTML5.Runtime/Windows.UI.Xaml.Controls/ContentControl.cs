
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
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
        FrameworkElement _dataTemplateRenderedContent;

        /// <summary>
        /// Gets or sets the content of a ContentControl.
        /// </summary>
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
        /// <summary>
        /// Identifies the Content dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(ContentControl), new PropertyMetadata(null, Content_Changed));

        static internal void Content_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var contentControl = (ContentControl)d;

            //we may want to throw the event here instead of in OnContentChanged since we throw the event every time and OnContentChanged can be overriden.
            if (!contentControl.HasTemplate)
            {
                contentControl.OnContentChanged(e.OldValue, e.NewValue);
            }
            //else, it should be directly handled by a Binding in the template.
        }

        /// <summary>
        /// Gets or sets the data template that is used to display the content of the
        /// ContentControl.
        /// </summary>
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }
        /// <summary>
        /// Identifies the ContentTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(ContentControl), new PropertyMetadata(null));


        //static void ContentTemplate_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var contentControl = (ContentControl)d;
        //    object newText = e.NewValue;

        //    //todo: apply the new template to the content (if the content is set)

        //    //if (INTERNAL_VisualTreeManager.IsElementInVisualTree(contentControl))
        //    //    apply template
        //}

      

        /// <summary>
        /// Invoked when the value of the Content property changes.
        /// </summary>
        /// <param name="oldContent">The old value of the Content property.</param>
        /// <param name="newContent">The new value of the Content property.</param>
        protected virtual void OnContentChanged(object oldContent, object newContent)
        {
            if (!this.HasTemplate)
            {

                //-----------------------------
                // DETACH PREVIOUS CONTENT
                //-----------------------------
                if (oldContent is UIElement)
                {
                    INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull((UIElement)oldContent, this);
                }
                if (_dataTemplateRenderedContent != null)
                {
                    INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(_dataTemplateRenderedContent, this);
                }
                _dataTemplateRenderedContent = null;

                //-----------------------------
                // ATTACH NEW CONTENT
                //-----------------------------


                if (newContent is UIElement)
                {
                    INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached((UIElement)newContent, this);
                }
                else if (INTERNAL_VisualTreeManager.IsElementInVisualTree(this))
                {
                    if (ContentTemplate != null)
                    {
                        // Apply the data template:
                        _dataTemplateRenderedContent = ContentTemplate.INTERNAL_InstantiateFrameworkTemplate();
                        _dataTemplateRenderedContent.DataContext = newContent;
                        INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(_dataTemplateRenderedContent, this);
                    }
                    else if (newContent != null)
                    {
                        // Show the string:
                        INTERNAL_HtmlDomManager.SetContentString(this, newContent.ToString(), removeTextWrapping: true);
                    }
                }
            }
        }

    }
}
