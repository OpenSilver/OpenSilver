

/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Represents a collection of collapsed and expanded AccordionItem controls.
    /// </summary>
    [OpenSilver.NotImplemented]
    public class Accordion : ItemsControl, IUpdateVisualState
    {
        /// <summary>
        /// Update the visual state of the control.
        /// </summary>
        /// <param name="useTransitions">
        /// A value indicating whether to automatically generate transitions to
        /// the new state, or instantly transition to the new state.
        /// </param>
        void IUpdateVisualState.UpdateVisualState(bool useTransitions)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Unselects all the AccordionItems in the Accordion control.
        /// </summary>
        /// <remarks>
        /// If the Accordion SelectionMode is Zero or ZeroOrMore all
        /// AccordionItems would be Unselected. If SelectionMode is One or
        /// OneOrMode  than all items would be Unselected and selected. Only the
        /// first AccordionItem would still be selected.
        /// </remarks>
        [OpenSilver.NotImplemented]
        public void UnselectAll()
        {
            UpdateAccordionItemsSelection(false);
        }

        /// <summary>
        /// Updates all accordionItems to be selected or unselected.
        /// </summary>
        /// <param name="selectedValue">
        /// True to select all items, false to unselect.
        /// </param>
        /// <remarks>
        /// Will not attempt to change a locked accordionItem.
        /// </remarks>
        private void UpdateAccordionItemsSelection(bool selectedValue)
        {
            throw new NotImplementedException();
        }


        #region public AccordionSelectionMode SelectionMode
        /// <summary>
        /// Gets or sets the AccordionSelectionMode used to determine the minimum
        /// and maximum selected AccordionItems allowed in the Accordion.
        /// </summary>
        [OpenSilver.NotImplemented]
        public AccordionSelectionMode SelectionMode
        {
            get => (AccordionSelectionMode)GetValue(SelectionModeProperty);
            set => SetValue(SelectionModeProperty, value);
        }

        /// <summary>
        /// Identifies the SelectionMode dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register(
                "SelectionMode",
                typeof(AccordionSelectionMode),
                typeof(Accordion),
                new PropertyMetadata(AccordionSelectionMode.One, OnSelectionModePropertyChanged));

        /// <summary>
        /// SelectionModeProperty property changed handler.
        /// </summary>
        /// <param name="d">Accordion that changed its SelectionMode.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSelectionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion public AccordionSelectionMode SelectionMode

        #region public object SelectedItem
        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        /// <remarks>
        /// The default value is null.
        /// When multiple items are allowed (IsMaximumOneSelected false),
        /// return the first of the selectedItems.
        /// </remarks>
        [OpenSilver.NotImplemented]
        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        /// <summary>
        /// Identifies the SelectedItem dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                "SelectedItem",
                typeof(object),
                typeof(Accordion),
                new PropertyMetadata(null, OnSelectedItemPropertyChanged));

        /// <summary>
        /// SelectedItemProperty property changed handler.
        /// </summary>
        /// <param name="d">Accordion that changed its SelectedItem.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnSelectedItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion public object SelectedItem

        #region public int SelectedIndex

        [OpenSilver.NotImplemented]
        public int SelectedIndex
        {
            get
            {
                return (int)GetValue(SelectedIndexProperty);
            }
            set
            {
                SetValue(SelectedIndexProperty, value);
            }
        }

        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty SelectedIndexProperty = 
            DependencyProperty.Register(
                "SelectedIndex", 
                typeof(int), 
                typeof(Accordion), 
                new PropertyMetadata(-1, OnSelectedIndexPropertyChanged));

        [OpenSilver.NotImplemented]
        private static void OnSelectedIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) { }
        #endregion


        #region public SelectionSequence SelectionSequence
        /// <summary>
        /// Gets or sets the SelectionSequence used to determine
        /// the order of AccordionItem selection.
        /// </summary>
        [OpenSilver.NotImplemented]
        public SelectionSequence SelectionSequence
        {
            get => (SelectionSequence)GetValue(SelectionSequenceProperty);
            set => SetValue(SelectionSequenceProperty, value);
        }

        /// <summary>
        /// Identifies the SelectionSequence dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty SelectionSequenceProperty =
            DependencyProperty.Register(
                "SelectionSequence",
                typeof(SelectionSequence),
                typeof(Accordion),
                new PropertyMetadata(SelectionSequence.Simultaneous, OnSelectionSequencePropertyChanged));

        /// <summary>
        /// Called when SelectionSequenceProperty changed.
        /// </summary>
        /// <param name="d">Accordion that changed its SelectionSequence property.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/>
        /// instance containing the event data.</param>
        private static void OnSelectionSequencePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
        #endregion public SelectionSequence SelectionSequence

        [OpenSilver.NotImplemented]
        public event SelectionChangedEventHandler SelectionChanged;

        #region public Style AccordionButtonStyle
        /// <summary>
        /// Gets or sets the Style that is applied to AccordionButton elements
        /// in the AccordionItems.
        /// </summary>
        [OpenSilver.NotImplemented]
        public Style AccordionButtonStyle
        {
            get => GetValue(AccordionButtonStyleProperty) as Style;
            set => SetValue(AccordionButtonStyleProperty, value);
        }

        /// <summary>
        /// The name used to indicate AccordionButtonStyle property.
        /// </summary>
        private const string AccordionButtonStyleName = "AccordionButtonStyle";

        /// <summary>
        /// Identifies the AccordionButtonStyle dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty AccordionButtonStyleProperty =
            DependencyProperty.Register(
                AccordionButtonStyleName,
                typeof(Style),
                typeof(Accordion),
                new PropertyMetadata(null, OnAccordionButtonStylePropertyChanged));

        /// <summary>
        /// AccordionButtonStyleProperty property changed handler.
        /// </summary>
        /// <param name="d">Accordion that changed its AccordionButtonStyle.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnAccordionButtonStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }
        #endregion public Style AccordionButtonStyle

        #region public DataTemplate ContentTemplate
        /// <summary>
        /// Gets or sets the DataTemplate used to display the content
        /// of each generated AccordionItem.
        /// </summary>
        /// <remarks>Either ContentTemplate or ItemTemplate is used.
        /// Setting both will result in an exception.</remarks>
        [OpenSilver.NotImplemented]
        public DataTemplate ContentTemplate
        {
            get => (DataTemplate)GetValue(ContentTemplateProperty);
            set => SetValue(ContentTemplateProperty, value);
        }

        /// <summary>
        /// Identifies the ContentTemplate dependency property.
        /// </summary>
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty ContentTemplateProperty =
            DependencyProperty.Register(
                "ContentTemplate",
                typeof(DataTemplate),
                typeof(Accordion),
                new PropertyMetadata(null));
        #endregion public DataTemplate ContentTemplate

        #region public ExpandDirection ExpandDirection
        /// <summary>
        /// Gets or sets the ExpandDirection property of each 
        /// AccordionItem in the Accordion control and the direction in which
        /// the Accordion does layout.
        /// </summary>
        /// <remarks>Setting the ExpandDirection will set the expand direction 
        /// on the accordionItems.</remarks>
        public ExpandDirection ExpandDirection
        {
            get { return (ExpandDirection)GetValue(ExpandDirectionProperty); }
            set { SetValue(ExpandDirectionProperty, value); }
        }

        /// <summary>
        /// Identifies the ExpandDirection dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpandDirectionProperty =
            DependencyProperty.Register(
                "ExpandDirection",
                typeof(ExpandDirection),
                typeof(Accordion),
                new PropertyMetadata(ExpandDirection.Down, OnExpandDirectionPropertyChanged));

        /// <summary>
        /// ExpandDirectionProperty property changed handler.
        /// </summary>
        /// <param name="d">Accordion that changed its ExpandDirection.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnExpandDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
        }
        #endregion public ExpandDirection ExpandDirection
    }
}
