
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

using OpenSilver.Internal;
using OpenSilver.Internal.Xaml.Context;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace System.Windows.Controls
{
    /// <summary>
    /// Displays the content of a <see cref="ContentControl"/>.
    /// </summary>
    [ContentProperty(nameof(Content))]
    public class ContentPresenter : FrameworkElement
    {
        private static readonly UncommonField<DataTemplate> StringFormattingTemplateField = new();
        private static readonly UncommonField<DataTemplate> AccessTextFormattingTemplateField = new();

        private DataTemplate _templateCache;
        private bool _templateIsCurrent;
        private bool _contentIsItem;

        static ContentPresenter()
        {
            // Default template for strings
            var template = new DataTemplate
            {
                Template = new CompiledTemplateContent(
                    new XamlContext(),
                    static (owner, context) =>
                    {
                        var text = new TextBlock();
                        text.SetTemplatedParent(context.TemplateOwnerReference);
                        text.SetBinding(TextBlock.TextProperty, Binding.Empty);
                        return text;
                    })
            };
            template.Seal();
            StringContentTemplate = template;

            // Default template for strings when hosted in ContentPresenter with RecognizesAccessKey=true
            template = new DataTemplate
            {
                Template = new CompiledTemplateContent(
                    new XamlContext(),
                    static (owner, context) =>
                    {
                        var text = new AccessText();
                        text.SetTemplatedParent(context.TemplateOwnerReference);
                        text.SetBinding(AccessText.TextProperty, Binding.Empty);
                        return text;
                    })
            };
            template.Seal();
            AccessTextContentTemplate = template;

            // Default template
            template = new DefaultTemplate();
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
        public ContentPresenter() { }

        /// <summary>
        /// Identifies the <see cref="Content"/> dependency property
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            ContentControl.ContentProperty.AddOwner(
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
            get => GetValue(ContentProperty);
            set => SetValueInternal(ContentProperty, value);
        }

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContentPresenter ctrl = (ContentPresenter)d;

            // if we're already marked to reselect the template, there's nothing more to do
            if (!ctrl._templateIsCurrent)
                return;

            bool mismatch;

            if (ctrl.ContentTemplate is not null)
            {
                mismatch = false; // explicit template - do not re-apply
            }
            else if (ctrl.ContentTemplateSelector is not null)
            {
                mismatch = true; // template selector - always re-select
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

                mismatch = oldDataType != newDataType;

                // but mismatch if we're displaying strings via a default template
                // and the presence of an AccessKey changes
                if (!mismatch &&
                    ctrl.RecognizesAccessKey &&
                    ReferenceEquals(typeof(string), newDataType) &&
                    ctrl.IsUsingDefaultStringTemplate)
                {
                    string oldString = (string)e.OldValue;
                    string newString = (string)e.NewValue;

                    bool oldHasAccessKey = oldString.IndexOf(AccessText.AccessKeyMarker) > -1;
                    bool newHasAccessKey = newString.IndexOf(AccessText.AccessKeyMarker) > -1;

                    if (oldHasAccessKey != newHasAccessKey)
                    {
                        mismatch = true;
                    }
                }
            }

            // if the content and (old) template don't match, reselect the template
            if (mismatch)
            {
                ctrl._templateIsCurrent = false;
            }

            // keep the DataContext in sync with Content
            if (ctrl._templateIsCurrent && ctrl.Template != UIElementContentTemplate)
            {
                ctrl.UpdateDataContext();
            }

            ctrl.InvalidateMeasure();
        }

        /// <summary>
        /// Identifies the <see cref="ContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty =
            ContentControl.ContentTemplateProperty.AddOwner(
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
            get => (DataTemplate)GetValue(ContentTemplateProperty);
            set => SetValueInternal(ContentTemplateProperty, value);
        }

        private static void OnContentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContentPresenter ctrl = (ContentPresenter)d;
            ctrl._templateIsCurrent = false;

            // if ContentTemplate is really changing, remove the old template
            ctrl.Template = null;

            ctrl.InvalidateMeasure();
        }

        /// <summary>
        /// Identifies the <see cref="ContentTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateSelectorProperty =
            ContentControl.ContentTemplateSelectorProperty.AddOwner(
                typeof(ContentPresenter),
                new PropertyMetadata(null, OnContentTemplateSelectorChanged));

        /// <summary>
        /// Gets or sets the <see cref="DataTemplateSelector"/>, which allows the application 
        /// writer to provide custom logic for choosing the template that is used to display 
        /// the content of the control.
        /// </summary>
        /// <returns>
        /// A <see cref="DataTemplateSelector"/> object that supplies logic to return a 
        /// <see cref="DataTemplate"/> to apply. The default is null.
        /// </returns>
        public DataTemplateSelector ContentTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(ContentControl.ContentTemplateSelectorProperty);
            set => SetValueInternal(ContentControl.ContentTemplateSelectorProperty, value);
        }

        private static void OnContentTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContentPresenter ctrl = (ContentPresenter)d;
            ctrl._templateIsCurrent = false;
            ctrl.OnContentTemplateSelectorChanged((DataTemplateSelector)e.OldValue, (DataTemplateSelector)e.NewValue);

            ctrl.InvalidateMeasure();
        }

        /// <summary>
        /// Invoked when the <see cref="ContentTemplateSelector"/> property changes.
        /// </summary>
        /// <param name="oldContentTemplateSelector">
        /// The old value of the <see cref="ContentTemplateSelector"/> property.
        /// </param>
        /// <param name="newContentTemplateSelector">
        /// The new value of the <see cref="ContentTemplateSelector"/> property.
        /// </param>
        protected virtual void OnContentTemplateSelectorChanged(DataTemplateSelector oldContentTemplateSelector, DataTemplateSelector newContentTemplateSelector)
        {
            // if ContentTemplateSelector is really changing (and in use), remove the old template
            Template = null;
        }

        /// <summary>
        /// Identifies the <see cref="ContentStringFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentStringFormatProperty =
            DependencyProperty.Register(
                nameof(ContentStringFormat),
                typeof(string),
                typeof(ContentPresenter),
                new PropertyMetadata(null, OnContentStringFormatChanged));

        /// <summary>
        /// Gets or sets a composite string that specifies how to format the <see cref="Content"/>
        /// property if it is displayed as a string.
        /// </summary>
        /// <returns>
        /// A composite string that specifies how to format the <see cref="Content"/> property if 
        /// it is displayed as a string. The default is null.
        /// </returns>
        public string ContentStringFormat
        {
            get => (string)GetValue(ContentStringFormatProperty);
            set => SetValueInternal(ContentStringFormatProperty, value);
        }

        private static void OnContentStringFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContentPresenter ctrl = (ContentPresenter)d;
            ctrl.OnContentStringFormatChanged((string)e.OldValue, (string)e.NewValue);
        }

        /// <summary>
        /// Invoked when the <see cref="ContentStringFormat"/> property changes.
        /// </summary>
        /// <param name="oldContentStringFormat">
        /// The old value of the <see cref="ContentStringFormat"/> property.
        /// </param>
        /// <param name="newContentStringFormat">
        /// The new value of the <see cref="ContentStringFormat"/> property.
        /// </param>
        protected virtual void OnContentStringFormatChanged(string oldContentStringFormat, string newContentStringFormat)
        {
            // force on-demand regeneration of the formatting templates for XML and String content
            StringFormattingTemplateField.ClearValue(this);
            AccessTextFormattingTemplateField.ClearValue(this);
        }

        /// <summary>
        /// Identifies the <see cref="ContentSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentSourceProperty =
            DependencyProperty.Register(
                nameof(ContentSource),
                typeof(string),
                typeof(ContentPresenter),
                new PropertyMetadata(nameof(Content)));

        /// <summary>
        /// Gets or sets the base name to use during automatic aliasing.
        /// </summary>
        /// <returns>
        /// The base name to use during automatic aliasing. The default is "Content".
        /// </returns>
        public string ContentSource
        {
            get => (string)GetValue(ContentSourceProperty);
            set => SetValueInternal(ContentSourceProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="RecognizesAccessKey"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RecognizesAccessKeyProperty =
            DependencyProperty.Register(
                nameof(RecognizesAccessKey),
                typeof(bool),
                typeof(ContentPresenter),
                new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Gets or sets a value that indicates whether the <see cref="ContentPresenter"/> should 
        /// use AccessText in its style.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the <see cref="ContentPresenter"/> should use AccessText in 
        /// its style; otherwise, <see langword="false"/>. The default is <see langword="false"/>.
        /// </returns>
        public bool RecognizesAccessKey
        {
            get => (bool)GetValue(RecognizesAccessKeyProperty);
            set => SetValueInternal(RecognizesAccessKeyProperty, value);
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
            get => _templateCache;
            set => SetValueInternal(TemplateProperty, value);
        }

        // Property invalidation callback invoked when TemplateProperty is invalidated
        private static void OnTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContentPresenter cp = (ContentPresenter)d;
            StyleHelper.UpdateTemplateCache(cp, (FrameworkTemplate)e.OldValue, (FrameworkTemplate)e.NewValue, TemplateProperty);

            cp.InvalidateMeasure();
        }

        // Internal Helper so the FrameworkElement could see this property
        internal override FrameworkTemplate TemplateInternal => Template;

        // Internal Helper so the FrameworkElement could see the template cache
        internal override FrameworkTemplate TemplateCache
        {
            get => _templateCache;
            set => _templateCache = (DataTemplate)value;
        }

        internal static DataTemplate DefaultContentTemplate { get; }

        internal static DataTemplate StringContentTemplate { get; }

        internal static DataTemplate AccessTextContentTemplate { get; }

        internal static DataTemplate UIElementContentTemplate { get; }

        private DataTemplate FormattingAccessTextContentTemplate
        {
            get
            {
                DataTemplate template = AccessTextFormattingTemplateField.GetValue(this);
                if (template is null)
                {
                    template = new DataTemplate
                    {
                        Template = new CompiledTemplateContent(
                            new XamlContext(),
                            static (owner, context) =>
                            {
                                var contentPresenter = (ContentPresenter)owner;
                                var text = new AccessText();
                                text.SetTemplatedParent(context.TemplateOwnerReference);
                                var binding = new Binding
                                {
                                    StringFormat = contentPresenter.ContentStringFormat,
                                };
                                text.SetBinding(AccessText.TextProperty, binding);
                                return text;
                            }),
                    };

                    template.Seal();

                    AccessTextFormattingTemplateField.SetValue(this, template);
                }
                return template;
            }
        }

        private DataTemplate FormattingStringContentTemplate
        {
            get
            {
                DataTemplate template = StringFormattingTemplateField.GetValue(this);
                if (template is null)
                {
                    template = new DataTemplate
                    {
                        Template = new CompiledTemplateContent(
                            new XamlContext(),
                            static (owner, context) =>
                            {
                                var contentPresenter = (ContentPresenter)owner;
                                var text = new TextBlock();
                                text.SetTemplatedParent(context.TemplateOwnerReference);
                                var binding = new Binding
                                {
                                    StringFormat = contentPresenter.ContentStringFormat,
                                };
                                text.SetBinding(TextBlock.TextProperty, binding);
                                return text;
                            }),
                    };

                    template.Seal();

                    StringFormattingTemplateField.SetValue(this, template);
                }
                return template;
            }
        }

        private bool IsUsingDefaultStringTemplate
        {
            get
            {
                if (Template == StringContentTemplate || Template == AccessTextContentTemplate)
                {
                    return true;
                }

                DataTemplate template = StringFormattingTemplateField.GetValue(this);
                if (template is not null && template == Template)
                {
                    return true;
                }

                template = AccessTextFormattingTemplateField.GetValue(this);
                if (template is not null && template == Template)
                {
                    return true;
                }

                return false;
            }
        }

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

                UpdateDataContext();
            }

            Template = newTemplate;
        }

        private void UpdateDataContext()
        {
            if (Content is not UIElement)
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
            object content = Content;

            // ContentTemplate has first stab
            DataTemplate template = ContentTemplate;

            // no ContentTemplate set, try ContentTemplateSelector
            if (template is null)
            {
                if (ContentTemplateSelector is DataTemplateSelector contentTemplateSelector)
                {
                    template = contentTemplateSelector.SelectTemplate(content, this);
                }
            }

            // no ContentTemplate set, try the default templates
            if (template is null)
            {
                // Lookup template for typeof(Content) in resource dictionaries.
                if (content is not null)
                {
                    template = (DataTemplate)FindTemplateResourceInternal(this, content);
                }

                // default templates for well known types
                if (template is null)
                {
                    if (content is UIElement)
                    {
                        template = UIElementContentTemplate;
                    }
                    else if (content is string s)
                    {
                        template = SelectTemplateForString(s);
                    }
                    else
                    {
                        template = DefaultContentTemplate;
                    }
                }
            }

            return template;
        }

        private DataTemplate SelectTemplateForString(string s)
        {
            if (RecognizesAccessKey && s.IndexOf(AccessText.AccessKeyMarker) > -1)
            {
                return string.IsNullOrEmpty(ContentStringFormat) ? AccessTextContentTemplate : FormattingAccessTextContentTemplate;
            }
            else
            {
                return string.IsNullOrEmpty(ContentStringFormat) ? StringContentTemplate : FormattingStringContentTemplate;
            }
        }

        //  Searches through resource dictionaries to find a DataTemplate
        //  that matches the type of the 'item' parameter.  Failing an exact
        //  match of the type, return something that matches one of its parent
        //  types.
        internal static object FindTemplateResourceInternal(DependencyObject target, object item)
        {
            // Data styling doesn't apply to UIElement.
            if (item is null || item is UIElement)
            {
                return null;
            }

            Type dataType = item.GetType();

            var keys = new List<DataTemplateKey>();

            // construct the list of acceptable keys, in priority ord
            int exactMatch = 1;    // number of entries that count as an exact match

            // add compound keys for the dataType and all its base types
            while (dataType is not null)
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
                if (FindTemplateResourceFromApp(target, keys, exactMatch, ref bestMatch) is object appResource)
                {
                    resource = appResource;
                }
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

            if (Application.Current is Application app)
            {
                // If the element is rooted to a Window and App exists, defer to App.
                for (k = 0; k < bestMatch; ++k)
                {
                    if (app.FindImplicitResource(keys[k]) is object appResource)
                    {
                        bestMatch = k;
                        resource = appResource;

                        if (bestMatch < exactMatch)
                        {
                            return resource;
                        }
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
            Debug.Assert(target is not null, "Don't call FindTemplateResource with a null target object");

            ResourceDictionary table;
            object resource = null;

            FrameworkElement fe = target as FrameworkElement;

            while (fe is not null)
            {
                object candidate;

                // -------------------------------------------
                //  Lookup ResourceDictionary on the current instance
                // -------------------------------------------

                // Fetch the ResourceDictionary
                // for the given target element
                table = GetInstanceResourceDictionary(fe);
                if (table is not null)
                {
                    candidate = FindBestMatchInResourceDictionary(table, keys, exactMatch, ref bestMatch);
                    if (candidate is not null)
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
                //  Lookup ResourceDictionary on the current instance's Style, if one exists.
                // -------------------------------------------

                table = GetStyleResourceDictionary(fe);
                if (table is not null)
                {
                    candidate = FindBestMatchInResourceDictionary(table, keys, exactMatch, ref bestMatch);
                    if (candidate is not null)
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
                //  Lookup ResourceDictionary on the current instance's Theme Style, if one exists.
                // -------------------------------------------

                table = GetThemeStyleResourceDictionary(fe);
                if (table is not null)
                {
                    candidate = FindBestMatchInResourceDictionary(table, keys, exactMatch, ref bestMatch);
                    if (candidate is not null)
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
                //  Lookup ResourceDictionary on the current instance's Template, if one exists.
                // -------------------------------------------

                table = GetTemplateResourceDictionary(fe);
                if (table is not null)
                {
                    candidate = FindBestMatchInResourceDictionary(table, keys, exactMatch, ref bestMatch);
                    if (candidate is not null)
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

        // Return a reference to the ResourceDictionary set on the instance of
        //  the given FrameworkElement, if such a ResourceDictionary exists.
        private static ResourceDictionary GetInstanceResourceDictionary(FrameworkElement fe) =>
            fe.HasResources ? fe.Resources : null;

        // Return a reference to the ResourceDictionary attached to the Style of
        //  the given FrameworkElement, if such a ResourceDictionary exists.
        private static ResourceDictionary GetStyleResourceDictionary(FrameworkElement fe) =>
            fe.Style is Style style && style.HasResources ? style.Resources : null;

        // Return a reference to the ResourceDictionary attached to the Theme Style of
        //  the given FrameworkElement, if such a ResourceDictionary exists.
        private static ResourceDictionary GetThemeStyleResourceDictionary(FrameworkElement fe) =>
            fe.ThemeStyle is Style themeStyle && themeStyle.HasResources ? themeStyle.Resources : null;

        // Return a reference to the ResourceDictionary attached to the Template of
        //  the given FrameworkElement, if such a ResourceDictionary exists.
        private static ResourceDictionary GetTemplateResourceDictionary(FrameworkElement fe) =>
            fe.TemplateInternal is FrameworkTemplate template && template.HasResources ? template.Resources : null;

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
            if (table is not null)
            {
                for (k = 0; k < bestMatch; ++k)
                {
                    if (table[keys[k]] is object candidate)
                    {
                        resource = candidate;
                        bestMatch = k;

                        // if we found an exact match, no need to continue
                        if (bestMatch < exactMatch)
                        {
                            return resource;
                        }
                    }
                }
            }

            return resource;
        }

        internal void PrepareContentPresenter(object item, DataTemplate itemTemplate, DataTemplateSelector itemTemplateSelector, string stringFormat)
        {
            if (item != this)
            {
                // copy templates from parent ItemsControl
                if (_contentIsItem || HasDefaultValue(ContentProperty))
                {
                    Content = item;
                    _contentIsItem = true;
                }

                if (itemTemplate is not null)
                {
                    ContentTemplate = itemTemplate;
                }

                if (itemTemplateSelector is not null)
                {
                    ContentTemplateSelector = itemTemplateSelector;
                }

                if (stringFormat is not null)
                {
                    ContentStringFormat = stringFormat;
                }
            }
        }

        internal void ClearContentPresenter(object item)
        {
            if (this != item)
            {
                if (_contentIsItem)
                {
                    ClearValue(ContentProperty);
                }
            }
        }

        // called when a resource change affects implicit data templates
        internal void ReevaluateTemplate()
        {
            // run the template algorithm again
            if (Template != ChooseTemplate())
            {
                // if it chooses a different template, mark the current template
                // as no longer current, and ask for re-measure
                _templateIsCurrent = false;
                InvalidateMeasure();
            }
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            int count = VisualChildrenCount;

            if (count > 0)
            {
                if (GetVisualChild(0) is UIElement child)
                {
                    child.Measure(availableSize);
                    return child.DesiredSize;
                }
            }

            return new Size();
        }

        /// <inheritdoc/>
        protected override Size ArrangeOverride(Size finalSize)
        {
            int count = VisualChildrenCount;

            if (count > 0)
            {
                if (GetVisualChild(0) is UIElement child)
                {
                    child.Arrange(new Rect(finalSize));
                }
            }
            return finalSize;
        }

        private sealed class UseContentTemplate : DataTemplate
        {
            internal override bool BuildVisualTree(IFrameworkElement container)
            {
                ContentPresenter cp = (ContentPresenter)container;
                FrameworkElement child = cp.Content as FrameworkElement;
                if (child is not null)
                {
                    if (VisualTreeHelper.GetParent(child) is FrameworkElement parent)
                    {
                        parent.TemplateChild = null;
                    }
                }

                cp.TemplateChild = child;

                return true;
            }
        }

        private sealed class DefaultTemplate : DataTemplate
        {
            internal override bool BuildVisualTree(IFrameworkElement container)
            {
                ContentPresenter cp = (ContentPresenter)container;
                FrameworkElement result = DefaultExpansion(cp.Content, cp);

                cp.TemplateChild = result;

                return result is not null;
            }

            private FrameworkElement DefaultExpansion(object content, ContentPresenter container)
            {
                if (content is null)
                {
                    return null;
                }

                var textBlock = new TextBlock();
                textBlock.SetTemplatedParent(new(container));

                if (container.ContentStringFormat is string stringFormat)
                {
                    textBlock.SetBinding(TextBlock.TextProperty, new Binding
                    {
                        StringFormat = stringFormat,
                    });
                }
                else
                {
                    textBlock.SetBinding(TextBlock.TextProperty, Binding.Empty);
                }

                return textBlock;
            }
        }
    }
}

