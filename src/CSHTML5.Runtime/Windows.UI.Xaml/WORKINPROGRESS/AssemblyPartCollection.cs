﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if WORKINPROGRESS

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    public sealed partial class AssemblyPartCollection : PresentationFrameworkCollection<AssemblyPart>
    {

    }
}
#endif