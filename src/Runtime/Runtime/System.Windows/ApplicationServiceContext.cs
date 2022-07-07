
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

using System.Collections.Generic;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
	/// <summary>
	/// Represents the initial state of a Silverlight application when application extension
	/// services are started.
	/// </summary>
	public class ApplicationServiceContext
	{
		internal ApplicationServiceContext() { }

		/// <summary>
		/// Gets the initialization parameters specified by the host Web page when embedding
		/// the Silverlight plug-in.
		/// </summary>
		/// <returns>
		/// The initialization parameters as a dictionary of key/value pairs.
		/// </returns>
		public Dictionary<string, string> ApplicationInitParams => (Dictionary<string, string>)Application.Current.Host.InitParams;
	}
}
