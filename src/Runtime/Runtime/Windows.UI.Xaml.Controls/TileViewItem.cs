using System;
using System.Collections.Generic;
using System.Text;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public class TileViewItem : HeaderedContentControl
    {
        private Button _maximizeButton;
        internal TileView _tileViewParent = null; // Note: if this item is in TileView.Items, it is set during TileView.OnApplyTemplate

        public TileViewItem()
        {
            this.DefaultStyleKey = typeof(TileViewItem);
        }

#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate() 
#endif
        {
            base.OnApplyTemplate();
            _maximizeButton = GetTemplateChild("PART_MaximizeButton") as Button;

            _maximizeButton.Click += MaximizeButton_Click;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            // if the parent is a TileView, tell it that this tile wants to be maximized.
            if (_tileViewParent != null)
            {
                _tileViewParent.MaximizeTile(this);
            }
        }

        internal void Maximize()
        {
            VisualStateManager.GoToState(this, "Maximized", false);
            OnMaximize?.Invoke(this, null);
        }

        internal void Minimize()
        {
            VisualStateManager.GoToState(this, "Minimized", false);
            OnMinimize?.Invoke(this, null);
        }

        public event EventHandler OnMaximize;
        public event EventHandler OnMinimize;
    }
}
