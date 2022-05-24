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
    internal class XamlContextStack
    {
        public XamlContextStack()
        {
            Grow();
        }

        public XamlObjectFrame CurrentFrame { get; private set; }

        public XamlObjectFrame PreviousFrame => CurrentFrame.Previous;

        public int Depth { get; private set; }

        public void PushScope()
        {
            Grow();
            Depth++;
        }

        public void PopScope()
        {
            Depth--;
            CurrentFrame = CurrentFrame.Previous;
        }

        //allocate a new frame as the new currentFrame;
        private void Grow()
        {
            XamlObjectFrame lastFrame = CurrentFrame;
            CurrentFrame = new XamlObjectFrame();
            CurrentFrame.Previous = lastFrame;
        }

        public XamlContextStack DeepCopy()
        {
            return new XamlContextStack()
            {
                CurrentFrame = CurrentFrame?.DeepCopy(),
                Depth = Depth,
            };
        }
    }
}
