

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
    public sealed partial class PathFigureCollection : List<PathFigure> // : IList<PathFigure>, IEnumerable<PathFigure>
    {
        /// <summary>
        /// Initializes a new instance of the PathFigureCollection class.
        /// </summary>
        public PathFigureCollection()
        {

        }

        /// <summary>
        /// Initializes a new instance that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity"> int - The number of elements that the new list is initially capable of storing. </param>
        public PathFigureCollection(int capacity) : base(capacity)
        {

        }

        internal void SetParentPath(Path path)
        {
            foreach (PathFigure figure in this)
            {
                figure.SetParentPath(path);
            }
        }
    }
}
