

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


namespace System.Windows.Interactivity
{
    internal class NameResolvedEventArgs : EventArgs
    {
        private object oldObject;
        private object newObject;

        public object OldObject
        {
            get { return oldObject; }
        }

        public object NewObject
        {
            get { return newObject; }
        }

        public NameResolvedEventArgs(object oldObject, object newObject)
        {
            this.oldObject = oldObject;
            this.newObject = newObject;
        }
    }
}