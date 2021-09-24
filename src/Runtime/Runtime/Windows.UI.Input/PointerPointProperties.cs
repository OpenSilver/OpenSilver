using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if !MIGRATION
namespace Windows.UI.Input
{
    public partial class PointerPointProperties
    {
        /// <summary>
        /// Gets a value (the raw value reported by the device) that indicates the change in wheel button input from the last pointer event.
        /// Note: the value should not be Blindly trusted as it currently has no other choice than relying on a deprecated js event property (said property has also never existed on FireFox).
        /// If that property cannot be used, the getter will return a scroll value that depends on the browser, the device itself, and the system's settings for the mouse so you will need to take that into consideration.
        /// </summary>
        public int MouseWheelDelta { get; internal set; }
    }
}
#endif