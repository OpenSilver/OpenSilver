
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

using System.Collections.Generic;
using OpenSilver.Internal.Data;

namespace System.Windows
{
    /// <summary>
    /// Implements a data structure for describing a property as a path below another
    /// property, or below an owning type. Property paths are used in data binding to
    /// objects, and in storyboards and timelines for animations.
    /// </summary>
    public sealed class PropertyPath : DependencyObject
    {
        private IReadOnlyList<SourceValueInfo> _arySVI;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyPath"/> class.
        /// </summary>
        /// <param name="path">
        /// A property path string.
        /// </param>
        public PropertyPath(string path)
        {
            Path = path;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyPath"/> class.
        /// </summary>
        /// <param name="dependencyProperty">
        /// A dependency property identifier.
        /// </param>
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
        public PropertyPath(string path, params object[] pathParameters)
            : this(path)
        {
            if (pathParameters != null && pathParameters.Length > 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pathParameters));
            }
        }

        /// <summary>
        /// Gets the path value held by this <see cref="PropertyPath"/>.
        /// </summary>
        /// <returns>
        /// The path value held by this <see cref="PropertyPath"/>.
        /// </returns>
        public string Path { get; }

        internal DependencyProperty DependencyProperty { get; }

        internal IReadOnlyList<SourceValueInfo> SVI => _arySVI ??= ParsePath(Path);

        private static List<SourceValueInfo> ParsePath(string path)
        {
            var steps = new List<SourceValueInfo>();
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
            return steps;
        }
    }

    internal struct SourceValueInfo
    {
        public PropertyNodeType type;
        public string propertyName;
        public string param; // parameter for indexer
    }
}
