
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

namespace System.Windows.Media;

/// <summary>
/// Describes the simulation style of a font.
/// </summary>
public enum StyleSimulations
{
    /// <summary>
    /// No font style simulation.
    /// </summary>
    None = 0,

    /// <summary>
    /// Bold style simulation.
    /// </summary>
    BoldSimulation = 1,

    /// <summary>
    /// Italic style simulation.
    /// </summary>
    ItalicSimulation = 2,

    /// <summary>
    /// Bold and Italic style simulation.
    /// </summary>
    BoldItalicSimulation = 3
}
