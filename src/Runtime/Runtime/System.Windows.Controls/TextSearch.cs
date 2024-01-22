
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

using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Threading;

namespace System.Windows.Controls
{
    /// <summary>
    /// Enables the user to search a list of items in an <see cref="ItemsControl"/>
    /// using keyboard input.
    /// </summary>
    public sealed class TextSearch : DependencyObject
    {
        /// <summary>
        /// Make a new TextSearch instance attached to the given object.
        /// Create the instance in the same context as the given DO.
        /// </summary>
        /// <param name="itemsControl"></param>
        private TextSearch(ItemsControl itemsControl)
        {
            _attachedTo = itemsControl ?? throw new ArgumentNullException(nameof(itemsControl));

            ResetState();
        }

        /// <summary>
        /// Get the instance of TextSearch attached to the given ItemsControl or make one and attach it if it's not.
        /// </summary>
        /// <param name="itemsControl"></param>
        /// <returns></returns>
        internal static TextSearch EnsureInstance(ItemsControl itemsControl)
        {
            TextSearch instance = (TextSearch)itemsControl.GetValue(TextSearchInstanceProperty);

            if (instance == null)
            {
                instance = new TextSearch(itemsControl);
                itemsControl.SetValueInternal(TextSearchInstanceProperty, instance);
            }

            return instance;
        }

        /// <summary>
        /// Identifies the TextSearch.TextPath attached property.
        /// </summary>
        public static readonly DependencyProperty TextPathProperty =
            DependencyProperty.RegisterAttached(
                "TextPath",
                typeof(string),
                typeof(TextSearch),
                null);

        /// <summary>
        /// Returns the name of the property that identifies an item in the specified element's
        /// collection.
        /// </summary>
        /// <param name="element">
        /// The element from which the property value is read.
        /// </param>
        /// <returns>
        /// The name of the property that identifies the item to the user.
        /// </returns>
        public static string GetTextPath(DependencyObject element)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            return (string)element.GetValue(TextPathProperty);
        }

        /// <summary>
        /// Writes the TextSearch.TextPath attached property to the specified element.
        /// </summary>
        /// <param name="element">
        /// The element to which the property value is written.
        /// </param>
        /// <param name="path">
        /// The name of the property that identifies an item.
        /// </param>
        public static void SetTextPath(DependencyObject element, string path)
        {
            if (element is null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            element.SetValueInternal(TextPathProperty, path);
        }

        /// <summary>
        /// Instance of TextSearch -- attached property so that the instance can be stored on the element
        /// which wants the service.
        /// </summary>
        private static readonly DependencyProperty TextSearchInstanceProperty =
            DependencyProperty.RegisterAttached(
                "TextSearchInstance",
                typeof(TextSearch),
                typeof(TextSearch),
                new PropertyMetadata((object)null));

        /// <summary>
        /// Called by consumers of TextSearch when a TextInput event is received
        /// to kick off the algorithm.
        /// </summary>
        /// <param name="nextChar"></param>
        /// <returns></returns>
        internal bool DoSearch(string nextChar)
        {
            bool repeatedChar = false;

            int startItemIndex = 0;

            ItemCollection itemCollection = _attachedTo.Items;

            // If TextSearch is not active, then we should start
            // the search from the beginning.  If it is active, we should
            // start the search from the currently-matched item.
            if (IsActive)
            {
                // ISSUE: This falls victim to duplicate elements being in the view.
                //        To mitigate this, we could remember ItemUI ourselves.

                startItemIndex = MatchedItemIndex;
            }

            // If they pressed the same character as last time, we will do the fallback search.
            //     Fallback search is if they type "bob" and then press "b"
            //     we'll look for "bobb" and when we don't find it we should
            //     find the next item starting with "bob".
            if (_charsEntered.Count > 0
                && (string.Compare(_charsEntered[_charsEntered.Count - 1], nextChar, StringComparison.OrdinalIgnoreCase) == 0))
            {
                repeatedChar = true;
            }

            // Get the primary TextPath from the ItemsControl to which we are attached.
            string primaryTextPath = GetPrimaryTextPath(_attachedTo);

            bool wasNewCharUsed = false;

            int matchedItemIndex = FindMatchingPrefix(_attachedTo, primaryTextPath, Prefix,
                                                      nextChar, startItemIndex, repeatedChar, ref wasNewCharUsed);

            // If there was an item that matched, move to that item in the collection
            if (matchedItemIndex != -1)
            {
                // Don't have to move currency if it didn't actually move.
                // startItemIndex is the index of the current item only if IsActive is true,
                // So, we have to move currency when IsActive is false.
                if (!IsActive || matchedItemIndex != startItemIndex)
                {
                    object matchedItem = itemCollection[matchedItemIndex];
                    // Let the control decide what to do with matched-item
                    _attachedTo.NavigateToItem(matchedItem, matchedItemIndex);
                    // Store current match
                    MatchedItemIndex = matchedItemIndex;
                }

                // Update the prefix if it changed
                if (wasNewCharUsed)
                {
                    AddCharToPrefix(nextChar);
                }

                // User has started typing (successfully), so we're active now.
                if (!IsActive)
                {
                    IsActive = true;
                }
            }

            // Reset the timeout and remember this character, but only if we're
            // active -- this is because if we got called but the match failed
            // we don't need to set up a timeout -- no state needs to be reset.
            if (IsActive)
            {
                ResetTimeout();
            }

            return matchedItemIndex != -1;
        }

        /// <summary>
        /// Searches through the given itemCollection for the first item matching the given prefix.
        /// </summary>
        /// <remarks>
        ///     --------------------------------------------------------------------------
        ///     Incremental Type Search algorithm
        ///     --------------------------------------------------------------------------
        ///
        ///     Given a prefix and new character, we loop through all items in the collection
        ///     and look for an item that starts with the new prefix.  If we find such an item,
        ///     select it.  If the new character is repeated, we look for the next item after
        ///     the current one that begins with the old prefix**.  We can optimize by
        ///     performing both of these searches in parallel.
        ///
        ///     **NOTE: Win32 will only do this if the old prefix is of length 1 - in other
        ///             words, first-character-only matching.  The algorithm described here
        ///             is an extension of ITS as implemented in Win32.  This variant was
        ///             described to me by JeffBog as what was done in AFC - but I have yet
        ///             to find a listbox which behaves this way.
        ///
        ///     --------------------------------------------------------------------------
        /// </remarks>
        /// <returns>Item that matches the given prefix</returns>
        private static int FindMatchingPrefix(ItemsControl itemsControl,
            string primaryTextPath,
            string prefix,
            string newChar,
            int startItemIndex,
            bool lookForFallbackMatchToo,
            ref bool wasNewCharUsed)
        {
            ItemCollection itemCollection = itemsControl.Items;

            // Using indices b/c this is a better way to uniquely
            // identify an element in the collection.
            int matchedItemIndex = -1;
            int fallbackMatchIndex = -1;

            int count = itemCollection.Count;

            // Return immediately with no match if there were no items in the view.
            if (count == 0)
            {
                return -1;
            }

            string newPrefix = prefix + newChar;

            // With an empty prefix, we'd match anything
            if (string.IsNullOrEmpty(newPrefix))
            {
                return -1;
            }

            // Hook up the binding we will apply to each object.  Get the
            // PrimaryTextPath off of the attached instance and then make
            // a binding with that path.

            bool firstItem = true;

            wasNewCharUsed = false;

            // ISSUE: what about changing the collection while this is running?
            for (int currentIndex = startItemIndex; currentIndex < count;)
            {
                object item = itemCollection[currentIndex];

                if (item != null)
                {
                    string itemString = GetPrimaryText(item, primaryTextPath, itemsControl);

                    // See if the current item matches the newPrefix, if so we can
                    // stop searching and accept this item as the match.
                    if (itemString != null && itemString.StartsWith(newPrefix, StringComparison.OrdinalIgnoreCase))
                    {
                        // Accept the new prefix as the current prefix.
                        wasNewCharUsed = true;
                        matchedItemIndex = currentIndex;
                        break;
                    }

                    // Find the next string that matches the last prefix.  This
                    // string will be used in the case that the new prefix isn't
                    // matched. This enables pressing the last character multiple
                    // times and cylcing through the set of items that match that
                    // prefix.
                    //
                    // Unlike the above search, this search must start *after*
                    // the currently selected item.  This search also shouldn't
                    // happen if there was no previous prefix to match against
                    if (lookForFallbackMatchToo)
                    {
                        if (!firstItem && prefix != string.Empty)
                        {
                            if (itemString != null)
                            {
                                if (fallbackMatchIndex == -1 && itemString.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                                {
                                    fallbackMatchIndex = currentIndex;
                                }
                            }
                        }
                        else
                        {
                            firstItem = false;
                        }
                    }
                }

                // Move next and wrap-around if we pass the end of the container.
                currentIndex++;
                if (currentIndex >= count)
                {
                    currentIndex = 0;
                }

                // Stop where we started but only after the first pass
                // through the loop -- we should process the startItem.
                if (currentIndex == startItemIndex)
                {
                    break;
                }
            }

            // In the case that the new prefix didn't match anything and
            // there was a fallback match that matched the old prefix, move
            // to that one.
            if (matchedItemIndex == -1 && fallbackMatchIndex != -1)
            {
                matchedItemIndex = fallbackMatchIndex;
            }

            return matchedItemIndex;
        }

        private void ResetTimeout()
        {
            // Called when we get some input. Start or reset the timer.
            // Queue an inactive priority work item and set its deadline.
            if (_timeoutTimer == null)
            {
                _timeoutTimer = new DispatcherTimer();
                _timeoutTimer.Tick += OnTimeout;
            }
            else
            {
                _timeoutTimer.Stop();
            }

            // Schedule this operation to happen a certain number of milliseconds from now.
            _timeoutTimer.Interval = TimeOut;
            _timeoutTimer.Start();
        }

        private void AddCharToPrefix(string newChar)
        {
            Prefix += newChar;
            _charsEntered.Add(newChar);
        }

        private static string GetPrimaryTextPath(ItemsControl itemsControl)
        {
            string primaryTextPath = (string)itemsControl.GetValue(TextPathProperty);

            if (string.IsNullOrEmpty(primaryTextPath))
            {
                primaryTextPath = itemsControl.DisplayMemberPath;
            }

            return primaryTextPath;
        }

        private static string GetPrimaryText(object item, string primaryTextPath, DependencyObject primaryTextBindingHome)
        {
            // Order of precedence for getting Primary Text is as follows:
            //
            // 1) PrimaryText (WPF only)
            // 2) PrimaryTextPath (TextSearch.TextPath or ItemsControl.DisplayMemberPath)
            // 3) GetPlainText()
            // 4) ToString()

            // Here hopefully they've supplied a path into their object which we can use.
            if (!string.IsNullOrEmpty(primaryTextPath) && primaryTextBindingHome != null)
            {
                _ = BindingOperations.SetBinding(primaryTextBindingHome, BindingExpressionBase.NoTargetProperty,
                    new Binding(primaryTextPath)
                    {
                        Mode = BindingMode.OneWay,
                        Source = item,
                    });

                object primaryText = primaryTextBindingHome.GetValue(BindingExpressionBase.NoTargetProperty);
                primaryTextBindingHome.ClearValue(BindingExpressionBase.NoTargetProperty);
                
                return ConvertToPlainText(primaryText);
            }

            return ConvertToPlainText(item);
        }

        private static string ConvertToPlainText(object o)
        {
            // Try to return FrameworkElement.GetPlainText()
            if (o is FrameworkElement fe)
            {
                string text = fe.GetPlainText();

                if (text != null)
                {
                    return text;
                }
            }

            // Try to convert the item to a string
            return (o != null) ? o.ToString() : string.Empty;
        }

        private void OnTimeout(object sender, EventArgs e) => ResetState();

        private void ResetState()
        {
            // Reset the prefix string back to empty.
            IsActive = false;
            Prefix = string.Empty;
            MatchedItemIndex = -1;
            if (_charsEntered == null)
            {
                _charsEntered = new List<string>(10);
            }
            else
            {
                _charsEntered.Clear();
            }

            if (_timeoutTimer != null)
            {
                _timeoutTimer.Stop();
            }
            _timeoutTimer = null;
        }

        /// <summary>
        /// Time until the search engine resets.
        /// </summary>
        private TimeSpan TimeOut
        {
            get
            {
                // NOTE: NtUser does the following (file: windows/ntuser/kernel/sysmet.c)
                //     gpsi->dtLBSearch = dtTime * 4;            // dtLBSearch   =  4  * gdtDblClk
                //     gpsi->dtScroll = gpsi->dtLBSearch / 5;  // dtScroll     = 4/5 * gdtDblClk
                //
                // 4 * DoubleClickSpeed seems too slow for the search
                // So for now we'll do 2 * DoubleClickSpeed

                return TimeSpan.FromMilliseconds(500 * 2);
            }
        }

        private string Prefix { get; set; }

        private bool IsActive { get; set; }

        private int MatchedItemIndex { get; set; }

        // Element to which this TextSearch instance is attached.
        private ItemsControl _attachedTo;
        private List<string> _charsEntered;
        private DispatcherTimer _timeoutTimer;
    }
}
