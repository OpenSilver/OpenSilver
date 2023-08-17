using System.Runtime.InteropServices;

namespace OpenSilver.Simulator.XamlInspection
{
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [ComVisible(true)]
    public class XamlInspectorCallback
    {
        private bool _isExecutingMouseEvent;

        public void OnMouseMove(int x, int y)
        {
            if (_isExecutingMouseEvent)
            {
                return;
            }

            _isExecutingMouseEvent = true;
            XamlInspectionHelper.HighlightElementAtPoint(x, y);
            _isExecutingMouseEvent = false;
        }

        public void OnMouseDown(int x, int y)
        {
            if (_isExecutingMouseEvent)
            {
                return;
            }

            _isExecutingMouseEvent = true;
            XamlInspectionHelper.SelectElementAtPoint(x, y);
            _isExecutingMouseEvent = false;
        }
    }
}
