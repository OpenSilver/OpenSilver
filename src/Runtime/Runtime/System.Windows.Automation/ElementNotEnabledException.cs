
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

namespace System.Windows.Automation
{
    /// <summary>
    /// The exception that is thrown when an attempt is made through UI automation to
    /// manipulate a control that is not enabled.
    /// </summary>
	public class ElementNotEnabledException : Exception
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="ElementNotEnabledException"/> class.
        /// </summary>
        public ElementNotEnabledException() { }
    }
}
