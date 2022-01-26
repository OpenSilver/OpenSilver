

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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
	//
	// Summary:
	//     Represents a composition related to text input which includes the composition
	//     text itself.
    [OpenSilver.NotImplemented]
	public sealed partial class TextComposition
	{
		//
		// Summary:
		//     Gets the composition text for this text composition.
		//
		// Returns:
		//     The composition text for this text composition.
        [OpenSilver.NotImplemented]
		public string CompositionText { get; }
	}
}
