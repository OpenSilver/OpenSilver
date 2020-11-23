

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

        private new Panel TemplateChild
        {
            get { return this._itemsHost; }
            set 
            {
                this._itemsHost = value;
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

            // Update template cache
            ip._templateCache = (ItemsPanelTemplate)e.NewValue;

            ip.ClearPanel();

            if (VisualTreeHelper.GetParent(ip) != null)
            {
                ip.InvalidateMeasureInternal();
            }
        }

        private void ClearPanel()
        {
            Panel oldPanel = this.TemplateChild;
            if (oldPanel != null)
            {
                oldPanel.IsItemsHost = false;
            }
            this.TemplateChild = null;
            this.Owner.ItemContainerGenerator.INTERNAL_Clear();
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

        internal override sealed Size MeasureCore()
        {
            if (!this.ApplyTemplate())
            {
                if (this.TemplateChild != null)
                {
                    INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(this.TemplateChild, this, 0);
                }
            }
            return new Size(0, 0);
        }

        // todo: Move Control.ApplyTemplate() over to FrameworkElement
        private bool ApplyTemplate()
        {
            bool visualsCreated = false;
            FrameworkElement visualChild = null;

            if (this._templateCache != null)
            {
                ItemsPanelTemplate template = this.Template;

                // we only apply the template if no template has been
                // rendered already for this control.
                if (this.TemplateChild == null)
                {
                    visualChild = template.INTERNAL_InstantiateFrameworkTemplate();
                    if (visualChild != null)
                    {
                        visualsCreated = true;
                    }
                }
            }

            if (visualsCreated)
            {
                Panel panel = visualChild as Panel;
                if (panel == null)
                {
                    throw new InvalidOperationException(
                        string.Format("VisualTree of ItemsPanelTemplate must contain a Panel. '{0}' is not a Panel.", 
                                      panel.GetType()));
                }
                panel.IsItemsHost = true;
                this.TemplateChild = panel;

                // Call the OnApplyTemplate method
                this.OnApplyTemplate();
            }

            return visualsCreated;
        }

        private void OnPreApplyTemplate()
        {
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
            Panel panel = this.TemplateChild;
            if (panel == null || panel.HasChildren)
            {
                throw new InvalidOperationException("Content of ItemsPanelTemplate must be a single Panel (with no children).");
            }

            // Attach children to panel
            this.Owner.Refresh(false);
        }

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();

            this.OnPreApplyTemplate();
            this.InvalidateMeasureInternal();
        }
    }
}