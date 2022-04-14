
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

#if WCF_STACK || BRIDGE

using System;

namespace System.ServiceModel
{
    /// <summary>
    /// Enables an object to extend another object through aggregation.
    /// </summary>
    /// <typeparam name="T">
    /// The object that participates in the custom behavior.
    /// </typeparam>
    public partial interface IExtension<T> where T : IExtensibleObject<T>
    {
        /// <summary>
        /// Enables an extension object to find out when it has been aggregated. Called
        /// when the extension is added to the <see cref="IExtensibleObject{T}.Extensions"/>
        /// property.
        /// </summary>
        /// <param name="owner">
        /// The extensible object that aggregates this extension.
        /// </param>
        void Attach(T owner);

        /// <summary>
        /// Enables an object to find out when it is no longer aggregated. Called when
        /// an extension is removed from the <see cref="IExtensibleObject{T}.Extensions"/>
        /// property.
        /// </summary>
        /// <param name="owner">
        /// The extensible object that aggregates this extension.
        /// </param>
        void Detach(T owner);
    }
}

#endif
