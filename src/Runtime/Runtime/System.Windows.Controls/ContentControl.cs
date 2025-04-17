
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

using System.Collections;
using System.Windows.Markup;
using System.Windows.Data;
using OpenSilver.Internal;
using OpenSilver.Internal.Controls;
using OpenSilver.Internal.Xaml.Context;

namespace System.Windows.Controls
{
    /// <summary>
    /// Represents a control with a single piece of content. Controls such as Button,
    /// CheckBox, and ScrollViewer directly or indirectly inherit from this class.
    /// </summary>
    [ContentProperty(nameof(Content))]
    public class ContentControl : Control
    {
        static ContentControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ContentControl), new PropertyMetadata(typeof(ContentControl)));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentControl" /> class.
        /// </summary>
        public ContentControl() { }

        /// <summary>
        /// Gets or sets the content of a <see cref="ContentControl"/>.
        /// </summary>
        /// <returns>
        /// An object that contains the control's content. The default value is null.
        /// </returns>
        public object Content
        {
            get => GetValue(ContentProperty);
            set => SetValueInternal(ContentProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="Content"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(
                nameof(Content),
                typeof(object),
                typeof(ContentControl),
                new FrameworkPropertyMetadata(null, OnContentChanged));

        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ContentControl)d).OnContentChanged(e.OldValue, e.NewValue);
        }

        /// <summary>
        /// Gets or sets the data template used to display the content of the <see cref="ContentControl"/>.
        /// </summary>
        /// <returns>
        /// A data template. The default value is null.
        /// </returns>
        public DataTemplate ContentTemplate
        {
            get => (DataTemplate)GetValue(ContentTemplateProperty);
            set => SetValueInternal(ContentTemplateProperty, value);
        }

        /// <summary>
        /// Identifies the <see cref="ContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register(
                nameof(ContentTemplate),
                typeof(DataTemplate),
                typeof(ContentControl),
                new PropertyMetadata(null, OnContentTemplateChanged));

        private static void OnContentTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContentControl ctrl = (ContentControl)d;
            ctrl.OnContentTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
        }

        /// <summary>
        /// Called when the <see cref="ContentTemplate"/> property changes.
        /// </summary>
        /// <param name="oldContentTemplate">
        /// The old value of the <see cref="ContentTemplate"/> property.
        /// </param>
        /// <param name="newContentTemplate">
        /// The new value of the <see cref="ContentTemplate"/> property.
        /// </param>
        protected virtual void OnContentTemplateChanged(DataTemplate oldContentTemplate, DataTemplate newContentTemplate)
        {
        }

        /// <summary>
        /// Identifies the <see cref="ContentTemplateSelector"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentTemplateSelectorProperty =
            DependencyProperty.Register(
                nameof(ContentTemplateSelector),
                typeof(DataTemplateSelector),
                typeof(ContentControl),
                new PropertyMetadata(null, OnContentTemplateSelectorChanged));

        /// <summary>
        /// Gets or sets a template selector that enables an application writer to provide custom template-selection logic.
        /// </summary>
        /// <returns>
        /// A data template selector. The default value is null.
        /// </returns>
        public DataTemplateSelector ContentTemplateSelector
        {
            get => (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty);
            set => SetValueInternal(ContentTemplateSelectorProperty, value);
        }

        private static void OnContentTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ContentControl ctrl = (ContentControl)d;
            ctrl.OnContentTemplateSelectorChanged((DataTemplateSelector)e.NewValue, (DataTemplateSelector)e.NewValue);
        }

        /// <summary>
        /// Called when the <see cref="ContentTemplateSelector"/> property changes.
        /// </summary>
        /// <param name="oldContentTemplateSelector">
        /// The old value of the <see cref="ContentTemplateSelector"/> property.
        /// </param>
        /// <param name="newContentTemplateSelector">
        /// The new value of the <see cref="ContentTemplateSelector"/> property.
        /// </param>
        protected virtual void OnContentTemplateSelectorChanged(DataTemplateSelector oldContentTemplateSelector, DataTemplateSelector newContentTemplateSelector)
        {
        }

        /// <summary>
        /// Called when the <see cref="Content"/> property changes.
        /// </summary>
        /// <param name="oldContent">
        /// The old value of the <see cref="Content"/> property.
        /// </param>
        /// <param name="newContent">
        /// The new value of the <see cref="Content"/> property.
        /// </param>
        protected virtual void OnContentChanged(object oldContent, object newContent)
        {
            // Remove the old content child
            RemoveLogicalChild(oldContent);

            if (ContentIsNotLogical)
            {
                return;
            }

            // We want to update the logical parent only if we don't have one already.
            if (newContent is FrameworkElement fe)
            {
                if (fe.Parent is not null)
                {
                    return;
                }
            }

            AddLogicalChild(newContent);
        }

        /// <summary>
        ///    Indicates whether Content should be a logical child or not.
        /// </summary>
        internal bool ContentIsNotLogical { get; set; }

        /// <summary>
        /// Gets an enumerator to the content control's logical child elements.
        /// </summary>
        /// <returns>
        /// An enumerator. The default value is null.
        /// </returns>
        protected internal override IEnumerator LogicalChildren
        {
            get
            {
                object content = Content;
                
                if (ContentIsNotLogical || content is null)
                {
                    return EmptyEnumerator.Instance;
                }

                // If the current ContentControl is in a Template.VisualTree and is meant to host
                // the content for the container then that content shows up as the logical child
                // for the container and not for the current ContentControl.
                if (content is FrameworkElement fe)
                {
                    DependencyObject logicalParent = fe.Parent;
                    if (logicalParent is not null && logicalParent != this)
                    {
                        return EmptyEnumerator.Instance;
                    }
                }

                return new ContentModelTreeEnumerator(this, content);
            }
        }

        internal override FrameworkTemplate TemplateInternal => base.TemplateInternal ?? DefaultTemplate;

        private static FrameworkTemplate DefaultTemplate { get; } = new UseContentTemplate();

        /// <summary>
        /// Prepare to display the item.
        /// </summary>
        internal void PrepareContentControl(object item, DataTemplate itemTemplate, DataTemplateSelector itemTemplateSelector)
        {
            if (item != this)
            {
                // don't treat Content as a logical child
                ContentIsNotLogical = true;

                Content = item;
                
                if (itemTemplate is not null)
                {
                    ContentTemplate = itemTemplate;
                }

                if (itemTemplateSelector is not null)
                {
                    ContentTemplateSelector = itemTemplateSelector;
                }
            }
            else
            {
                ContentIsNotLogical = false;
            }
        }

        internal void ClearContentControl(object item)
        {
            if (this != item)
            {
                ClearValue(ContentProperty);
            }
        }

        internal override string GetPlainText() => ContentObjectToString(Content);

        internal static string ContentObjectToString(object content)
        {
            if (content is not null)
            {
                if (content is FrameworkElement feContent)
                {
                    return feContent.GetPlainText();
                }

                return content.ToString();
            }

            return string.Empty;
        }

        private sealed class UseContentTemplate : FrameworkTemplate
        {
            private readonly TemplateContent _defaultTemplate =
                new TemplateContent(
                    new XamlContext(),
                    static (owner, context) =>
                    {
                        var grid = new Grid();
                        grid.SetTemplatedParent(context.TemplateOwnerReference);
                        var tb = new TextBlock();
                        tb.SetTemplatedParent(context.TemplateOwnerReference);
                        tb.SetBinding(TextBlock.TextProperty, Binding.Empty);
                        grid.Children.Add(tb);
                        return grid;
                    });

            public UseContentTemplate()
            {
                Seal();
            }

            internal override bool BuildVisualTree(IFrameworkElement container)
            {
                var cc = (ContentControl)container;
                object content = cc.Content;
                if (content is FrameworkElement fe)
                {
                    cc.TemplateChild = fe;
                    return true;
                }
                else if (content is not null && (content is not string s || s.Length > 0))
                {
                    cc.TemplateChild = (FrameworkElement)_defaultTemplate.LoadContent(cc);
                    return true;
                }

                return false;
            }
        }
    }
}
