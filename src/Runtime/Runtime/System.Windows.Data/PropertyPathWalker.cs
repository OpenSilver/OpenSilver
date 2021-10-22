
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
    internal class PropertyPathWalker : IPropertyPathNodeListener
    {
        private readonly string _path;
        private readonly bool _isDataContextBound;
        private readonly IPropertyPathNode _firstNode;
        private IPropertyPathWalkerListener _listener;
        private object _source;

        internal PropertyPathWalker(string path, bool isDatacontextBound)
        {
            _path = path;
            _isDataContextBound = isDatacontextBound;

            if (_isDataContextBound)
            {
                _firstNode = new DependencyPropertyNode(FrameworkElement.DataContextProperty);
            }

            ParsePath(_path, out IPropertyPathNode head, out IPropertyPathNode tail);

            if (_firstNode == null)
            {
                _firstNode = head ?? new StandardPropertyPathNode();
            }
            else
            {
                _firstNode.Next = head;
            }

            FinalNode = tail ?? _firstNode;

            FinalNode.Listen(this);
        }

        internal bool IsDataContextBound => _isDataContextBound;

        internal IPropertyPathNode FinalNode { get; }

        internal object ValueInternal { get; private set; }

        internal bool IsPathBroken
        {
            get
            {
                IPropertyPathNode node = _firstNode;
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

        internal void Listen(IPropertyPathWalkerListener listener)
        {
            _listener = listener;
        }

        internal void Unlisten(IPropertyPathWalkerListener listener)
        {
            if (_listener == listener)
            {
                _listener = null;
            }
        }

        internal void Update(object source)
        {
            _source = source;
            _firstNode.SetSource(source);
        }

        void IPropertyPathNodeListener.ValueChanged(IPropertyPathNode node)
        {
            ValueInternal = node.Value;
            IPropertyPathWalkerListener listener = _listener;
            if (listener != null)
            {
                listener.ValueChanged();
            }
        }

        private void ParsePath(string path, out IPropertyPathNode head, out IPropertyPathNode tail)
        {
            head = null;
            tail = null;

            var parser = new PropertyPathParser(path);
            PropertyNodeType type;

            while ((type = parser.Step(out string typeName, out string propertyName, out string index)) != PropertyNodeType.None)
            {
                IPropertyPathNode node;
                switch (type)
                {
                    case PropertyNodeType.AttachedProperty:
                    case PropertyNodeType.Property:
                        node = new StandardPropertyPathNode(typeName, propertyName);
                        break;
                    case PropertyNodeType.Indexed:
                        node = new IndexedPropertyPathNode(index);
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
