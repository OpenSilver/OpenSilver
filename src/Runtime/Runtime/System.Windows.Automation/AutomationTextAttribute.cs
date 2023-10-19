
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

namespace System.Windows.Automation
{
    /// <summary>
    /// Identifies UI Automation text attributes.
    /// </summary>
    public class AutomationTextAttribute
    {
        private static readonly Dictionary<AutomationIdentifierConstants.TextAttributes, AutomationTextAttribute> _idTable
            = new Dictionary<AutomationIdentifierConstants.TextAttributes, AutomationTextAttribute>();

        private AutomationTextAttribute(AutomationIdentifierConstants.TextAttributes id, string programmaticName)
        {
            Id = id;
            ProgrammaticName = programmaticName;
        }

        internal static AutomationTextAttribute Register(AutomationIdentifierConstants.TextAttributes id, string programmaticName)
        {
            lock (_idTable)
            {
                // See if instance already exists...
                AutomationTextAttribute autoid = _idTable[id];
                if (autoid != null)
                {
                    return autoid;
                }

                // If not, create one...
                autoid = new AutomationTextAttribute(id, programmaticName);

                _idTable[id] = autoid;
                return autoid;
            }
        }

        internal static AutomationTextAttribute LookupById(AutomationIdentifierConstants.TextAttributes id)
        {
            lock (_idTable)
            {
                return _idTable[id];
            }
        }

        /// <summary>
        /// Returns underlying identifier as used by provider interfaces.
        /// </summary>
        /// <remarks>
        /// Use <see cref="LookupById"/> method to convert back from Id to an
        /// <see cref="AutomationTextAttribute"/>
        /// </remarks>
        internal AutomationIdentifierConstants.TextAttributes Id { get; }

        /// <summary>
        /// Returns the programmatic name passed in on registration.
        /// </summary>
        /// <remarks>
        /// Appends the type to the programmatic name.
        /// </remarks>
        internal string ProgrammaticName { get; }
    }
}
