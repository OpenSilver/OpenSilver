namespace System.Windows.Input
{
    [OpenSilver.NotImplemented]
	public sealed partial class TouchDevice : DependencyObject
	{
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty IdProperty = DependencyProperty.Register("IdProperty", typeof(int), typeof(TouchDevice), new PropertyMetadata());
        [OpenSilver.NotImplemented]
		public static readonly DependencyProperty DirectlyOverProperty = DependencyProperty.Register("DirectlyOverProperty", typeof(UIElement), typeof(TouchDevice), new PropertyMetadata());
        [OpenSilver.NotImplemented]
		public int Id
		{
			get
			{
				return (int)this.GetValue(TouchDevice.IdProperty);
			}
		}

        [OpenSilver.NotImplemented]
		public UIElement DirectlyOver
		{
			get
			{
				return (UIElement)this.GetValue(TouchDevice.DirectlyOverProperty);
			}
		}

        [OpenSilver.NotImplemented]
		public TouchDevice()
		{
		}
	}
}
