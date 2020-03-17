using System;
using System.Windows;
using System.Windows.Threading;

namespace System.Windows.Controls
{
    public class BusyIndicator : ContentControl
    {
        #region Data
        /// <summary>
        /// Timer used to delay the initial display and avoid flickering.
        /// </summary>
        private DispatcherTimer _displayAfterTimer;

        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register("IsBusy", typeof(bool), typeof(BusyIndicator), new PropertyMetadata(false, OnIsBusyChanged));
        public static readonly DependencyProperty BusyContentProperty = DependencyProperty.Register("BusyContent", typeof(object), typeof(BusyIndicator), new PropertyMetadata(null));
        public static readonly DependencyProperty BusyContentTemplateProperty = DependencyProperty.Register("BusyContentTemplate", typeof(DataTemplate), typeof(BusyIndicator), new PropertyMetadata(null));
        public static readonly DependencyProperty DisplayAfterProperty = DependencyProperty.Register("DisplayAfter", typeof(TimeSpan), typeof(BusyIndicator), new PropertyMetadata(TimeSpan.FromSeconds(0.1)));
        public static readonly DependencyProperty OverlayStyleProperty = DependencyProperty.Register("OverlayStyle", typeof(Style), typeof(BusyIndicator), new PropertyMetadata(null));
        public static readonly DependencyProperty ProgressBarStyleProperty = DependencyProperty.Register("ProgressBarStyle", typeof(Style), typeof(BusyIndicator), new PropertyMetadata(null));
        #endregion

        #region Constructor
        public BusyIndicator()
        {
            //todo: add this
            this.DefaultStyleKey = typeof(BusyIndicator);
            this._displayAfterTimer = new DispatcherTimer();
            this.Loaded += delegate
            {
                this._displayAfterTimer.Tick += new EventHandler(this.DisplayAfterTimerElapsed);
            };
            this.Unloaded += delegate
            {
                this._displayAfterTimer.Tick -= new EventHandler(this.DisplayAfterTimerElapsed);
                this._displayAfterTimer.Stop();
            };
        }
        #endregion

        #region Public Properties
        public bool IsBusy
        {
            get { return (bool)this.GetValue(IsBusyProperty); }
            set { this.SetValue(IsBusyProperty, value); }
        }

        public object BusyContent
        {
            get { return this.GetValue(BusyIndicator.BusyContentProperty); }
            set { this.SetValue(BusyIndicator.BusyContentProperty, value); }
        }

        public DataTemplate BusyContentTemplate
        {
            get { return (DataTemplate)this.GetValue(BusyContentTemplateProperty); }
            set { this.SetValue(BusyContentTemplateProperty, value); }
        }

        public TimeSpan DisplayAfter
        {
            get { return (TimeSpan)this.GetValue(BusyIndicator.DisplayAfterProperty); }
            set { this.SetValue(BusyIndicator.DisplayAfterProperty, value); }
        }

        public Style OverlayStyle
        {
            get { return (Style)this.GetValue(BusyIndicator.OverlayStyleProperty); }
            set { this.SetValue(BusyIndicator.OverlayStyleProperty, value); }
        }

        public Style ProgressBarStyle
        {
            get { return (Style)this.GetValue(ProgressBarStyleProperty); }
            set { this.SetValue(ProgressBarStyleProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the BusyContent is visible.
        /// </summary>
        protected bool IsContentVisible { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Overrides the OnApplyTemplate method.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.ChangeVisualState(false);
        }

        /// <summary>
        /// Changes the control's visual state(s).
        /// </summary>
        /// <param name="useTransitions">True if state transitions should be used.</param>
        protected virtual void ChangeVisualState(bool useTransitions)
        {
            VisualStateManager.GoToState(this, this.IsBusy ? "Busy" : "Idle", useTransitions);
            VisualStateManager.GoToState(this, this.IsContentVisible ? "Visible" : "Hidden", useTransitions);
        }

        /// <summary>
        /// IsBusyProperty property changed handler.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected virtual void OnIsBusyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.IsBusy)
            {
                if (this.DisplayAfter.Equals(TimeSpan.Zero))
                {
                    // Go visible now
                    this.IsContentVisible = true;
                }
                else
                {
                    // Set a timer to go visible
                    this._displayAfterTimer.Interval = this.DisplayAfter;
                    this._displayAfterTimer.Start();
                }
            }
            else
            {
                // No longer visible
                this._displayAfterTimer.Stop();
                this.IsContentVisible = false;
            }
            this.ChangeVisualState(true);
        }
        #endregion

        #region Internal API

        #region Internal Methods
        /// <summary>
        /// Handler for the DisplayAfterTimer.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event arguments.</param>
        private void DisplayAfterTimerElapsed(object sender, EventArgs e)
        {
            this._displayAfterTimer.Stop();
            this.IsContentVisible = true;
            this.ChangeVisualState(true);
        }

        private static void OnIsBusyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BusyIndicator)d).OnIsBusyChanged(e);
        }
        #endregion

        #endregion
    }
}
