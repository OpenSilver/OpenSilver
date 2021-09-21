

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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace CSHTML5.Internal.System.Windows.Data
#else
namespace CSHTML5.Internal.Windows.UI.Xaml.Data
#endif
{
    internal enum PropertyNodeType
    {
        None = 0,
        AttachedProperty = 1,
        Indexed = 2,
        Property = 3,
    }

    //Property path syntax: http://msdn.microsoft.com/en-us/library/cc645024(v=vs.95).aspx
    internal partial class PropertyPathParser
    {
        string Path;

        internal PropertyPathParser(string path)
        {
            this.Path = path;
        }

        internal PropertyNodeType Step(out string typeName, out string propertyName, out string index) //Note: index can be considered a string or an int, or even a list of "parameters" (for example [20,30] can be two int)
        {
            var type = PropertyNodeType.None;
            var path = this.Path;
            if (path.Length == 0 || path == ".")
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
                    throw new ArgumentException("Invalid property path. Attached property is missing the closing bracket");

                var tickOpen = path.IndexOf('\''); //I don't see where it is in the link above this class' declaration but I let it for the time being
                var tickClose = 0;
                int typeOpen;
                int typeClose;
                int propOpen;
                int propClose;

                typeOpen = path.IndexOf('\'');
                if (typeOpen > 0)
                {
                    typeOpen++;

                    typeClose = path.IndexOf('\'', typeOpen + 1);
                    if (typeClose < 0)
                        throw new Exception("Invalid property path, Unclosed type name '" + path + "'.");

                    propOpen = path.IndexOf('.', typeClose);
                    if (propOpen < 0)
                        throw new Exception("Invalid properth path, No property indexer found '" + path + "'.");

                    propOpen++;
                }
                else
                {
                    typeOpen = 1;
                    typeClose = path.IndexOf('.', typeOpen);
                    if (typeClose < 0)
                        throw new Exception("Invalid property path, No property indexer found on '" + path + "'.");
                    propOpen = typeClose + 1;
                }

                propClose = end;

                typeName = path.Substring(typeOpen, typeClose - typeOpen);
                propertyName = path.Substring(propOpen, propClose - propOpen);

                index = null;
                if (path.Length > (end + 1) && path[end + 1] == '.')
                    end++;
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
            Path = path;

            return type;
        }
    }
}
