
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
    [ContentProperty(nameof(Content))]
    public partial class ContentPresenter : FrameworkElement
    {
        private DataTemplate _templateCache;
        private bool _templateIsCurrent;

        static ContentPresenter()
        {
            // Default template
            DataTemplate template = new DefaultTemplate();
            template.Seal();
            DefaultContentTemplate = template;

            // Default template when content is UIElement.
            template = new UseContentTemplate();
            template.Seal();
            UIElementContentTemplate = template;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentPresenter"/> class.
        /// </summary>
        public ContentPresenter()
        {
        }

        /// <summary>
        /// Identifies the <see cref="Content"/> dependency property
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
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContentPresenter ctrl = (ContentPresenter)d;

            // if we're already marked to reselect the template, there's nothing more to do
            if (!ctrl._templateIsCurrent)
                return;

            bool mismatch;

            if (ctrl.ContentTemplate != null)
            {
                mismatch = false; // explicit template - do not re-apply
            }
            else if (ctrl.Template == UIElementContentTemplate)
            {
                mismatch = true; // direct template - always re-apply
                ctrl.Template = null; // and release the old content so it can be re-used elsewhere
            }
            else if (ctrl.Template == DefaultContentTemplate)
            {
                mismatch = true; // default template - always re-apply
            }
            else
            {
                // implicit template - re-apply if content type changed
                Type oldDataType = e.OldValue?.GetType();
                Type newDataType = e.NewValue?.GetType();

                mismatch = (oldDataType != newDataType);
            }

            // if the content and (old) template don't match, reselect the template
            if (mismatch)
            {
                ctrl._templateIsCurrent = false;
            }

            // keep the DataContext in sync with Content
            if (ctrl._templateIsCurrent && ctrl.Template != UIElementContentTemplate)
            {
                ctrl.DataContext = e.NewValue;
            }

            if (ctrl.IsConnectedToLiveTree)
            {
                ctrl.InvalidateMeasureInternal();
            }
        }

        /// <summary>
        /// Identifies the <see cref="ContentTemplate"/> dependency
        /// property.
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
            get { return (DataTemplate)GetValue(ContentTemplateProperty); }
            set { SetValue(ContentTemplateProperty, value); }
        }

        private static void OnContentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContentPresenter ctrl = (ContentPresenter)d;
            ctrl._templateIsCurrent = false;

            // if ContentTemplate is really changing, remove the old template
            ctrl.Template = null;

            if (ctrl.IsConnectedToLiveTree)
            {
                ctrl.InvalidateMeasureInternal();
            }
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
            get { return _templateCache; }
            set { SetValue(TemplateProperty, value); }
        }

        // Property invalidation callback invoked when TemplateProperty is invalidated
        private static void OnTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContentPresenter cp = (ContentPresenter)d;
            UpdateTemplateCache(cp, (FrameworkTemplate)e.OldValue, (FrameworkTemplate)e.NewValue, TemplateProperty);

            if (cp.IsConnectedToLiveTree)
            {
                cp.InvalidateMeasureInternal();
            }
        }

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

        internal static DataTemplate DefaultContentTemplate { get; }

        internal static DataTemplate UIElementContentTemplate { get; }

        internal override void OnPreApplyTemplate()
        {
            base.OnPreApplyTemplate();

            if (!_templateIsCurrent)
            {
                EnsureTemplate();
                _templateIsCurrent = true;
            }
        }

        private void EnsureTemplate()
        {
            DataTemplate oldTemplate = Template;
            DataTemplate newTemplate = null;

            for (_templateIsCurrent = false; !_templateIsCurrent;)
            {
                // normally this loop will execute exactly once.  The only exception
                // is when setting the DataContext causes the ContentTemplate or
                // ContentTemplateSelector to change, presumably because they are
                // themselves data-bound (see bug 128119).  In that case, we need
                // to call ChooseTemplate again, to pick up the new template.
                // We detect this case because _templateIsCurrent is reset to false
                // in OnContentTemplate[Selector]Changed, causing a second iteration
                // of the loop.
                _templateIsCurrent = true;
                newTemplate = ChooseTemplate();

                // if the template is changing, it's important that the code that cleans
                // up the old template runs while the CP's DataContext is still set to
                // the old Content.  The way to get this effect is:
                //      a. change the template to null
                //      b. change the data context
                //      c. change the template to the new value

                if (oldTemplate != newTemplate)
                {
                    Template = null;
                }

                if (newTemplate != UIElementContentTemplate)
                {
                    // set data context to the content, so that the template can bind to
                    // properties of the content.
                    DataContext = Content;
                }
                else
                {
                    // If we're using the content directly, clear the data context.
                    // The content expects to inherit.
                    ClearValue(DataContextProperty);
                }
            }

            Template = newTemplate;
        }

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
            DataTemplate template = null;
            object content = Content;

            // ContentTemplate has first stab
            template = ContentTemplate;

            // no ContentTemplate set, try the default templates
            if (template == null)
            {
                // Lookup template for typeof(Content) in resource dictionaries.
                if (content != null)
                {
                    template = (DataTemplate)FindTemplateResourceInternal(this, content);
                }

                // default templates for well known types
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
            }

            return template;
        }

        //  Searches through resource dictionaries to find a DataTemplate
        //  that matches the type of the 'item' parameter.  Failing an exact
        //  match of the type, return something that matches one of its parent
        //  types.
        internal static object FindTemplateResourceInternal(DependencyObject target, object item)
        {
            // Data styling doesn't apply to UIElement.
            if (item == null || (item is UIElement))
            {
                return null;
            }

            Type dataType = item.GetType();

            List<DataTemplateKey> keys = new List<DataTemplateKey>();

            // construct the list of acceptable keys, in priority ord
            int exactMatch = 1;    // number of entries that count as an exact match

            // add compound keys for the dataType and all its base types
            while (dataType != null)
            {
                keys.Add(new DataTemplateKey(dataType));

                dataType = dataType.BaseType;
                if (dataType == typeof(object)) // don't search for Object - perf (Note: Silverlight also includes object)
                {
                    dataType = null;
                }
            }

            int bestMatch = keys.Count; // index of best match so far

            // Search the parent chain
            object resource = FindTemplateResourceInTree(target, keys, exactMatch, ref bestMatch);

            if (bestMatch >= exactMatch)
            {
                // Exact match not found in the parent chain.  Try App Resources.
                object appResource = FindTemplateResourceFromApp(target, keys, exactMatch, ref bestMatch);

                if (appResource != null)
                    resource = appResource;
            }

            return resource;
        }

        // Find a data template resource
        private static object FindTemplateResourceFromApp(
            DependencyObject target,
            List<DataTemplateKey> keys,
            int exactMatch,
            ref int bestMatch)
        {
            object resource = null;
            int k;

            Application app = Application.Current;
            if (app != null)
            {
                // If the element is rooted to a Window and App exists, defer to App.
                for (k = 0; k < bestMatch; ++k)
                {
                    object appResource = app.FindResourceInternal(keys[k]);
                    if (appResource != null)
                    {
                        bestMatch = k;
                        resource = appResource;

                        if (bestMatch < exactMatch)
                            return resource;
                    }
                }
            }

            return resource;
        }

        // Search the parent chain for a DataTemplate in a ResourceDictionary.
        private static object FindTemplateResourceInTree(
            DependencyObject target,
            List<DataTemplateKey> keys,
            int exactMatch,
            ref int bestMatch)
        {
            Debug.Assert(target != null, "Don't call FindTemplateResource with a null target object");

            ResourceDictionary table;
            object resource = null;

            FrameworkElement fe = target as FrameworkElement;

            while (fe != null)
            {
                object candidate;

                // -------------------------------------------
                //  Lookup ResourceDictionary on the current instance
                // -------------------------------------------

                // Fetch the ResourceDictionary
                // for the given target element
                table = fe.HasResources ? fe.Resources : null;
                if (table != null)
                {
                    candidate = FindBestMatchInResourceDictionary(table, keys, exactMatch, ref bestMatch);
                    if (candidate != null)
                    {
                        resource = candidate;
                        if (bestMatch < exactMatch)
                        {
                            // Exact match found, stop here.
                            return resource;
                        }
                    }
                }

                // -------------------------------------------
                //  Find the next parent instance to lookup
                // -------------------------------------------

                // Get Framework Parent (priority to logical parent)
                fe = (fe.Parent ?? VisualTreeHelper.GetParent(fe)) as FrameworkElement;
            }

            return resource;
        }

        // Given a ResourceDictionary and a set of keys, try to find the best
        //  match in the resource dictionary.
        private static object FindBestMatchInResourceDictionary(
            ResourceDictionary table,
            List<DataTemplateKey> keys,
            int exactMatch,
            ref int bestMatch)
        {
            object resource = null;
            int k;

            // Search target element's ResourceDictionary for the resource
            if (table != null)
            {
                for (k = 0; k < bestMatch; ++k)
                {
                    object candidate = table[keys[k]];
                    if (candidate != null)
                    {
                        resource = candidate;
                        bestMatch = k;

                        // if we found an exact match, no need to continue
                        if (bestMatch < exactMatch)
                            return resource;
                    }
                }
            }

            return resource;
        }

        internal void PrepareContentPresenter(object item, DataTemplate template)
        {
            if (item != this)
            {
                ContentTemplate = template;
                Content = item;
            }
        }

        internal void ClearContentPresenter(object item)
        {
            if (this != item)
            {
                ClearValue(ContentProperty);
            }
        }

        private class UseContentTemplate : DataTemplate
        {
            internal override bool BuildVisualTree(FrameworkElement container)
            {
                FrameworkElement child = ((ContentPresenter)container).Content as FrameworkElement;
                if (child != null)
                {
                    FrameworkElement parent = VisualTreeHelper.GetParent(child) as FrameworkElement;
                    if (parent != null)
                    {
                        parent.TemplateChild = null;
                    }
                }

                container.TemplateChild = child;

                return true;
            }
        }

        private class DefaultTemplate : DataTemplate
        {
            internal override bool BuildVisualTree(FrameworkElement container)
            {
                ContentPresenter cp = (ContentPresenter)container;
                FrameworkElement result = DefaultExpansion(cp.Content, cp);

                container.TemplateChild = result;

                return result != null;
            }

            private FrameworkElement DefaultExpansion(object content, ContentPresenter container)
            {
                if (content == null)
                {
                    return null;
                }

                TextBlock textBlock = new TextBlock();
                textBlock.SetBinding(TextBlock.TextProperty, new Binding());
                textBlock.TemplatedParent = container;

                return textBlock;
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            IEnumerable<DependencyObject> childElements = VisualTreeHelper.GetVisualChildren(this);
            if (childElements.Count() > 0)
            {
                UIElement elementChild = ((UIElement)childElements.ElementAt(0));
                elementChild.Measure(availableSize);
                return elementChild.DesiredSize;
            }

            if (Content == null)
                return new Size();

            Size actualSize = new Size(double.IsNaN(Width) ? ActualWidth : Width, double.IsNaN(Height) ? ActualHeight : Height);
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

