
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

#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Controls.Primitives;
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
    /// Displays the content of a ItemsPresenter.
    /// </summary>
    public partial class ItemsPresenter : FrameworkElement
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
            set { SetValue(TemplateProperty, value); }
        }

        private static void OnTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemsPresenter ip = (ItemsPresenter)d;
            ip.ClearPanel();
            FrameworkElement.UpdateTemplateCache(ip, (FrameworkTemplate)e.OldValue, (FrameworkTemplate)e.NewValue, TemplateProperty);

            if (ip.IsConnectedToLiveTree)
            {
                ip.InvalidateMeasureInternal();
            }
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
            this.InvalidateMeasureInternal();
            _ = (this.TemplateChild as Panel)?.Children;
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

#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
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

        protected override Size MeasureOverride(Size availableSize)
        {
            FrameworkElement child = this.TemplateChild;
            if (child == null)
            {
                return new Size();
            }

            child.Measure(availableSize);
            return child.DesiredSize;
        }
    }
}