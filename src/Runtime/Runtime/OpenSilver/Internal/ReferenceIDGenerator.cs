
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

using System.Threading;

namespace OpenSilver.Internal
{
    /// <summary>
    /// This class has a method that generates IDs in sequence (0, 1, 2, 3...)
    /// </summary>
    internal sealed class ReferenceIDGenerator
    {
        private int _id = 0;

        internal int NewId() => Interlocked.Increment(ref _id);
    }
}
