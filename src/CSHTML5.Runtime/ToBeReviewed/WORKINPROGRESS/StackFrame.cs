
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



using System.Reflection;

namespace System.Diagnostics
{
    public class StackFrame
    {
        public StackFrame(int skipFrames)
        {
        }

        public MethodBase GetMethod()
        {
            return this.GetType().GetMethod("Error");
        }

        public void Error()
        {
            throw new NotImplementedException("StackFrame error - Error - ");
        }
    }

}