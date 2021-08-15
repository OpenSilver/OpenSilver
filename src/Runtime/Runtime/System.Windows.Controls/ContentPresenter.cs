﻿

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
using System;
using System.Windows.Markup;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

#if MIGRATION
using System.Windows.Data;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Displays the content of a <see cref="ContentControl"/>.
    /// </summary>
    [ContentProperty("Content")]
    public partial class ContentPresenter : FrameworkElement
    {
        #region Data

        private DataTemplate _templateCache;

        private static readonly DataTemplate _defaultTemplate;
        private static readonly DataTemplate _uiElementTemplate;

        #endregion Data

        #region Constructor

        static ContentPresenter()
        {
            // Default template
            DataTemplate template = new DataTemplate();
            template._methodToInstantiateFrameworkTemplate = owner =>
            {
                TemplateInstance templateInstance = new TemplateInstance();

                TextBlock textBlock = new TextBlock();
                textBlock.SetBinding(TextBlock.TextProperty, new Binding(""));

                templateInstance.TemplateContent = textBlock;

                return templateInstance;
            };
            template.Seal();
            _defaultTemplate = template;

            // Default template when content is UIElement.
            template = new UseContentTemplate();
            template.Seal();
            _uiElementTemplate = template;
        }

        public ContentPresenter()
        {

        }

        #endregion Constructor

        #region Dependency Properties

        /// <summary>
        /// Identifies the Content dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                nameof(Content),
                typeof(object),
                typeof(ContentPresenter),
                new PropertyMetadata(null, OnContentChanged));

        /// <summary>
        /// Gets or sets the data that is used to generate the child elements of a <see cref="ContentPresenter"/>.
        /// </summary>
        /// <returns>
        /// The data that is used to generate the child elements. The default is null.
        /// </returns>
        public object Content
        {
            get { return this.GetValue(ContentProperty); }
            set { this.SetValue(ContentProperty, value); }
        }

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContentPresenter cp = (ContentPresenter)d;
            bool reevaluateTemplate;

            if (cp.ContentTemplate != null)
            {
                reevaluateTemplate = false; // explicit template - do not re-apply
            }
            else if (cp.Template == UIElementContentTemplate)
            {
                reevaluateTemplate = true; // direct template - always re-apply
                cp.Template = null; // and release the old content so it can be re-used elsewhere
            }
            else
            {
                Debug.Assert(cp.Template == null ||
                             cp.Template == DefaultContentTemplate);
                reevaluateTemplate = true; // default template - always re-apply
            }

            if (e.NewValue is UIElement)
            {
                // If we're using the content directly, clear the data context.
                // The content expects to inherit.
                cp.ClearValue(DataContextProperty);
            }
            else
            {
                // set data context to the content, so that the template can bind to
                // properties of the content.
                cp.DataContext = e.NewValue;
            }

            if (reevaluateTemplate)
            {
                cp.ReevaluateTemplate();
            }
        }

        /// <summary>
        /// Identifies the ContentTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register(
                nameof(ContentTemplate),
                typeof(DataTemplate),
                typeof(ContentPresenter),
                new PropertyMetadata(null, OnContentTemplateChanged));

        /// <summary>
        /// Gets or sets the template that is used to display the content of the control.
        /// </summary>
        /// <returns>
        /// A <see cref="DataTemplate"/> that defines the visualization of the content.
        /// The default is null.
        /// </returns>
        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)this.GetValue(ContentTemplateProperty); }
            set { this.SetValue(ContentTemplateProperty, value); }
        }

        private static void OnContentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContentPresenter cp = (ContentPresenter)d;
            cp.ReevaluateTemplate();
        }

        /// <summary>
        /// TemplateProperty
        /// </summary>
        internal static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register(
                nameof(Template),
                typeof(DataTemplate),
                typeof(ContentPresenter),
                new PropertyMetadata(null, OnTemplateChanged));

        /// <summary>
        /// Template Property
        /// </summary>
        private DataTemplate Template
        {
            get { return this._templateCache; }
            set { this.SetValue(TemplateProperty, value); }
        }

        // Property invalidation callback invoked when TemplateProperty is invalidated
        private static void OnTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContentPresenter cp = (ContentPresenter)d;
            FrameworkElement.UpdateTemplateCache(cp, (FrameworkTemplate)e.OldValue, (FrameworkTemplate)e.NewValue, TemplateProperty);

            if (VisualTreeHelper.GetParent(cp) != null)
            {
                cp.InvalidateMeasureInternal();
            }
        }

        #endregion Dependency Properties

        #region Internal Properties

        // Internal Helper so the FrameworkElement could see this property
        internal override FrameworkTemplate TemplateInternal
        {
            get { return Template; }
        }

        // Internal Helper so the FrameworkElement could see the template cache
        internal override FrameworkTemplate TemplateCache
        {
            get { return _templateCache; }
            set { _templateCache = (DataTemplate)value; }
        }

        internal static DataTemplate DefaultContentTemplate
        {
            get
            {
                return _defaultTemplate;
            }
        }

        internal static DataTemplate UIElementContentTemplate
        {
            get
            {
                return _uiElementTemplate;
            }
        }

        #endregion Internal Properties

        #region Private Methods

        /// <summary>
        /// Return the template to use.  This may depend on the Content, or
        /// other properties.
        /// </summary>
        /// <remarks>
        /// The base class implements the following rules:
        ///   (a) If ContentTemplate is set, use it.
        ///   (b) Look for a DataTemplate whose DataType matches the
        ///         Content among the resources known to the ContentPresenter
        ///         (including application, theme, and system resources).
        ///         If one is found, use it.
        ///   (c) If the type of Content is "common", use a standard template.
        ///         The common types are String, UIElement.
        ///   (d) Otherwise, use a default template that essentially converts
        ///         Content to a string and displays it in a TextBlock.
        /// </remarks>
        private DataTemplate ChooseTemplate()
        {
            object content = this.Content;
            DataTemplate template = this.ContentTemplate;

            // default templates
            if (template == null)
            {
                if (content is UIElement)
                {
                    template = UIElementContentTemplate;
                }
                else
                {
                    template = DefaultContentTemplate;
                }
            }

            return template;
        }

        private void ReevaluateTemplate()
        {
            DataTemplate newTemplate = this.ChooseTemplate();

            if (this.Template != newTemplate)
            {
                this.Template = newTemplate;
            }
        }

        internal void PrepareContentPresenter(object item, DataTemplate template)
        {
            if (item != this)
            {
                this.ContentTemplate = template;
                this.Content = item;
            }
        }

        internal void ClearContentPresenter(object item)
        {
            if (this != item)
            {
                this.ClearValue(ContentProperty);
            }
        }

        #endregion Internal Methods

        #region Private classes

        private class UseContentTemplate : DataTemplate
        {
            public UseContentTemplate()
            {
                this._methodToInstantiateFrameworkTemplate = owner =>
                {
                    TemplateInstance template = new TemplateInstance();

                    FrameworkElement root = ((ContentPresenter)owner).Content as FrameworkElement;

                    template.TemplateContent = root;

                    return template;
                };
            }
        }

        #endregion Private classes

        protected override Size MeasureOverride(Size availableSize)
        {
            IEnumerable<DependencyObject> childElements = VisualTreeHelper.GetVisualChildren(this);
            if (childElements.Count() > 0)
            {
                UIElement elementChild = ((UIElement)childElements.ElementAt(0));
                elementChild.Measure(availableSize);
                return elementChild.DesiredSize;
            }

            if (this.Content == null)
                return new Size();

            Size actualSize = new Size(Double.IsNaN(Width) ? ActualWidth : Width, Double.IsNaN(Height) ? ActualHeight : Height);
            return actualSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            IEnumerable<DependencyObject> childElements = VisualTreeHelper.GetVisualChildren(this);
            if (childElements.Count() > 0)
            {
                UIElement elementChild = ((UIElement)childElements.ElementAt(0));
                elementChild.Arrange(new Rect(finalSize));
            }
            return finalSize;
        }
    }
}

