using System.Windows;
using System;

#if MIGRATION
namespace System.Windows.Input
#else
namespace Windows.UI.Xaml.Input
#endif
{
    [OpenSilver.NotImplemented]
	public sealed partial class TouchPointCollection : PresentationFrameworkCollection<TouchPoint>
	{
		internal TouchPointCollection()
		{
		}

        internal override void AddOverride(TouchPoint value)
        {
            this.AddInternal(value);
        }

        internal override void ClearOverride()
        {
            this.ClearInternal();
        }

        internal override void InsertOverride(int index, TouchPoint value)
        {
            this.InsertInternal(index, value);
        }

        internal override void RemoveAtOverride(int index)
        {
            this.RemoveAtInternal(index);
        }

        internal override bool RemoveOverride(TouchPoint value)
        {
            return this.RemoveInternal(value);
        }

        internal override TouchPoint GetItemOverride(int index)
        {
            return this.GetItemInternal(index);
        }

        internal override void SetItemOverride(int index, TouchPoint value)
        {
            this.SetItemInternal(index, value);
        }
    }
}
