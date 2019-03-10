
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


using System;
using System.ComponentModel;
using System.IO;

namespace System.Net
{
#if WORKINPROGRESS
    public class OpenWriteCompletedEventArgs //: AsyncCompletedEventArgs
    {
        #region Not supported yet
        // Summary:
        //     Gets a writable stream that is used to send data to a server.
        //
        // Returns:
        //     A System.IO.Stream where you can write data to be uploaded.
        //
        // Exceptions:
        //   System.InvalidOperationException:
        //     The asynchronous request was cancelled.
        public Stream Result
        {
            get { return null; }
        }
        #endregion
    }
#endif
}
