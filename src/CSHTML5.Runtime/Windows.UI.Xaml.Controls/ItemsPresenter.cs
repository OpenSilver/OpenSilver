

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
using System.Collections.Specialized;

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
#if WORKINPROGRESS
        private ItemsPanelTemplate _templateCache;
        private ItemsControl _owner;
        private Panel _itemsHost;

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
                DependencyProperty.Register("Template",
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

            ip.OnPanelChanged();
        }

        internal void OnPanelChanged()
        {
            if (this.ItemsHost != null)
            {
                // Panel is no longer ItemsHost
                this.ItemsHost.IsItemsHost = false;

                // Detach old panel
                INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(this.ItemsHost, this);
            }

            if (this._templateCache != null)
            {
                // Create an instance of the Panel
                FrameworkElement visualTree = this._templateCache.INTERNAL_InstantiateFrameworkTemplate();
                Panel panel = visualTree as Panel;
                if (panel != null)
                {
                    // Make sure that the panel contains no children
                    if (panel.Children.Count > 0)
                    {
                        throw new InvalidOperationException("VisualTree of ItemsPanelTemplate must be a single element.");
                    }

                    this._itemsHost = panel;

                    // set IsItemsHost flag
                    panel.IsItemsHost = true;
                }
                else
                {
                    throw new InvalidOperationException(string.Format("VisualTree of ItemsPanelTemplate must contain a Panel. '{0}' is not a Panel.", visualTree.GetType()));
                }

                // attach the newly generated panel.
#if REWORKLOADED
                this.AddVisualChild(panel);
#else
                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(panel, this);
#endif

                // Attach children to panel if any
                if (this.Owner != null && this.Owner.Items.Count > 0)
                {
                    this.Owner.UpdateChildrenInVisualTree(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            }
            else
            {
                this._itemsHost = null;
                this.Owner.ItemContainerGenerator.INTERNAL_Clear();
            }
        }

        internal void AttachToOwner(ItemsControl owner)
        {
            this._owner = owner;

            // Get Template from ItemsControl.ItemsPanel
            this.Template = owner.ItemsPanel;
        }

        protected internal override void INTERNAL_OnAttachedToVisualTree()
        {
            base.INTERNAL_OnAttachedToVisualTree();

            if (this.ItemsHost != null)
            {
#if REWORKLOADED
                this.AddVisualChild(this.ItemsHost);
#else
                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(this.ItemsHost, this);
#endif
            }
        }
#endif
    }
}