#if WORKINPROGRESS

#if !MIGRATION
using System;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Xaml;
#endif

#if MIGRATION
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;

namespace System.Windows
#else
namespace Windows.Foundation
#endif
{
    public class LayoutManager
    {
        public static readonly LayoutManager Current = new LayoutManager();

        private HashSet<UIElement> measureQueue;
        private HashSet<UIElement> arrangeQueue;
        private HashSet<UIElement> updatedElements;

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
                updateLayoutOperation = Dispatcher.INTERNAL_GetCurrentDispatcher().InvokeAsync(UpdateLayout, DispatcherPriority.Render);
            }
        }

        public void UpdateLayout()
        {
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
                        element.InvalidateArrange();
                        element.InvalidateParentMeasure();
                        element.InvalidateParentArrange();
                    }
                }

                while (arrangeQueue.Count > 0)
                {
                    UIElement element = GetTopElement(arrangeQueue);
                    //Console.WriteLine($"LayoutManager UpdateLayout / Arrange {element}");
                    element.Arrange(element.PreviousFinalRect);
                }

                while (updatedElements.Count > 0 && measureQueue.Count == 0 && arrangeQueue.Count == 0) // LayoutUpdated can invalidate other elements
                {
                    UIElement element = RemoveUpdatedElementAndReturn();
                    element.RaiseLayoutUpdated();
                }
            }
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
#endif