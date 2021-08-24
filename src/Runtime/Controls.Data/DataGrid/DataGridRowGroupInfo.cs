// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

#if MIGRATION
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    internal class DataGridRowGroupInfo
    {
        public DataGridRowGroupInfo(
            CollectionViewGroup collectionViewGroup, 
            Visibility visibility, 
            int level,
            int slot,
            int lastSubItemSlot)
        {
            this.CollectionViewGroup = collectionViewGroup;
            this.Visibility = visibility;
            this.Level = level;
            this.Slot = slot;
            this.LastSubItemSlot = lastSubItemSlot;
        }

        public CollectionViewGroup CollectionViewGroup
        {
            get;
            private set;
        }

        public int LastSubItemSlot
        {
            get;
            set;
        }

        public int Level
        {
            get;
            private set;
        }

        public int Slot
        {
            get;
            set;
        }

        public Visibility Visibility
        {
            get;
            set;
        }
    }
}
