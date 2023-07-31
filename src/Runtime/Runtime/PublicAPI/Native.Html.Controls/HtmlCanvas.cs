

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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CSHTML5.Internal;
using System.Windows.Markup;
using CSHTML5.Native.Html.Input;
#if MIGRATION
using System.Windows;
using System.Windows.Input;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.Foundation;
#endif

namespace CSHTML5.Native.Html.Controls
{
    /// <summary>
    /// Defines an area within which you can explicitly position child objects, using
    /// coordinates that are relative to the Canvas area. To render modifications,
    /// you need to call the Draw() method.
    /// Full documentation is available at:
    /// http://cshtml5.com/links/how-to-use-the-html5-canvas.aspx
    /// </summary>
    /// <example>
    /// You can add a Canvas to the XAML as follows:
    /// <code lang="XAML" xml:space="preserve">
    /// <native:HtmlCanvas Width="100" Height="100" xmlns:native="using:CSHTML5.Native.Html.Controls">
    ///     <!-- Place children here -->
    /// </native:HtmlCanvas>
    /// </code>
    /// Or in C#:
    /// <code lang="C#">
    /// HtmlCanvas myCanvas = new HtmlCanvas();
    /// myCanvas.Width = 100;
    /// myCanvas.Height = 100;
    /// </code>
    /// </example>
    [ContentProperty("Children")]
    public class HtmlCanvas : FrameworkElement
    {
        private object _jsCanvas;
        private object _jsContext2d;
        private ElementStyle _currentDrawingStyle;

        /// <summary>
        /// Gets the collection of child elements of the canvas.
        /// </summary>
        public List<HtmlCanvasElement> Children;

        private HtmlCanvasElement[] _LastPointerMove;

        /// <summary>
        /// Create an html5 native canvas
        /// </summary>
        public HtmlCanvas()
        {
            Children = new List<HtmlCanvasElement>();

#if MIGRATION
            this.MouseMove += HtmlCanvas_MouseMove;
            this.MouseLeftButtonDown += HtmlCanvas_MouseLeftButtonDown;
            this.MouseLeftButtonUp += HtmlCanvas_MouseLeftButtonUp;
            this.MouseRightButtonUp += HtmlCanvas_MouseRightButtonUp;
#else
            this.PointerMoved += HtmlCanvas_PointerMoved;
            this.PointerPressed += HtmlCanvas_PointerPressed;
            this.PointerReleased += HtmlCanvas_PointerReleased;
            this.RightTapped += HtmlCanvas_RightTapped;
#endif
        }

        internal sealed override bool EnablePointerEventsCore => true;

#if MIGRATION
        void HtmlCanvas_MouseMove(object sender, MouseEventArgs e)
#else
        void HtmlCanvas_PointerMoved(object sender, PointerRoutedEventArgs e)
#endif
        {
            // Get the cursor position relative to this HtmlCanvas
#if MIGRATION
            Point pos = e.GetPosition(this);
#else
            Point pos = e.GetCurrentPoint(this).Position;
#endif

            // Get a stack of the HtmlCanvasElement directly under the cursor and all his parents
            // (Parent1, Parent2, ..., ElementDirectlyUnderTheCursor)
            HtmlCanvasElement[] elements = GetPointedElements(this, pos.X, pos.Y).ToArray();

            HtmlCanvasPointerRoutedEventArgs e2 = new HtmlCanvasPointerRoutedEventArgs(e, this);

            // Loop backward on every element of the stack
            for (int i = 0; i < elements.Length && !e.Handled; ++i)
            {
                // Remove the last element of the stack and call its OnPointerMoved() method
                elements[i].OnPointerMoved(e2);
            }

            if (_LastPointerMove != null)
            {
                e2 = new HtmlCanvasPointerRoutedEventArgs(e, this);
                for (int i = 0; i < elements.Length && !e.Handled; ++i)
                {
                    if (Array.IndexOf(_LastPointerMove, elements[i]) == -1)
                    {
                        elements[i].OnPointerEntered(e2);
        }
                }

                e2 = new HtmlCanvasPointerRoutedEventArgs(e, this);
                for (int i = 0; i < _LastPointerMove.Length && !e.Handled; ++i)
                {
                    if (Array.IndexOf(elements, _LastPointerMove[i]) == -1)
                    {
                        _LastPointerMove[i].OnPointerExited(e2);
                    }
                }
            }

            _LastPointerMove = elements;
        }

#if MIGRATION
        void HtmlCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
#else
        void HtmlCanvas_PointerPressed(object sender, PointerRoutedEventArgs e)
#endif
        {
            // Get the cursor position relative to this HtmlCanvas
#if MIGRATION
            Point pos = e.GetPosition(this);
#else
            Point pos = e.GetCurrentPoint(this).Position;
#endif

            // Get a stack of the HtmlCanvasElement directly under the cursor and all his parents
            // (Parent1, Parent2, ..., ElementDirectlyUnderTheCursor)
            Stack<HtmlCanvasElement> elements = GetPointedElements(this, pos.X, pos.Y);

            HtmlCanvasPointerRoutedEventArgs e2 = new HtmlCanvasPointerRoutedEventArgs(e, this);

            // Loop backward on every element of the stack
            while (!e.Handled && elements.Count > 0)
            {
                // Remove the last element of the stack and call its OnPointerMoved() method
                var el = elements.Pop();
                el.OnPointerPressed(e2);
            }
        }
        
#if MIGRATION
        void HtmlCanvas_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
#else
        void HtmlCanvas_RightTapped(object sender, RightTappedRoutedEventArgs e)
#endif
        {
            // Get the cursor position relative to this HtmlCanvas
#if MIGRATION
            Point pos = e.GetPosition(this);
#else
            Point pos = e.GetCurrentPoint(this).Position;
#endif

            // Get a stack of the HtmlCanvasElement directly under the cursor and all his parents
            // (Parent1, Parent2, ..., ElementDirectlyUnderTheCursor)
            Stack<HtmlCanvasElement> elements = GetPointedElements(this, pos.X, pos.Y);

            HtmlCanvasPointerRoutedEventArgs e2 = new HtmlCanvasPointerRoutedEventArgs(e, this);

            // Loop backward on every element of the stack
            while (!e.Handled && elements.Count > 0)
            {
                // Remove the last element of the stack and call its OnPointerMoved() method
                var el = elements.Pop();
                el.OnRightTapped(e2);
            }
        }

#if MIGRATION
        void HtmlCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
#else
        void HtmlCanvas_PointerReleased(object sender, PointerRoutedEventArgs e)
#endif
        {
            // Get the cursor position relative to this HtmlCanvas
#if MIGRATION
            Point pos = e.GetPosition(this);
#else
            Point pos = e.GetCurrentPoint(this).Position;
#endif

            // Get a stack of the HtmlCanvasElement directly under the cursor and all his parents
            // (Parent1, Parent2, ..., ElementDirectlyUnderTheCursor)
            Stack<HtmlCanvasElement> elements = GetPointedElements(this, pos.X, pos.Y);

            HtmlCanvasPointerRoutedEventArgs e2 = new HtmlCanvasPointerRoutedEventArgs(e, this);

            // Loop backward on every element of the stack
            while (!e.Handled && elements.Count > 0)
            {
                // Remove the last element of the stack and call its OnPointerMoved() method
                elements.Pop().OnPointerReleased(e2);
            }
        }

        /// <summary>
        /// Return the path to the topmost element that is under the pointer
        /// </summary>
        /// <param name="canvas">The container of the elements</param>
        /// <returns>The path to the topmost element that is under the pointer</returns>
        static internal Stack<HtmlCanvasElement> GetPointedElements(HtmlCanvas canvas, double x, double y)
        {
            // Store the path to the element that is the topmost element. During traversal, this contains the path to the element that is considered topmost, until replaced by another element that is even more in the foreground:
            var finalPath = new Stack<HtmlCanvasElement>();

            // Store the path to the current element:
            var currentPath = new Stack<HtmlCanvasElement>();

            // Same as above, but stores the path in an alternative way, that is, by storing the index of each element in the parent collection:
            var currentPathIndexes = new Stack<int>();

            // Work with every Canvas child:
            for (int i = 0; i < canvas.Children.Count; ++i)
            {
                // Note: currentPath should be empty at this point.

                // Start the traversal by inserting the first element:
                currentPath.Push(canvas.Children[i]);
                currentPathIndexes.Push(0);

                while (currentPath.Count > 0)
                {
                    HtmlCanvasElement currentElement = currentPath.Peek();
                    int index = currentPathIndexes.Pop(); // The current index inside the Children collection of the "currentElement"

                    // Refresh 'finalPath' if another element is found under the cursor
                    if (currentElement.IsHitTestVisible && currentElement.Visibility == Visibility.Visible && currentElement.IsPointed(x, y)
                        && Array.IndexOf(finalPath.ToArray(), currentElement) == -1)
                    {
                        finalPath.Clear();
                        var path = currentPath.ToArray(); // Right now some methods for the Stack are not implemented in JSIL
                        for (int j = path.Length - 1; j >= 0; --j) // so we use a ToArray() method, which is not the best option for performances
                        {
                            finalPath.Push(path[j]);
                        }
                    }

                    if (currentElement.IsHitTestVisible && currentElement.Visibility == Visibility.Visible &&
                        currentElement is ContainerElement && index < ((ContainerElement)currentElement).Children.Count)
                    {
                        // Increment the last element of the "currentPathIndexes":
                        currentPathIndexes.Push(index + 1);

                        // Move x and y relative to the last element position
                        x -= currentElement.X;
                        y -= currentElement.Y;
                        // Add the child:
                        currentPath.Push(((ContainerElement)currentElement).Children[index]);
                        currentPathIndexes.Push(0);
                    }
                    else
                    {
                        // Remove the last element from the paths to "go up":
                        currentPath.Pop();
                        if (currentPath.Count > 0)
                        {
                            // Remove x and y relative to the last element position
                            x += currentPath.Peek().X;
                            y += currentPath.Peek().Y;
                        }
                    }
                }
            }
            return finalPath;
        }

        static internal Stack<HtmlCanvasElement> SearchElement(HtmlCanvas canvas, HtmlCanvasElement elem)
        {
            // Store the path to the current element:
            var path = new Stack<HtmlCanvasElement>();

            // Same as above, but stores the path in an alternative way, that is, by storing the index of each element in the parent collection:
            var currentPathIndexes = new Stack<int>();

            // Work with every Canvas child:
            for (int i = 0; i < canvas.Children.Count; ++i)
            {
                // Start the traversal by inserting the first element:
                path.Push(canvas.Children[i]);
                currentPathIndexes.Push(0);

                while (path.Count > 0)
                {
                    HtmlCanvasElement currentElement = path.Peek();
                    int index = currentPathIndexes.Pop(); // The current index inside the Children collection of the "currentElement"

                    // Return 'path' if the element was found
                    if (currentElement == elem)
                    {
                        return path;
                    }

                    if (currentElement.IsHitTestVisible && currentElement is ContainerElement && index < ((ContainerElement)currentElement).Children.Count)
                    {
                        // Increment the last element of the "currentPathIndexes":
                        currentPathIndexes.Push(index + 1);

                        // Add the child:
                        path.Push(((ContainerElement)currentElement).Children[index]);
                        currentPathIndexes.Push(0);
                    }
                    else
                    {
                        // Remove the last element from the paths to "go up":
                        path.Pop();
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Create an html5 native canvas
        /// </summary>
        /// <param name="Width">The Width of the canvas.</param>
        /// <param name="Height">The Height of the canvas.</param>
        public HtmlCanvas(double Width, double Height)
            : base()
        {
            this.Width = Width;
            this.Height = Height;
        }

        public override object CreateDomElement(object parentRef, out object domElementWhereToPlaceChildren)
        {
            //------------------
            // It is important to create at least 2 divs so that horizontal and vertical alignments work properly (cf. "ApplyHorizontalAlignment" and "ApplyVerticalAlignment" methods)
            //------------------

            object div1 = INTERNAL_HtmlDomManager.CreateDomLayoutElementAndAppendIt("canvas", parentRef, this);
            domElementWhereToPlaceChildren = div1;

            // Use the div2 as the js canvas object
            this._jsCanvas = div1;
            this._jsContext2d = Interop.ExecuteJavaScriptAsync("$0.getContext('2d')", this._jsCanvas);

            Interop.ExecuteJavaScriptAsync("$0.onselectstart = function() { return false; }", this._jsCanvas);

            return div1;
        }

        /// <summary>
        /// Render the canvas content
        /// </summary>
        public void Draw()
        {
            if (this.IsLoaded)
            {
                Interop.ExecuteJavaScriptAsync("$0.width = $0.scrollWidth", this._jsCanvas);
                Interop.ExecuteJavaScriptAsync("$0.height = $0.scrollHeight", this._jsCanvas);
                foreach (HtmlCanvasElement elem in this.Children)
                {
                    this._currentDrawingStyle = elem.Draw(this._currentDrawingStyle, this._jsContext2d);
                }
            }
            else
                throw new Exception("The 'Draw' method of the HtmlCanvas control can only be called after the control has been added to the Visual Tree. For example, you can register to the Loaded event of the HtmlCanvas and call the Draw method at that moment. To know whether the HtmlCanvas is in the Visual Tree or not, you can read the IsLoaded property of the HtmlCanvas.");
        }
    }
}
