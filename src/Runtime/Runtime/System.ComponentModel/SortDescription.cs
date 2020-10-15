

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

using System.Globalization;

namespace System.ComponentModel
{
    /// <summary>
    /// Defines a property and direction to sort a list by.
    /// </summary>
    public struct SortDescription
    {
        //------------------------------------------------------
        //
        //  Public Constructors
        //
        //------------------------------------------------------

        #region Public Constructors

        /// <summary>
        /// Create a sort description.
        /// </summary>
        /// <param name="propertyName">Property to sort by</param>
        /// <param name="direction">Specifies the direction of sort operation</param>
        /// <exception cref="InvalidEnumArgumentException"> direction is not a valid value for ListSortDirection </exception>
        public SortDescription(string propertyName, ListSortDirection direction)
        {
            if (direction != ListSortDirection.Ascending && direction != ListSortDirection.Descending)
                throw new InvalidEnumArgumentException("direction", (int)direction, typeof(ListSortDirection));

            _propertyName = propertyName;
            _direction = direction;
            _sealed = false;
        }

        #endregion Public Constructors


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// Property name to sort by.
        /// </summary>
        public string PropertyName
        {
            get { return _propertyName; }
            set
            {
                if (_sealed)
                    throw new InvalidOperationException(string.Format("Cannot modify a '{0}' after it is sealed.", "SortDescription"));

                _propertyName = value;
            }
        }

        /// <summary>
        /// Sort direction.
        /// </summary>
        public ListSortDirection Direction
        {
            get { return _direction; }
            set
            {
                if (_sealed)
                    throw new InvalidOperationException(string.Format("Cannot modify a '{0}' after it is sealed.", "SortDescription"));

                if (value < ListSortDirection.Ascending || value > ListSortDirection.Descending)
                    throw new InvalidEnumArgumentException("value", (int)value, typeof(ListSortDirection));

                _direction = value;
            }
        }

        /// <summary>
        /// Returns true if the SortDescription is in use (sealed).
        /// </summary>
        public bool IsSealed
        {
            get { return _sealed; }
        }

        #endregion Public Properties

        //------------------------------------------------------
        //
        //  Public methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary> Override of Object.Equals </summary>
        public override bool Equals(object obj)
        {
            return (obj is SortDescription) ? (this == (SortDescription)obj) : false;
        }

        /// <summary> Equality operator for SortDescription. </summary>
        public static bool operator ==(SortDescription sd1, SortDescription sd2)
        {
            return sd1.PropertyName == sd2.PropertyName &&
                    sd1.Direction == sd2.Direction;
        }

        /// <summary> Inequality operator for SortDescription. </summary>
        public static bool operator !=(SortDescription sd1, SortDescription sd2)
        {
            return !(sd1 == sd2);
        }

        /// <summary> Override of Object.GetHashCode </summary>
        public override int GetHashCode()
        {
            int result = Direction.GetHashCode();
            if (PropertyName != null)
            {
                result = unchecked(PropertyName.GetHashCode() + result);
            }
            return result;
        }

        #endregion Public Methods

        //------------------------------------------------------
        //
        //  Internal methods
        //
        //------------------------------------------------------

        #region Internal Methods

        internal void Seal()
        {
            _sealed = true;
        }

        #endregion Internal Methods

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private string _propertyName;
        private ListSortDirection _direction;
        bool _sealed;

        #endregion Private Fields
    }
}

#if !NETSTANDARD

namespace System.ComponentModel
{
    public class InvalidEnumArgumentException : ArgumentException
    {
        /// <devdoc>
        /// <para>Initializes a new instance of the <see cref='System.ComponentModel.InvalidEnumArgumentException'/> class without a message.</para>
        /// </devdoc>
        public InvalidEnumArgumentException() : this(null)
        {
        }

        /// <devdoc>
        /// <para>Initializes a new instance of the <see cref='System.ComponentModel.InvalidEnumArgumentException'/> class with 
        ///    the specified message.</para>
        /// </devdoc>
        public InvalidEnumArgumentException(string message)
            : base(message)
        {
        }

        /// <devdoc>
        ///     Initializes a new instance of the Exception class with a specified error message and a 
        ///     reference to the inner exception that is the cause of this exception.
        ///     FxCop CA1032: Multiple constructors are required to correctly implement a custom exception.
        /// </devdoc>
        public InvalidEnumArgumentException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <devdoc>
        /// <para>Initializes a new instance of the <see cref='System.ComponentModel.InvalidEnumArgumentException'/> class with a 
        ///    message generated from the argument, invalid value, and enumeration
        ///    class.</para>
        /// </devdoc>
        public InvalidEnumArgumentException(string argumentName, int invalidValue, Type enumClass)
            : base(string.Format("The value of argument '{0}' ({1}) is invalid for Enum type '{2}'.",
                                 argumentName,
                                 invalidValue.ToString(),
                                 enumClass.Name), argumentName)
        {
        }
    }
}

#endif