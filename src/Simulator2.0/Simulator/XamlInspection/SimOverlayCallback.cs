using System.Runtime.InteropServices;

namespace OpenSilver.Simulator.XamlInspection
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class SimOverlayCallback
    {
        private bool _isExecutingMouseEvent;

        public async void OnMouseMove(int x, int y)
        {
            if (_isExecutingMouseEvent) return;
            _isExecutingMouseEvent = true;
            XamlInspectionHelper.HighlightElementAtPoint(x, y);
            _isExecutingMouseEvent = false;
        }

        public async void OnMouseDown(int x, int y)
        {
            if (_isExecutingMouseEvent) return;
            _isExecutingMouseEvent = true;
            XamlInspectionHelper.SelectElementAtPoint(x, y);
            _isExecutingMouseEvent = false;
        }
    }
}
