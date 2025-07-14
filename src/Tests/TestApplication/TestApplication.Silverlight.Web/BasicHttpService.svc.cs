
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
using System.ServiceModel.Activation;

namespace TestApplication.Silverlight.Web
{
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class BasicHttpService
    {
        [OperationContract]
        public string Echo(string message)
        {
            return $"Echo response to '{message}'.";
        }

        [OperationContract]
        public BodyMemberResponseMessage BodyMember(BodyMemberRequestMessage message)
        {
            return new BodyMemberResponseMessage
            {
                Response = $"BodyMember response to '{message.Request}'."
            };
        }
    }

    [MessageContract]
    public class BodyMemberRequestMessage
    {
        [MessageBodyMember(
            Name = "BodyMemberRequest",
            Namespace = ""
        )]
        public string Request { get; set; }
    }

    [MessageContract]
    public class BodyMemberResponseMessage
    {
        [MessageBodyMember(
            Name = "BodyMemberResponse",
            Namespace = "")]
        public string Response { get; set; }
    }
}
