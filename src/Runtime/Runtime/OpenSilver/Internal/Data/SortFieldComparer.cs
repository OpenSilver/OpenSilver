using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace OpenSilver.Internal.Data;

/// <summary>
/// IComparer class to sort by class property value (using reflection).
/// </summary>
internal sealed class SortFieldComparer : IComparer
{
    /// <summary>
    /// Create a comparer, using the SortDescription and a Type;
    /// tries to find a reflection PropertyInfo for each property name
    /// </summary>
    /// <param name="collectionView">CollectionView that contains list of property names and direction to sort by</param>
    public SortFieldComparer(ICollectionView collectionView)
    {
        _sortFields = collectionView.SortDescriptions;
        _fields = CreatePropertyInfo(_sortFields);
        _comparer = CultureSensitiveComparer.GetComparer(collectionView.Culture);
    }

    /// <summary>
    /// Compares two objects and returns a value indicating whether one is less than, equal to or greater than the other.
    /// </summary>
    /// <param name="x">first item to compare</param>
    /// <param name="y">second item to compare</param>
    /// <returns>Negative number if x is less than y, zero if equal, and a positive number if x is greater than y</returns>
    /// <remarks>
    /// Compares the 2 items using the list of property names and directions.
    /// </remarks>
    public int Compare(object x, object y)
    {
        int result = 0;

        // compare both objects by each of the properties until property values don't match
        for (int k = 0; k < _fields.Length; ++k)
        {
            // if the property type is not yet determined, try
            // obtaining it from the objects
            Type propertyType = _fields[k].PropertyType;
            if (propertyType is null)
            {
                if (x is not null)
                {
                    _fields[k].PropertyType = TypeHelper.GetNestedPropertyType(x.GetType(), _fields[k].PropertyPath);
                    propertyType = _fields[k].PropertyType;
                }
                if (_fields[k].PropertyType is null && y is not null)
                {
                    _fields[k].PropertyType = TypeHelper.GetNestedPropertyType(y.GetType(), _fields[k].PropertyPath);
                    propertyType = _fields[k].PropertyType;
                }
            }

            object v1 = _fields[k].GetValue(x);
            object v2 = _fields[k].GetValue(y);

            // this will handle the case with string comparisons
            if (propertyType == typeof(string))
            {
                result = _comparer.Compare(v1, v2);
            }
            else
            {
                // try to also set the value for the comparer if this was 
                // not already calculated
                IComparer comparer = _fields[k].Comparer;
                if (propertyType is not null && comparer is null)
                {
                    _fields[k].Comparer = typeof(Comparer<>).MakeGenericType(propertyType).GetProperty("Default").GetValue(null, null) as IComparer;
                    comparer = _fields[k].Comparer;
                }

                result = (comparer is not null) ? comparer.Compare(v1, v2) : 0 /*both values equal*/;
            }

            if (_fields[k].Descending)
            {
                result = -result;
            }

            if (result != 0)
            {
                break;
            }
        }

        return result;
    }

    /// <summary>
    /// Steps through the given list using the comparer to find where
    /// to insert the specified item to maintain sorted order
    /// </summary>
    /// <param name="x">Item to insert into the list</param>
    /// <param name="list">List where we want to insert the item</param>
    /// <returns>Index where we should insert into</returns>
    public int FindInsertIndex(object x, IList list)
    {
        int min = 0;
        int max = list.Count - 1;
        int index;

        // run a binary search to find the right index
        // to insert into.
        while (min <= max)
        {
            index = (min + max) / 2;

            int result = Compare(x, list[index]);
            if (result == 0)
            {
                return index;
            }
            else if (result > 0)
            {
                min = index + 1;
            }
            else
            {
                max = index - 1;
            }
        }

        return min;
    }

    private static SortPropertyInfo[] CreatePropertyInfo(SortDescriptionCollection sortFields)
    {
        SortPropertyInfo[] fields = new SortPropertyInfo[sortFields.Count];
        for (int k = 0; k < sortFields.Count; ++k)
        {
            // remember PropertyPath and Direction, used when actually sorting
            fields[k].PropertyPath = sortFields[k].PropertyName;
            fields[k].Descending = sortFields[k].Direction == ListSortDirection.Descending;
        }
        return fields;
    }

    /// <summary>
    /// Helper for SortList to handle nested properties (e.g. Address.Street)
    /// </summary>
    /// <param name="item">parent object</param>
    /// <param name="propertyPath">property names path</param>
    /// <param name="propertyType">property type that we want to check for</param>
    /// <returns>child object</returns>
    internal static object InvokePath(object item, string propertyPath, Type propertyType)
    {
        object propertyValue = TypeHelper.GetNestedPropertyValue(item, propertyPath, propertyType, out Exception exception);
        if (exception is not null)
        {
            throw exception;
        }
        return propertyValue;
    }

    private struct SortPropertyInfo
    {
        internal IComparer Comparer;
        internal bool Descending;
        internal string PropertyPath;
        internal Type PropertyType;

        internal object GetValue(object o)
        {
            object value;
            if (string.IsNullOrEmpty(PropertyPath))
            {
                value = (PropertyType == o.GetType()) ? o : null;
            }
            else
            {
                value = InvokePath(o, PropertyPath, PropertyType);
            }

            return value;
        }
    }

    private readonly SortPropertyInfo[] _fields;
    private readonly SortDescriptionCollection _sortFields;
    private readonly IComparer<object> _comparer;
}
