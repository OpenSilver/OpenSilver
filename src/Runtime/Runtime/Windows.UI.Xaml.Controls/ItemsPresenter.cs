

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
        private Panel _itemsHost; // template child

        internal sealed override FrameworkElement TemplateChild
        {
            get { return base.TemplateChild; }
            set
            {
                Panel panel = null;
                if (value != null)
                {
                    panel = value as Panel;
                    if (panel == null)
                    {
                        throw new InvalidOperationException(string.Format("VisualTree of ItemsPanelTemplate must contain a Panel. '{0}' is not a Panel.", value.GetType()));
                    }
                    panel.IsItemsHost = true;
                }
                this._itemsHost = panel;
                base.TemplateChild = value;
            }
        }

        internal ItemsControl Owner
        {
            get { return _owner; }
        }

        internal Panel ItemsHost
        {
            get { return _itemsHost; }
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

            if (VisualTreeHelper.GetParent(ip) != null)
            {
                ip.InvalidateMeasureInternal();
            }
        }

        private void ClearPanel()
        {
            Panel oldPanel = this.ItemsHost;
            if (oldPanel != null)
            {
                oldPanel.IsItemsHost = false;
            }
        }

        private void AttachToOwner()
        {
            // Find the templated parent
            // todo: change this once FrameworkElement.TemplatedParent is implemented.
            ItemsControl owner = this._owner;
            DependencyObject elt = this;
            while (owner == null)
            {
                // First try to find the visual parent.
                DependencyObject parent = VisualTreeHelper.GetParent(elt);

                // If visual parent is null, it could be that the element is the content
                // of popup (cf. ComboBox default style), so we handle this case here.
                if (parent == null)
                {
                    parent = (elt as PopupRoot)?.INTERNAL_LinkedPopup;
                    if (parent == null)
                    {
                        // no more visual parent, exit.
                        break;
                    }
                }

                owner = parent as ItemsControl;
                elt = parent;
            }

            this._owner = owner;

            ItemsPanelTemplate template = null;
            if (this._owner != null)
            {
                this._owner.ItemsPresenter = this;

                // create the panel, based on ItemsControl.ItemsPanel
                template = this._owner.ItemsPanel;
            }

            this.Template = template;
        }

        internal static ItemsPresenter FromPanel(Panel panel)
        {
            if (panel == null)
                return null;

            return VisualTreeHelper.GetParent(panel) as ItemsPresenter;
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
            base.OnApplyTemplate();

            // verify that the template produced a panel with no children
            Panel panel = this.ItemsHost;
            if (panel == null || panel.HasChildren)
            {
                throw new InvalidOperationException("Content of ItemsPanelTemplate must be a single Panel (with no children).");
            }

            // Attach children to panel
            this.Owner.Refresh(false);
        }
    }
}