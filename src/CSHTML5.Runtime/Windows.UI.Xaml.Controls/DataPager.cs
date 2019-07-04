
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



using CSHTML5.Internal;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;

using System.Windows.Input;

#if MIGRATION
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
#else
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public enum DataPager_DisplayMode
    {
        FirstLastPreviousNextNumeric,
        FirstLastNumeric,
        FirstLastPreviousNext,
        Numeric,
        PreviousNext,
        PreviousNextNumeric
    }

    public class DataPager : Control
    {
        TextBox _textBox;

        StackPanel _buttonPanel; // the panel that contains numeric buttons if any

        public DataPager()
        {
#if WORKINPROGRESS
            this.DefaultStyleKey = typeof(DataPager);
#else
            this.INTERNAL_SetDefaultStyle(INTERNAL_DefaultDataPagerStyle.GetDefaultStyle());
#endif
        }

#region DisplayMode

        /// <summary>
        /// Gets or sets the currently selected mode.
        /// </summary>
        public DataPager_DisplayMode DisplayMode
        {
            get { return (DataPager_DisplayMode)GetValue(DisplayModeProperty); }
            set { SetValue(DisplayModeProperty, value); }
        }

        /// <summary>
        /// Identifies the dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayModeProperty =
            DependencyProperty.Register(
            "DisplayMode",
            typeof(DataPager_DisplayMode),
            typeof(DataPager),
            new PropertyMetadata(DataPager_DisplayMode.Numeric, OnDisplayModeChanged));

        private static void OnDisplayModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPager datapager = (DataPager)d;

            if (datapager.Source != null && (DataPager_DisplayMode)e.NewValue != (DataPager_DisplayMode)e.OldValue)
                datapager.GenerateControls();
        }

#endregion

#region NumericButtonCount

        /// <summary>
        /// The max number of numeric button that can be displayed
        /// </summary>
        public int NumericButtonCount
        {
            get { return (int)GetValue(NumericButtonCountProperty); }
            set { SetValue(NumericButtonCountProperty, value); }
        }

        /// <summary>
        /// Identifies the dependency property.
        /// </summary>
        public static readonly DependencyProperty NumericButtonCountProperty =
            DependencyProperty.Register(
            "NumericButtonCount",
            typeof(int),
            typeof(DataPager),
            new PropertyMetadata(0, OnNumericButtonCountChanged));

        private static void OnNumericButtonCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPager datapager = (DataPager)d;

            if (datapager.Source != null && (int)e.NewValue != (int)e.OldValue)
                datapager.GenerateControls();
        }

#endregion

#region Source

        /// <summary>
        /// The max number of numeric buttons that can be displayed
        /// </summary>
        public PagedCollectionView Source
        {
            get { return (PagedCollectionView)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Identifies the dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(
            "Source",
            typeof(PagedCollectionView),
            typeof(DataPager),
            new PropertyMetadata(OnSourceChanged));

        private static void OnSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPager datapager = (DataPager)d;

            if (e.NewValue != null && e.NewValue != e.OldValue)
            {
                if (e.OldValue != null)
                {
                    ((PagedCollectionView)e.OldValue).CollectionChanged -= datapager.Source_CollectionChanged;
                    ((PagedCollectionView)e.OldValue).PageChanged -= datapager.Source_PageChanged;
                }

                datapager.Source.PageSize = datapager.PageSize;

                datapager.NumberOfPages = datapager.CountNumberPage();

                datapager.CurrentPage = datapager.CurrentPage; // re run the check because the current page can now be out of bounds

                datapager.Source.CollectionChanged += datapager.Source_CollectionChanged;

                datapager.Source.PageChanged += datapager.Source_PageChanged;

                datapager.GenerateControls();
            }
            else
                datapager.NumberOfPages = 0;
        }

#endregion

#region PageSize

        /// <summary>
        /// Gets or sets the maximum number of items to display on a page.
        /// -1 means infinity. Default is -1.
        /// </summary>
        public int PageSize
        {
            get { return (int)GetValue(PageSizeProperty); }
            set { SetValue(PageSizeProperty, value); }
        }

        /// <summary>
        /// Identifies the dependency property.
        /// </summary>
        public static readonly DependencyProperty PageSizeProperty =
            DependencyProperty.Register(
            "PageSize",
            typeof(int),
            typeof(DataPager),
            new PropertyMetadata(-1, OnPageSizeChanged));

        /// <summary>
        /// the data PageSize
        /// </summary>
        /// <param name="d">TempCanvas that changed its SelectedDate.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs.</param>
        private static void OnPageSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPager datapager = (DataPager)d;

            if (e.NewValue != null && datapager.Source != null && (int)e.NewValue != (int)e.OldValue)
            {
                datapager.Source.PageSize = (int)e.NewValue;

                datapager.NumberOfPages = datapager.CountNumberPage();
            }
        }

#endregion

#region CurrentPage

        /// <summary>
        /// // the current page that is shown
        /// </summary>
        public int CurrentPage
        {
            get { return (int)GetValue(CurrentPageProperty); }
            set { SetValue(CurrentPageProperty, GetNewCurrentPageIndex(value)); }
        }

        /// <summary>
        /// Identifies the dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register(
            "CurrentPage",
            typeof(int),
            typeof(DataPager),
            new PropertyMetadata(0, OnCurrentPageChanged));

        private static void OnCurrentPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataPager datapager = (DataPager)d;

            if (e.NewValue != null && datapager.Source != null && (int)e.NewValue != (int)e.OldValue)
            {
                int newIndex = datapager.GetNewCurrentPageIndex((int)e.NewValue);

                if (datapager.CurrentPage == newIndex) // we do not want that MoveToPage to be called several times, so we check the index before
                {
                    datapager.Source.MoveToPage(((int)e.NewValue) - 1); // -1 because Source has an zero-based page index
                }

                if (datapager._buttonPanel != null)
                    datapager.RefreshControl();
            }
        }

#endregion

#region Numeric button style

        /// <summary>
        /// Gets or sets an instance Style that is applied for this object during rendering.
        /// </summary>
        public Style NumericButtonStyle
        {
            get { return (Style)GetValue(NumericButtonStyleProperty); }
            set { SetValue(NumericButtonStyleProperty, value); }
        }
        /// <summary>
        /// Identifies the Style dependency property.
        /// </summary>
        public static readonly DependencyProperty NumericButtonStyleProperty =
            DependencyProperty.Register("NumericButtonStyle", typeof(Style), typeof(DataPager), new PropertyMetadata(null));

#endregion

        // compute the number of pages in the PagedCollectionView
        int CountNumberPage()
        {
            int nbElements = Source.TotalItemCount;

            if (nbElements == 0) return 0; // no element => no page

            if (PageSize == -1) return 1; // unlimited => one page

            int nbPage = nbElements / PageSize;

            return nbPage + (nbElements % PageSize == 0 ? 0 : 1); // +1 if some elements need to be in an imcomplete page
        }

        // contrain the page index to remain within the allowed range
        int GetNewCurrentPageIndex(int requestedIndex)
        {
            int newIndex;

            if (NumberOfPages == 0)
                newIndex = 0;
            else if (requestedIndex < 1)
                newIndex = 1;
            else if (requestedIndex > NumberOfPages)
                newIndex = NumberOfPages;
            else
                newIndex = requestedIndex;

            return newIndex;
        }


        // the total number of pages that is used as limit
        public int NumberOfPages { get; private set; }

        // get the original data, before paging, grouping, filtering and sorting
        public IEnumerable SourceCollection { get { return Source.SourceCollection; } }

#if MIGRATION
        public override void OnApplyTemplate()
#else
        protected override void OnApplyTemplate()
#endif
        {
            base.OnApplyTemplate();

            _buttonPanel = this.GetTemplateChild("NumericButtonPanel") as StackPanel;

            if (_buttonPanel != null)
            {
                EventRegister("FirstPageButton");
                EventRegister("LastPageButton");
                EventRegister("PreviousPageButton");
                EventRegister("NextPageButton");
                EventRegister("CurrentPageTextBox");

                if (Source != null)
                    GenerateControls();
            }
        }

        void EventRegister(string childName)
        {
            object child = this.GetTemplateChild(childName);
            if (child is Button)
            {
                Button button = (Button)child;
                button.Click -= OnPageButtonClick;
                button.Click += OnPageButtonClick;
            }
            if (child is TextBox)
            {
                TextBox text = (TextBox)child;
                _textBox = text;
                text.TextChanged -= OnPageSelectionTextChanged;
                text.TextChanged += OnPageSelectionTextChanged;
            }
        }

        // add one button to the button panel, buttons are not regenerated everytime, index are refreshed in RefreshControl
        void AddButton()
        {
            string nameAndContent = _buttonPanel.Children.Count.ToString() + 1;

            Button newButton = new Button();
            newButton.Style = NumericButtonStyle;

            newButton.Cursor = Cursors.Hand; // seems to not be by default

            newButton.Content = nameAndContent;
            newButton.Name = nameAndContent;
            newButton.Click += OnPageButtonClick;

            _buttonPanel.Children.Add(newButton);
        }

        // determine which is the first button index to display 
        int GetStartButtonIndex()
        {
            if (CurrentPage == 0) // no element
                return 0;

            int nbButtonFromMiddle = NumericButtonCount / 2;

            int nbPageIncreaseIfNecessary = Math.Max(NumericButtonCount, NumberOfPages); // in case of fewer pages than buttons, we consider the nb of pages instead.

            int min = CurrentPage - nbButtonFromMiddle;
            int max = CurrentPage + nbButtonFromMiddle - (NumericButtonCount % 2 == 0 ? 1 : 0); // 1 or 0 because nbButtonFromMiddle is rounded by the int format.

            if (min < 1)
            {
                min += -min + 1; // + 1 because the first page is displayed with index 1, but it still represents the view 0
                max += -min + 1;
            }
            if (max > nbPageIncreaseIfNecessary)
            {
                min -= max - nbPageIncreaseIfNecessary;
                max -= max - nbPageIncreaseIfNecessary;
            }
            return min;
        }

        // refresh the index of the button and the textbox
        void RefreshControl()
        {
            int newButtonNb = GetStartButtonIndex();

            foreach (UIElement element in _buttonPanel.Children)
            {
                if (element is Button) // not supposed to be other thing
                {
                    Button button = (Button)element;

                    button.Name = newButtonNb.ToString();
                    button.Content = newButtonNb.ToString();

                    if (newButtonNb == CurrentPage)
                        VisualStateManager.GoToState(button, "Checked", false);
                    else
                        VisualStateManager.GoToState(button, "Unchecked", false);

                    newButtonNb++;
                }
            }

            if (_textBox != null)
                _textBox.Text = CurrentPage.ToString();
        }

        // when the original source changes, we need to make sure there is still the right number of buttons and the right number of pages
        void RefereshNumberOfButtonsAndPages()
        {
            int oldNumberOfPages = NumberOfPages;
            NumberOfPages = CountNumberPage();

            int nbButtonMax = Math.Min(NumericButtonCount, NumberOfPages);

            if (_buttonPanel != null)
            {
                // "add" and "remove" can't be both != 0
                int add = Math.Max(nbButtonMax - _buttonPanel.Children.Count, 0);
                int remove = Math.Max(_buttonPanel.Children.Count - nbButtonMax, 0);

                for (int i = 0; i < add; i++)
                {
                    AddButton();
                }

                for (int i = 0; i < remove; i++)
                {
                    _buttonPanel.Children.RemoveAt(0);
                }

                if (_buttonPanel.Children.Count == 0) // because 0 button is not nice (this button will be a 0 zero button)
                {
                    add = 1;
                    AddButton();
                }

                //todo: allow other behaviour?
                if (NumberOfPages <= 1)
                    this.Visibility = Visibility.Collapsed;
                else
                    this.Visibility = Visibility.Visible;

                CurrentPage = GetNewCurrentPageIndex(CurrentPage);

                if (add != 0 || remove != 0 || oldNumberOfPages != NumberOfPages)
                    RefreshControl();
            }
        }

        // create the buttons if numeric, set the click events and go to the right visual state
        void GenerateControls()
        {
            string mode = DisplayMode.ToString();

            VisualStateManager.GoToState(this, mode, false);

            if (_buttonPanel != null)
                RefereshNumberOfButtonsAndPages();
        }

        // the method called when we click on any button 
        public void OnPageButtonClick(object sender, RoutedEventArgs e)
        {
            string buttonName = sender is Button ? ((Button)sender).Name : ((Button)sender).Name;

            switch (buttonName)
            {
                case "FirstPageButton":
                    CurrentPage = 1;
                    break;
                case "LastPageButton":
                    CurrentPage = NumberOfPages;
                    break;

                case "PreviousPageButton":
                    CurrentPage--;
                    break;

                case "NextPageButton":
                    CurrentPage++;
                    break;

                default:
                    int buttonNumber = int.Parse(buttonName);
                    CurrentPage = buttonNumber;
                    break;
            }
        }

        // the method called if we change the textbox that contains the page number
        public void OnPageSelectionTextChanged(object sender, TextChangedEventArgs e)
        {
            int newPageIndex;

            TextBox textBox = (TextBox)e.OriginalSource;

            if (textBox.Text != string.Empty)
            {
                if (int.TryParse(textBox.Text, out newPageIndex))
                {
                    if (newPageIndex != CurrentPage)
                        CurrentPage = newPageIndex;
                }
                else
                {
                    textBox.Text = CurrentPage.ToString();
                }
            }
        }

        void Source_PageChanged(object sender, EventArgs e)
        {
            CurrentPage = Source.PageIndex + 1;
        }

        void Source_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefereshNumberOfButtonsAndPages();
        }
    }
}
