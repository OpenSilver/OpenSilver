using System;
using System.Collections.Generic;
using System.Text;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public class TileViewItem : ContentControl
    {
        Button _maximizeButton;
        //Button _minimizeButton;
        internal TileView _TileViewParent = null; //Note: if this item is in TileView.Items, it is set during TileView.OnApplyTemplate

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
            //_minimizeButton = GetTemplateChild("PART_MinimizeButton") as Button;

            _maximizeButton.Click += MaximizeButton_Click;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            //if the parent is a TileView, tell it that this tile wants to be maximized.
            if (_TileViewParent != null)
            {
                _TileViewParent.MaximizeTile(this);
            }
        }

        internal void Maximize()
        {
            VisualStateManager.GoToState(this, "Maximized", false);
            if (OnMaximize != null)
            {
                OnMaximize(this, null);
            }
        }
        internal void Minimize()
        {
            VisualStateManager.GoToState(this, "Minimized", false);
            if (OnMinimize != null)
            {
                OnMinimize(this, null);
            }
        }
        public event EventHandler OnMaximize;
        public event EventHandler OnMinimize;

        public object Header
        {
            get { return (object)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(TileViewItem), new PropertyMetadata(null));
    }
}
