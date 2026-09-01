
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

namespace OpenSilver.Compatibility;

/// <summary>
/// Specifies which control template conventions, such as template part names, are used for 
/// compatibility with WPF and Silverlight.
/// </summary>
public enum TemplateKind
{
    /// <summary>
    /// The control inspects the applied template and automatically determines whether it is
    /// a WPF or Silverlight control template conventions.
    /// </summary>
    Auto = 0,

    /// <summary>
    /// The WPF control template conventions.
    /// </summary>
    Wpf = 1,

    /// <summary>
    /// The Silverlight control template conventions.
    /// </summary>
    Silverlight = 2,
}
