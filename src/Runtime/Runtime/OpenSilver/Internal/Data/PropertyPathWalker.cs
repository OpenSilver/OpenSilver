
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

#if MIGRATION
using System.Windows.Data;
#else
using Windows.UI.Xaml.Data;
#endif

namespace OpenSilver.Internal.Data
{
    internal class PropertyPathWalker
    {
        private readonly BindingExpression _expr;

        internal PropertyPathWalker(BindingExpression be)
        {
            Binding binding = be.ParentBinding;

            _expr = be;
            ListenForChanges = binding.Mode != BindingMode.OneTime;

            string path = binding.XamlPath ?? binding.Path.Path ?? string.Empty;
            ParsePath(path, out IPropertyPathNode head, out IPropertyPathNode tail);

            FirstNode = head;
            FinalNode = tail;
        }

        internal bool ListenForChanges { get; }

        internal IPropertyPathNode FirstNode { get; }

        internal IPropertyPathNode FinalNode { get; }

        internal bool IsPathBroken
        {
            get
            {
                IPropertyPathNode node = FirstNode;
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

        internal void Update(object source)
        {
            FirstNode.Source = source;
        }

        internal void ValueChanged()
        {
            _expr.ValueChanged();
        }

        private void ParsePath(string path, out IPropertyPathNode head, out IPropertyPathNode tail)
        {
            head = null;
            tail = null;

            var parser = new PropertyPathParser(path, true);
            PropertyNodeType type;

            while ((type = parser.Step(out string typeName, out string propertyName, out string index)) != PropertyNodeType.None)
            {
                PropertyPathNode node;
                switch (type)
                {
                    case PropertyNodeType.AttachedProperty:
                    case PropertyNodeType.Property:
                        node = new StandardPropertyPathNode(this, typeName, propertyName);
                        break;
                    case PropertyNodeType.Indexed:
                        node = new IndexedPropertyPathNode(this, index);
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                if (head == null)
                {
                    head = tail = node;
                    continue;
                }

                tail.Next = node;
                tail = node;
            }

            if (head == null)
            {
                head = tail = new SourcePropertyNode(this);
            }
        }
    }
}
