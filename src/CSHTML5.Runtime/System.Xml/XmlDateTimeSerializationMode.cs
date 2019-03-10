
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Xml
{
    /// <summary>
    /// Specifies how to treat the time value when converting between string and
    /// System.DateTime.
    /// </summary>
    public enum XmlDateTimeSerializationMode
    {
        /// <summary>
        /// Treat as local time. If the System.DateTime object represents a Coordinated
        /// Universal Time (UTC), it is converted to the local time.
        /// </summary>
        Local = 0,
        /// <summary>
        /// Treat as a UTC. If the System.DateTime object represents a local time, it
        /// is converted to a UTC.
        /// </summary>
        Utc = 1,
        /// <summary>
        /// Treat as a local time if a System.DateTime is being converted to a string.
        /// </summary>
        Unspecified = 2,
     
        /// <summary>
        /// Time zone information should be preserved when converting.
        /// </summary>
        RoundtripKind = 3,
    }
}