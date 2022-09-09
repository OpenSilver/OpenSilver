
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
namespace System.Windows.Automation
#else
namespace Windows.UI.Xaml.Automation
#endif
{
    /// <summary>
    /// Identifies a property of <see cref="AutomationElementIdentifiers"/>.
    /// </summary>
    public class AutomationProperty
    {
        private static readonly Dictionary<AutomationIdentifierConstants.Properties, AutomationProperty> _idTable 
            = new Dictionary<AutomationIdentifierConstants.Properties, AutomationProperty>();

        private AutomationProperty(AutomationIdentifierConstants.Properties id, string programmaticName)
        {
            Id = id;
            ProgrammaticName = programmaticName;
        }

        internal static AutomationProperty Register(AutomationIdentifierConstants.Properties id, string programmaticName)
        {
            lock (_idTable)
            {
                // See if instance already exists...
                if (_idTable.TryGetValue(id, out AutomationProperty autoid))
                {
                    return autoid;
                }

                // If not, create one...
                autoid = new AutomationProperty(id, programmaticName);

                _idTable[id] = autoid;
                return autoid;
            }
        }

        internal static AutomationProperty LookupById(AutomationIdentifierConstants.Properties id)
        {
            lock (_idTable)
            {
                if (_idTable.TryGetValue(id, out AutomationProperty autoid))
                {
                    return autoid;
                }

                return null;
            }
        }

        /// <summary>
        /// Returns underlying identifier as used by provider interfaces.
        /// </summary>
        /// <remarks>
        /// Use <see cref="LookupById"/> method to convert back from Id to an
        /// <see cref="AutomationProperty"/>
        /// </remarks>
        internal AutomationIdentifierConstants.Properties Id { get; }

        /// <summary>
        /// Returns the programmatic name passed in on registration.
        /// </summary>
        /// <remarks>
        /// Appends the type to the programmatic name.
        /// </remarks>
        internal string ProgrammaticName { get; }
    }
}
