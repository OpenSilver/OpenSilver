using System;
using System.Collections.Generic;
using System.Linq;

#if MIGRATION
using System.Windows.Threading;
#else
using Windows.Foundation;
using Windows.UI.Core;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    internal class LayoutManager
    {
        public static readonly LayoutManager Current = new LayoutManager();

        private HashSet<UIElement> measureQueue;
        private HashSet<UIElement> arrangeQueue;
        private HashSet<UIElement> updatedElements;

        private bool _updateInProgress;
        private object queueLock = new object();
        private object updatedElementsLock = new object();

        private DispatcherOperation updateLayoutOperation;

        private LayoutManager()
        {
            measureQueue = new HashSet<UIElement>();
            arrangeQueue = new HashSet<UIElement>();
            updatedElements = new HashSet<UIElement>();
        }

        public void AddMeasure(UIElement element)
        {
            if (element == null)
                return;

            lock (queueLock)
            {
                measureQueue.Add(element);
            }
            BeginUpdateLayout();
        }

        public void RemoveMeasure(UIElement element)
        {
            lock (queueLock)
            {
                measureQueue.Remove(element);
            }
        }

        public void AddArrange(UIElement element)
        {
            if (element == null)
                return;

            lock (queueLock)
            {
                arrangeQueue.Add(element);
            }
            BeginUpdateLayout();
        }

        public void RemoveArrange(UIElement element)
        {
            lock (queueLock)
            {
                arrangeQueue.Remove(element);
            }
        }

        public void AddUpdatedElement(UIElement element)
        {
            if (updateLayoutOperation == null || updateLayoutOperation.Status != DispatcherOperationStatus.Executing)
            {
                // element was updated manually (not through the UpdateLayout loop)
                foreach (UIElement pathElement in GetElementPath(element))
                {
                    pathElement.RaiseLayoutUpdated();
                }

                return;
            }

            lock (updatedElementsLock)
            {
                foreach (UIElement pathElement in GetElementPath(element))
                {
                    updatedElements.Add(pathElement);
                }
            }
        }

        public UIElement RemoveUpdatedElementAndReturn()
        {
            lock (updatedElementsLock)
            {
                if (updatedElements.Count > 0)
                {
                    UIElement updated = updatedElements.First();
                    updatedElements.Remove(updated);
                    return updated;
                }
            }
            return null;
        }

        public void BeginUpdateLayout()
        {
            if (updateLayoutOperation == null || updateLayoutOperation.Status == DispatcherOperationStatus.Completed)
            {
#if MIGRATION
                updateLayoutOperation = Dispatcher.INTERNAL_GetCurrentDispatcher().InvokeAsync(UpdateLayout, DispatcherPriority.Render);
#else
                updateLayoutOperation = CoreDispatcher.INTERNAL_GetCurrentDispatcher().InvokeAsync(UpdateLayout, DispatcherPriority.Render);
#endif
            }
        }

        public void UpdateLayout()
        {
            if (_updateInProgress) return;

            _updateInProgress = true;
            while (measureQueue.Count > 0 || arrangeQueue.Count > 0)
            {
                while (measureQueue.Count > 0)
                {
                    UIElement element = GetTopElement(measureQueue);
                    Size previousDesiredSize = element.DesiredSize;
                    //Console.WriteLine($"LayoutManager UpdateLayout/Measure {element}");
                    element.Measure(element.PreviousAvailableSize);

                    if (previousDesiredSize != element.DesiredSize)
                    {
                        element.InvalidateParentMeasure();
                        element.InvalidateParentArrange();
                    }
                }

                while (arrangeQueue.Count > 0)
                {
                    UIElement element = GetTopElement(arrangeQueue);
                    //Console.WriteLine($"LayoutManager UpdateLayout / Arrange {element}");

                    Rect previousRect = element.PreviousFinalRect;
                    FrameworkElement fe = element as FrameworkElement;
                    if (fe.IsCustomLayoutRoot)
                    {
                        if (fe.IsAutoWidthOnCustomLayoutInternal)
                            previousRect.Width = fe.DesiredSize.Width;

                        if (fe.IsAutoHeightOnCustomLayoutInternal)
                            previousRect.Height = fe.DesiredSize.Height;
                    }
                    element.Arrange(previousRect);
                }

                while (updatedElements.Count > 0 && measureQueue.Count == 0 && arrangeQueue.Count == 0) // LayoutUpdated can invalidate other elements
                {
                    UIElement element = RemoveUpdatedElementAndReturn();
                    element.RaiseLayoutUpdated();
                }
            }
            _updateInProgress = false;
        }

        private UIElement GetTopElement(IEnumerable<UIElement> measureQueue)
        {
            UIElement topElement = null;

            lock (queueLock)
            {
                foreach (UIElement element in measureQueue)
                {
                    if (topElement == null || topElement.VisualLevel > element.VisualLevel)
                    {
                        topElement = element;
                    }
                }
            }
            return topElement;
        }

        private static IEnumerable<UIElement> GetElementPath(UIElement element)
        {
            while (element != null)
            {
                yield return element;
                element = (UIElement)element.INTERNAL_VisualParent;
            }
        }
    }
}