

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

namespace System.ComponentModel
{
    //
    // Summary:
    //     Defines the direction and the property name that will be used as the criteria
    //     for sorting a collection.
    public struct SortDescription
    {
        //
        // Summary:
        //     Initializes a new instance of the System.ComponentModel.SortDescription structure.
        //
        // Parameters:
        //   propertyName:
        //     The name of the property to sort the list by.
        //
        //   direction:
        //     The sort order.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The propertyName parameter is null.
        //
        //   T:System.ArgumentException:
        //     The propertyName parameter is empty.-or-The direction parameter does not specify
        //     a valid value.
        public SortDescription(string propertyName, ListSortDirection direction)
        {
            this.IsSealed = true;
            this.PropertyName = propertyName;
            this.Direction = direction;
        }

        //
        // Summary:
        //     Gets or sets a value that indicates whether to sort in ascending or descending
        //     order.
        //
        // Returns:
        //     A value that indicates the sort direction.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     System.ComponentModel.SortDescription.IsSealed is true.
        //
        //   T:System.ArgumentException:
        //     The specified value is not a valid sort direction.
        public ListSortDirection Direction { get; set; }
        //
        // Summary:
        //     Gets a value that indicates whether this structure is in an immutable state.
        //
        // Returns:
        //     true if this object is being used; otherwise, false.
        public bool IsSealed { get; }
        //
        // Summary:
        //     Gets or sets the property name being used as the sorting criteria.
        //
        // Returns:
        //     The name of the property to sort by.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     System.ComponentModel.SortDescription.IsSealed is true.
        public string PropertyName { get; set; }

        //
        // Summary:
        //     Compares the specified instance and the current instance of System.ComponentModel.SortDescription
        //     for value equality.
        //
        // Parameters:
        //   obj:
        //     The System.ComponentModel.SortDescription instance to compare.
        //
        // Returns:
        //     true if obj and this System.ComponentModel.SortDescription instance have the
        //     same System.ComponentModel.SortDescription.PropertyName and System.ComponentModel.SortDescription.Direction
        //     values; otherwise, false.
        public override bool Equals(object obj)
        {
            return false;
        }
        //
        // Summary:
        //     Returns the hash code for the current instance.
        //
        // Returns:
        //     The hash code for the current instance.
        public override int GetHashCode()
        {
            return 0;
        }

        //
        // Summary:
        //     Compares two System.ComponentModel.SortDescription instances for value equality.
        //
        // Parameters:
        //   sd1:
        //     The first System.ComponentModel.SortDescription instance to compare.
        //
        //   sd2:
        //     The second System.ComponentModel.SortDescription instance to compare.
        //
        // Returns:
        //     true if the two System.ComponentModel.SortDescription instances have the same
        //     System.ComponentModel.SortDescription.PropertyName and System.ComponentModel.SortDescription.Direction
        //     values; otherwise, false.
        public static bool operator ==(SortDescription sd1, SortDescription sd2)
        {
            return false;
        }
        //
        // Summary:
        //     Compares two System.ComponentModel.SortDescription instances for value inequality.
        //
        // Parameters:
        //   sd1:
        //     The first System.ComponentModel.SortDescription instance to compare.
        //
        //   sd2:
        //     The second System.ComponentModel.SortDescription instance to compare.
        //
        // Returns:
        //     true if the two System.ComponentModel.SortDescription instances do not have the
        //     same System.ComponentModel.SortDescription.PropertyName and System.ComponentModel.SortDescription.Direction
        //     values; otherwise, false.
        public static bool operator !=(SortDescription sd1, SortDescription sd2)
        {
            return false;
        }
    }
}

#endif