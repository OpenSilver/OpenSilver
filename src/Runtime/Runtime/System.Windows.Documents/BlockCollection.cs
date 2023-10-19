
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

namespace System.Windows.Documents
{
	/// <summary>
	/// Represents a collection of <see cref="Block"/> elements.
	/// </summary>
	public sealed class BlockCollection : TextElementCollection<Block>
	{
		internal BlockCollection(DependencyObject owner, bool isOwnerParent)
			: base(owner)
		{
		}
	}
}
