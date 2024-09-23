
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

using System.Windows.Browser.Internal;

namespace System.Windows.Browser
{
    /// <summary>
    /// Provides methods for browser elements that are explicitly supplied by the HTML
    /// features exposed by Silverlight.
    /// </summary>
    public abstract class HtmlObject : ScriptObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlObject"/> class.
        /// </summary>
		[OpenSilver.NotImplemented]
        protected HtmlObject()
            : this(null)
        {
        }

        internal HtmlObject(IJSObjectRef jsObject)
            : base(jsObject)
        {
        }

        /// <summary>
        /// Attaches the specified .NET Framework event handler (<see cref="EventHandler"/>) to
        /// the specified event on the current Document Object Model (DOM) object.
        /// </summary>
        /// <param name="eventName">
        /// A named event on the DOM object.
        /// </param>
        /// <param name="handler">
        /// The method that handles the event.
        /// </param>
        /// <returns>
        /// true if the event was successfully attached; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// eventName is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// eventName is null.-or-handler is null.
        /// </exception>
		[OpenSilver.NotImplemented]
        public bool AttachEvent(string eventName, EventHandler handler)
        {
            return false;
        }

        /// <summary>
        /// Attaches the specified .NET Framework event handler (<see cref="EventHandler{TEventArgs}"/>) to
        /// the specified event on the current Document Object Model (DOM) object.
        /// </summary>
        /// <param name="eventName">
        /// A named event on the DOM object.
        /// </param>
        /// <param name="handler">
        /// The method that handles the event.
        /// </param>
        /// <returns>
        /// true if the event was successfully attached; otherwise, false.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// eventName is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// eventName is null.-or-handler is null.
        /// </exception>
		[OpenSilver.NotImplemented]
        public bool AttachEvent(string eventName, EventHandler<HtmlEventArgs> handler)
        {
            return false;
        }

        /// <summary>
        /// Detaches the specified.NET Framework event handler (<see cref="EventHandler"/>) from
        /// the specified event on the current Document Object Model (DOM) object.
        /// </summary>
        /// <param name="eventName">
        /// A named event on the DOM object. 
        /// </param>
        /// <param name="handler">
        /// The method that handles the event.
        /// </param>
        /// <exception cref="ArgumentException">
        /// eventName is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// eventName is null.-or-handler is null.
        /// </exception>
		[OpenSilver.NotImplemented]
        public void DetachEvent(string eventName, EventHandler handler)
        {
        }
    
        /// <summary>
        /// Detaches the specified .NET Framework event handler (<see cref="EventHandler{TEventArgs}"/>) from
        /// the specified event on the current Document Object Model (DOM) object.
        /// </summary>
        /// <param name="eventName">
        /// A named event on the DOM object.
        /// </param>
        /// <param name="handler">
        /// The method that handles the event.
        /// </param>
        /// <exception cref="ArgumentException">
        /// eventName is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// eventName is null.-or-handler is null.
        /// </exception>
		[OpenSilver.NotImplemented]
        public void DetachEvent(string eventName, EventHandler<HtmlEventArgs> handler)
        {
        }

        /// <summary>
        /// Converts the current object to a specified type. This method is not supported.
        /// </summary>
        /// <param name="targetType">
        /// The conversion type.
        /// </param>
        /// <param name="allowSerialization">
        /// true to allow JavaScript Object Notation (JSON) serialization; otherwise, false.
        /// </param>
        /// <returns>
        /// None.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// In all cases.
        /// </exception>
		[OpenSilver.NotImplemented]
        protected internal override object ConvertTo(Type targetType, bool allowSerialization)
        {
            return null;
        }
    }
}
