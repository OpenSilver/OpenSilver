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

namespace OpenSilver.Internal.Xaml.Context
{
    internal class XamlObjectFrame
    {
        private int _depth;
        private XamlObjectFrame _previous;

        public XamlObjectFrame() { }

        public XamlObjectFrame Previous
        {
            get => _previous;
            set
            {
                _previous = value;
                _depth = (_previous == null) ? 0 : _previous._depth + 1;
            }
        }

        public int Depth => _depth;

        public object Instance { get; set; }

        public XamlObjectFrame DeepCopy()
        {
            return new XamlObjectFrame
            {
                Instance = Instance,
                _previous = _previous?.DeepCopy(),
                _depth = _depth
            };
        }
    }
}
