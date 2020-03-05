#if WORKINPROGRESS

using System;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel
{
    public partial struct SortDescription
    {
		/// <summary>
		/// Initializes a new instance of the System.ComponentModel.SortDescription structure.
		/// </summary>
		/// <param name="propertyName">The name of the property to sort the list by.</param>
		/// <param name="direction">The sort order.</param>
		/// <exception cref="ArgumentNullException">The propertyName parameter is null.</exception>
		/// <exception cref="ArgumentException">The propertyName parameter is empty.-or-The direction parameter does not specify a valid value.</exception>
		public SortDescription(string propertyName, ListSortDirection direction)
		{
			PropertyName = propertyName;
			Direction = direction;
			IsSealed = false;
		}

		/// <summary>
		/// Gets or sets a value that indicates whether to sort in ascending or descending order.
		/// </summary>
		/// <returns>A value that indicates the sort direction.</returns>
		/// <exception cref="InvalidOperationException">System.ComponentModel.SortDescription.IsSealed is true.</exception>
		/// <exception cref="ArgumentException">The specified value is not a valid sort direction.</exception>
		public ListSortDirection Direction { get; set; }
		
		/// <summary>
		/// Gets a value that indicates whether this structure is in an immutable state.
		/// </summary>
		/// <returns>true if this object is being used; otherwise, false.</returns>
		public bool IsSealed { get; }
		
		/// <summary>
		/// Gets or sets the property name being used as the sorting criteria.
		/// </summary>
		/// <returns>The name of the property to sort by.</returns>
		/// <exception cref="InvalidOperationException">System.ComponentModel.SortDescription.IsSealed is true.</exception>
		public string PropertyName { get; set; }

		/// <summary>
		/// Compares the specified instance and the current instance of System.ComponentModel.SortDescription for value equality.
		/// </summary>
		/// <param name="obj">The System.ComponentModel.SortDescription instance to compare.</param>
		/// <returns>
		/// true if obj and this System.ComponentModel.SortDescription instance have the
		/// same System.ComponentModel.SortDescription.PropertyName and System.ComponentModel.SortDescription.Direction
		/// values; otherwise, false.
		/// </returns>
		public override bool Equals(object obj)
		{
			return default(bool);
		}

		/// <summary>
		/// Returns the hash code for the current instance.
		/// </summary>
		/// <returns>The hash code for the current instance.</returns>
		public override int GetHashCode()
		{
			return default(int);
		}

		/// <summary>
		/// Compares two System.ComponentModel.SortDescription instances for value equality.
		/// </summary>
		/// <param name="sd1">The first System.ComponentModel.SortDescription instance to compare.</param>
		/// <param name="sd2">The second System.ComponentModel.SortDescription instance to compare.</param>
		/// <returns>
		/// true if the two System.ComponentModel.SortDescription instances have the same
		/// System.ComponentModel.SortDescription.PropertyName and System.ComponentModel.SortDescription.Direction
		/// values; otherwise, false.
		/// </returns>
		public static bool operator ==(SortDescription sd1, SortDescription sd2)
		{
			return default(bool);
		}

		/// <summary>
		/// Compares two System.ComponentModel.SortDescription instances for value inequality.
		/// </summary>
		/// <param name="sd1">The first System.ComponentModel.SortDescription instance to compare.</param>
		/// <param name="sd2">The second System.ComponentModel.SortDescription instance to compare.</param>
		/// <returns>
		/// true if the two System.ComponentModel.SortDescription instances do not have the
		/// same System.ComponentModel.SortDescription.PropertyName and System.ComponentModel.SortDescription.Direction
		/// values; otherwise, false.
		/// </returns>
		public static bool operator !=(SortDescription sd1, SortDescription sd2)
		{
			return default(bool);
		}
    }
}

#endif