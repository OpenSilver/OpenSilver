using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;

namespace TestApplication.Tests
{
    public partial class CursorTest : Page
    {
        // Handler used to demonstrate Mouse.AddQueryCursorHandler / Mouse.RemoveQueryCursorHandler
        // (the attached-event equivalent of the UIElement.QueryCursor instance event).
        private readonly QueryCursorEventHandler _attachedQueryCursorHandler;

        public CursorTest()
        {
            InitializeComponent();

            _attachedQueryCursorHandler = AttachedQueryCursorZone_QueryCursor;
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        #region 2) ForceCursor

        // Toggles ForceCursor on the outer border. If the mouse is currently over the border,
        // FrameworkElement.OnCursorChanged calls Mouse.UpdateCursor() automatically, so the
        // effect is visible immediately without having to move the mouse away and back.
        private void ForceCursorCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            ForceCursorOuterBorder.ForceCursor = ForceCursorCheckBox.IsChecked == true;
        }

        #endregion

        #region 3) QueryCursor event

        // Chooses a different cursor depending on which third of the border the pointer is over,
        // showing how the QueryCursor event lets code decide the cursor dynamically instead of
        // relying only on the static Cursor property.
        private void QueryCursorZone_QueryCursor(object sender, QueryCursorEventArgs e)
        {
            var border = (Border)sender;
            double x = e.GetPosition(border).X;
            double third = border.ActualWidth / 3d;

            string zoneName;
            if (x < third)
            {
                e.Cursor = Cursors.IBeam;
                zoneName = "Left (IBeam)";
            }
            else if (x < third * 2)
            {
                e.Cursor = Cursors.Cross;
                zoneName = "Middle (Cross)";
            }
            else
            {
                e.Cursor = Cursors.Hand;
                zoneName = "Right (Hand)";
            }

            // Mark the event as handled so that no ancestor overrides our choice
            // (unless an ancestor uses ForceCursor).
            e.Handled = true;

            QueryCursorStatus.Text = $"Current zone: {zoneName}";
        }

        // Toggles a Mouse.QueryCursor attached-event handler on and off, demonstrating the
        // static Mouse.AddQueryCursorHandler / Mouse.RemoveQueryCursorHandler API as an
        // alternative to subscribing to the instance QueryCursor event in XAML.
        private void AttachedQueryCursorCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            if (AttachedQueryCursorCheckBox.IsChecked == true)
            {
                Mouse.AddQueryCursorHandler(AttachedQueryCursorZone, _attachedQueryCursorHandler);
            }
            else
            {
                Mouse.RemoveQueryCursorHandler(AttachedQueryCursorZone, _attachedQueryCursorHandler);
            }

            // The mouse might already be over the element when the handler is added/removed,
            // so force the cursor to be recomputed right away.
            Mouse.UpdateCursor();
        }

        private void AttachedQueryCursorZone_QueryCursor(object sender, QueryCursorEventArgs e)
        {
            e.Cursor = Cursors.SizeAll;
            e.Handled = true;
        }

        #endregion

        #region 4) Mouse.OverrideCursor

        // Mouse.OverrideCursor forces a single cursor application-wide, overriding whatever
        // any individual element's Cursor/ForceCursor/QueryCursor logic would otherwise pick.
        private void OverrideCursorCheckBox_CheckedChanged(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = OverrideCursorCheckBox.IsChecked == true ? Cursors.Wait : null;
        }

        #endregion

        #region 5) Mouse.SetCursor / Mouse.UpdateCursor

        // Immediately (and imperatively) sets the mouse cursor, then restores the cursor that
        // would normally apply by calling Mouse.UpdateCursor after a short delay.
        private async void SetCursorButton_Click(object sender, RoutedEventArgs e)
        {
            SetCursorButton.IsEnabled = false;

            Dispatcher.BeginInvoke(async () =>
            {
                SetCursorStatus.Text = "Mouse.SetCursor(Cursors.Cross) was called: the pointer should now be a cross, everywhere, until it is restored.";

                Mouse.SetCursor(Cursors.Cross);

                await Task.Delay(1000);

                // Recompute the cursor as if the mouse had just moved, restoring whatever cursor
                // is appropriate for the element currently under the pointer.
                Mouse.UpdateCursor();

                SetCursorStatus.Text = "Mouse.UpdateCursor() restored the normal cursor for whatever is under the pointer.";
                SetCursorButton.IsEnabled = true;
            });
        }

        #endregion
    }
}
