
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

using System.Collections;
using System.Collections.Generic;

namespace System.Windows.Media.Animation
{
    internal class INTERNAL_ResolvedKeyFramesEntries<T> : IEnumerable<int> where T : IKeyFrame
    {
        private List<int> _resolvedKeyFramesIndexes;

        internal INTERNAL_ResolvedKeyFramesEntries()
        {
            _resolvedKeyFramesIndexes = new List<int>(); 
        }

        internal INTERNAL_ResolvedKeyFramesEntries(IList<T> keyFrames) : this()
        {
            int index = 0;
            foreach(T keyFrame in keyFrames)
            {
                ResolveKeyFrame(index++, keyFrames);
            }
        }

        private void ResolveKeyFrame(int index, IList<T> keyFrames)
        {
            bool isAdded = false;
            int currentIndex = 0;
            T keyFrame = keyFrames[index];
            while (!isAdded && currentIndex < _resolvedKeyFramesIndexes.Count)
            {
                if (keyFrame.KeyTime.TimeSpan < keyFrames[_resolvedKeyFramesIndexes[currentIndex]].KeyTime.TimeSpan)
                {
                    _resolvedKeyFramesIndexes.Insert(currentIndex, index);
                    isAdded = true;
                }
                else
                {
                    currentIndex++;
                }
            }
            if(!isAdded)
            {
                _resolvedKeyFramesIndexes.Add(index);
            }
        }

        internal int GetNextKeyFrameIndex(int resolvedKeyFrameCount)
        {
            if(resolvedKeyFrameCount >= _resolvedKeyFramesIndexes.Count || resolvedKeyFrameCount < 0)
            {
                return -1;
            }
            else
            {
                return _resolvedKeyFramesIndexes[resolvedKeyFrameCount];
            }
        }

        public IEnumerator<int> GetEnumerator()
        {
            return _resolvedKeyFramesIndexes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
