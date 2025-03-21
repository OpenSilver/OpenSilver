
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

using System.Diagnostics;

namespace System.Windows.Media.Animation;

/// <summary>
/// A trigger action that stops a <see cref="Storyboard"/>.
/// </summary>
public sealed class StopStoryboard : ControllableStoryboardAction
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StopStoryboard"/> class.
    /// </summary>
    public StopStoryboard() { }

    internal override void Invoke(IFrameworkElement containingFE, Storyboard storyboard)
    {
        Debug.Assert(containingFE is not null, "Caller of internal function failed to verify that we have a FE.");

        if (containingFE is FrameworkElement fe)
        {
            storyboard.Stop(fe);
        }
    }
}
