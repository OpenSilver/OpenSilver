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


using System.ComponentModel;

namespace OpenSilver
{
    /// <summary>
    /// Represents a stub class for XAML designer.
    /// </summary>
    /// <remarks>
    /// This class serves as a utility to ensure no conflicts with other possible constructors.
    /// It is not intended to have any functionality or to be instantiated directly.
    /// Instead, its presence is a signal for XAML Designer to recognize a specialized constructor.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class XamlDesignerConstructorStub
    {
        // Currently, the class is empty. However, future enhancements or modifications
        // can be added if necessary.
    }
}
