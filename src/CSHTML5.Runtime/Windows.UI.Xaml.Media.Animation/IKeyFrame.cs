using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Media.Animation
#else
namespace Windows.UI.Xaml.Media.Animation
#endif
{
    public partial interface IKeyFrame
    {
        /// <summary>
        /// The key time associated with the key frame.
        /// </summary>
        /// <value></value>
        KeyTime KeyTime { get; set; }

        /// <summary>
        /// The value associated with the key frame.
        /// </summary>
        /// <value></value>
        object Value { get; set; }
    }
}
