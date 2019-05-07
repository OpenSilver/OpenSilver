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
    internal class ResolvedKeyFramesEntries : IEnumerable<int>
    {

        private List<int> _resolvedKeyFramesIndexes;

        internal ResolvedKeyFramesEntries()
        {
            _resolvedKeyFramesIndexes = new List<int>(); 
        }

        internal ResolvedKeyFramesEntries(IList<DoubleKeyFrame> keyFrames) : this()
        {
            int index = 0;
            foreach(DoubleKeyFrame keyFrame in keyFrames)
            {
                ResolveKeyFrame(index++, keyFrames);
            }
        }

        private void ResolveKeyFrame(int index, IList<DoubleKeyFrame> keyFrames)
        {
            bool isAdded = false;
            int currentIndex = 0;
            DoubleKeyFrame keyFrame = keyFrames[index];
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
            if(resolvedKeyFrameCount >= _resolvedKeyFramesIndexes.Count)
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
