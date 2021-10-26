//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

#if MIGRATION
namespace System.Windows.Navigation
#else
namespace Windows.UI.Xaml.Navigation
#endif
{
    internal static class Guard
    {
#region Methods

        internal static void ArgumentNotNull(object arg, string name)
        {
            if (arg == null)
            {
                throw new ArgumentNullException(name);
            }
        }

#endregion
    }
}
