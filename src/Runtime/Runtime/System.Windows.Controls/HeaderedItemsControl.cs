// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Diagnostics;
using System.Windows.Data;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a control that contains a collection of items and a header.
    /// </summary>
    /// <QualityBand>Stable</QualityBand>
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(ContentPresenter))]
    public class HeaderedItemsControl : ItemsControl
    {
        /// <summary>
        /// Gets or sets a value indicating whether the Header property has been
        /// set to the item of an ItemsControl.
        /// </summary>
        internal bool HeaderIsItem { get; set; }

        #region public object Header
        /// <summary>
        /// Gets or sets the item that labels the control.
        /// </summary>
        /// <value>
        /// The item that labels the control. The default value is null.
        /// </value>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the
        /// <see cref="HeaderedItemsControl.Header" />
        /// dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the
        /// <see cref="HeaderedItemsControl.Header" />
        /// dependency property.
        /// </value>
        /// <remarks>
        /// Note: WPF defines this property via a call to AddOwner of
        /// HeaderedContentControl's HeaderProperty.
        /// </remarks>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                "Header",
                typeof(object),
                typeof(HeaderedItemsControl),
                new PropertyMetadata(OnHeaderPropertyChanged));

        /// <summary>
        /// HeaderProperty property changed handler.
        /// </summary>
        /// <param name="d">
        /// HeaderedItemsControl that changed its Header.
        /// </param>
        /// <param name="e">Event arguments.</param>
        private static void OnHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeaderedItemsControl source = d as HeaderedItemsControl;
            source.OnHeaderChanged(e.OldValue, e.NewValue);
        }
        #endregion public object Header

        #region public DataTemplate HeaderTemplate
        /// <summary>
        /// Gets or sets a data template that is used to display the contents of
        /// the control's header.
        /// </summary>
        /// <value>
        /// Gets or sets a data template that is used to display the contents of
        /// the control's header. The default is null.
        /// </value>
        public DataTemplate HeaderTemplate
        {
            get { return GetValue(HeaderTemplateProperty) as DataTemplate; }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the
        /// <see cref="HeaderedItemsControl.HeaderTemplate" />
        /// dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the
        /// <see cref="HeaderedItemsControl.HeaderTemplate" />
        /// dependency property.
        /// </value>
        /// <remarks>
        /// Note: WPF defines this property via a call to AddOwner of
        /// HeaderedContentControl's HeaderTemplateProperty.
        /// </remarks>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(
                "HeaderTemplate",
                typeof(DataTemplate),
                typeof(HeaderedItemsControl),
                new PropertyMetadata(OnHeaderTemplatePropertyChanged));

        /// <summary>
        /// HeaderTemplateProperty property changed handler.
        /// </summary>
        /// <param name="d">
        /// HeaderedItemsControl that changed its HeaderTemplate.
        /// </param>
        /// <param name="e">Event arguments.</param>
        private static void OnHeaderTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeaderedItemsControl source = d as HeaderedItemsControl;
            DataTemplate oldHeaderTemplate = e.OldValue as DataTemplate;
            DataTemplate newHeaderTemplate = e.NewValue as DataTemplate;
            source.OnHeaderTemplateChanged(oldHeaderTemplate, newHeaderTemplate);
        }
        #endregion public DataTemplate HeaderTemplate

        #region public Style ItemContainerStyle
        /// <summary>
        /// Gets or sets the <see cref="Style" /> that is
        /// applied to the container element generated for each item.
        /// </summary>
        /// <value>
        /// The <see cref="Style" /> that is applied to the
        /// container element generated for each item. The default is null.
        /// </value>
        public new Style ItemContainerStyle
        {
            get { return base.ItemContainerStyle; }
            set { base.ItemContainerStyle = value; }
        }

        /// <summary>
        /// Identifies the
        /// <see cref="HeaderedItemsControl.ItemContainerStyle" />
        /// dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the
        /// <see cref="HeaderedItemsControl.ItemContainerStyle" />
        /// dependency property.
        /// </value>
        public new static readonly DependencyProperty ItemContainerStyleProperty = ItemsControl.ItemContainerStyleProperty;
        #endregion public Style ItemContainerStyle

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="HeaderedItemsControl" /> class.
        /// </summary>
        public HeaderedItemsControl()
        {
            DefaultStyleKey = typeof(HeaderedItemsControl);
        }

        /// <summary>
        /// Called when the value of the
        /// <see cref="HeaderedItemsControl.Header" />
        /// property changes.
        /// </summary>
        /// <param name="oldHeader">
        /// The old value of the
        /// <see cref="HeaderedItemsControl.Header" />
        /// property.
        /// </param>
        /// <param name="newHeader">
        /// The new value of the
        /// <see cref="HeaderedItemsControl.Header" />
        /// property.
        /// </param>
        protected virtual void OnHeaderChanged(object oldHeader, object newHeader)
        {
        }

        /// <summary>
        /// Called when the value of the
        /// <see cref="HeaderedItemsControl.HeaderTemplate" />
        /// property changes.
        /// </summary>
        /// <param name="oldHeaderTemplate">
        /// The old value of the
        /// <see cref="HeaderedItemsControl.HeaderTemplate" />
        /// property.
        /// </param>
        /// <param name="newHeaderTemplate">
        /// The new value of the
        /// <see cref="HeaderedItemsControl.HeaderTemplate" />
        /// property.
        /// </param>
        protected virtual void OnHeaderTemplateChanged(DataTemplate oldHeaderTemplate, DataTemplate newHeaderTemplate)
        {
        }

        /// <summary>
        /// Prepares the specified element to display the specified item.
        /// </summary>
        /// <param name="element">
        /// The container element used to display the specified item.
        /// </param>
        /// <param name="item">The content to display.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            ItemsControlHelper.PrepareContainerForItemOverride(element, ItemContainerStyle);
            PreparePrepareHeaderedItemsControlContainerForItemOverride(element, item, this, ItemContainerStyle);

            base.PrepareContainerForItemOverride(element, item);
        }

        /// <summary>
        /// Prepares the specified container to display the specified item.
        /// </summary>
        /// <param name="element">
        /// Container element used to display the specified item.
        /// </param>
        /// <param name="item">Specified item to display.</param>
        /// <param name="parent">The parent ItemsControl.</param>
        /// <param name="parentItemContainerStyle">
        /// The ItemContainerStyle for the parent ItemsControl.
        /// </param>
        internal static void PreparePrepareHeaderedItemsControlContainerForItemOverride(DependencyObject element, object item, ItemsControl parent, Style parentItemContainerStyle)
        {
            HeaderedItemsControl headeredItemsControl = element as HeaderedItemsControl;
            if (headeredItemsControl != null)
            {
                PrepareHeaderedItemsControlContainer(headeredItemsControl, item, parent, parentItemContainerStyle);
            }
        }

        /// <summary>
        /// Prepare a PrepareHeaderedItemsControlContainer container for an
        /// item.
        /// </summary>
        /// <param name="control">Container to prepare.</param>
        /// <param name="item">Item to be placed in the container.</param>
        /// <param name="parentItemsControl">The parent ItemsControl.</param>
        /// <param name="parentItemContainerStyle">
        /// The ItemContainerStyle for the parent ItemsControl.
        /// </param>
        private static void PrepareHeaderedItemsControlContainer(HeaderedItemsControl control, object item, ItemsControl parentItemsControl, Style parentItemContainerStyle)
        {
            if (control != item)
            {
                // Copy the ItemsControl properties from parent to child
                DataTemplate parentItemTemplate = parentItemsControl.ItemTemplate;
                if (parentItemTemplate != null)
                {
                    control.SetValue(HeaderedItemsControl.ItemTemplateProperty, parentItemTemplate);
                }
                if (parentItemContainerStyle != null && HasDefaultValue(control, HeaderedItemsControl.ItemContainerStyleProperty))
                {
                    control.SetValue(HeaderedItemsControl.ItemContainerStyleProperty, parentItemContainerStyle);
                }

                // Copy the Header properties from parent to child
                if (control.HeaderIsItem || HasDefaultValue(control, HeaderedItemsControl.HeaderProperty))
                {
                    control.Header = item;
                    control.HeaderIsItem = true;
                }
                if (parentItemTemplate != null)
                {
                    control.SetValue(HeaderedItemsControl.HeaderTemplateProperty, parentItemTemplate);
                }
                if (parentItemContainerStyle != null)
                {
                    object style = control.ReadLocalValue(HeaderedItemsControl.StyleProperty);
                    if (style == null || style == DependencyProperty.UnsetValue)
                    {
                        control.SetValue(HeaderedItemsControl.StyleProperty, parentItemContainerStyle);
                    }
                }

                // Note: this is where we would apply the HeaderTemplateSelector
                // (if implemented) or attempt to lookup the implicit template
                // for the type of the item if the headerTemplate were null.

                // Setup a hierarchical template
                HierarchicalDataTemplate headerTemplate = parentItemTemplate as HierarchicalDataTemplate;
                if (headerTemplate != null)
                {
                    if (headerTemplate.ItemsSource != null && HasDefaultValue(control, HeaderedItemsControl.ItemsSourceProperty))
                    {
                        control.SetBinding(
                            HeaderedItemsControl.ItemsSourceProperty,
                            new Binding
                            {
                                Converter = headerTemplate.ItemsSource.Converter,
                                ConverterCulture = headerTemplate.ItemsSource.ConverterCulture,
                                ConverterParameter = headerTemplate.ItemsSource.ConverterParameter,
                                Mode = headerTemplate.ItemsSource.Mode,
                                NotifyOnValidationError = headerTemplate.ItemsSource.NotifyOnValidationError,
                                Path = headerTemplate.ItemsSource.Path,
                                Source = control.Header,
                                ValidatesOnExceptions = headerTemplate.ItemsSource.ValidatesOnExceptions
                            });
                    }
                    if (headerTemplate.IsItemTemplateSet && control.ItemTemplate == parentItemTemplate)
                    {
                        control.ClearValue(HeaderedItemsControl.ItemTemplateProperty);
                        if (headerTemplate.ItemTemplate != null)
                        {
                            control.ItemTemplate = headerTemplate.ItemTemplate;
                        }
                    }
                    if (headerTemplate.IsItemContainerStyleSet && control.ItemContainerStyle == parentItemContainerStyle)
                    {
                        control.ClearValue(HeaderedItemsControl.ItemContainerStyleProperty);
                        if (headerTemplate.ItemContainerStyle != null)
                        {
                            control.ItemContainerStyle = headerTemplate.ItemContainerStyle;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Check whether a control has the default value for a property.
        /// </summary>
        /// <param name="control">The control to check.</param>
        /// <param name="property">The property to check.</param>
        /// <returns>
        /// True if the property has the default value; false otherwise.
        /// </returns>
        private static bool HasDefaultValue(Control control, DependencyProperty property)
        {
            Debug.Assert(control != null, "control should not be null!");
            Debug.Assert(property != null, "property should not be null!");
            return control.ReadLocalValue(property) == DependencyProperty.UnsetValue;
        }
    }
}