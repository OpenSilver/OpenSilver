
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
using CSHTML5.Internal.System.Windows.Data;
#else
using CSHTML5.Internal.Windows.UI.Xaml.Data;
#endif

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    internal class PropertyPathWalker
    {
        private readonly string _path;
        private readonly PropertyPathNode _firstNode;
        private readonly BindingExpression _expr;
        private object _source;

        internal PropertyPathWalker(BindingExpression be)
        {
            Binding binding = be.ParentBinding;

            _expr = be;
            _path = binding.XamlPath ?? binding.Path.Path ?? string.Empty;
            IsDataContextBound = binding.ElementName == null && binding.Source == null && binding.RelativeSource == null;
            ListenForChanges = binding.Mode != BindingMode.OneTime;

            if (IsDataContextBound)
            {
                _firstNode = new DataContextNode(this);
            }

            ParsePath(_path, out PropertyPathNode head, out PropertyPathNode tail);

            if (_firstNode == null)
            {
                _firstNode = head ?? new StandardPropertyPathNode(this);
            }
            else
            {
                _firstNode.Next = head;
            }

            FinalNode = tail ?? _firstNode;
        }

        internal bool IsDataContextBound { get; }

        internal bool ListenForChanges { get; }

        internal PropertyPathNode FinalNode { get; }

        internal object ValueInternal { get; private set; }

        internal bool IsPathBroken
        {
            get
            {
                PropertyPathNode node = _firstNode;
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
            _source = source;
            _firstNode.SetSource(source);
        }

        internal void ValueChanged(PropertyPathNode node)
        {
            ValueInternal = node.Value;
            _expr.ValueChanged();
        }

        private void ParsePath(string path, out PropertyPathNode head, out PropertyPathNode tail)
        {
            head = null;
            tail = null;

            var parser = new PropertyPathParser(path);
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
        }
    }
}
