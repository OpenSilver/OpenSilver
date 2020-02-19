using System;
using System.Collections;
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
    internal partial class INTERNAL_ResolvedKeyFramesEntries<T> : IEnumerable<int> where T : IKeyFrame
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
