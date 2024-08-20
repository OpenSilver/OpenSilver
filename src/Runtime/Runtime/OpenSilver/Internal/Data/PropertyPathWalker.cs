
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
using System.Windows.Data;

namespace OpenSilver.Internal.Data;

internal sealed class PropertyPathWalker
{
    private readonly IPropertyPathNode _head;
    private readonly IPropertyPathNode _tail;

    internal PropertyPathWalker(BindingExpression bindExpr)
    {
        (_head, _tail) = ParsePath(bindExpr);
    }

    internal bool IsEmpty => _head is SourcePropertyNode;

    internal IPropertyPathNode FinalNode => _tail;

    internal bool IsPathBroken
    {
        get
        {
            IPropertyPathNode node = _head;
            while (node != null)
            {
                if (node.IsBroken)
                {
                    return true;
                }

                node = node.Next;
            }

            return false;
        }
    }

    internal void AttachDataItem(object item, bool transferValue) => _head.SetSource(item, transferValue);

    internal void DetachDataItem() => _head.SetSource(null, false);

    private static (IPropertyPathNode head, IPropertyPathNode tail) ParsePath(BindingExpression bindExpr)
    {
        Binding binding = bindExpr.ParentBinding;
        string path = binding.XamlPath ?? binding.Path.Path ?? string.Empty;

        IPropertyPathNode head = null;
        IPropertyPathNode tail = null;

        var parser = new PropertyPathParser(path, true);
        PropertyNodeType type;

        while ((type = parser.Step(out string typeName, out string propertyName, out string index)) != PropertyNodeType.None)
        {
            PropertyPathNode node = type switch
            {
                PropertyNodeType.AttachedProperty or PropertyNodeType.Property => new StandardPropertyPathNode(bindExpr, typeName, propertyName),
                PropertyNodeType.Indexed => new IndexedPropertyPathNode(bindExpr, index),
                _ => throw new InvalidOperationException(),
            };

            if (head == null)
            {
                head = tail = node;
                continue;
            }

            tail.Next = node;
            tail = node;
        }

        head ??= tail = new SourcePropertyNode(bindExpr);

        return (head, tail);
    }
}
