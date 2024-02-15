
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
	/// Provides static, predefined format names for data objects. Use the named constants
	/// to identify the format of the data that you request from an <see cref="IDataObject"/>
	/// object.
	/// </summary>
	public static class DataFormats
	{
		/// <summary>
		/// Specifies the Microsoft Windows file drop format.
		/// </summary>
		/// <returns>
		/// The string specifying the Microsoft Windows file drop format.
		/// </returns>
		public static readonly string FileDrop = "FileDrop";
	}
}
