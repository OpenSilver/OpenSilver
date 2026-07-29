
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

using System.Windows.Controls;
using System.Windows.Markup;

namespace System.Windows
{
    /// <summary>
    /// Describes the visual structure of a data object.
    /// </summary>
    [DictionaryKeyProperty(nameof(DataTemplateKey))]
    public class DataTemplate : FrameworkTemplate
    {
        private Type _dataType;
        private TriggerCollection _triggers;

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
                if (TemplateKey.ValidateDataType(value) is Exception ex)
                {
                    throw ex;
                }

                CheckSealed();
                _dataType = value;
            }
        }

        /// <summary>
        /// Gets the default key of the <see cref="DataTemplate"/>.
        /// </summary>
        /// <returns>
        /// The default key of the <see cref="DataTemplate"/>.
        /// </returns>
        public object DataTemplateKey => DataType is not null ? new DataTemplateKey(DataType) : null;

        /// <summary>
        /// Gets a collection of <see cref="TriggerBase"/> objects that apply property changes
        /// or perform actions based on specified conditions.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="TriggerBase"/> objects. The default is an empty collection.
        /// </returns>
        public TriggerCollection Triggers
        {
            get
            {
                if (_triggers is null)
                {
                    _triggers = [];
                    
                    if (IsSealed())
                    {
                        _triggers.Seal();
                    }
                }
                return _triggers;
            }
        }

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
                return Template.LoadContent<FrameworkElement>(null) as DependencyObject;
            }
            else
            {
                return null;
            }
        }

        internal override Type TargetTypeInternal => typeof(ContentPresenter);

        internal override TriggerCollection TriggersInternal => _triggers;
    }
}
