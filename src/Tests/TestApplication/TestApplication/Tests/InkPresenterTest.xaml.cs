using System;
using System.Windows;
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
        private void OnIP_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            InkPad.CaptureMouse();
            StylusPointCollection MyStylusPointCollection = new StylusPointCollection();
            MyStylusPointCollection.Add(e.StylusDevice.GetStylusPoints(InkPad));
            LastStroke = new Stroke(MyStylusPointCollection);
            InkPad.Strokes.Add(LastStroke);
        }

        private void OnIP_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            InkPad.ReleaseMouseCapture();
        }

        //StylusPoint objects are collected from the MouseEventArgs and added to MyStroke. 
        private void OnIP_MouseMove(object sender, MouseEventArgs e)
        {
            if (LastStroke != null && InkPad.IsMouseCaptured)
            {
                LastStroke.StylusPoints.Add(e.StylusDevice.GetStylusPoints(InkPad));
            }
        }

        //MyStroke is completed
        private void OnIP_LostMouseCapture(object sender, MouseEventArgs e)
        {

        }

        private void OnClearInkPad(object sender, RoutedEventArgs e)
        {
            LastStroke = null;
            InkPad.Strokes.Clear();
        }

        private void OnUndoLastStroke(object sender, RoutedEventArgs e)
        {
            if (InkPad.Strokes.Contains(LastStroke))
            {
                InkPad.Strokes.Remove(LastStroke);
            }
        }

        private void OnRedoLastStroke(object sender, RoutedEventArgs e)
        {
            if (!InkPad.Strokes.Contains(LastStroke))
            {
                InkPad.Strokes.Add(LastStroke);
            }
        }
    }
}
