namespace System.Windows.Input
{
    [OpenSilver.NotImplemented]
    public sealed partial class TouchPoint : DependencyObject
    {
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty TouchDeviceProperty = DependencyProperty.Register("TouchDeviceProperty", typeof(TouchDevice), typeof(TouchPoint), new PropertyMetadata());
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("PositionProperty", typeof(Point), typeof(TouchPoint), new PropertyMetadata());
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty ActionProperty = DependencyProperty.Register("ActionProperty", typeof(TouchAction), typeof(TouchPoint), new PropertyMetadata());
        [OpenSilver.NotImplemented]
        public TouchDevice TouchDevice
        {
            get
            {
                return (TouchDevice)this.GetValue(TouchPoint.TouchDeviceProperty);
            }
        }

        [OpenSilver.NotImplemented]
        public Point Position
        {
            get
            {
                return (Point)this.GetValue(TouchPoint.PositionProperty);
            }
        }

        [OpenSilver.NotImplemented]
        public TouchAction Action
        {
            get
            {
                return (TouchAction)this.GetValue(TouchPoint.ActionProperty);
            }
        }

        [OpenSilver.NotImplemented]
        public TouchPoint()
        {
        }
    }
}
