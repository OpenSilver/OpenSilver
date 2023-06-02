
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
using System.Windows.Media.Animation;
#else
using Windows.UI.Xaml.Media.Animation;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Serves as the base class for <see cref="BeginStoryboard"/>.
    /// </summary>
    public abstract class TriggerAction : DependencyObject
    {
        internal TriggerAction() { }

        /// <summary>
        ///     Called when all conditions have been satisfied for this action to be
        /// invoked.  (Conditions are not described on this TriggerAction object,
        /// but on the Trigger object holding it.)
        /// </summary>
        /// <remarks>
        ///     This variant is called when the Trigger lives on an element, as
        /// opposed to Style, so it is given only the reference to the element.
        /// </remarks>
        internal abstract void Invoke(IFrameworkElement fe);
    }
}
