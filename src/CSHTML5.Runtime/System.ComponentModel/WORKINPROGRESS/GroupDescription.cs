

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


#if WORKINPROGRESS

using System.Collections.ObjectModel;
using System.Globalization;

namespace System.ComponentModel
{
    public abstract partial class GroupDescription : INotifyPropertyChanged
    {
        protected event PropertyChangedEventHandler PropertyChanged;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }

        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.GroupDescription class.
        protected GroupDescription()
        {
            
        }

        //
        // Summary:
        //     Gets the collection of group names.
        //
        // Returns:
        //     The collection of group names.
        public ObservableCollection<object> GroupNames { get; }

        //
        // Summary:
        //     Returns the group name or names for the specified item.
        //
        // Parameters:
        //   item:
        //     The item to return the group name for.
        //
        //   level:
        //     The level of the group within the grouping hierarchy.
        //
        //   culture:
        //     The culture information that affects grouping.
        //
        // Returns:
        //     An object that represents the group name or names.
        public abstract object GroupNameFromItem(object item, int level, CultureInfo culture);
        //
        // Summary:
        //     Indicates whether the specified item belongs in the specified group.
        //
        // Parameters:
        //   groupName:
        //     The name of the group to check.
        //
        //   itemName:
        //     The name of the item to check.
        //
        // Returns:
        //     true if the item belongs in the group; otherwise, false.
        public virtual bool NamesMatch(object groupName, object itemName)
        {
            return default(bool);
        }
        //
        // Summary:
        //     Indicates whether the group names should be serialized.
        //
        // Returns:
        //     true if the group names should be serialized; otherwise, false.
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeGroupNames()
        {
            return default(bool);
        }
        //
        // Summary:
        //     Raises the System.ComponentModel.GroupDescription.PropertyChanged event.
        //
        // Parameters:
        //   e:
        //     The event data.
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            
        }
    }
}

#endif