
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

namespace OpenSilver.Internal.Data
{
    internal enum PropertyNodeType
    {
        None = 0,
        AttachedProperty = 1,
        Indexed = 2,
        Property = 3,
    }

    internal ref struct PropertyPathParser
    {
        private readonly string _originalPath;
        private readonly bool _isParsingBinding;

        private ReadOnlySpan<char> _path;

        internal PropertyPathParser(string path, bool isParsingBinding)
        {
            _originalPath = path;
            _path = path.AsSpan();
            _isParsingBinding = isParsingBinding;
        }

        internal PropertyNodeType Step(out string typeName, out string propertyName, out string index)
        {
            ReadOnlySpan<char> path = _path;

            if (path.Length == 0 || (path.Length == 1 && path[0] == '.'))
            {
                typeName = null;
                propertyName = null;
                index = null;
                return PropertyNodeType.None;
            }

            PropertyNodeType type;
            if (path[0] == '(')
            {
                type = PropertyNodeType.AttachedProperty;
                int end = path.IndexOf(')');
                if (end == -1)
                {
                    throw new Exception($"Invalid property path : '{path}'.");
                }

                var slice = path.Slice(1, end - 1);
                int typeEnd = slice.LastIndexOf('.');
                if (typeEnd == -1)
                {
                    if (_isParsingBinding)
                    {
                        throw new Exception($"Invalid property path : '{path}'.");
                    }

                    typeName = null;
                    propertyName = slice.ToString();
                    index = null;
                }
                else
                {
                    typeEnd++;
                    typeName = path.Slice(1, typeEnd - 1).ToString();
                    propertyName = path.Slice(typeEnd + 1, end - typeEnd - 1).ToString();
                    index = null;
                }

                if (path.Length > (end + 1) && path[end + 1] == '.')
                {
                    end++;
                }

                path = path.Slice(end + 1);
            }
            else if (path[0] == '[')
            {
                type = PropertyNodeType.Indexed;
                int end = path.IndexOf(']');

                typeName = null;
                propertyName = null;
                index = path.Slice(1, end - 1).ToString();
                path = path.Slice(end + 1);
                if (path.Length > 0 && path[0] == '.')
                {
                    path = path.Slice(1);
                }
            }
            else
            {
                type = PropertyNodeType.Property;
                int end = path.IndexOfAny('.', '[');
                if (end == -1)
                {
                    propertyName = _originalPath.Length == _path.Length ? _originalPath : path.ToString();
                    path = ReadOnlySpan<char>.Empty;
                }
                else
                {
                    propertyName = path.Slice(0, end).ToString();
                    if (path[end] == '.')
                    {
                        path = path.Slice(end + 1);
                    }
                    else
                    {
                        path = path.Slice(end);
                    }
                }

                typeName = null;
                index = null;
            }

            _path = path;

            return type;
        }
    }
}
