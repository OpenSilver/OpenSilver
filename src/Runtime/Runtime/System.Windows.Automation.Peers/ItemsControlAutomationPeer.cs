
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
using System.Collections.Generic;

#if MIGRATION
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Automation.Peers
#else
namespace Windows.UI.Xaml.Automation.Peers
#endif
{
    /// <summary>
    /// Exposes <see cref="ItemsControl"/> types to UI automation.
    /// </summary>
    public abstract class ItemsControlAutomationPeer : FrameworkElementAutomationPeer
    {
        /// <summary>
        /// Provides initialization for base class values when called by the constructor of a derived class.
        /// </summary>
        /// <param name="owner">
        /// The <see cref="ItemsControl"/> to associate with this <see cref="ItemsControlAutomationPeer"/>.
        /// </param>
        protected ItemsControlAutomationPeer(ItemsControl owner)
            : base(owner)
        {
        }

        /// <summary>
        /// Gets an object that implements the requested pattern, based on the patterns supported
        /// by this <see cref="ItemsControlAutomationPeer"/>.
        /// </summary>
        /// <param name="patternInterface">
        /// One of the enumeration values that indicates the desired control pattern.
        /// </param>
        /// <returns>
        /// The object that implements the pattern interface, or null if the specified pattern
        /// interface is not implemented by this peer.
        /// </returns>
        public override object GetPattern(PatternInterface patternInterface)
        {
            if (patternInterface == PatternInterface.Scroll)
            {
                ItemsControl owner = (ItemsControl)Owner;
                UIElement uie = owner.ItemsHost;
                while (uie != null && uie != owner)
                {
                    uie = VisualTreeHelper.GetParent(uie) as UIElement;
                    if (uie is ScrollViewer)
                    {
                        AutomationPeer peer = CreatePeerForElement(uie);
                        if (peer is IScrollProvider)
                        {
                            return peer;
                        }

                        break;
                    }
                }
            }

            return base.GetPattern(patternInterface);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ItemAutomationPeer"/> for a data item.
        /// The item exists in the <see cref="ItemsControl.Items"/> collection of the associated 
        /// <see cref="ItemsControl"/> owner.
        /// </summary>
        /// <param name="item">
        /// The data item that is associated with this <see cref="ItemAutomationPeer"/>.
        /// </param>
        /// <returns>
        ///  An object that exposes the data item to UI automation, or null. See Remarks.
        /// </returns>
        /// <remarks>
        /// An <see cref="ItemsControl"/> is capable of containing data items that are not a 
        /// <see cref="UIElement"/>, or that do not have an associated peer class per the 
        /// <see cref="UIElement.OnCreateAutomationPeer"/> implementation. In these cases, 
        /// <see cref="CreateItemAutomationPeer"/> returns null.
        /// </remarks>
        protected virtual ItemAutomationPeer CreateItemAutomationPeer(object item)
            => item is UIElement uie ? CreatePeerForElement(uie) as ItemAutomationPeer : null;

        /// <summary>
        /// Gets automation peers for the collection of child elements of the element that
        /// is associated with this <see cref="ItemsControlAutomationPeer"/> .
        /// </summary>
        /// <returns>
        /// A list of automation peers for child elements.
        /// </returns>
        protected override List<AutomationPeer> GetChildrenCore()
        {
            List<AutomationPeer> children = null;

            ItemsControl owner = (ItemsControl)Owner;
            ItemCollection items = owner.Items;
            int count = items.Count;
            if (count > 0)
            {
                children = new List<AutomationPeer>(count);
                foreach (object item in items)
                {
                    ItemAutomationPeer peer = CreateItemAutomationPeerHelper(item);
                    if (peer != null)
                    {
                        children.Add(peer);
                    }
                }
            }

            return children;
        }

        internal ItemAutomationPeer CreateItemAutomationPeerHelper(object item)
        {
            ItemAutomationPeer peer;

            if (((ItemsControl)Owner).ItemContainerGenerator.ContainerFromItem(item) is UIElement container)
            {
                peer = CreatePeerForElement(container) as ItemAutomationPeer;
            }
            else
            {
                peer = CreateItemAutomationPeer(item);
            }

            if (peer != null && peer.ItemsControlAutomationPeer is null)
            {
                peer.ItemsControlAutomationPeer = this;
            }

            return peer;
        }
    }
}
