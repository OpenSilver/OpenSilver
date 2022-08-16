
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CSHTML5.Internal;
using OpenSilver.Internal.Data;

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
        private SourceValueInfo[] _arySVI;

        /// <summary>
        /// Initializes a new instance of the PropertyPath class based on a path string.
        /// </summary>
        /// <param name="path">The path string to construct with.</param>
        public PropertyPath(string path)
        {
            Path = path;

            INTERNAL_AccessPropertyContainer = accessVisualStateProperty;
            INTERNAL_PropertySetAnimationValue = setVisualStateProperty;
        }

        public PropertyPath(DependencyProperty dependencyProperty)
        {
            Path = dependencyProperty.Name;
            DependencyProperty = dependencyProperty;

            INTERNAL_AccessPropertyContainer = (d) => Enumerable.Empty<Tuple<DependencyObject, DependencyProperty, int?>>();
            INTERNAL_PropertySetAnimationValue = (target, value) => target.SetAnimationValue(DependencyProperty, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyPath"/> class.
        /// </summary>
        /// <param name="path">
        /// The path string for this <see cref="PropertyPath"/>.
        /// </param>
        /// <param name="pathParameters">
        /// Do not use.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Provided an array of length greater than zero for pathParameters.
        /// </exception>
        public PropertyPath(string path, params object[] pathParameters) : this(path)
        {
            if (pathParameters != null && pathParameters.Length > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pathParameters));
            }
        }

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
        [Obsolete("Please use PropertyPath(string) instead. This constructor will be removed in a future release.")]
        public PropertyPath(string path, string dependencyPropertyName, Func<DependencyObject, IEnumerable<Tuple<DependencyObject, DependencyProperty, int?>>> accessPropertyContainer,
            Action<DependencyObject, object> propertySetVisualState,
            Action<DependencyObject, object> propertySetAnimationValue,
            Action<DependencyObject, object> propertySetLocalValue,
            Func<DependencyObject, object> propertyGetVisualState)
        {
            Path = path;

            INTERNAL_AccessPropertyContainer = accessPropertyContainer;
            INTERNAL_PropertySetAnimationValue = propertySetAnimationValue;
        }

        /// <summary>
        /// Gets the path value held by this PropertyPath. Returns the path value held by this PropertyPath.
        /// </summary>
        public string Path { get; }

        internal DependencyProperty DependencyProperty { get; }

        /// <summary>
        /// When set, defines a method designed to access the element that contains the property whose Value will be accessed.
        /// </summary>
        /// <ignore/>
        internal Func<DependencyObject, IEnumerable<Tuple<DependencyObject, DependencyProperty, int?>>> INTERNAL_AccessPropertyContainer { get; }

        /// <summary>
        /// Sets the Animation value (value is second parameter) of the previously defined property on the DependencyObject (first parameter).
        /// </summary>
        /// <ignore/>
        internal Action<DependencyObject, object> INTERNAL_PropertySetAnimationValue { get; }

        internal SourceValueInfo[] SVI
        {
            get
            {
                if (_arySVI == null)
                {
                    _arySVI = ParsePath(Path);
                }

                return _arySVI;
            }
        }

        private static SourceValueInfo[] ParsePath(string path)
        {
            List<SourceValueInfo> steps = new List<SourceValueInfo>();
            var parser = new PropertyPathParser(path, false);
            while (true)
            {
                switch (parser.Step(out _, out string property, out string index))
                {
                    case PropertyNodeType.Property:
                    case PropertyNodeType.AttachedProperty:
                        steps.Add(new SourceValueInfo
                        {
                            type = PropertyNodeType.Property,
                            propertyName = property,
                        });
                        break;

                    case PropertyNodeType.Indexed:
                        steps.Add(new SourceValueInfo
                        {
                            type = PropertyNodeType.Indexed,
                            propertyName = "Item[]",
                            param = index,
                        });
                        break;

                    case PropertyNodeType.None:
                        goto exit;
                }
            }

        exit:
            return steps.ToArray();
        }

        private IEnumerable<Tuple<DependencyObject, DependencyProperty, int?>> accessVisualStateProperty(DependencyObject rootTargetObjectInstance)
        {
            DependencyObject currentTarget = rootTargetObjectInstance;
            for (int i = 0; i < SVI.Length - 1; i++)
            {
                SourceValueInfo svi = _arySVI[i];
                switch (svi.type)
                {
                    case PropertyNodeType.Property:
                        DependencyProperty dp = DependencyPropertyFromName(svi.propertyName, currentTarget.GetType());
                        currentTarget = AsDependencyObject(currentTarget.GetValue(dp));
                        yield return new Tuple<DependencyObject, DependencyProperty, int?>(currentTarget, dp, null);
                        break;

                    case PropertyNodeType.Indexed:
                        IList list = currentTarget as IList;
                        if (!(currentTarget is IList))
                        {
                            throw new InvalidOperationException($"'{currentTarget}' must implement IList.");
                        }
                        int index = -1;
                        if (!int.TryParse(svi.param, out index))
                        {
                            throw new InvalidOperationException($"'{svi.param}' can't be converted to an integer value.");
                        }

                        currentTarget = AsDependencyObject(list[index]);
                        yield return new Tuple<DependencyObject, DependencyProperty, int?>(currentTarget, null, index);
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        private void setVisualStateProperty(DependencyObject finalTargetInstance, object value)
        {
            if (SVI.Length == 0)
            {
                throw new InvalidOperationException();
            }

            SourceValueInfo svi = _arySVI[_arySVI.Length - 1];
            DependencyProperty dp = DependencyPropertyFromName(svi.propertyName, finalTargetInstance.GetType());

            finalTargetInstance.SetAnimationValue(dp, value);
        }

        private static DependencyObject AsDependencyObject(object o)
        {
            return o as DependencyObject ?? throw new InvalidOperationException($"'{o}' must be a DependencyObject.");
        }

        private static DependencyProperty DependencyPropertyFromName(string name, Type ownerType)
        {
            DependencyProperty dp = INTERNAL_TypeToStringsToDependencyProperties.GetPropertyInTypeOrItsBaseTypes(ownerType, name);

            return dp ?? throw new InvalidOperationException($"No DependencyProperty named '{name}' could be found in '{ownerType}'.");
        }
    }

    internal struct SourceValueInfo
    {
        public PropertyNodeType type;
        public string propertyName;
        public string param; // parameter for indexer
    }
}
