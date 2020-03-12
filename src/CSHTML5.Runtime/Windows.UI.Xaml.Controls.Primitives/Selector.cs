

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


#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


#if MIGRATION
using System.Windows.Media;
#else
using Windows.UI.Xaml.Media;
#endif

#if MIGRATION
namespace System.Windows.Controls.Primitives
#else
namespace Windows.UI.Xaml.Controls.Primitives
#endif
{
    //[MarshalingBehavior(MarshalingType.Agile)]
    //[Threading(ThreadingModel.Both)]
    //[Version(100794368)]
    //[WebHostHidden]
    /// <summary>
    /// Represents a control that allows a user to select an item from a collection
    /// of items.
    /// </summary>
    public partial class Selector : ItemsControl
    {
        bool _selectionChangeIsOnIndex = false;
        bool _selectionChangeIsOnItem = false;
        bool _selectionChangeIsOnValue = false;

        protected bool ChangingSelectionInHtml { get; set; }
        protected bool ChangingSelectionProgrammatically { get; set; }


        protected override void OnItemsSourceChanged_BeforeVisualUpdate(IEnumerable oldValue, IEnumerable newValue)
        {
            SelectedIndex = -1;
        }

        //// Returns:
        ////     True if the SelectedItem is always synchronized with the current item in
        ////     the ItemCollection; false if the SelectedItem is never synchronized with
        ////     the current item; null if the SelectedItem is synchronized with the current
        ////     item only if the Selector uses an ICollectionView. The default value is null/indeterminate.
        ////     If you are programming using C# or Visual Basic, the type of this property
        ////     is projected as bool? (a nullable Boolean).
        ///// <summary>
        ///// Gets or sets a value that indicates whether a Selector should keep the SelectedItem
        ///// synchronized with the current item in the Items property.
        ///// </summary>
        //public bool? IsSynchronizedWithCurrentItem
        //{
        //    get { return (bool?)GetValue(IsSynchronizedWithCurrentItemProperty); }
        //    set { SetValue(IsSynchronizedWithCurrentItemProperty, value); }
        //}
        //public static readonly DependencyProperty IsSynchronizedWithCurrentItemProperty =
        //    DependencyProperty.Register("IsSynchronizedWithCurrentItem", typeof(bool?), typeof(Selector), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the index of the selected item.
        /// </summary>
        public int SelectedIndex
        {
            get { return (int)GetValue(SelectedIndexProperty); }
            set { SetValue(SelectedIndexProperty, value); }
        }
        /// <summary>
        /// Identifies the SelectedIndex dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIndexProperty =
            DependencyProperty.Register("SelectedindexProperty", typeof(int), typeof(Selector), new PropertyMetadata(-1, SelectedIndex_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });
        private static void SelectedIndex_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Selector selector = (Selector)d;
            selector.ManageSelectedIndex_Changed(e);
        }

        protected virtual void ManageSelectedIndex_Changed(DependencyPropertyChangedEventArgs e)
        {
            // Note: in this method, we use "Convert.ToInt32()" intead of casting to "(int)" because otherwise the JS code is not compatible with IE 9 (or Windows Phone 8.0).

            //if (!AreObjectsEqual(e.OldValue, e.NewValue))
            //{
                int newValue = Convert.ToInt32(e.NewValue);
                if (newValue < Items.Count) //The new value is ignored if it is bigger or equal than the amount of elements in the list of Items.
                {
                    if (!_selectionChangeIsOnValue && !_selectionChangeIsOnItem) //we only want to change the other ones if the change comes from SelectedIndex (otherwise it's already done by the one that was originally changed (SelectedItem or SelectedValue)
                    {
                        object oldItem = SelectedItem;
                        _selectionChangeIsOnIndex = true;
                        if (newValue == -1)
                        {
                            SetLocalValue(SelectedItemProperty, null); //we call SetLocalvalue directly to avoid replacing the BindingExpression that could be here on Mode = TwoWay
                            SetLocalValue(SelectedValueProperty, null); //we call SetLocalvalue directly to avoid replacing the BindingExpression that could be here on Mode = TwoWay

                            //todo: update binding of SelectedIndex

                            //selector.SelectedItem = null;
                            //selector.SelectedValue = null;
                        }
                        else
                        {
                            object item = Items[newValue]; //todo: make sure that the index always corresponds (I think it does but I didn't check)
                            SetLocalValue(SelectedItemProperty, item); //we call SetLocalvalue directly to avoid replacing the BindingExpression that could be here on Mode = TwoWay
                            SetLocalValue(SelectedValueProperty, PropertyPathHelper.AccessValueByApplyingPropertyPathIfAny(item, SelectedValuePath)); //we call SetLocalvalue directly to avoid replacing the BindingExpression that could be here on Mode = TwoWay

                            //todo: update binding of SelectedIndex

                            //selector.SelectedItem = item;
                            //selector.SelectedValue = selector.AccessValueByApplyingPropertyPathIfAny(item, selector.SelectedValuePath);
                        }
                        _selectionChangeIsOnIndex = false;

                        List<object> oldItems = new List<object>();
                        oldItems.Add(oldItem);
                        List<object> newItems = new List<object>();
                        newItems.Add(SelectedItem);

                        RefreshSelectedItem();

                        if (!ChangingSelectionInHtml) //the SelectionChanged event is already fired from the javascript event.
                        {
                            OnSelectionChanged(new SelectionChangedEventArgs(oldItems, newItems));
                        }
                    }
                    if (!ChangingSelectionInHtml)
                    {
                        ChangingSelectionProgrammatically = true; //so that it doesn't end up in a loop
                        ApplySelectedIndex(newValue);
                        ChangingSelectionProgrammatically = false;
                    }
                }
            //}
        }

        protected override void OnChildItemRemoved(object item)
        {
            base.OnChildItemRemoved(item);

            // Ensure that, when an item is removed from the list of items, we deselect it:
            if (this.SelectedItem == item)
            {
                SetLocalValue(SelectedIndexProperty, -1); //we call SetLocalvalue directly to avoid replacing the BindingExpression that could be here on Mode = TwoWay
                SetLocalValue(SelectedItemProperty, null); //we call SetLocalvalue directly to avoid replacing the BindingExpression that could be here on Mode = TwoWay
                SetLocalValue(SelectedValueProperty, null); //we call SetLocalvalue directly to avoid replacing the BindingExpression that could be here on Mode = TwoWay

                //todo: update binding of SelectedIndex, SelectedValue, and SelectedItem
            }
        }

        protected virtual void ApplySelectedIndex(int index)
        {
            // This is overridden for example by classes that implement selection through though native HTML controls, such as the native ComboBox.
        }

        /// <summary>
        /// Gets or sets the selected item.
        /// </summary>
        public object SelectedItem
        {
            get { return (object)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }
        /// <summary>
        /// Identifies the SelectedItem dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(object), typeof(Selector), new PropertyMetadata(null, SelectedItem_changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });
        private static void SelectedItem_changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!AreObjectsEqual(e.OldValue, e.NewValue))
            {
                Selector selector = (Selector)d;
                object newValue = (object)e.NewValue;
                if (!selector._selectionChangeIsOnValue && !selector._selectionChangeIsOnIndex) //we only want to change the other ones if the change comes from SelectedItem (otherwise it's already done by the one that was originally changed (SelectedIndex or SelectedValue)
                {
                    selector._selectionChangeIsOnItem = true;
                    if (newValue == null)
                    {
                        selector.SetLocalValue(SelectedIndexProperty, -1); //we call SetLocalvalue directly to avoid replacing the BindingExpression that could be here on Mode = TwoWay
                        selector.SetLocalValue(SelectedValueProperty, null); //we call SetLocalvalue directly to avoid replacing the BindingExpression that could be here on Mode = TwoWay

                        //todo: update binding of SelectedIndex

                        //selector.SelectedIndex = -1;
                        //selector.SelectedValue = null;
                    }
                    else
                    {
                        selector.SetLocalValue(SelectedIndexProperty, GetIndexOfElementInItems(selector, newValue)); //we call SetLocalvalue directly to avoid replacing the BindingExpression that could be here on Mode = TwoWay
                        selector.SetLocalValue(SelectedValueProperty, PropertyPathHelper.AccessValueByApplyingPropertyPathIfAny(newValue, selector.SelectedValuePath)); //we call SetLocalvalue directly to avoid replacing the BindingExpression that could be here on Mode = TwoWay

                        //todo: update binding of SelectedIndex

                        //selector.SelectedIndex = GetIndexOfElementInItems(selector, newValue);
                        //selector.SelectedValue = selector.AccessValueByApplyingPropertyPathIfAny(newValue, selector.SelectedValuePath);
                    }
                    selector._selectionChangeIsOnItem = false;

                    List<object> oldItems = new List<object>();
                    oldItems.Add(e.OldValue);
                    List<object> newItems = new List<object>();
                    newItems.Add(e.NewValue);

                    selector.RefreshSelectedItem();

                    selector.OnSelectionChanged(new SelectionChangedEventArgs(oldItems, newItems));
                }
            }
        }

        // Returns:
        //     The value of the selected item, obtained by using the SelectedValuePath,
        //     or null if no item is selected. The default value is null.
        /// <summary>
        /// Gets or sets the value of the selected item, obtained by using the SelectedValuePath.
        /// </summary>
        public object SelectedValue
        {
            get { return (object)GetValue(SelectedValueProperty); }
            set { SetValue(SelectedValueProperty, value); }
        }
        /// <summary>
        /// Identifies the SelectedValue dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register("SelectedValue", typeof(object), typeof(Selector), new PropertyMetadata(null, SelectedValue_Changed)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });
        private static void SelectedValue_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!AreObjectsEqual(e.OldValue, e.NewValue)) 
            {
                Selector selector = (Selector)d;
                object newValue = (object)e.NewValue;
                object oldItem = selector.SelectedItem;
                if (!selector._selectionChangeIsOnItem && !selector._selectionChangeIsOnIndex) //we only want to change the other ones if the change comes from SelectedItem (otherwise it's already done by the one that was originally changed (SelectedIndex or SelectedValue)
                {
                    selector._selectionChangeIsOnValue = true;
                    if (newValue == null)
                    {
                        selector.SetLocalValue(SelectedIndexProperty, -1); //we call SetLocalvalue directly to avoid replacing the BindingExpression that could be here on Mode = TwoWay
                        selector.SetLocalValue(SelectedItemProperty, null); //we call SetLocalvalue directly to avoid replacing the BindingExpression that could be here on Mode = TwoWay

                        //todo: update binding of SelectedIndex

                        //selector.SelectedIndex = -1;
                        //selector.SelectedItem = null;
                    }
                    else
                    {
                        var selectedPropertyPath = selector.SelectedValuePath;
                        object item = selector.Items.FirstOrDefault(element => Object.Equals(PropertyPathHelper.AccessValueByApplyingPropertyPathIfAny(element, selectedPropertyPath), newValue)); //todo: perf? //Note: there is no way we can know which value was intended in the case of multiple items with the same values.
                        selector.SetLocalValue(SelectedIndexProperty, GetIndexOfElementInItems(selector, item)); //we call SetLocalvalue directly to avoid replacing the BindingExpression that could be here on Mode = TwoWay
                        selector.SetLocalValue(SelectedItemProperty, item); //we call SetLocalvalue directly to avoid replacing the BindingExpression that could be here on Mode = TwoWay

                        //todo: update binding of SelectedIndex

                        //selector.SelectedIndex = GetIndexOfElementInItems(selector, item);
                        //selector.SelectedItem = item;
                    }

                    selector._selectionChangeIsOnValue = false;
                    List<object> oldItems = new List<object>();
                    oldItems.Add(oldItem);
                    List<object> newItems = new List<object>();
                    newItems.Add(selector.SelectedItem);
                    selector.RefreshSelectedItem();
                    selector.OnSelectionChanged(new SelectionChangedEventArgs(oldItems, newItems));
                }
            }
        }

        // IMPORTANT
        // In Bridge, item1 == item2 doesn't work, so we let Bridge convert the code below in order to work properly
        //   For example, in Bridge, if we use 2 classes, even if they have the same value and everything, using the symbol "==" doesn't work.
        //   this is why in C# we use <class name>.Equals(<other classname>) in order to see if they're equal
        //   and that's the same with Bridge
#if !BRIDGE 
        [JSReplacement("$item1 == $item2")] //the c# version doesn't work in javascript for types like int 
#endif
        private static bool AreObjectsEqual(object item1, object item2)
        {
            // we need to check if both items or null separatly because of a Bridge issue : item1.Equals(item2) throws an error if item1 is null which is normal) and/or item2 is null (error : "cannot get property "low" of null")
            if(item1 == null)
            {
                return item2 == null;
            }
            else if(item2 == null)
            {
                return item1 == null;
            }
            else
            {
                return item1.Equals(item2);
            }
        }

        //todo: remove the following method when the bug with ObservableCollection.IndexOf (that makes it not work in the simulator) will be fixed
        private static int GetIndexOfElementInItems(Selector selector, object element)
        {
            int i = 0;
            foreach (object currentItem in selector.Items)
            {
                if (element.Equals(currentItem))
                {
                    break;
                }
                ++i;
            }
            if (i >= selector.Items.Count)
            {
                i = -1;
            }
            return i;
        }

        /// <summary>
        /// Gets or sets the property path that is used to get the SelectedValue property
        /// of the SelectedItem property.
        /// </summary>
        public string SelectedValuePath
        {
            get { return (string)GetValue(SelectedValuePathProperty); }
            set { SetValue(SelectedValuePathProperty, value); }
        }
        /// <summary>
        /// Identifies the SelectedValuePath dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedValuePathProperty =
            DependencyProperty.Register("SelectedValuePath", typeof(string), typeof(Selector), new PropertyMetadata(string.Empty)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });


        internal virtual void RefreshSelectedItem()
        {
            //I do not think there is anything to do here, the method will probably be overriden.
        }


        #region selection changed event
        //note about this event: we need to register to this event, pre-handle it and only then change the value in c# since the user will probably want the old value if he registers to it.

        /// <summary>
        /// Occurs when the selection is changed.
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Raises the TextChanged event
        /// </summary>
        /// <param name="eventArgs">The arguments for the event.</param>
        protected virtual void OnSelectionChanged(SelectionChangedEventArgs eventArgs)
        {
            if (SelectionChanged != null)
            {
                SelectionChanged(this, eventArgs);
            }
        }

        #endregion



        #region things to replace with selectors Controltemplates

        /// <summary>
        /// Gets or sets the bakground color of the selected Items.
        /// </summary>
        public Brush SelectedItemBackground
        {
            get { return (Brush)GetValue(SelectedItemBackgroundProperty); }
            set { SetValue(SelectedItemBackgroundProperty, value); }
        }
        /// <summary>
        /// Identifies the SelectedItemBackground dependency property
        /// </summary>
        public static readonly DependencyProperty SelectedItemBackgroundProperty =
            DependencyProperty.Register("SelectedItemBackground", typeof(Brush), typeof(Selector), new PropertyMetadata(new SolidColorBrush(Colors.Blue))
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        /// <summary>
        /// Gets or sets the foreground color of the selected Items.
        /// </summary>
        public Brush SelectedItemForeground
        {
            get { return (Brush)GetValue(SelectedItemForegroundProperty); }
            set { SetValue(SelectedItemForegroundProperty, value); }
        }
        /// <summary>
        /// Identifies the SelectedItemForeground dependency property
        /// </summary>
        public static readonly DependencyProperty SelectedItemForegroundProperty =
            DependencyProperty.Register("SelectedItemForeground", typeof(Brush), typeof(Selector), new PropertyMetadata(new SolidColorBrush(Colors.White))
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });


        [Obsolete("Use RowBackground instead.")]
        /// <summary>
        /// Gets or sets the bakground color of the Items that are not selected.
        /// </summary>
        public Brush UnselectedItemBackground
        {
            get { return RowBackground; }
            set { RowBackground = value; }
        }
       
        /// <summary>
        /// Gets or sets the bakground color of the Items that are not selected.
        /// </summary>
        public Brush RowBackground
        {
            get { return (Brush)GetValue(RowBackgroundProperty); }
            set { SetValue(RowBackgroundProperty, value); }
        }
        /// <summary>
        /// Identifies the RowBackground dependency property
        /// </summary>
        public static readonly DependencyProperty RowBackgroundProperty =
            DependencyProperty.Register("RowBackground", typeof(Brush), typeof(Selector), new PropertyMetadata(new SolidColorBrush(Colors.White))
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });


        /// <summary>
        /// Gets or sets the foreground color of the Items that are not selected.
        /// </summary>
        public Brush UnselectedItemForeground
        {
            get { return (Brush)GetValue(UnselectedItemForegroundProperty); }
            set { SetValue(UnselectedItemForegroundProperty, value); }
        }
        /// <summary>
        /// Identifies the UnselectedItemForeground dependency property
        /// </summary>
        public static readonly DependencyProperty UnselectedItemForegroundProperty =
            DependencyProperty.Register("UnselectedItemForeground", typeof(Brush), typeof(Selector), new PropertyMetadata(null)
            { CallPropertyChangedWhenLoadedIntoVisualTree = WhenToCallPropertyChangedEnum.IfPropertyIsSet });

        #endregion

        //// Summary:
        ////     Gets a value that indicates whether the specified Selector has the focus.
        ////
        //// Parameters:
        ////   element:
        ////     The Selector to evaluate.
        ////
        //// Returns:
        ////     Ttrue to indicate that the Selector has the focus; otherwise, false.
        //public static bool GetIsSelectionActive(DependencyObject element)
    }
}