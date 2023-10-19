
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

namespace System.Windows
{
	/// <summary>
	/// Defines methods that application extension services must implement in order to
	/// enable an application to start and stop the service.
	/// </summary>
	public partial interface IApplicationService
	{
		/// <summary>
		/// Called by an application in order to initialize the application extension service.
		/// </summary>
		/// <param name="context">
		/// Provides information about the application state.
		/// </param>
		void StartService(ApplicationServiceContext context);

		/// <summary>
		/// Called by an application in order to stop the application extension service.
		/// </summary>
		void StopService();
	}
}
