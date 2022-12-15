

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


using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Defines behavior aspects of a dependency property, including conditions it
    /// was registered with.
    /// </summary>
    public partial class PropertyMetadata
    {
        private object _defaultValue;
        private bool _inherits;
        private WhenToCallPropertyChangedEnum _callPropertyChangedWhenLoadedIntoVisualTree;
        private PropertyChangedCallback _propertyChangedCallback;
        private MethodToUpdateDom _methodToUpdateDom;
        private MethodToUpdateDom2 _methodToUpdateDom2;
        private CoerceValueCallback _coerceValueCallback;

        internal bool IsDefaultValueModified { get; private set; }
        internal bool Sealed { get; private set; }

        internal void Seal()
        {
            this.Sealed = true;
        }

        protected bool IsSealed
        {
            get { return this.Sealed; }
        }

        /// <summary>
        /// Gets the default value for the dependency property.
        /// </summary>
        public object DefaultValue
        {
            get
            {
                return _defaultValue;
            }
            set
            {
                if (value == DependencyProperty.UnsetValue)
                {
                    throw new ArgumentException("Default value cannot be 'Unset'.");
                }

                this.CheckSealed();
                
                if (_defaultValue != value)
                {
                    _defaultValue = value;
                    IsDefaultValueModified = true; 
                }
            }
        }

        /// <summary>
        /// Gets the method that is called when the property value changes.
        /// </summary>
        public PropertyChangedCallback PropertyChangedCallback
        {
            get
            {
                return this._propertyChangedCallback;
            }
            set
            {
                this.CheckSealed();
                this._propertyChangedCallback = value;
            }
        }
        /// <summary>
        /// Gets the method that is called when the object is added to the visual tree, and when the property value changes while the object is already in the visual tree.
        /// </summary>
        public MethodToUpdateDom MethodToUpdateDom
        {
            get
            {
                return this._methodToUpdateDom;
            }
            set
            {
                this.CheckSealed();
                this._methodToUpdateDom = value;
            }
        }

        internal MethodToUpdateDom2 MethodToUpdateDom2
        {
            get
            {
                return this._methodToUpdateDom2;
            }
            set
            {
                this.CheckSealed();
                this._methodToUpdateDom2 = value;
            }
        }

        /// <summary>
        /// Gets the method that will be called on update of value.
        /// </summary>
        public CoerceValueCallback CoerceValueCallback
        {
            get
            {
                return this._coerceValueCallback;
            }
            set
            {
                this.CheckSealed();
                this._coerceValueCallback = value;
            }
        }
        /// <summary>
        /// Determines if the property's value can be inherited from a parent element to a child element.
        /// </summary>
        public bool Inherits
        {
            get
            {
                return this._inherits;
            }
            set
            {
                this.CheckSealed();
                this._inherits = value;
            }
        }
        /// <summary>
        /// Determines if the callback method should be called when the element is added to the visual tree.
        /// </summary>
        public WhenToCallPropertyChangedEnum CallPropertyChangedWhenLoadedIntoVisualTree
        {
            get
            {
                return this._callPropertyChangedWhenLoadedIntoVisualTree;
            }
            set
            {
                this.CheckSealed();
                this._callPropertyChangedWhenLoadedIntoVisualTree = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of Windows.UI.Xaml.PropertyMetadata.
        /// </summary>
        public PropertyMetadata()
        {
            CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.Never;
        }

        /// <summary>
        /// Initializes a new instance of the PropertyMetadata class, using a property
        /// default value.
        /// </summary>
        /// <param name="defaultValue">A default value for the property where this PropertyMetadata is applied.</param>
        public PropertyMetadata(object defaultValue) : this()
        {
            this.DefaultValue = defaultValue;
        }

        /// <summary>
        /// Initializes a new instance of the PropertyMetadata class, using a callback reference.
        /// </summary>
        /// <param name="propertyChangedCallback">A reference to the callback to call for property changed behavior.</param>
        public PropertyMetadata(PropertyChangedCallback propertyChangedCallback) : this()
        {
            this.PropertyChangedCallback = propertyChangedCallback;
        }

        /// <summary>
        /// Initializes a new instance of the PropertyMetadata class, using a property
        /// default value and callback reference.
        /// </summary>
        /// <param name="defaultValue">A default value for the property where this PropertyMetadata is applied.</param>
        /// <param name="propertyChangedCallback">A reference to the callback to call for property changed behavior.</param>
        public PropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback) : this()
        {
            this.DefaultValue = defaultValue;
            this.PropertyChangedCallback = propertyChangedCallback;
        }

        /// <summary>
        ///     Type meta construction
        /// </summary>
        /// <param name="defaultValue">Default value of property</param>
        /// <param name="propertyChangedCallback">Called when the property has been changed</param>
        /// <param name="coerceValueCallback">Called on update of value</param>
        public PropertyMetadata(object defaultValue, PropertyChangedCallback propertyChangedCallback, CoerceValueCallback coerceValueCallback) : this()
        {
            this.DefaultValue = defaultValue;
            this.PropertyChangedCallback = propertyChangedCallback;
            this.CoerceValueCallback = coerceValueCallback;
        }

        internal CSSEquivalentGetter GetCSSEquivalent;
        internal CSSEquivalentsGetter GetCSSEquivalents;

        internal void CheckSealed()
        {
            if (this.Sealed)
            {
                throw new InvalidOperationException("Cannot change property metadata after it has been associated with a property.");
            }
        }
    }

    internal delegate CSSEquivalent CSSEquivalentGetter(DependencyObject d);
    internal delegate List<CSSEquivalent> CSSEquivalentsGetter(DependencyObject d);
    public delegate void MethodToUpdateDom(DependencyObject d, object newValue);
    internal delegate void MethodToUpdateDom2(DependencyObject d, object oldValue, object newValue);
}
