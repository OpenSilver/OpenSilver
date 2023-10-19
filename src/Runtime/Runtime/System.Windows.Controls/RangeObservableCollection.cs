
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace System.Windows.Controls
{
    public class RangeObservableCollection<T> : ObservableCollection<T>
    {
        private bool _suppressNotification = false;

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (!_suppressNotification)
                base.OnCollectionChanged(e);
        }

        private void UpdateCollection(ICollection<T> list, Action<T> action)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (!list.Any())
            {
                return;
            }

            _suppressNotification = true;

            try
            {
                foreach (var item in list)
                {
                    action(item);
                }
            }
            finally
            {
                _suppressNotification = false;
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void AddRange(IEnumerable<T> list)
        {
            UpdateCollection(list?.ToList(), Add);
        }

        public void RemoveRange(IEnumerable<T> list)
        {
            UpdateCollection(list?.ToList(), t => Remove(t));
        }
    }
}
