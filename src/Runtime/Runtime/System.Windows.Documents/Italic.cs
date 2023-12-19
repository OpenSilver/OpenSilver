
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

namespace System.Windows.Documents;

/// <summary>
/// Provides an inline-level flow content element that causes content to render with
/// an italic font style.
/// </summary>
public sealed class Italic : Span
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Italic"/> class.
    /// </summary>
    public Italic()
    {
        FontStyle = FontStyles.Italic;
    }
}
