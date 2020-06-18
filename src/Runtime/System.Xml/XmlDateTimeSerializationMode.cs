
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
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