
//-----------------------------------------------------------------------------
//  CONFIDENTIALITY NOTICE:
//  This code is the sole property of Userware and is strictly confidential.
//  Unless you have a written agreement in effect with Userware that states
//  otherwise, you are not authorized to view, copy, modify, or compile this
//  source code, and you should destroy all the copies that you possess.
//  Any redistribution in source code form is strictly prohibited.
//  Redistribution in binary form is allowed provided that you have obtained
//  prior written consent from Userware. You can contact Userware at:
//  contact@userware-solutions.com - Copyright (c) 2016 Userware
//-----------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Windows.Markup
{
    /// <summary>
    /// Represents a service that reports situational object-property relationships
    /// for markup extension evaluation.
    /// </summary>
    public interface IProvideValueTarget
    {
        /// <summary>
        /// Gets the target object being reported.
        /// </summary>
        object TargetObject { get; }

        /// <summary>
        /// Gets an identifier for the target property being reported.
        /// </summary>
        object TargetProperty { get; }
    }
}
