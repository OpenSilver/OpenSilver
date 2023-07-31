
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
using System.Diagnostics;

#if MIGRATION
using System.Windows.Threading;
#else
using Windows.Foundation;
using Windows.UI.Core;
using Dispatcher = Windows.UI.Core.CoreDispatcher;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    internal sealed class LayoutManager
    {
        private UIElement _forceLayoutElement; //set in extreme situations, forces the update of the whole tree containing the element
        private UIElement _lastExceptionElement; //set on exception in Measure or Arrange.

        private InternalMeasureQueue _measureQueue;
        private InternalArrangeQueue _arrangeQueue;
        private SizeChangedInfo _sizeChangedChain;
        private LayoutEventList _layoutEvents;

        private int _arrangesOnStack;
        private int _measuresOnStack;

        private bool _isUpdating;
        private bool _isInUpdateLayout;
        private bool _gotException; //true if UpdateLayout exited with exception
        private bool _layoutRequestPosted;
        private bool _inFireLayoutUpdated;
        private bool _inFireSizeChanged;
        private bool _firePostLayoutEvents;

        private LayoutManager(Dispatcher dispatcher)
        {
            Debug.Assert(dispatcher != null);
            Dispatcher = dispatcher;
        }

        public static LayoutManager Current { get; } = new LayoutManager(Dispatcher.CurrentDispatcher);

        public Dispatcher Dispatcher { get; }

        internal InternalMeasureQueue MeasureQueue => _measureQueue ??= new InternalMeasureQueue();

        internal InternalArrangeQueue ArrangeQueue => _arrangeQueue ??= new InternalArrangeQueue();

        internal LayoutEventList LayoutEvents => _layoutEvents ??= new LayoutEventList();

        private bool HasDirtiness => !MeasureQueue.IsEmpty || !ArrangeQueue.IsEmpty;

        public void UpdateLayout()
        {
            //make UpdateLayout to be a NOP if called during UpdateLayout.
            if (_isInUpdateLayout ||
                _measuresOnStack > 0 ||
                _arrangesOnStack > 0)
            {
                return;
            }

            int cnt = 0;
            bool gotException = true;
            UIElement currentElement = null;

            try
            {
                InvalidateTreeIfRecovering();

                while (HasDirtiness || _firePostLayoutEvents)
                {
                    if (++cnt > 153)
                    {
                        //loop detected. Lets go over to background to let input/user to correct the situation.
                        //most frequently, we get such a loop as a result of input detecting a mouse in the "bad spot"
                        //and some event handler oscillating a layout-affecting property depending on hittest result
                        //of the mouse. Going over to background will not break the loopp but will allow user to
                        //move the mouse so that it goes out of the "bad spot".
                        Dispatcher.InvokeAsync(UpdateLayoutBackground, DispatcherPriority.Background);
                        currentElement = null;
                        gotException = false;
                        return;
                    }

                    //this flag stops posting update requests to MediaContext - we are already in one
                    //note that _isInUpdateLayout is close but different - _isInUpdateLayout is reset
                    //before firing LayoutUpdated so that event handlers could call UpdateLayout but
                    //still could not cause posting of MediaContext work item. Posting MediaContext workitem
                    //causes infinite loop in MediaContext.
                    _isUpdating = true;
                    _isInUpdateLayout = true;

                    // Disable processing of the queue during blocking operations to prevent unrelated reentrancy.
                    using (Dispatcher.DisableProcessing())
                    {
                        while (true)
                        {
                            currentElement = MeasureQueue.GetTopMost();

                            if (currentElement == null) break; //exit if no more Measure candidates

                            currentElement.Measure(currentElement.PreviousAvailableSize);
                        }

                        while (MeasureQueue.IsEmpty)
                        {
                            currentElement = ArrangeQueue.GetTopMost();

                            if (currentElement == null) break; //exit if no more Measure candidates

                            Rect finalRect = GetProperArrangeRect(currentElement);

                            currentElement.Arrange(finalRect);
                        }

                        //if Arrange dirtied the tree go clean it again
                        //it is not neccesary to check ArrangeQueue sicnce we just exited from Arrange loop
                        if (!MeasureQueue.IsEmpty) continue;

                        //let LayoutUpdated handlers to call UpdateLayout
                        //note that it means we can get reentrancy into UpdateLayout past this point,
                        //if any of event handlers call UpdateLayout sync. Need to protect from reentrancy
                        //in the firing methods below.
                        _isInUpdateLayout = false;
                    }

                    FireSizeChangedEvents();
                    if (HasDirtiness) continue;
                    FireLayoutUpdateEvent();
                    if (HasDirtiness) continue;
                    FireSizeChangedEvents(); // if nothing is dirty, one last chance for any size changes to announce.
                }

                currentElement = null;
                gotException = false;
            }
            finally
            {
                _isUpdating = false;
                _layoutRequestPosted = false;
                _isInUpdateLayout = false;

                if (gotException)
                {
                    //set indicator
                    _gotException = true;
                    _forceLayoutElement = currentElement;

                    Dispatcher.InvokeAsync(UpdateLayoutBackground, DispatcherPriority.ApplicationIdle);
                }
            }
        }

        //Debuggability support - see LayoutInformation class in Framework
        internal UIElement GetLastExceptionElement()
        {
            return _lastExceptionElement;
        }

        internal void SetLastExceptionElement(UIElement e)
        {
            _lastExceptionElement = e;
        }

        private void MarkTreeDirty(UIElement e)
        {
            //walk up until we are the topmost UIElement in the tree.
            while (true)
            {
                UIElement p = UIElement.GetLayoutParent(e);
                if (p == null) break;
                e = p;
            }

            MarkTreeDirtyHelper(e);
            MeasureQueue.Add(e);
            ArrangeQueue.Add(e);
        }

        private void MarkTreeDirtyHelper(UIElement uie)
        {
            //now walk down and mark all UIElements dirty
            if (uie != null)
            {
                uie.InvalidateMeasureInternal();
                uie.InvalidateArrangeInternal();

                //walk children doing the same, don't stop if they are already dirty since there can
                //be insulated dirty islands below
                int cnt = uie.VisualChildrenCount;

                for (int i = 0; i < cnt; i++)
                {
                    UIElement child = uie.GetVisualChild(i);
                    if (child != null)
                    {
                        MarkTreeDirtyHelper(child);
                    }
                }
            }
        }

        private void InvalidateTreeIfRecovering()
        {
            if (_forceLayoutElement != null || _gotException)
            {
                if (_forceLayoutElement != null)
                {
                    MarkTreeDirty(_forceLayoutElement);
                }

                _forceLayoutElement = null;
                _gotException = false;
            }
        }

        internal void EnterMeasure()
        {
            _lastExceptionElement = null;
            _measuresOnStack++;

            _firePostLayoutEvents = true;
        }

        internal void ExitMeasure()
        {
            _measuresOnStack--;
        }

        internal void EnterArrange()
        {
            _lastExceptionElement = null;
            _arrangesOnStack++;

            _firePostLayoutEvents = true;
        }

        internal void ExitArrange()
        {
            _arrangesOnStack--;
        }

        internal void AddToSizeChangedChain(SizeChangedInfo info)
        {
            //this typically will cause firing of SizeChanged from top to down. However, this order is not
            //specified for any users and is subject to change without notice.
            info.Next = _sizeChangedChain;
            _sizeChangedChain = info;
        }

        private void FireSizeChangedEvents()
        {
            //no reentrancy. It may happen if one of handlers calls UpdateLayout synchronously
            if (_inFireSizeChanged) return;

            try
            {
                _inFireSizeChanged = true;

                //loop for SizeChanged
                while (_sizeChangedChain != null)
                {
                    SizeChangedInfo info = _sizeChangedChain;
                    _sizeChangedChain = info.Next;

                    info.Element.SizeChangedInfo = null;

                    info.Element.OnRenderSizeChanged(info);

                    //if callout dirtified the tree, return to cleaning
                    if (HasDirtiness) break;
                }
            }
            finally
            {
                _inFireSizeChanged = false;
            }
        }

        //walks the list, fires events to alive handlers and removes dead ones
        private void FireLayoutUpdateEvent()
        {
            //no reentrancy. It may happen if one of handlers calls UpdateLayout synchronously
            if (_inFireLayoutUpdated) return;

            try
            {
                _inFireLayoutUpdated = true;
                _firePostLayoutEvents = false;

                LayoutEventList.ListItem[] copy = LayoutEvents.CopyToArray();

                for (int i = 0; i < copy.Length; i++)
                {
                    LayoutEventList.ListItem item = copy[i];
                    //store handler here in case if thread gets pre-empted between check for IsAlive and invocation
                    //and GC can run making something that was alive not callable.
                    EventHandler e = null;
                    try
                    {
                        // this will return null if element is already GC'ed
                        e = (EventHandler)item.Target;
                    }
                    catch (InvalidOperationException) //this will happen if element is being resurrected after finalization
                    {
                        e = null;
                    }

                    if (e != null)
                    {
                        e(null, EventArgs.Empty);
                        // if handler dirtied the tree, go clean it again before calling other handlers
                        if (HasDirtiness) break;
                    }
                    else
                    {
                        LayoutEvents.Remove(item);
                    }
                }
            }
            finally
            {
                _inFireLayoutUpdated = false;
            }
        }

        private Rect GetProperArrangeRect(UIElement element)
        {
            Rect arrangeRect = element.PreviousArrangeRect;

            // ELements without a parent (top level) get Arrange at DesiredSize
            // if they were measured "to content" (as infinity indicates).
            // If we arrange the element that is temporarily disconnected
            // so it is not a top-level one, the assumption is that it will be
            // layout-invalidated and/or recomputed by the parent when reconnected.
            if (UIElement.GetLayoutParent(element) == null)
            {
                arrangeRect.X = arrangeRect.Y = 0;

                if (double.IsPositiveInfinity(element.PreviousAvailableSize.Width))
                    arrangeRect.Width = element.DesiredSize.Width;

                if (double.IsPositiveInfinity(element.PreviousAvailableSize.Height))
                    arrangeRect.Height = element.DesiredSize.Height;
            }

            return arrangeRect;
        }

        // posts a layout update
        private void NeedsRecalc()
        {
            if (!_layoutRequestPosted && !_isUpdating)
            {
                Dispatcher.InvokeAsync(UpdateLayout, DispatcherPriority.Render);
                _layoutRequestPosted = true;
            }
        }

        private void UpdateLayoutBackground()
        {
            NeedsRecalc();
        }

        internal sealed class InternalMeasureQueue : LayoutQueue
        {
            internal override void SetRequest(UIElement e, Request r)
            {
                e.MeasureRequest = r;
            }

            internal override Request GetRequest(UIElement e)
            {
                return e.MeasureRequest;
            }

            internal override bool CanRelyOnParentRecalc(UIElement parent)
            {
                return !parent.IsMeasureValid
                    && !parent.MeasureInProgress; //if parent's measure is in progress, we might have passed this child already
            }

            internal override void Invalidate(UIElement e)
            {
                e.InvalidateMeasureInternal();
            }
        }

        internal sealed class InternalArrangeQueue : LayoutQueue
        {
            internal override void SetRequest(UIElement e, Request r)
            {
                e.ArrangeRequest = r;
            }

            internal override Request GetRequest(UIElement e)
            {
                return e.ArrangeRequest;
            }

            internal override bool CanRelyOnParentRecalc(UIElement parent)
            {
                return !parent.IsArrangeValid
                    && !parent.ArrangeInProgress; //if parent's arrange is in progress, we might have passed this child already
            }

            internal override void Invalidate(UIElement e)
            {
                e.InvalidateArrangeInternal();
            }
        }

        internal abstract class LayoutQueue
        {
            internal abstract Request GetRequest(UIElement e);
            internal abstract void SetRequest(UIElement e, Request r);
            internal abstract bool CanRelyOnParentRecalc(UIElement parent);
            internal abstract void Invalidate(UIElement e);

            internal sealed class Request
            {
                internal UIElement Target;
                internal Request Next;
                internal Request Prev;
            }

            internal bool IsEmpty { get { return _head == null; } }

            internal void Add(UIElement e)
            {
                if (GetRequest(e) != null) return;
                if (e.ReadVisualFlag(VisualFlags.IsLayoutSuspended)) return;

                RemoveOrphans(e);

                UIElement parent = UIElement.GetLayoutParent(e);
                if (parent != null && CanRelyOnParentRecalc(parent)) return;

                LayoutManager layoutManager = Current;

                //10 is arbitrary number here, simply indicates the queue is
                //about to be filled. If not queue is not almost full, simply add
                //the element to it. If it is almost full, start conserve entries
                //by escalating invalidation to all the ancestors until the top of
                //the visual tree, and only add root of visula tree to the queue.
                AddRequest(e);

                layoutManager.NeedsRecalc();
            }

            internal void Remove(UIElement e)
            {
                Request r = GetRequest(e);
                if (r == null) return;
                RemoveRequest(r);
                SetRequest(e, null);
            }

            internal void RemoveOrphans(UIElement parent)
            {
                Request r = _head;
                while (r != null)
                {
                    UIElement child = r.Target;
                    Request next = r.Next;
                    ulong parentTreeLevel = parent.TreeLevel;

                    if ((child.TreeLevel == parentTreeLevel + 1)
                       && (UIElement.GetLayoutParent(child) == parent))
                    {
                        RemoveRequest(GetRequest(child));
                        SetRequest(child, null);
                    }

                    r = next;
                }
            }

            internal UIElement GetTopMost()
            {
                UIElement found = null;
                ulong treeLevel = ulong.MaxValue;

                for (Request r = _head; r != null; r = r.Next)
                {
                    UIElement t = r.Target;
                    ulong l = t.TreeLevel;

                    if (l < treeLevel)
                    {
                        treeLevel = l;
                        found = r.Target;
                    }
                }

                return found;
            }

            private void AddRequest(UIElement e)
            {
                Request r = GetNewRequest(e);

                if (r != null)
                {
                    r.Next = _head;
                    if (_head != null) _head.Prev = r;
                    _head = r;

                    SetRequest(e, r);
                }
            }

            private Request GetNewRequest(UIElement e)
            {
                Request r;
                if (_pocket != null)
                {
                    r = _pocket;
                    _pocket = r.Next;
                    _pocketSize--;
                    r.Next = r.Prev = null;
                }
                else
                {
                    r = new Request();
                }

                r.Target = e;
                return r;
            }

            private void RemoveRequest(Request entry)
            {
                if (entry.Prev == null) _head = entry.Next;
                else entry.Prev.Next = entry.Next;

                if (entry.Next != null) entry.Next.Prev = entry.Prev;

                ReuseRequest(entry);
            }

            private void ReuseRequest(Request r)
            {
                r.Target = null; //let target die

                r.Next = _pocket;
                _pocket = r;
                _pocketSize++;
            }

            private Request _head;
            private Request _pocket;
            private int _pocketSize;
        }
    }

    internal class LayoutEventList
    {
        //size of the pre-allocated free list
        private const int PocketCapacity = 153;

        internal class ListItem : WeakReference
        {
            internal ListItem() : base(null) { }
            internal ListItem Next;
            internal ListItem Prev;
            internal bool InUse;
        }

        internal LayoutEventList()
        {
            ListItem t;
            for (int i = 0; i < PocketCapacity; i++)
            {
                t = new ListItem();
                t.Next = _pocket;
                _pocket = t;
            }
            _pocketSize = PocketCapacity;
        }

        internal ListItem Add(object target)
        {
            ListItem t = GetNewListItem(target);

            t.Next = _head;
            if (_head != null) _head.Prev = t;
            _head = t;

            _count++;
            return t;
        }

        internal void Remove(ListItem t)
        {
            //already removed item can be passed again
            //(once removed by handler and then by firing code)
            if (!t.InUse) return;

            if (t.Prev == null) _head = t.Next;
            else t.Prev.Next = t.Next;

            if (t.Next != null) t.Next.Prev = t.Prev;

            ReuseListItem(t);
            _count--;
        }

        private ListItem GetNewListItem(object target)
        {
            ListItem t;
            if (_pocket != null)
            {
                t = _pocket;
                _pocket = t.Next;
                _pocketSize--;
                t.Next = t.Prev = null;
            }
            else
            {
                t = new ListItem();
            }

            t.Target = target;
            t.InUse = true;
            return t;
        }

        private void ReuseListItem(ListItem t)
        {
            t.Target = null; //let target die
            t.Next = t.Prev = null;
            t.InUse = false;

            if (_pocketSize < PocketCapacity)
            {
                t.Next = _pocket;
                _pocket = t;
                _pocketSize++;
            }
        }

        internal ListItem[] CopyToArray()
        {
            ListItem[] copy = new ListItem[_count];
            ListItem t = _head;
            int cnt = 0;
            while (t != null)
            {
                copy[cnt++] = t;
                t = t.Next;
            }
            return copy;
        }

        internal int Count
        {
            get
            {
                return _count;
            }
        }

        private ListItem _head;
        private ListItem _pocket;
        private int _pocketSize;
        private int _count;
    }
}