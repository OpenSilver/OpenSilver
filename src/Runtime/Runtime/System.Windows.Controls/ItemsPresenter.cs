
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

namespace System.Windows.Controls
{
    /// <summary>
    /// Displays the content of a ItemsPresenter.
    /// </summary>
    public class ItemsPresenter : FrameworkElement
    {
        private ItemsPanelTemplate _templateCache;
        private ItemsControl _owner; // templated parent.
        private ItemContainerGenerator _generator;

        internal sealed override FrameworkElement TemplateChild
        {
            get { return base.TemplateChild; }
            set
            {
                if (value != null)
                {
                    Panel panel = value as Panel;
                    if (panel == null)
                    {
                        throw new InvalidOperationException(string.Format("VisualTree of ItemsPanelTemplate must contain a Panel. '{0}' is not a Panel.", value.GetType()));
                    }
                    panel.IsItemsHost = true;
                }

                base.TemplateChild = value;
            }
        }

        internal ItemsControl Owner
        {
            get { return _owner; }
        }

        internal ItemContainerGenerator Generator
        {
            get { return _generator; }
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
            set { _templateCache = (ItemsPanelTemplate)value; }
        }

        /// <summary>
        /// TemplateProperty
        /// </summary>
        internal static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register(
                nameof(Template),
                typeof(ItemsPanelTemplate),
                typeof(ItemsPresenter),
                new PropertyMetadata(null, OnTemplateChanged));

        /// <summary>
        /// Template Property
        /// </summary>
        internal ItemsPanelTemplate Template
        {
            get { return _templateCache; }
            set { SetValueInternal(TemplateProperty, value); }
        }

        private static void OnTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsPresenter ip = (ItemsPresenter)d;
            ip.ClearPanel();
            UpdateTemplateCache(ip, (FrameworkTemplate)e.OldValue, (FrameworkTemplate)e.NewValue, TemplateProperty);

            ip.InvalidateMeasure();
        }

        private void ClearPanel()
        {
            Panel oldPanel = this.TemplateChild as Panel;
            if (oldPanel != null)
            {
                oldPanel.IsItemsHost = false;
            }
        }

        private void AttachToOwner()
        {
            ItemsControl owner = this.TemplatedParent as ItemsControl;
            ItemContainerGenerator generator = null;

            if (owner != null)
            {
                // top-level presenter - get information from ItemsControl
                generator = owner.ItemContainerGenerator;
            }

            this._owner = owner;
            this.UseGenerator(generator);

            ItemsPanelTemplate template = null;
            if (this._owner != null)
            {
                // create the panel, based on ItemsControl.ItemsPanel
                template = this._owner.ItemsPanel;
            }

            this.Template = template;
        }

        private void UseGenerator(ItemContainerGenerator generator)
        {
            if (generator == this._generator)
                return;

            if (this._generator != null)
                this._generator.PanelChanged -= new EventHandler(this.OnPanelChanged);

            this._generator = generator;

            if (this._generator != null)
                this._generator.PanelChanged += new EventHandler(this.OnPanelChanged);
        }

        private void OnPanelChanged(object sender, EventArgs e)
        {
            // something has changed that affects the ItemsPresenter.
            // Re-measure.  This will recalculate everything from scratch.
            InvalidateMeasure();
        }

        internal static ItemsPresenter FromPanel(Panel panel)
        {
            if (panel == null)
                return null;

            return panel.TemplatedParent as ItemsPresenter;
        }

        /// <summary>
        /// Called when the Template's tree is about to be generated
        /// </summary>
        internal override void OnPreApplyTemplate()
        {
            base.OnPreApplyTemplate();
            this.AttachToOwner();
        }

        public override void OnApplyTemplate()
        {
            // verify that the template produced a panel with no children
            Panel panel = this.TemplateChild as Panel;
            if (panel == null || panel.HasChildren)
            {
                throw new InvalidOperationException("Content of ItemsPanelTemplate must be a single Panel (with no children).");
            }

            this.OnPanelChanged(this, EventArgs.Empty);

            base.OnApplyTemplate();
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size availableSize)
        {
            int count = VisualChildrenCount;

            if (count > 0)
            {
                UIElement child = GetVisualChild(0);
                if (child != null)
                {
                    child.Measure(availableSize);
                    return child.DesiredSize;
                }
            }

            return new Size();
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            int count = VisualChildrenCount;

            if (count > 0)
            {
                UIElement child = GetVisualChild(0);
                if (child != null)
                {
                    child.Arrange(new Rect(finalSize));
                }
            }
            return finalSize;
        }
    }
}