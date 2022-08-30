
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

using System.ComponentModel;

namespace System.Runtime.InteropServices.Automation
{
    /// <summary>
    /// Provides access to registered Automation servers.
    /// </summary>
    [OpenSilver.NotImplemented]
    public static class AutomationFactory
    {    
        /// <summary>
        /// Gets a value that indicates whether the Automation feature in Silverlight is
        /// available to the application.
        /// </summary>
        /// <returns>
        /// true if the Automation feature in Silverlight is available to the application;
        /// otherwise, false.
        /// </returns>
        public static bool IsAvailable { get; }
        
        /// <summary>
        /// Activates and returns a reference to the registered Automation server with the
        /// specified programmatic identifier (ProgID).
        /// </summary>
        /// <param name="progID">
        /// The ProgID of the registered automation server to activate.
        /// </param>
        /// <returns>
        /// A late-bound reference to the specified automation server.
        /// </returns>
        /// <exception cref="Exception">
        /// No object was found registered for the specified progID.
        /// </exception>
        public static object CreateObject(string progID) => throw new NotImplementedException();
        
        /// <summary>
        /// Not yet implemented.
        /// </summary>
        /// <typeparam name="T">
        /// The type of object to create.
        /// </typeparam>
        /// <returns>
        /// null.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static T CreateObject<T>() => default;

        /// <summary>
        /// Gets an object that represents the specified event of the specified Automation server.
        /// </summary>
        /// <param name="automationObject">
        /// A reference to the Automation server to retrieve an event for.
        /// </param>
        /// <param name="eventName">
        /// The name of the event to retrieve.
        /// </param>
        /// <returns>
        /// An object that represents the specified event.
        /// </returns>
        public static AutomationEvent GetEvent(object automationObject, string eventName)
            => throw new NotImplementedException();

        /// <summary>
        /// Gets a reference to the previously activated and currently running Automation
        /// server with the specified programmatic identifier (ProgID).
        /// </summary>
        /// <param name="progID">
        /// The ProgID of the registered Automation server to retrieve a reference to.
        /// </param>
        /// <returns>
        /// A late-bound reference to the specified Automation server.
        /// </returns>
        /// <exception cref="Exception">
        /// No object was found registered for the specified progID.
        /// </exception>
        public static object GetObject(string progID)
            => throw new NotImplementedException();
    }
}

#endif
