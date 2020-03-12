

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


using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Contains property setters that can be shared between instances of a type.
    /// </summary>
    [ContentProperty("Setters")]
    public partial class Style : DependencyObject //was sealed but we unsealed it because telerik has xaml files with styles as their roots (and the file we generate from xaml files create a type that inherits the type of the root of the xaml).
    {
        private SetterBaseCollection _setters;
        private Type _targetType;

        /// <summary>
        /// Initializes a new instance of the Style class, with no initial TargetType and an empty Setters collection.
        /// </summary>
        public Style()
        {
            _setters = new SetterBaseCollection();
        }

        /// <summary>
        /// Initializes a new instance of the Style class, with an initial TargetType and an empty Setters collection.
        /// </summary>
        /// <param name="targetType">The type on which the Style will be used.</param>
        public Style(Type targetType)
        {

            _setters = new SetterBaseCollection();
        }

        /// <summary>
        /// Gets or sets a defined style that is the basis of the current style.
        /// Returns a defined style that is the basis of the current style. The default value is null.
        /// </summary>
        public Style BasedOn
        {
            get { return (Style)GetValue(BasedOnProperty); }
            set { SetValue(BasedOnProperty, value); }
        }

        public static readonly DependencyProperty BasedOnProperty =
            DependencyProperty.Register("BasedOn", typeof(Style), typeof(Style), new PropertyMetadata(null, BasedOn_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        static void BasedOn_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //todo: fill this
        }


        public bool IsSealed { get; private set; }

        ///// <summary>
        ///// Gets a value that indicates whether the style is read-only and cannot be changed.
        ///// Returns true if the style is read-only; otherwise, false.
        ///// </summary>
        //public bool IsSealed
        //{
        //    get { return (bool)GetValue(IsSealedProperty); }
        //    internal set { SetValue(IsSealedProperty, value); }
        //}
        ///// <summary>
        ///// Identifies the IsSealed dependency property.
        ///// </summary>
        //public static readonly DependencyProperty IsSealedProperty =
        //    DependencyProperty.Register("IsSealed", typeof(bool), typeof(DependencyObject), new PropertyMetadata(false, IsSealed_Changed));

        //static void IsSealed_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    //todo: fill this
        //}


#if FOR_DESIGN_TIME
        [global::System.ComponentModel.DesignerSerializationVisibility(global::System.ComponentModel.DesignerSerializationVisibility.Content)]
#endif


        /// <summary>
        /// Gets a collection of Setter objects.
        /// </summary>
        public SetterBaseCollection Setters
        {
            get { return _setters; }
        }


        /// <summary>
        /// This event is used to notify all Items using the style that a setter has been changed
        /// </summary>
        internal event RoutedEventHandler SetterValueChanged;
        internal void NotifySetterValueChanged(Setter setter)
        {
            if (SetterValueChanged != null)
            {
                SetterValueChanged(setter, null);
            }
        }

        /// <summary>
        /// Gets or sets the type for which the style is intended.
        /// Returns the type of object to which the style is applied.
        /// </summary>
        public Type TargetType
        {
            get { return _targetType; }
            set { _targetType = value; }
        }



        /// <summary>
        /// Locks the style so that the TargetType property or any Setter in the Setters collection cannot be changed.
        /// </summary>
        public void Seal()
        {
            IsSealed = true;
        }


        internal object GetActiveValue(DependencyProperty dependencyProperty, HashSet2<Style> stylesAlreadyVisited) // Note: "stylesAlreadyVisited" is here to prevent an infinite recursion.
        {
            stylesAlreadyVisited.Add(this);
            if (Setters != null)
            {
                foreach (Setter setter in Setters)
                {
                    if (setter.Property != null) // Note: it can be null for example in the XAML text editor during design time, because the "DependencyPropertyConverter" class returns "null".
                    {
                        if (setter.Property == dependencyProperty)
                        {
                            return setter.Value;
                        }
                    }
                }
            }
            if (BasedOn != null && !stylesAlreadyVisited.Contains(BasedOn)) // Note: "stylesAlreadyVisited" is here to prevent an infinite recursion.
            {
                return BasedOn.GetActiveValue(dependencyProperty, stylesAlreadyVisited);
            }
            return null;
        }

        Dictionary<DependencyProperty, Setter> _dictionaryOfSetters; //todo: set this to null if a setter is changed, added or removed or make the corresponding change to it. In fact, the list of setters is not an ObservableCollection so we are unable to detect when the list changes.
        internal Dictionary<DependencyProperty, Setter> GetDictionaryOfSettersFromStyle()
        {
            if (_dictionaryOfSetters == null)
            {
                _dictionaryOfSetters = new Dictionary<DependencyProperty, Setter>();
                Style currentStyle = this;
                HashSet2<Style> stylesAlreadyVisited = new HashSet2<Style>(); // This is to prevent an infinite loop below.
                while (currentStyle != null && !stylesAlreadyVisited.Contains(currentStyle))
                {
                    if (currentStyle.Setters != null)
                    {
                        foreach (Setter setter in currentStyle.Setters)
                        {
                            if (setter.Property != null) // Note: it can be null for example in the XAML text editor during design time, because the "DependencyPropertyConverter" class returns "null".
                            {
                                if (!_dictionaryOfSetters.ContainsKey(setter.Property))
                                {
                                    _dictionaryOfSetters.Add(setter.Property, setter);
                                }
                            }
                        }
                    }
                    stylesAlreadyVisited.Add(currentStyle);
                    currentStyle = currentStyle.BasedOn;
                }
            }
            return _dictionaryOfSetters;
        }
    }
}
