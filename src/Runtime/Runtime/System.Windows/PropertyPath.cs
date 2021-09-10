
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
using System.Collections.ObjectModel;
using System.Reflection;

#if MIGRATION
using CSHTML5.Internal.System.Windows.Data;
#else
using CSHTML5.Internal.Windows.UI.Xaml.Data;
#endif

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
    public sealed partial class PropertyPath : DependencyObject
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

            INTERNAL_AccessPropertyContainer = defaulAccessVisualStateProperty;
            INTERNAL_PropertySetVisualState = defaultSetVisualStateProperty;
            INTERNAL_PropertySetAnimationValue = defaultSetAnimationVisualStateProperty;
            INTERNAL_PropertySetLocalValue = defaultSetLocalVisualStateProperty;
            INTERNAL_PropertyGetVisualState = defaultGetVisualStateProperty;
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
        private IEnumerable<Tuple<DependencyObject, DependencyProperty, int?>> defaulAccessVisualStateProperty(DependencyObject rootTargetObjectInstance)
        {
            PropertyPathParser parser = new PropertyPathParser(Path);
            string typeName, propertyName, index;
            PropertyNodeType type;
            var nodes = new List<Tuple<PropertyNodeType, Tuple<string/*type*/, string/*property*/, string/*index*/>>>();
            while ((type = parser.Step(out typeName, out propertyName, out index)) != PropertyNodeType.None)
            {
                nodes.Add(
                    new Tuple<PropertyNodeType, Tuple<string, string, string>>(
                        type, new Tuple<string, string, string>(typeName, propertyName, index)));
            }
            int count = Math.Max(0, nodes.Count - 1);
            List<Tuple<DependencyObject, DependencyProperty, int?>> list = new List<Tuple<DependencyObject, DependencyProperty, int?>>(count);
            for (int j = 0; j < count; ++j)
            {
                type = nodes[j].Item1;
                typeName = nodes[j].Item2.Item1;
                propertyName = nodes[j].Item2.Item2;
                index = nodes[j].Item2.Item3;

                Tuple<DependencyObject, DependencyProperty, int?> tuple;
                DependencyObject targetDO;
                switch (type)
                {
                    case PropertyNodeType.AttachedProperty:
                    case PropertyNodeType.Property:
                        targetDO = list.Count == 0 ?
                            rootTargetObjectInstance :
                            list[list.Count - 1].Item1;
                        Type targetType = targetDO.GetType();
                        PropertyInfo prop = targetType.GetProperty(propertyName);
                        DependencyObject value = (DependencyObject)prop.GetValue(targetDO);
                        DependencyProperty dp = (DependencyProperty)prop.DeclaringType.GetField(propertyName + "Property").GetValue(null);
                        tuple = new Tuple<DependencyObject, DependencyProperty, int?>(
                            value,
                            dp,
                            null);
                        list.Add(tuple);
                        yield return tuple;
                        break;
                    case PropertyNodeType.Indexed:
                        int i;
                        int.TryParse(index, out i);
                        targetDO = rootTargetObjectInstance;
                        if (list.Count > 0)
                        {
#if OPENSILVER
                            if (true)
#elif BRIDGE
                            if (CSHTML5.Interop.IsRunningInTheSimulator)
#endif
                            {
                                // Note: In OpenSilver, we want to enter this case both in the simulator and
                                // the browser.
                                targetDO = (DependencyObject)((dynamic)list[list.Count - 1].Item1)[i];
                            }
                            else
                            {
                                // Note: getItem() is the indexer's name in the Bridge implementation.
                                // The use of 'dynamic' makes the above line return undefined when the application
                                // is running in javascript with CSHTML5.
                                targetDO = (DependencyObject)((dynamic)list[list.Count - 1].Item1).getItem(i);
                            }
                        }
                        tuple = new Tuple<DependencyObject, DependencyProperty, int?>(
                            targetDO,
                            null,
                            i);
                        list.Add(tuple);
                        yield return tuple;
                        break;
                }
            }
            if (!INTERNAL_IsDirectlyDependencyPropertyPath)
            {
                bool success = false;
                DependencyProperty dp = null;
                try
                {
                    string name = nodes[nodes.Count - 1].Item2.Item2;
                    DependencyObject finalDO = count == 0 ? rootTargetObjectInstance : list[list.Count - 1].Item1;
                    dp = (DependencyProperty)finalDO.GetType()
                        .GetField(name + "Property", BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public)
                        .GetValue(null);
                    success = dp != null;
                }
                finally
                {
                    if (success)
                    {
                        INTERNAL_IsDirectlyDependencyPropertyPath = true;
                        INTERNAL_DependencyProperty = dp;
                        INTERNAL_DependencyPropertyName = dp.Name;
                    }
                }
            }
        }

        private void defaultSetVisualStateProperty(DependencyObject finalTargetInstance, object value)
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

        private void defaultSetAnimationVisualStateProperty(DependencyObject finalTargetInstance, object value)
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

        private void defaultSetLocalVisualStateProperty(DependencyObject finalTargetInstance, object value)
        {
            if (INTERNAL_IsDirectlyDependencyPropertyPath)
            {
                (finalTargetInstance).SetCurrentValue(INTERNAL_DependencyProperty, value);
            }
            else
            {
                throw new InvalidOperationException("The constructor: PropertyPath(string path) for storyboards is not supported yet. Please use PropertyPath(DependencyProperty dependencyProperty) or define your storyboard in the XAML.");
            }
        }

        private object defaultGetVisualStateProperty(DependencyObject finalTargetInstance)
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

        private string _path;

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

        public Collection<object> PathParameters { get; }

        [OpenSilver.NotImplemented]
        // The parameters are stored but their use in resolution is not implemented yet
        public PropertyPath(string path, params object[] pathParameters) : this(path)
        {
            if (pathParameters != null && pathParameters.Length > 0)
            {
                PathParameters = new ObservableCollection<object>(pathParameters);
            }
        }
    }
}
