
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

using System.Diagnostics;

namespace System.Windows
{
    /// <summary>
    /// Represents a collection of <see cref="EventTrigger"/> objects.
    /// </summary>
    public sealed class TriggerCollection : PresentationFrameworkCollection<TriggerBase>
    {
        private readonly IInternalFrameworkElement _owner;

        internal TriggerCollection() : base(false) { }

        internal TriggerCollection(IInternalFrameworkElement owner)
            : base(false)
        {
            Debug.Assert(owner is not null);
            _owner = owner;
        }

        internal override void AddOverride(TriggerBase value)
        {
            AddDependencyObjectInternal(value);
            
            if (_owner != null)
            {
                EventTrigger.ProcessOneTrigger(_owner, value);
            }
        }

        internal override void ClearOverride()
        {
            if (_owner != null)
            {
                EventTrigger.DisconnectAllTriggers(_owner);
            }
            
            ClearDependencyObjectInternal();
        }

        internal override void InsertOverride(int index, TriggerBase value)
        {
            InsertDependencyObjectInternal(index, value);

            if (_owner != null)
            {
                EventTrigger.ProcessOneTrigger(_owner, value);
            }
        }

        internal override void RemoveAtOverride(int index)
        {
            TriggerBase trigger = this[index];
            RemoveAtDependencyObjectInternal(index);

            if (_owner != null)
            {
                EventTrigger.DisconnectOneTrigger(_owner, trigger);
            }
        }

        internal override TriggerBase GetItemOverride(int index)
        {
            return GetItemInternal(index);
        }

        internal override void SetItemOverride(int index, TriggerBase value)
        {
            TriggerBase oldTrigger = this[index];
            SetItemDependencyObjectInternal(index, value);
            
            if (_owner != null)
            {
                EventTrigger.DisconnectOneTrigger(_owner, oldTrigger);
                EventTrigger.ProcessOneTrigger(_owner, value);
            }
        }
    }
}
