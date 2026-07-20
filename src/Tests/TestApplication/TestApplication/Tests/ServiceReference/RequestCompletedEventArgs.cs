
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


using System;
using System.ComponentModel;

namespace TestApplication.Tests.ServiceReference
{
    public class RequestCompletedEventArgs : AsyncCompletedEventArgs
    {
        private readonly object[] _result;

        public string Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return _result[0] as string;
            }
        }

        public RequestCompletedEventArgs(object[] result, Exception error, bool cancelled, object userState) :
            base(error, cancelled, userState)
        {
            if (error == null && !cancelled)
            {
                _result = result;
            }
        }
    }
}
