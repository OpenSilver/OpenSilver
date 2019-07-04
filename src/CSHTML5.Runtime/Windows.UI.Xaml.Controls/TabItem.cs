
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if MIGRATION
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
#else
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a selectable item in a
    /// <see cref="T:System.Windows.Controls.TabControl" />.
    /// </summary>
    /// <QualityBand>Mature</QualityBand>
    public partial class TabItem : ContentControl
    {
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:System.Windows.Controls.TabItem" /> class.
        /// </summary>
        public TabItem()
            : base()
        {
#if MIGRATION
            MouseLeftButtonDown += OnMouseLeftButtonDown;
            MouseEnter += OnMouseEnter;
            MouseLeave += OnMouseLeave;
#else
            PointerPressed += OnPointerPressed;
            PointerEntered += OnPointerEntered;
            PointerExited += OnPointerExited;
#endif
            GotFocus += delegate { IsFocused = true; };
            LostFocus += delegate { IsFocused = false; };
            //IsEnabledChanged += new DependencyPropertyChangedEventHandler(OnIsEnabledChanged);
            //DefaultStyleKey = typeof(TabItem);

            // Set default style:
#if WORKINPROGRESS
            this.DefaultStyleKey = typeof(TabItem);
#else
            this.INTERNAL_SetDefaultStyle(INTERNAL_DefaultTabItemStyle.GetDefaultStyle());
#endif
        }

        /// <summary>
        /// Builds the visual tree for the
        /// <see cref="T:System.Windows.Controls.TabItem" /> when a new template
        /// is applied.
        /// </summary>
#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            // Clear previous content from old ContentControl
            ContentControl cc = (IsSelected ? ElementHeaderTopSelected : ElementHeaderTopUnselected);
            if (cc != null)
            {
                cc.Content = null;
            }

            // Get the parts
            ElementTemplateTopSelected = GetTemplateChild(ElementTemplateTopSelectedName) as FrameworkElement;
            ElementTemplateTopUnselected = GetTemplateChild(ElementTemplateTopUnselectedName) as FrameworkElement;
            ElementHeaderTopSelected = GetTemplateChild(ElementHeaderTopSelectedName) as ContentControl;
            ElementHeaderTopUnselected = GetTemplateChild(ElementHeaderTopUnselectedName) as ContentControl;

            // Load Header
            UpdateHeaderVisuals();

            // Update visuals
            ChangeVisualState(false);
        }

#region Header
        /// <summary>
        /// Gets or sets the header of the
        /// <see cref="T:System.Windows.Controls.TabItem" />.
        /// </summary>
        /// <value>
        /// The current header of the
        /// <see cref="T:System.Windows.Controls.TabItem" />.
        /// </value>
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the
        /// <see cref="P:System.Windows.Controls.TabItem.Header" />
        /// dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the
        /// <see cref="P:System.Windows.Controls.TabItem.Header" />
        /// dependency property.
        /// </value>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                "Header",
                typeof(object),
                typeof(TabItem),
                new PropertyMetadata(null, OnHeaderChanged));

        /// <summary>
        /// Header property changed handler.
        /// </summary>
        /// <param name="d">TabItem that changed its Header.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnHeaderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItem ctrl = (TabItem)d;

            ctrl.HasHeader = (e.NewValue != null) ? true : false;
            ctrl.OnHeaderChanged(e.OldValue, e.NewValue);
        }

        /// <summary>
        /// Called when the
        /// <see cref="P:System.Windows.Controls.TabItem.Header" /> property
        /// changes.
        /// </summary>
        /// <param name="oldHeader">
        /// The previous value of the
        /// <see cref="P:System.Windows.Controls.TabItem.Header" />
        /// property.
        /// </param>
        /// <param name="newHeader">
        /// The new value of the
        /// <see cref="P:System.Windows.Controls.TabItem.Header" />
        /// property.
        /// </param>
        protected virtual void OnHeaderChanged(object oldHeader, object newHeader)
        {
            UpdateHeaderVisuals();
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void UpdateHeaderVisuals()
        {
            ContentControl header = (IsSelected ? ElementHeaderTopSelected : ElementHeaderTopUnselected);
            if (header != null)
            {
                header.ContentTemplate = this.HeaderTemplate;
                header.Content = this.Header;
            }
        }
#endregion Header

#region HasHeader
        /// <summary>
        /// Gets a value indicating whether the
        /// <see cref="T:System.Windows.Controls.TabItem" /> has a header.
        /// </summary>
        /// <value>
        /// True if <see cref="P:System.Windows.Controls.TabItem.Header" /> is
        /// not null; otherwise, false.
        /// </value>
        public bool HasHeader
        {
            get { return (bool)GetValue(HasHeaderProperty); }
            private set { SetValue(HasHeaderProperty, value); }
        }

        /// <summary>
        /// Identifies the
        /// <see cref="P:System.Windows.Controls.TabItem.HasHeader" />
        /// dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the
        /// <see cref="P:System.Windows.Controls.TabItem.HasHeader" />
        /// dependency property.
        /// </value>
        public static readonly DependencyProperty HasHeaderProperty =
            DependencyProperty.Register(
                "HasHeader",
                typeof(bool),
                typeof(TabItem),
                new PropertyMetadata(false));
#endregion HasHeader

#region HeaderTemplate
        /// <summary>
        /// Gets or sets the template that is used to display the content of the
        /// <see cref="T:System.Windows.Controls.TabItem" /> header.
        /// </summary>
        /// <value>
        /// The current template that is used to display
        /// <see cref="T:System.Windows.Controls.TabItem" /> header content.
        /// </value>
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        /// <summary>
        /// Identifies the
        /// <see cref="P:System.Windows.Controls.TabItem.HeaderTemplate" />
        /// dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the
        /// <see cref="P:System.Windows.Controls.TabItem.HeaderTemplate" />
        /// dependency property.
        /// </value>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(
                "HeaderTemplate",
                typeof(DataTemplate),
                typeof(TabItem),
                new PropertyMetadata(null, OnHeaderTemplateChanged));

        /// <summary>
        /// HeaderTemplate property changed handler.
        /// </summary>
        /// <param name="d">TabItem that changed its HeaderTemplate.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnHeaderTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItem ctrl = (TabItem)d;
            ctrl.OnHeaderTemplateChanged((DataTemplate)e.OldValue, (DataTemplate)e.NewValue);
        }

        /// <summary>
        /// Called when the
        /// <see cref="P:System.Windows.Controls.TabItem.HeaderTemplate" />
        /// property changes.
        /// </summary>
        /// <param name="oldHeaderTemplate">
        /// The previous value of the
        /// <see cref="P:System.Windows.Controls.TabItem.HeaderTemplate" />
        /// property.
        /// </param>
        /// <param name="newHeaderTemplate">
        /// The new value of the
        /// <see cref="P:System.Windows.Controls.TabItem.HeaderTemplate" />
        /// property.
        /// </param>
        protected virtual void OnHeaderTemplateChanged(DataTemplate oldHeaderTemplate, DataTemplate newHeaderTemplate)
        {
            UpdateHeaderVisuals();
        }
#endregion HeaderTemplate

#region IsSelected
        /// <summary>
        /// Gets or sets a value indicating whether the
        /// <see cref="T:System.Windows.Controls.TabItem" /> is currently
        /// selected.
        /// </summary>
        /// <value>
        /// True if the <see cref="T:System.Windows.Controls.TabItem" /> is
        /// selected; otherwise, false.
        /// </value>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// Identifies the
        /// <see cref="P:System.Windows.Controls.TabItem.IsSelected" />
        /// dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the
        /// <see cref="P:System.Windows.Controls.TabItem.IsSelected" />
        /// dependency property.
        /// </value>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(
                "IsSelected",
                typeof(bool),
                typeof(TabItem),
                new PropertyMetadata(false, OnIsSelectedChanged));

        /// <summary>
        /// IsSelected changed handler.
        /// </summary>
        /// <param name="d">TabItem that changed IsSelected.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItem tabItem = d as TabItem;
            //Debug.Assert(tabItem != null, "TabItem should not be null!");

            bool isSelected = (bool)e.NewValue;

            RoutedEventArgs args = new RoutedEventArgs();

            if (isSelected)
            {
                tabItem.OnSelected(args);
            }
            else
            {
                tabItem.OnUnselected(args);
            }
        }

        /// <summary>
        /// Called to indicate that the
        /// <see cref="P:System.Windows.Controls.TabItem.IsSelected" /> property
        /// has changed to true.
        /// </summary>
        /// <param name="e">
        /// A <see cref="T:System.Windows.RoutedEventArgs" /> that contains the
        /// event data.
        /// </param>
        protected virtual void OnSelected(RoutedEventArgs e)
        {
            if (TabControlParent != null && TabControlParent.SelectedItem != this)
            {
                TabControlParent.SelectedItem = this;
                UpdateTabItemVisuals();
            }

        }

        /// <summary>
        /// Called to indicate that the
        /// <see cref="P:System.Windows.Controls.TabItem.IsSelected" /> property
        /// has changed to false.
        /// </summary>
        /// <param name="e">
        /// A <see cref="T:System.Windows.RoutedEventArgs" /> that contains the
        /// event data.
        /// </param>
        protected virtual void OnUnselected(RoutedEventArgs e)
        {
            if (TabControlParent != null && TabControlParent.SelectedItem == this)
            {
                TabControlParent.SelectedIndex = -1;
            }
            UpdateTabItemVisuals();
        }
#endregion IsSelected

        /// <summary>
        /// This method is invoked when the Content property changes.
        /// </summary>
        /// <param name="oldContent">
        /// The previous <see cref="T:System.Windows.Controls.TabItem" />
        /// content.
        /// </param>
        /// <param name="newContent">
        /// The new <see cref="T:System.Windows.Controls.TabItem" /> content.
        /// </param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);

            TabControl tabControl = TabControlParent;
            if (tabControl != null)
            {
                // If this is the selected TabItem then we should update
                // TabControl.SelectedContent
                if (IsSelected)
                {
                    tabControl.SelectedContent = newContent;
                }
            }
        }

        /// <summary>
        /// Called when the IsEnabled property changes.
        /// </summary>
        /// <param name="sender">
        /// Control that triggers this property change.
        /// </param>
        /// <param name="e">Property changed args.</param>
        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //Debug.Assert(e.NewValue is bool, "New value should be a Boolean!");
            bool isEnabled = (bool)e.NewValue;
            ContentControl header = (IsSelected ? ElementHeaderTopSelected : ElementHeaderTopUnselected);
            if (header != null)
            {
                if (!isEnabled)
                {
                    _isMouseOver = false;
                }

                UpdateVisualState();
            }
        }

#region IsFocused
        /// <summary>
        /// Gets a value indicating whether this element has logical focus.
        /// </summary>
        public bool IsFocused
        {
            get { return (bool)GetValue(IsFocusedProperty); }
            internal set { SetValue(IsFocusedProperty, value); }
        }

        /// <summary>
        /// Identifies the
        /// <see cref="P:System.Windows.Controls.TabItem.IsFocused" />
        /// dependency property.
        /// </summary>
        /// <value>
        /// The identifier for the
        /// <see cref="P:System.Windows.Controls.TabItem.IsFocused" />
        /// dependency property.
        /// </value>
        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.Register(
                "IsFocused",
                typeof(bool),
                typeof(TabItem),
                new PropertyMetadata(false, OnIsFocusedPropertyChanged));

        /// <summary>
        /// IsFocusedProperty property changed handler.
        /// </summary>
        /// <param name="d">TabItem that changed IsFocused.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnIsFocusedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TabItem ti = d as TabItem;
            //Debug.Assert(ti != null, "TabItem should not be null!");

            ti.OnIsFocusChanged(e);
        }

        /// <summary>
        /// Called when the IsFocused property changes.
        /// </summary>
        /// <param name="e">
        /// A <see cref="T:System.Windows.DependencyPropertyChangedEventArgs" />
        /// that contains the event data.
        /// </param>
        //[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "e", Justification = "Compat with WPF.")]
        protected virtual void OnIsFocusChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateVisualState();
        }
#endregion IsFocused

        /// <summary>
        /// Change to the correct visual state for the TabItem.
        /// </summary>
        internal void UpdateVisualState()
        {
            ChangeVisualState(true);
        }

        /// <summary>
        /// Change to the correct visual state for the TabItem.
        /// </summary>
        /// <param name="useTransitions">
        /// True to use transitions when updating the visual state, false to
        /// snap directly to the new visual state.
        /// </param>
        private void ChangeVisualState(bool useTransitions)
        {
            // Choose the appropriate TabItem template to display based on which
            // TabStripPlacement we are using and whether the item is selected.
            UpdateTabItemVisuals();

            // Update the CommonStates group
            if (!IsEnabled || (TabControlParent != null && !TabControlParent.IsEnabled))
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateDisabled, VisualStates.StateNormal);
            }
            else if (_isMouseOver && !IsSelected)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateMouseOver, VisualStates.StateNormal);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateNormal);
            }

            // Update the SelectionStates group
            if (IsSelected)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateSelected, VisualStates.StateUnselected);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateUnselected);
            }

            // Update the FocusStates group
            if (IsFocused && IsEnabled)
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateFocused, VisualStates.StateUnfocused);
            }
            else
            {
                VisualStates.GoToState(this, useTransitions, VisualStates.StateUnfocused);
            }
        }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private void UpdateTabItemVisuals()
        {
            // update the template that is displayed
            FrameworkElement currentTemplate = (IsSelected ? ElementTemplateTopSelected : ElementTemplateTopUnselected);

            if (_previousTemplate != null && _previousTemplate != currentTemplate)
            {
                _previousTemplate.Visibility = Visibility.Collapsed;
            }
            _previousTemplate = currentTemplate;
            if (currentTemplate != null)
            {
                currentTemplate.Visibility = Visibility.Visible;
            }

            // update the ContentControl's header
            ContentControl currentHeader = (IsSelected ? ElementHeaderTopSelected : ElementHeaderTopUnselected);

            if (_previousHeader != null && _previousHeader != currentHeader)
            {
                _previousHeader.Content = null;
            }
            _previousHeader = currentHeader;
            UpdateHeaderVisuals();
        }

        /// <summary>
        /// Handles when the mouse leaves the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The MouseEventArgs.</param>
#if MIGRATION
        private void OnMouseLeave(object sender, MouseEventArgs e)
#else
        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
#endif
        {
            _isMouseOver = false;
            UpdateVisualState();
        }

        /// <summary>
        /// Handles when the mouse enters the control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The MouseEventArgs.</param>
#if MIGRATION
        private void OnMouseEnter(object sender, MouseEventArgs e)
#else
        private void OnPointerEntered(object sender, PointerRoutedEventArgs e)
#endif
        {
            _isMouseOver = true;
            UpdateVisualState();
        }

        /// <summary>
        /// Handles the mouse left button down.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The MouseButtonEventArgs.</param>
#if MIGRATION
        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
#else
        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
#endif
        {
            if (IsEnabled && TabControlParent != null && !IsSelected && !e.Handled)
            {
                //IsTabStop = true;
                //todo: we will probably need the next line?
                //e.Handled = Focus();
                TabControlParent.SelectedIndex = TabControlParent.Items.IndexOf(this);
            }
        }

        /// <summary>
        /// Gets a reference to the TabControl that holds this TabItem.  It will
        /// step up the UI tree to find the TabControl that contains this
        /// TabItem.
        /// </summary>
        private TabControl TabControlParent
        {
            get
            {
                // We need this for when the TabControl/TabItem is not in the
                // visual tree yet.
                TabControl tabCtrl = INTERNAL_VisualParent as TabControl;//Parent as TabControl;
                if (tabCtrl != null)
                {
                    return tabCtrl;
                }

                // Once the TabControl is added to the visual tree, the
                // TabItem's parent becomes the TabPanel, so we now have to step
                // up the visual tree to find the owning TabControl.
                DependencyObject obj = this as DependencyObject;
                while (obj != null)
                {
                    TabControl tc = obj as TabControl;
                    if (tc != null)
                    {
                        return tc;
                    }
                    obj = ((UIElement)obj).INTERNAL_VisualParent; //VisualTreeHelper.GetParent(obj) as DependencyObject;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the TabStripPlacement Top Selected template.
        /// </summary>
        internal FrameworkElement ElementTemplateTopSelected { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementTemplateTopSelectedName = "TemplateTopSelected";

        /// <summary>
        /// Gets or sets the TabStripPlacement Top Unselected template.
        /// </summary>
        internal FrameworkElement ElementTemplateTopUnselected { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementTemplateTopUnselectedName = "TemplateTopUnselected";

        /// <summary>
        /// Gets or sets the Header of the TabStripPlacement Top Selected
        /// template.
        /// </summary>
        internal ContentControl ElementHeaderTopSelected { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementHeaderTopSelectedName = "HeaderTopSelected";

        /// <summary>
        /// Gets or sets the Header of the TabStripPlacement Top Unselected
        /// template.
        /// </summary>
        internal ContentControl ElementHeaderTopUnselected { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        internal const string ElementHeaderTopUnselectedName = "HeaderTopUnselected";

        /// <summary>
        /// Gets or sets a value indicating whether Inherited code: Requires comment.
        /// </summary>
        private bool _isMouseOver { get; set; }

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private FrameworkElement _previousTemplate;

        /// <summary>
        /// Inherited code: Requires comment.
        /// </summary>
        private ContentControl _previousHeader;


#region Properties added by the CSHTML5 team to make styling of tab controls easier

        /// <summary>
        /// Gets or sets the bakground color of the selected INTERNAL_CorrespondingItem.
        /// </summary>
        public Brush SelectedBackground
        {
            get { return (Brush)GetValue(SelectedBackgroundProperty); }
            set { SetValue(SelectedBackgroundProperty, value); }
        }
        /// <summary>
        /// Identifies the SelectedBackground dependency property
        /// </summary>
        public static readonly DependencyProperty SelectedBackgroundProperty =
            DependencyProperty.Register("SelectedBackground", typeof(Brush), typeof(TabItem), new PropertyMetadata(new SolidColorBrush(Colors.White)));

        /// <summary>
        /// Gets or sets the foreground color of the selected INTERNAL_CorrespondingItem.
        /// </summary>
        public Brush SelectedForeground
        {
            get { return (Brush)GetValue(SelectedForegroundProperty); }
            set { SetValue(SelectedForegroundProperty, value); }
        }
        /// <summary>
        /// Identifies the SelectedForeground dependency property
        /// </summary>
        public static readonly DependencyProperty SelectedForegroundProperty =
            DependencyProperty.Register("SelectedForeground", typeof(Brush), typeof(TabItem), new PropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Gets or sets the accent color of the selected INTERNAL_CorrespondingItem.
        /// </summary>
        public Brush SelectedAccent
        {
            get { return (Brush)GetValue(SelectedAccentProperty); }
            set { SetValue(SelectedAccentProperty, value); }
        }
        /// <summary>
        /// Identifies the SelectedAccent dependency property
        /// </summary>
        public static readonly DependencyProperty SelectedAccentProperty =
            DependencyProperty.Register("SelectedAccent", typeof(Brush), typeof(TabItem), new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

#endregion
    }
}
