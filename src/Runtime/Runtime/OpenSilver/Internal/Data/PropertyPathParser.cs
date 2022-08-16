
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

    //Property path syntax: http://msdn.microsoft.com/en-us/library/cc645024(v=vs.95).aspx
    internal struct PropertyPathParser
    {
        private string _path;
        private readonly bool _isParsingBinding;

        internal PropertyPathParser(string path, bool isParsingBinding)
        {
            _path = path;
            _isParsingBinding = isParsingBinding;
        }

        internal PropertyNodeType Step(out string typeName, out string propertyName, out string index)
        {
            var type = PropertyNodeType.None;
            var path = _path;
            if (string.IsNullOrEmpty(path) || path == ".")
            {
                typeName = null;
                propertyName = null;
                index = null;
                return type;
            }

            int end = 0;
            if (path[0] == '(')
            {
                type = PropertyNodeType.AttachedProperty;
                end = path.IndexOf(')');
                if (end == -1)
                {
                    throw new Exception($"Invalid property path : '{path}'.");
                }

                int typeEnd = path.LastIndexOf('.', end);
                if (typeEnd == -1)
                {
                    if (_isParsingBinding)
                    {
                        throw new Exception($"Invalid property path : '{path}'.");
                    }

                    typeName = null;
                    propertyName = path.Substring(1, end - 1);
                    index = null;
                }
                else
                {
                    typeName = path.Substring(1, typeEnd - 1);
                    propertyName = path.Substring(typeEnd + 1, end - typeEnd - 1);
                    index = null;
                }

                if (path.Length > (end + 1) && path[end + 1] == '.')
                {
                    end++;
                }

                path = path.Substring(end + 1);
            }
            else if (path[0] == '[')
            {
                type = PropertyNodeType.Indexed;
                end = path.IndexOf(']');

                typeName = null;
                propertyName = null;
                index = path.Substring(1, end - 1);
                path = path.Substring(end + 1);
                if (path.Length > 0 && path[0] == '.')
                    path = path.Substring(1);
            }
            else
            {
                type = PropertyNodeType.Property;
                char[] splitters = {'.', '['};
                end = path.IndexOfAny(splitters);

                if (end == -1) {
                    propertyName = path;
                    path = "";
                }
                else
                {
                    propertyName = path.Substring(0, end);
                    if (path[end] == '.')
                        path = path.Substring(end + 1);
                    else
                        path = path.Substring(end);
                }

                typeName = null;
                index = null;
            }

            _path = path;

            return type;
        }
    }
}
