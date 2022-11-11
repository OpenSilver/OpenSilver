
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
using System.Diagnostics;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public partial class ItemsControl
    {
        // create an ItemInfo with as much information as can be deduced
        internal ItemInfo NewItemInfo(object item, DependencyObject container = null, int index = -1)
        {
            return new ItemInfo(item, container, index).Refresh(ItemContainerGenerator);
        }

        // create an ItemInfo for the given container
        internal ItemInfo ItemInfoFromContainer(DependencyObject container)
        {
            return NewItemInfo(ItemContainerGenerator.ItemFromContainer(container), container, ItemContainerGenerator.IndexFromContainer(container));
        }

        // create an ItemInfo for the given index
        internal ItemInfo ItemInfoFromIndex(int index)
        {
            return (index >= 0) ? NewItemInfo(Items[index], ItemContainerGenerator.ContainerFromIndex(index), index)
                                : null;
        }

        // create an unresolved ItemInfo
        internal ItemInfo NewUnresolvedItemInfo(object item)
        {
            return new ItemInfo(item, ItemInfo.UnresolvedContainer, -1);
        }

        [DebuggerDisplay("Index: {Index}  Item: {Item}")]
        internal sealed class ItemInfo
        {
            internal object Item { get; private set; }
            internal DependencyObject Container { get; set; }
            internal int Index { get; set; }

            internal static readonly DependencyObject SentinelContainer = new DependencyObject();
            internal static readonly DependencyObject UnresolvedContainer = new DependencyObject();
            internal static readonly DependencyObject KeyContainer = new DependencyObject();
            internal static readonly DependencyObject RemovedContainer = new DependencyObject();

            public ItemInfo(object item, DependencyObject container = null, int index = -1)
            {
                Item = item;
                Container = container;
                Index = index;
            }

            internal bool IsResolved { get { return Container != UnresolvedContainer; } }
            internal bool IsKey { get { return Container == KeyContainer; } }
            internal bool IsRemoved { get { return Container == RemovedContainer; } }

            internal ItemInfo Clone()
            {
                return new ItemInfo(Item, Container, Index);
            }

            internal static ItemInfo Key(ItemInfo info)
            {
                return (info.Container == UnresolvedContainer)
                    ? new ItemInfo(info.Item, KeyContainer, -1)
                    : info;
            }

            public override int GetHashCode()
            {
                return (Item != null) ? Item.GetHashCode() : 314159;
            }

            public override bool Equals(object o)
            {
                if (o == (object)this)
                    return true;

                ItemInfo that = o as ItemInfo;
                if (that == null)
                    return false;

                return Equals(that, matchUnresolved: false);
            }

            internal bool Equals(ItemInfo that, bool matchUnresolved)
            {
                // Removed matches nothing
                if (this.IsRemoved || that.IsRemoved)
                    return false;

                // items must match
                if (!ItemsControl.EqualsEx(this.Item, that.Item))
                    return false;

                // Key matches anything, except Unresolved when matchUnresovled is false
                if (this.Container == KeyContainer)
                    return matchUnresolved || that.Container != UnresolvedContainer;
                else if (that.Container == KeyContainer)
                    return matchUnresolved || this.Container != UnresolvedContainer;

                // Unresolved matches nothing
                if (this.Container == UnresolvedContainer || that.Container == UnresolvedContainer)
                    return false;

                return
                    (this.Container == that.Container)
                     ? (this.Container == SentinelContainer)
                         ? (this.Index == that.Index)      // Sentinel => negative indices are significant
                         : (this.Index < 0 || that.Index < 0 ||
                                this.Index == that.Index)   // ~Sentinel => ignore negative indices
                     : (this.Container == SentinelContainer) ||    // sentinel matches non-sentinel
                        (that.Container == SentinelContainer) ||
                        ((this.Container == null || that.Container == null) &&   // null matches non-null
                            (this.Index < 0 || that.Index < 0 ||                    // provided that indices match
                                this.Index == that.Index));
            }

            public static bool operator ==(ItemInfo info1, ItemInfo info2)
            {
                return Object.Equals(info1, info2);
            }

            public static bool operator !=(ItemInfo info1, ItemInfo info2)
            {
                return !Object.Equals(info1, info2);
            }

            // update container and index with current values
            internal ItemInfo Refresh(ItemContainerGenerator generator)
            {
                if (Container == null && Index < 0)
                {
                    Container = generator.ContainerFromItem(Item);
                }

                if (Index < 0 && Container != null)
                {
                    Index = generator.IndexFromContainer(Container);
                }

                if (Container == null && Index >= 0)
                {
                    Container = generator.ContainerFromIndex(Index);
                }

                if (Container == SentinelContainer && Index >= 0)
                {
                    Container = null;   // caller explicitly wants null container
                }

                return this;
            }

            // Don't call this on entries used in hashtables - it changes the hashcode
            internal void Reset(object item)
            {
                Item = item;
            }
        }
    }
}
