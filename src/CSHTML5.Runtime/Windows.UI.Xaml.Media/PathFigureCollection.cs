
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
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !MIGRATION
using Windows.UI.Xaml.Shapes;
#else
using System.Windows.Shapes;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Represents a collection of PathFigure objects that collectively make up the
    /// geometry of a PathGeometry.
    /// </summary>
    public sealed class PathFigureCollection : List<PathFigure> // : IList<PathFigure>, IEnumerable<PathFigure>
    {
        ///// <summary>
        ///// Initializes a new instance of the PathFigureCollection class.
        ///// </summary>
        //public PathFigureCollection() { }

        internal void SetParentPath(Path path)
        {
            foreach (PathFigure figure in this)
            {
                figure.SetParentPath(path);
            }
        }
    }
}
