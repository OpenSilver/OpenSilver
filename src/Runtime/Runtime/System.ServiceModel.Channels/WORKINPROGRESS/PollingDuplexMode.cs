
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

namespace System.ServiceModel.Channels
{
    /// <summary>
    /// Specifies the format of server responses to client polling when the communication
    /// is configured for duplex polling.
    /// </summary>
    public enum PollingDuplexMode
    {
        /// <summary>
        /// Specifies the server will return one message every time the client polls for
        /// a message and then close the polling connection. This is the default value.
        /// </summary>
        SingleMessagePerPoll,

        /// <summary>
        /// Specifies the server keeps the connection open for as long there are messages
        /// ready to be sent back to the client and sends as many messages back to the client
        /// as it can over a chunked HTTP response.
        /// </summary>
        MultipleMessagesPerPoll
    }
}
