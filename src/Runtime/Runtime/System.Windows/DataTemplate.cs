
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

using System.ComponentModel;

namespace System.Windows
{
    /// <summary>
    /// Describes the visual structure of a data object.
    /// </summary>
    public class DataTemplate : FrameworkTemplate
    {
        private Type _dataType;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataTemplate"/> class without initializing
        /// the <see cref="DataType"/> property.
        /// </summary>
        public DataTemplate() { }

        /// <summary>
        /// Gets or sets the type for which this <see cref="DataTemplate"/> is intended.
        /// </summary>
        /// <returns>
        /// The type of object to which this template is applied.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// When setting this property, the specified value is not of type <see cref="Type"/>.
        /// </exception>
        public Type DataType
        {
            get => _dataType;
            set
            {
                if (Windows.DataTemplateKey.ValidateDataType(value) is Exception ex)
                {
                    throw ex;
                }

                CheckSealed();
                _dataType = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public object DataTemplateKey => DataType is not null ? new DataTemplateKey(DataType) : null;

        /// <summary>
        /// Creates the <see cref="UIElement"/> objects in the <see cref="DataTemplate"/>.
        /// </summary>
        /// <returns>
        /// The root <see cref="UIElement"/> of the <see cref="DataTemplate"/>.
        /// </returns>
        public DependencyObject LoadContent()
        {
            if (Template is not null)
            {
                return Template.LoadContent(null) as DependencyObject;
            }
            else
            {
                return null;
            }
        }
    }
}
