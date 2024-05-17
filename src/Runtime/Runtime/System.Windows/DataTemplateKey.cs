
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

using System.Runtime.CompilerServices;

namespace System.Windows
{
    /// <summary>
    /// Represents the resource key for the <see cref="DataTemplate"/> class.
    /// </summary>
    public class DataTemplateKey
    {
        private object _dataType;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTemplateKey"/> class without
        /// initializing the <see cref="DataType"/> property.
        /// </summary>
        public DataTemplateKey() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTemplateKey"/> class, setting
        /// the <see cref="DataType"/> property to the specified value.
        /// </summary>
        /// <param name="dataType">
        /// The initial value of the <see cref="DataTemplate.DataType"/> property.
        /// </param>
        /// <exception cref="ArgumentException">
        /// dataType is not of type <see cref="Type"/>.
        /// </exception>
        public DataTemplateKey(object dataType)
        {
            DataType = dataType;
        }

        /// <summary>
        /// Gets or sets the type for which the template is intended.
        /// </summary>
        /// <returns>
        /// The type of object to which the template is applied.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// The value is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// When setting this property, the specified value is not of type <see cref="Type"/>.
        /// </exception>
        public object DataType
        {
            get => _dataType;
            set
            {
                if (ValidateDataType(value) is Exception ex)
                {
                    throw ex;
                }

                _dataType = value;
            }
        }

        /// <summary>
        /// Returns a value that indicates whether the specified object is a <see cref="DataTemplateKey"/>
        /// that has the same <see cref="DataType"/> property value as the current <see cref="DataTemplateKey"/>.
        /// </summary>
        /// <param name="o">
        /// The object to compare to the current object.
        /// </param>
        /// <returns>
        /// true if o is equivalent to the current object; otherwise, false.
        /// </returns>
        public override bool Equals(object o) => o is DataTemplateKey key && key._dataType == _dataType;

        /// <summary>
        /// Returns the hash code of the <see cref="DataType"/> property
        /// value.
        /// </summary>
        /// <returns>
        /// The hash code of the <see cref="DataType"/> property value,
        /// or 0 if <see cref="DataType"/> is null.
        /// </returns>
        public override int GetHashCode() => _dataType?.GetHashCode() ?? 0;

        // Validate against these rules
        //  1. dataType must not be null (except at initialization)
        //  2. dataType must be a Type (object data)
        internal static Exception ValidateDataType(object dataType, [CallerArgumentExpression(nameof(dataType))] string argName = null)
        {
            Exception result = null;

            if (dataType == null)
            {
                result = new ArgumentNullException(argName);
            }
            else if (dataType is not Type)
            {
                result = new ArgumentException(
                    $"'{dataType.GetType().Name}' is not a valid type for DataTemplate.DataType; it must be 'Type'.",
                    argName);
            }

            return result;
        }
    }
}