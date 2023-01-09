
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
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Describes how content is positioned vertically in a container.
    /// </summary>
    public enum AlignmentY
	{
        /// <summary>
        /// The contents align toward the upper edge of the container.
        /// </summary>
        Top = 0,
        /// <summary>
        /// The contents align toward the center of the container.
        /// </summary>
		Center = 1,
        /// <summary>
        /// The contents align toward the lower edge of the container.
        /// </summary>
		Bottom = 2
	}
}
