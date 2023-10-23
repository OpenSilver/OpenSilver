
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

namespace OpenSilver.Internal.Media.Animation;

internal struct ResolvedKeyFrameEntry : IComparable<ResolvedKeyFrameEntry>, IComparable
{
    internal int _originalKeyFrameIndex;
    internal TimeSpan _resolvedKeyTime;

    public readonly int CompareTo(ResolvedKeyFrameEntry otherEntry)
    {
        if (otherEntry._resolvedKeyTime > _resolvedKeyTime)
        {
            return -1;
        }
        else if (otherEntry._resolvedKeyTime < _resolvedKeyTime)
        {
            return 1;
        }
        else
        {
            if (otherEntry._originalKeyFrameIndex > _originalKeyFrameIndex)
            {
                return -1;
            }
            else if (otherEntry._originalKeyFrameIndex < _originalKeyFrameIndex)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }

    public readonly int CompareTo(object other) => CompareTo((ResolvedKeyFrameEntry)other);
}
