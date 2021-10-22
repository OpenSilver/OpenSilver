

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
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Base class for controls that represents controls that can spin.
    /// </summary>
    /// <remarks>
    /// Spinner abstract class defines and implements common and focused visual state groups.
    /// Spinner abstract class defines and implements Spin event and OnSpin method.
    /// </remarks>
    [OpenSilver.NotImplemented]
    public abstract partial class Spinner : Control, IUpdateVisualState
    {
        /// <summary>
        /// Occurs when spinning is initiated by the end-user.
        /// </summary>
        [OpenSilver.NotImplemented]
        public event EventHandler<SpinEventArgs> Spin;

        /// <summary>
        /// Update the visual state of the control.
        /// </summary>
        /// <param name="useTransitions">A value indicating whether to automatically generate transitions to the new state, or instantly transition to the new state.</param>
        void IUpdateVisualState.UpdateVisualState(bool useTransitions)
        {
            throw new NotImplementedException();
        }
    }
}
