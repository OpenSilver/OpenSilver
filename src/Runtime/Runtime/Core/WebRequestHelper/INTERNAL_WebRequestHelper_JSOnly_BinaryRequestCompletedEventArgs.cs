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

//extern alias custom;
namespace System.Net
{
    internal sealed class INTERNAL_WebRequestHelper_JSOnly_BinaryRequestCompletedEventArgs
    {
        public INTERNAL_WebRequestHelper_JSOnly_BinaryRequestCompletedEventArgs()
        { }

        internal INTERNAL_WebRequestHelper_JSOnly_BinaryRequestCompletedEventArgs(System.Net.INTERNAL_WebRequestHelper_JSOnly_BinaryRequestCompletedEventArgs e)
        {
            Result = e.Result;
            UserState = e.UserState;
            Error = e.Error;
            Cancelled = e.Cancelled;
        }

        public byte[] Result { get; set; }

        public object UserState { get; internal set; }

        public bool Cancelled { get; internal set; }

        public Exception Error { get; set; }
    }
}