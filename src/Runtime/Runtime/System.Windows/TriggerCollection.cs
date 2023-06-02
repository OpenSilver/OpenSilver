
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
using System.Diagnostics;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Represents a collection of <see cref="EventTrigger"/> objects.
    /// </summary>
    public sealed class TriggerCollection : PresentationFrameworkCollection<TriggerBase>
    {
        private readonly IInternalFrameworkElement _owner;

        internal TriggerCollection(IInternalFrameworkElement owner) : base(false) 
        {
            Debug.Assert(owner != null);
            _owner = owner;
        }

        internal override void AddOverride(TriggerBase value)
        {
            this.AddDependencyObjectInternal(value);
            EventTrigger.ProcessOneTrigger(_owner, value);
        }

        internal override void ClearOverride()
        {
            EventTrigger.DisconnectAllTriggers(_owner);
            this.ClearDependencyObjectInternal();
        }

        internal override void InsertOverride(int index, TriggerBase value)
        {
            this.InsertDependencyObjectInternal(index, value);
            EventTrigger.ProcessOneTrigger(_owner, value);
        }

        internal override void RemoveAtOverride(int index)
        {
            TriggerBase trigger = this[index];
            this.RemoveAtDependencyObjectInternal(index);
            EventTrigger.DisconnectOneTrigger(_owner, trigger);
        }

        internal override TriggerBase GetItemOverride(int index)
        {
            return this.GetItemInternal(index);
        }

        internal override void SetItemOverride(int index, TriggerBase value)
        {
            TriggerBase oldTrigger = this[index];
            this.SetItemDependencyObjectInternal(index, value);
            EventTrigger.DisconnectOneTrigger(_owner, oldTrigger);
            EventTrigger.ProcessOneTrigger(_owner, value);
        }
    }
}
