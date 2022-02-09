
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
    /// <summary>
	/// Argument passed to PreviewInvoke event. Assigning Cancelling to True will cancel the invoking of the trigger.
	/// </summary>
	/// <remarks>This is an infrastructure class. Behavior attached to a trigger base object can add its behavior as a listener to TriggerBase.PreviewInvoke.</remarks>
    public class PreviewInvokeEventArgs : EventArgs
    {
        public bool Cancelling { get; set; }
    }
}