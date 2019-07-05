
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using CSHTML5.Internal;
using DotNetForHtml5.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Implements a data structure for describing a property as a path below another
    /// property, or below an owning type. Property paths are used in data binding to objects.
    /// </summary>
#if FOR_DESIGN_TIME
    [TypeConverter(typeof(PropertyPathConverter))]
#endif
    public sealed class PropertyPath : DependencyObject
    {
        internal DependencyProperty INTERNAL_DependencyProperty; //this is only defined when the user uses the PropertyPath(DependencyProperty dependencyProperty) constructor.
        internal bool INTERNAL_IsDirectlyDependencyPropertyPath = false;

        /// <summary>
        /// Initializes a new instance of the PropertyPath class based on a path string.
        /// </summary>
        /// <param name="path">The path string to construct with.</param>
        public PropertyPath(string path)
        {
            _path = path;
        }

        public PropertyPath(DependencyProperty dependencyProperty)
        {
            INTERNAL_IsDirectlyDependencyPropertyPath = true;
            INTERNAL_DependencyProperty = dependencyProperty;
            _path = dependencyProperty.Name;
            INTERNAL_DependencyPropertyName = dependencyProperty.Name;
            INTERNAL_AccessPropertyContainer = defaulAccessVisualStateProperty;
            INTERNAL_PropertySetVisualState = defaultSetVisualStateProperty;
            INTERNAL_PropertySetAnimationValue = defaultSetAnimationVisualStateProperty;
            INTERNAL_PropertySetLocalValue = defaultSetLocalVisualStateProperty;
            INTERNAL_PropertyGetVisualState = defaultGetVisualStateProperty;
        }

        #region methods to access property for the PropertyPath(DependencyProperty) constructor
        public static global::System.Collections.Generic.IEnumerable<Tuple<DependencyObject, DependencyProperty, int?>> defaulAccessVisualStateProperty(DependencyObject rootTargetObjectInstance)
        {
            yield break;
        }

        public void defaultSetVisualStateProperty(DependencyObject finalTargetInstance, object value)
        {
            if (INTERNAL_IsDirectlyDependencyPropertyPath)
            {
                (finalTargetInstance).SetVisualStateValue(INTERNAL_DependencyProperty, value);
            }
            else
            {
                throw new InvalidOperationException("The constructor: PropertyPath(string path) for storyboards is not supported yet. Please use PropertyPath(DependencyProperty dependencyProperty) or define your storyboard in the XAML.");
            }
        }

        public void defaultSetAnimationVisualStateProperty(DependencyObject finalTargetInstance, object value)
        {
            if (INTERNAL_IsDirectlyDependencyPropertyPath)
            {
                (finalTargetInstance).SetAnimationValue(INTERNAL_DependencyProperty, value);
            }
            else
            {
                throw new InvalidOperationException("The constructor: PropertyPath(string path) for storyboards is not supported yet. Please use PropertyPath(DependencyProperty dependencyProperty) or define your storyboard in the XAML.");
            }
        }

        public void defaultSetLocalVisualStateProperty(DependencyObject finalTargetInstance, object value)
        {
            if (INTERNAL_IsDirectlyDependencyPropertyPath)
            {
                (finalTargetInstance).SetLocalValue(INTERNAL_DependencyProperty, value);
            }
            else
            {
                throw new InvalidOperationException("The constructor: PropertyPath(string path) for storyboards is not supported yet. Please use PropertyPath(DependencyProperty dependencyProperty) or define your storyboard in the XAML.");
            }
        }

        public global::System.Object defaultGetVisualStateProperty(DependencyObject finalTargetInstance)
        {
            if (INTERNAL_IsDirectlyDependencyPropertyPath)
            {
                return finalTargetInstance.GetVisualStateValue(INTERNAL_DependencyProperty);
            }
            else
            {
                throw new InvalidOperationException("The constructor: PropertyPath(string path) for storyboards is not supported yet. Please use PropertyPath(DependencyProperty dependencyProperty) or define your storyboard in the XAML.");
            }
        }
        #endregion

        /// <summary>
        /// Initializes a new Instance of the PropertyPath class based on methods to access the property from a DependencyObject.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dependencyPropertyName">The name of the property (consists only of the last part of the path we would put in the XAML).</param>
        /// <param name="accessPropertyContainer">The function that will access the object on which the property is from the DependencyObject put in its parameters(corresponds to the path in XAML without the last property).</param>
        /// <param name="propertySetVisualState">The function that sets the VisualState value on the given element.</param>
        /// <param name="propertySetAnimationValue"></param>
        /// <param name="propertySetLocalValue">The function that sets the Local value on the given element.</param>
        /// <param name="propertyGetVisualState">The function that gets the VisualState value on the given element.</param>
        /// <ignore/>
        public PropertyPath(string path, string dependencyPropertyName, Func<DependencyObject, IEnumerable<Tuple<DependencyObject, DependencyProperty, int?>>> accessPropertyContainer,
            Action<DependencyObject, Object> propertySetVisualState,
            Action<DependencyObject, Object> propertySetAnimationValue,
            Action<DependencyObject, Object> propertySetLocalValue,
            Func<DependencyObject, Object> propertyGetVisualState)
        {
            _path = path;
            INTERNAL_DependencyPropertyName = dependencyPropertyName;
            INTERNAL_AccessPropertyContainer = accessPropertyContainer;
            INTERNAL_PropertySetVisualState = propertySetVisualState;
            INTERNAL_PropertySetAnimationValue = propertySetAnimationValue;
            INTERNAL_PropertySetLocalValue = propertySetLocalValue;
            INTERNAL_PropertyGetVisualState = propertyGetVisualState;
        }


        internal string INTERNAL_DependencyPropertyName;
        /// <summary>
        /// When set, defines a method designed to access the element that contains the property whose Value will be accessed.
        /// </summary>
        /// <ignore/>
        internal Func<DependencyObject, IEnumerable<Tuple<DependencyObject, DependencyProperty, int?>>> INTERNAL_AccessPropertyContainer;
        /// <summary>
        /// Sets the VisualState value (value is second parameter) of the previously defined property on the DependencyObject (first parameter).
        /// </summary>
        /// <ignore/>
        internal Action<DependencyObject, Object> INTERNAL_PropertySetVisualState;
        /// <summary>
        /// Sets the Animation value (value is second parameter) of the previously defined property on the DependencyObject (first parameter).
        /// </summary>
        /// <ignore/>
        internal Action<DependencyObject, Object> INTERNAL_PropertySetAnimationValue;
        /// <summary>
        /// Sets the Local value (value is second parameter) of the previously defined property on the DependencyObject (first parameter).
        /// </summary>
        /// <ignore/>
        internal Action<DependencyObject, Object> INTERNAL_PropertySetLocalValue;
        /// <summary>
        /// Gets the VisualState value of the previously defined property on the DependencyObject set as parameter.
        /// </summary>
        /// <ignore/>
        internal Func<DependencyObject, Object> INTERNAL_PropertyGetVisualState;

        string _path;
        /// <summary>
        /// Gets the path value held by this PropertyPath. Returns the path value held by this PropertyPath.
        /// </summary>
        public string Path
        {
            get
            {
                return _path;
            }
        }

        static PropertyPath()
        {
            TypeFromStringConverters.RegisterConverter(typeof(PropertyPath), INTERNAL_ConvertFromString);
        }

        internal static object INTERNAL_ConvertFromString(string path)
        {
            return new PropertyPath(path);
        }

    }
}
