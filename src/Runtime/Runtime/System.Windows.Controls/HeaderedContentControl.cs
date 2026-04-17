
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

// Derived from WPF (dotnet/wpf, MIT license).

using System.ComponentModel;
using OpenSilver.Internal;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents the base class for all controls that contain single content and have
    /// a header.
    /// </summary>
    /// <remarks>
    /// <see cref="HeaderedContentControl"/> adds <see cref="Header"/>, <see cref="HasHeader"/>,
    /// <see cref="HeaderTemplate"/>, and <see cref="HeaderTemplateSelector"/> features to
    /// a <see cref="ContentControl"/>.
    /// </remarks>
    public class HeaderedContentControl : ContentControl
    {
        static HeaderedContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HeaderedContentControl), new FrameworkPropertyMetadata(typeof(HeaderedContentControl)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HeaderedContentControl"/> class.
        /// </summary>
        public HeaderedContentControl() { }

        /// <summary>
        /// Identifies the <see cref="Header"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                nameof(Header),
                typeof(object),
                typeof(HeaderedContentControl),
                new FrameworkPropertyMetadata(null, OnHeaderChanged));

        /// <summary>
        /// Gets or sets the data used for the header of each control.
        /// </summary>
        /// <returns>
        /// The header for each control. The default is null.
        /// </returns>
        public object Header
        {
            get => GetValue(HeaderProperty);
            set => SetValueInternal(HeaderProperty, value);
        }

        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (HeaderedContentControl)d;
            ctrl.SetValueInternal(HasHeaderPropertyKey, e.NewValue is not null);
            ctrl.OnHeaderChanged(e.OldValue, e.NewValue);
        }

        /// <summary>
        /// Called when the <see cref="Header"/> property changes.
        /// </summary>
        /// <param name="oldHeader">The old value of the <see cref="Header"/> property.</param>
        /// <param name="newHeader">The new value of the <see cref="Header"/> property.</param>
        protected virtual void OnHeaderChanged(object oldHeader, object newHeader)
        {
            RemoveLogicalChild(oldHeader);
            AddLogicalChild(newHeader);
        }

        private static readonly DependencyPropertyKey HasHeaderPropertyKey =
            DependencyProperty.RegisterReadOnly(
                nameof(HasHeader),
                typeof(bool),
                typeof(HeaderedContentControl),
                new FrameworkPropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Identifies the <see cref="HasHeader"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HasHeaderProperty = HasHeaderPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that indicates whether the <see cref="Header"/> is non-null.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the <see cref="Header"/> property is non-null; otherwise,
        /// <see langword="false"/>. The default is <see langword="false"/>.
        /// </value>
        [Browsable(false), ReadOnly(true)]
        public bool HasHeader => (bool)GetValue(HasHeaderProperty);

        /// <summary>
        /// Identifies the <see cref="HeaderTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(
                nameof(HeaderTemplate),
                typeof(DataTemplate),
                typeof(HeaderedContentControl),
                new FrameworkPropertyMetadata(null, OnHeaderTemplateChanged));

        /// <summary>
        /// Gets or sets the template used to display the content of the control's header.
        /// </summary>
        /// <returns>
        /// The template that is used to display the content of the control's header.
        /// The default is null.
        /// </returns>
        public DataTemplate HeaderTemplate
        {
            get => (DataTemplate)GetValue(HeaderTemplateProperty);
            set => SetValueInternal(HeaderTemplateProperty, value);
        }

        private static void OnHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (HeaderedContentControl)d;
            ctrl.OnHeaderTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
        }

        /// <summary>
        /// Called when the <see cref="HeaderTemplate"/> property changes.
        /// </summary>
        /// <param name="oldHeaderTemplate">
        /// The old value of the <see cref="HeaderTemplate"/> property.
        /// </param>
        /// <param name="newHeaderTemplate">
        /// The new value of the <see cref="HeaderTemplate"/> property.
        /// </param>
        protected virtual void OnHeaderTemplateChanged(DataTemplate oldHeaderTemplate, DataTemplate newHeaderTemplate)
        {
        }

        /// <summary>
        /// Identifies the <see cref="HeaderTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateSelectorProperty =
            DependencyProperty.Register(
                nameof(HeaderTemplateSelector),
                typeof(DataTemplateSelector),
                typeof(HeaderedContentControl),
                new FrameworkPropertyMetadata(null, OnHeaderTemplateSelectorChanged));

        /// <summary>
        /// Gets or sets a template selector that provides custom logic for choosing the
        /// template used to display the header.
        /// </summary>
        /// <returns>
        /// A data template selector. The default is null.
        /// </returns>
        public DataTemplateSelector HeaderTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(HeaderTemplateSelectorProperty);
            set => SetValueInternal(HeaderTemplateSelectorProperty, value);
        }

        private static void OnHeaderTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (HeaderedContentControl)d;
            ctrl.OnHeaderTemplateSelectorChanged((DataTemplateSelector)e.OldValue, (DataTemplateSelector)e.NewValue);
        }

        /// <summary>
        /// Called when the <see cref="HeaderTemplateSelector"/> property changes.
        /// </summary>
        /// <param name="oldHeaderTemplateSelector">
        /// The old value of the <see cref="HeaderTemplateSelector"/> property.
        /// </param>
        /// <param name="newHeaderTemplateSelector">
        /// The new value of the <see cref="HeaderTemplateSelector"/> property.
        /// </param>
        protected virtual void OnHeaderTemplateSelectorChanged(DataTemplateSelector oldHeaderTemplateSelector, DataTemplateSelector newHeaderTemplateSelector)
        {
        }

        internal override string GetPlainText() => ContentObjectToString(Header);
    }
}
