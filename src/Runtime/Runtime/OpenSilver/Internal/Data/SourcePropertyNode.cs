
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
    internal class SourcePropertyNode : IPropertyPathNode
    {
        private readonly PropertyPathWalker _listener;
        private object _source;

        public SourcePropertyNode(PropertyPathWalker listener) 
        {
            _listener = listener;
        }

        object IPropertyPathNode.Value => _source;

        object IPropertyPathNode.Source
        {
            get => _source;
            set
            {
                _source = value;
                _listener.ValueChanged();
            }
        }

        bool IPropertyPathNode.IsBroken => false;

        Type IPropertyPathNode.Type => null;

        IPropertyPathNode IPropertyPathNode.Next 
        { 
            get => null;
            set => throw new NotSupportedException();
        }

        void IPropertyPathNode.SetValue(object value) => throw new NotSupportedException();
    }
}
