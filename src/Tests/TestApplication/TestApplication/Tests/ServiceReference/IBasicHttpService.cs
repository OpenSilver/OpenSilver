
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


using System.ServiceModel;
using System.ServiceModel.Channels;

namespace TestApplication.Tests.ServiceReference
{
    [ServiceContract(Namespace = "", ConfigurationName = "LegacyBasicHttpServiceReference.BasicHttpService")]
    public interface IBasicHttpService
    {
        // Action is "*" to test if the actual Action will be retrieved from the Message parameter Header
        [OperationContract(AsyncPattern = true, Action = "*", ReplyAction = "*")]
        System.IAsyncResult BeginBodyMember(Message message, System.AsyncCallback callback, object asyncState);

        Message EndBodyMember(System.IAsyncResult result);
    }
}
