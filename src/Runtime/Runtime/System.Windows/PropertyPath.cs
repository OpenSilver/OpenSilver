
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
using System.ComponentModel;
using CSHTML5.Internal;
using OpenSilver.Internal;
using OpenSilver.Internal.Data;

namespace System.Windows
{
    /// <summary>
    /// Implements a data structure for describing a property as a path below another
    /// property, or below an owning type. Property paths are used in data binding to objects.
    /// </summary>
    public sealed class PropertyPath : DependencyObject
    {
        private SourceValueInfo[] _arySVI;

        /// <summary>
        /// Initializes a new instance of the PropertyPath class based on a path string.
        /// </summary>
        /// <param name="path">The path string to construct with.</param>
        public PropertyPath(string path)
        {
            Path = path;
        }

        public PropertyPath(DependencyProperty dependencyProperty)
        {
            Path = dependencyProperty.Name;
            DependencyProperty = dependencyProperty;
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
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete(Helper.ObsoleteMemberMessage + " Use PropertyPath(string) instead.", true)]
        public PropertyPath(string path, string dependencyPropertyName, Func<DependencyObject, IEnumerable<Tuple<DependencyObject, DependencyProperty, int?>>> accessPropertyContainer,
            Action<DependencyObject, object> propertySetVisualState,
            Action<DependencyObject, object> propertySetAnimationValue,
            Action<DependencyObject, object> propertySetLocalValue,
            Func<DependencyObject, object> propertyGetVisualState)
        {
            Path = path;
        }

        /// <summary>
        /// Gets the path value held by this PropertyPath. Returns the path value held by this PropertyPath.
        /// </summary>
        public string Path { get; }

        internal DependencyProperty DependencyProperty { get; }

        internal SourceValueInfo[] SVI => _arySVI ??= ParsePath(Path);

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

        internal DependencyObject GetFinalItem(DependencyObject rootItem)
        {
            DependencyObject finalTarget = rootItem;

            if (DependencyProperty == null)
            {
                for (int i = 0; i < SVI.Length - 1; i++)
                {
                    SourceValueInfo svi = _arySVI[i];
                    switch (svi.type)
                    {
                        case PropertyNodeType.Property:
                            DependencyProperty dp = DependencyPropertyFromName(svi.propertyName, finalTarget.GetType());
                            finalTarget = AsDependencyObject(finalTarget.GetValue(dp));
                            break;

                        case PropertyNodeType.Indexed:
                            if (finalTarget is not IList list)
                            {
                                throw new InvalidOperationException($"'{finalTarget}' must implement IList.");
                            }
                            if (!int.TryParse(svi.param, out int index))
                            {
                                throw new InvalidOperationException($"'{svi.param}' can't be converted to an integer value.");
                            }

                            finalTarget = AsDependencyObject(list[index]);
                            break;

                        default:
                            throw new InvalidOperationException();
                    }
                }
            }

            return finalTarget;
        }

        internal DependencyProperty GetFinalProperty(DependencyObject d)
        {
            if (DependencyProperty != null)
            {
                return DependencyProperty;
            }

            if (SVI.Length == 0)
            {
                throw new InvalidOperationException();
            }

            SourceValueInfo svi = _arySVI[_arySVI.Length - 1];
            return DependencyPropertyFromName(svi.propertyName, d.GetType());
        }

        private static DependencyObject AsDependencyObject(object o)
        {
            return o as DependencyObject ?? throw new InvalidOperationException($"'{o}' must be a DependencyObject.");
        }

        private static DependencyProperty DependencyPropertyFromName(string name, Type ownerType)
        {
            DependencyProperty dp = DependencyProperty.FromName(name, ownerType);

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
