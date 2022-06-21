using System;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;

namespace TestApplication.OpenSilver.Tests
{
    public partial class InkPresenterTest : Page
    {
        public InkPresenterTest()
        {
            this.InitializeComponent();
        }

        private Stroke LastStroke;

        //A new stroke object named MyStroke is created. MyStroke is added to the StrokeCollection of the InkPresenter named MyIP
        private void OnIP_MouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            InkPad.CaptureMouse();
            StylusPointCollection MyStylusPointCollection = new StylusPointCollection();
#if OPENSILVER
            var point = e.GetPosition(InkPad);
            MyStylusPointCollection.Add(new StylusPoint() { X = point.X, Y = point.Y });
#else
            MyStylusPointCollection.Add(e.StylusDevice.GetStylusPoints(MyIP));
#endif
            LastStroke = new Stroke(MyStylusPointCollection);
            InkPad.Strokes.Add(LastStroke);
        }

        //StylusPoint objects are collected from the MouseEventArgs and added to MyStroke. 
        private void OnIP_MouseMove(object sender, MouseEventArgs e)
        {
            if (LastStroke != null && InkPad.IsMouseCaptured)
            {
#if OPENSILVER
                var point = e.GetPosition(InkPad);
                LastStroke.StylusPoints.Add(new StylusPoint() { X = point.X, Y = point.Y });
#else
                NewStroke.StylusPoints.Add(e.StylusDevice.GetStylusPoints(MyIP));
#endif
            }

        }

        //MyStroke is completed
        private void OnIP_LostMouseCapture(object sender, MouseEventArgs e)
        {

        }

        private void OnClearInkPad(object sender, System.Windows.RoutedEventArgs e)
        {
            LastStroke = null;
            InkPad.Strokes.Clear();
        }

        private void OnUndoLastStroke(object sender, System.Windows.RoutedEventArgs e)
        {
            if (InkPad.Strokes.Contains(LastStroke))
            {
                InkPad.Strokes.Remove(LastStroke);
            }
        }

        private void OnRedoLastStroke(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!InkPad.Strokes.Contains(LastStroke))
            {
                InkPad.Strokes.Add(LastStroke);
            }
        }
    }
}
