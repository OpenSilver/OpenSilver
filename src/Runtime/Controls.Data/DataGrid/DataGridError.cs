// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Globalization;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
internal static class DataGridError
    {
        public static class DataGrid
        {
            public static InvalidOperationException CannotChangeItemsWhenLoadingRows()
            {
                return new InvalidOperationException(Resource.DataGrid_CannotChangeItemsWhenLoadingRows);
            }

            public static InvalidOperationException CannotChangeColumnCollectionWhileAdjustingDisplayIndexes()
            {
                return new InvalidOperationException(Resource.DataGrid_CannotChangeColumnCollectionWhileAdjustingDisplayIndexes);
            }

            public static InvalidOperationException ColumnCannotBeCollapsed()
            {
                return new InvalidOperationException(Resource.DataGrid_ColumnCannotBeCollapsed);
            }

            public static InvalidOperationException ColumnCannotBeReassignedToDifferentDataGrid()
            {
                return new InvalidOperationException(Resource.DataGrid_ColumnCannotBeReassignedToDifferentDataGrid);
            }

            public static ArgumentException ColumnNotInThisDataGrid()
            {
                return new ArgumentException(Resource.DataGrid_ColumnNotInThisDataGrid);
            }

            public static ArgumentException ItemIsNotContainedInTheItemsSource(string paramName)
            {
                return new ArgumentException(Resource.DataGrid_ItemIsNotContainedInTheItemsSource, paramName);
            }

            public static InvalidOperationException NoCurrentRow()
            {
                return new InvalidOperationException(Resource.DataGrid_NoCurrentRow);
            }

            public static InvalidOperationException NoOwningGrid(Type type)
            {
                return new InvalidOperationException(Format(Resource.DataGrid_NoOwningGrid, type.FullName));
            }

            public static InvalidOperationException UnderlyingPropertyIsReadOnly(string paramName)
            {
                return new InvalidOperationException(Format(Resource.DataGrid_UnderlyingPropertyIsReadOnly, paramName));
            }

            public static ArgumentException ValueCannotBeSetToInfinity(string paramName)
            {
                return new ArgumentException(Format(Resource.DataGrid_ValueCannotBeSetToInfinity, paramName));
            }

            public static ArgumentException ValueCannotBeSetToNAN(string paramName)
            {
                return new ArgumentException(Format(Resource.DataGrid_ValueCannotBeSetToNAN, paramName));
            }

            public static ArgumentNullException ValueCannotBeSetToNull(string paramName, string valueName)
            {
                return new ArgumentNullException(paramName, Format(Resource.DataGrid_ValueCannotBeSetToNull, valueName));
            }

            public static ArgumentException ValueIsNotAnInstanceOf(string paramName, Type type)
            {
                return new ArgumentException(paramName, Format(Resource.DataGrid_ValueIsNotAnInstanceOf, type.FullName));
            }

            public static ArgumentException ValueIsNotAnInstanceOfEitherOr(string paramName, Type type1, Type type2)
            {
                return new ArgumentException(paramName, Format(Resource.DataGrid_ValueIsNotAnInstanceOfEitherOr, type1.FullName, type2.FullName));
            }

            public static ArgumentOutOfRangeException ValueMustBeBetween(string paramName, string valueName, object lowValue, bool lowInclusive, object highValue, bool highInclusive)
            {
                string message = null;

                if (lowInclusive && highInclusive)
                {
                    message = Resource.DataGrid_ValueMustBeGTEandLTE;
                }
                else if (lowInclusive && !highInclusive)
                {
                    message = Resource.DataGrid_ValueMustBeGTEandLT;
                }
                else if (!lowInclusive && highInclusive)
                {
                    message = Resource.DataGrid_ValueMustBeGTandLTE;
                }
                else
                {
                    message = Resource.DataGrid_ValueMustBeGTandLT;
                }

                return new ArgumentOutOfRangeException(paramName, Format(message, valueName, lowValue, highValue));
            }

            public static ArgumentOutOfRangeException ValueMustBeGreaterThanOrEqualTo(string paramName, string valueName, object value)
            {
                return new ArgumentOutOfRangeException(paramName, Format(Resource.DataGrid_ValueMustBeGreaterThanOrEqualTo, valueName, value));
            }

            public static ArgumentOutOfRangeException ValueMustBeLessThanOrEqualTo(string paramName, string valueName, object value)
            {
                return new ArgumentOutOfRangeException(paramName, Format(Resource.DataGrid_ValueMustBeLessThanOrEqualTo, valueName, value));
            }

            public static ArgumentOutOfRangeException ValueMustBeLessThan(string paramName, string valueName, object value)
            {
                return new ArgumentOutOfRangeException(paramName, Format(Resource.DataGrid_ValueMustBeLessThan, valueName, value));
            }

        }

        // 








        public static class DataGridColumnHeader
        {
            public static NotSupportedException ContentDoesNotSupportUIElements()
            {
                return new NotSupportedException(Resource.DataGridColumnHeader_ContentDoesNotSupportUIElements);
            }
        }

        public static class DataGridLength
        {
            public static ArgumentException InvalidUnitType(string paramName)
            {
                return new ArgumentException(Format(Resource.DataGridLength_InvalidUnitType, paramName), paramName);
            }
        }

        public static class DataGridLengthConverter
        {
            public static NotSupportedException CannotConvertFrom(string paramName)
            {
                return new NotSupportedException(Format(Resource.DataGridLengthConverter_CannotConvertFrom, paramName));
            }

            public static NotSupportedException CannotConvertTo(string paramName)
            {
                return new NotSupportedException(Format(Resource.DataGridLengthConverter_CannotConvertTo, paramName));
            }

            public static NotSupportedException InvalidDataGridLength(string paramName)
            {
                return new NotSupportedException(Format(Resource.DataGridLengthConverter_InvalidDataGridLength, paramName));
            }
        }

        public static class DataGridRow
        {
            public static InvalidOperationException InvalidRowIndexCannotCompleteOperation()
            {
                return new InvalidOperationException(Resource.DataGridRow_InvalidRowIndexCannotCompleteOperation);
            }
        }

        public static class DataGridSelectedItemsCollection
        {
            public static InvalidOperationException CannotChangeSelectedItemsCollectionInSingleMode()
            {
                return new InvalidOperationException(Resource.DataGridSelectedItemsCollection_CannotChangeSelectedItemsCollectionInSingleMode);
            }
        }

        public static class DataGridTemplateColumn
        {
            public static TypeInitializationException MissingTemplateForType(Type type)
            {
                return new TypeInitializationException(Format(Resource.DataGridTemplateColumn_MissingTemplateForType, type.FullName), null);
            }
        }

        private static string Format(string formatString, params object[] args)
        {
            return String.Format(CultureInfo.CurrentCulture, formatString, args);
        }
    }
}
