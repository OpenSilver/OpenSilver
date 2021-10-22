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

#if MIGRATION

namespace System.Windows.Interactivity
{
	[OpenSilver.NotImplemented]
    public abstract partial class TriggerAction<T> : TriggerAction where T : DependencyObject
    {
		[OpenSilver.NotImplemented]
        protected TriggerAction()
        {

        }

		[OpenSilver.NotImplemented]
        protected new T AssociatedObject { get; private set; }

        //protected sealed override Type AssociatedObjectTypeConstraint { get; }
    }
}

#endif
