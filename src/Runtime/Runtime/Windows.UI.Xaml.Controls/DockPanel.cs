﻿

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


using CSHTML5.Internal;
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;

#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    /// <summary>
    /// Defines an area where you can arrange child elements either horizontally
    /// or vertically, relative to each other.
    /// </summary>
    public partial class DockPanel : Panel
    {
        Grid _grid;


        ///// <summary>
        ///// Initializes a new instance of the DockPanel class.
        ///// </summary>
        //public DockPanel();
        
        /// <summary>
        /// Gets or sets a value that indicates whether the last child element within
        /// a DockPanel stretches to fill the remaining available
        /// space.
        /// </summary>
        public bool LastChildFill
        {
            get { return (bool)GetValue(LastChildFillProperty); }
            set { SetValue(LastChildFillProperty, value); }
        }
        /// <summary>
        /// Identifies the DockPanel.LastChildFill dependency
        /// property.
        /// </summary>
#if WORKINPROGRESS
        public static readonly DependencyProperty LastChildFillProperty =
            DependencyProperty.Register("LastChildFill", typeof(bool), typeof(DockPanel), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsMeasure));
#else
        public static readonly DependencyProperty LastChildFillProperty =
            DependencyProperty.Register("LastChildFill", typeof(bool), typeof(DockPanel), new PropertyMetadata(true));
#endif
        /// <summary>
        /// Gets the value of the DockPanel.Dock attached property
        /// for a specified System.Windows.UIElement.
        /// </summary>
        /// <param name="element">The element from which the property value is read.</param>
        /// <returns>The DockPanel.Dock property value for the element.</returns>
        public static Dock GetDock(DependencyObject element)
        {
            return (Dock)element.GetValue(DockProperty);
        }

        /// <summary>
        /// Sets the value of the DockPanel.Dock attached property
        /// to a specified element.
        /// </summary>
        /// <param name="element">The element to which the attached property is written.</param>
        /// <param name="dock">The needed Dock value.</param>
        public static void SetDock(DependencyObject element, Dock dock)
        {
            element.SetValue(DockProperty, dock);
        }

        /// <summary>
        /// Identifies the DockPanel.Dock attached property.
        /// </summary>
        public static readonly DependencyProperty DockProperty =
            DependencyProperty.RegisterAttached("Dock", typeof(Dock), typeof(DockPanel), new PropertyMetadata(Dock.Left)); //this is the likely default value since Dock.Left is 0.



        internal protected override void INTERNAL_OnAttachedToVisualTree()
        {
            if (_grid == null)
            {
                _grid = new Grid();
                MakeUIStructure();
            }

            base.INTERNAL_OnAttachedToVisualTree();
        }

        private void MakeUIStructure()
        {
            //we go through the children and determine the rows, columns and positions in the grid:
            if (Children != null && Children.Count > 0)
            {
                int amountOfRows = 1; // = 1 for the remaining space
                int amountOfColumns = 1; // = 1 for the remaining space
                Dock lastChildDock = Dock.Left; //We only need it to know the amount of rows and columns and to know which row and column are the "remaining space" (sized at star) in the case where LastChildFill is true.

                //first pass: we count the amount of rows and columns.
                foreach (UIElement child in Children)
                {
                    //get the Dock value of the child:
                    Dock dock = DockPanel.GetDock(child);

                    if (dock == Dock.Left || dock == Dock.Right)
                    {
                        ++amountOfColumns;
                    }
                    else
                    {
                        ++amountOfRows;
                    }
                }
                if (LastChildFill) //if the last child fills the remaining space, we "remove" the row/column we "added" for this child.
                {
                    lastChildDock = GetDock(Children[Children.Count - 1]);
                    if (lastChildDock == Dock.Right || lastChildDock == Dock.Left)
                    {
                        --amountOfColumns;
                    }
                    else
                    {
                        --amountOfRows;
                    }
                }

                //second pass: we determine the Grid.Row, Grid.Column, Grid.RowSpan and Grid.ColumnsSpan for each child.
                int amountOfRightPlaced = 0;
                int amountOfLeftPlaced = 0;
                int amountOfTopPlaced = 0;
                int amountOfBottomPlaced = 0;

                foreach (UIElement child in Children)
                {
                    //get the Dock value of the child:
                    Dock dock = DockPanel.GetDock(child);

                    switch (dock)
                    {
                        case Dock.Left:
                            Grid.SetRow(child, amountOfTopPlaced);
                            Grid.SetColumn(child, amountOfLeftPlaced);
                            Grid.SetRowSpan(child, amountOfRows - amountOfTopPlaced - amountOfBottomPlaced);
                            Grid.SetColumnSpan(child, 1);
                            ++amountOfLeftPlaced;
                            break;
                        case Dock.Top:
                            Grid.SetRow(child, amountOfTopPlaced);
                            Grid.SetColumn(child, amountOfLeftPlaced);
                            Grid.SetRowSpan(child, 1);
                            Grid.SetColumnSpan(child, amountOfColumns - amountOfLeftPlaced - amountOfRightPlaced);
                            ++amountOfTopPlaced;
                            break;
                        case Dock.Right:
                            Grid.SetRow(child, amountOfTopPlaced);
                            Grid.SetColumn(child, amountOfColumns - amountOfRightPlaced - 1);
                            Grid.SetRowSpan(child, amountOfRows - amountOfTopPlaced - amountOfBottomPlaced);
                            Grid.SetColumnSpan(child, 1);
                            ++amountOfRightPlaced;
                            break;
                        case Dock.Bottom:
                            Grid.SetRow(child, amountOfRows - amountOfBottomPlaced - 1);
                            Grid.SetColumn(child, amountOfLeftPlaced);
                            Grid.SetRowSpan(child, 1);
                            Grid.SetColumnSpan(child, amountOfColumns - amountOfLeftPlaced - amountOfRightPlaced);
                            ++amountOfBottomPlaced;
                            break;
                        default:
                            break;
                    }
                }

                //we remove the grid because we will change its structure A LOT, and we want to avoid redrawing everything on each change:
                INTERNAL_VisualTreeManager.DetachVisualChildIfNotNull(_grid, this);

                ColumnDefinitionCollection columnsDefinitions = _grid.ColumnDefinitions;
                columnsDefinitions.Clear();
                for (int i = 0; i < amountOfColumns; ++i)
                {
                    columnsDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                }
                RowDefinitionCollection rowsDefinitions = _grid.RowDefinitions;
                rowsDefinitions.Clear();
                for (int i = 0; i < amountOfRows; ++i)
                {
                    rowsDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                }

                if (!LastChildFill)
                {
                    columnsDefinitions.ElementAt(amountOfLeftPlaced).Width = new GridLength(1, GridUnitType.Star);
                    rowsDefinitions.ElementAt(amountOfTopPlaced).Height = new GridLength(1, GridUnitType.Star);
                }
                else
                {
                    //the position of the "remaining space" depends on the last child's dock:
                    if (lastChildDock == Dock.Left)
                    {
                        columnsDefinitions.ElementAt(amountOfLeftPlaced - 1).Width = new GridLength(1, GridUnitType.Star); //minus 1 because the column index of the last child placed left is also the column index of the "remaining space".
                    }
                    else
                    {
                        columnsDefinitions.ElementAt(amountOfLeftPlaced).Width = new GridLength(1, GridUnitType.Star);
                    }

                    if (lastChildDock == Dock.Top)
                    {
                        rowsDefinitions.ElementAt(amountOfTopPlaced - 1).Height = new GridLength(1, GridUnitType.Star); //minus 1 because the column index of the last child placed left is also the column index of the "remaining space".
                    }
                    else
                    {
                        rowsDefinitions.ElementAt(amountOfTopPlaced).Height = new GridLength(1, GridUnitType.Star); //minus 1 because the column index of the last child placed left is also the column index of the "remaining space".
                    }
                }
                //the changes on the grid's structure are over so we can put it back.
                INTERNAL_VisualTreeManager.AttachVisualChildIfNotAlreadyAttached(_grid, this);
            }
        }

        internal override void OnChildrenAdded(UIElement newChild, int index)
        {
            this.MakeUIStructure();
            this._grid.Children.Insert(index, newChild);
        }

        internal override void OnChildrenRemoved(UIElement oldChild, int index)
        {
            this._grid.Children.RemoveAt(index);
        }

        internal override void OnChildrenReplaced(UIElement oldChild, UIElement newChild, int index)
        {
            if (oldChild == newChild)
            {
                return;
            }

            Debug.Assert(oldChild == this._grid.Children[index]);

            this.MakeUIStructure();
            this._grid.Children[index] = newChild;
        }

        internal override void OnChildrenMoved(UIElement oldChild, int newIndex, int oldIndex)
        {
            if (newIndex == oldIndex)
            {
                return;
            }

            this.MakeUIStructure();
            this._grid.Children.Move(oldIndex, newIndex);
        }

        internal override void OnChildrenReset()
        {
            this._grid.Children.Clear();

            this.MakeUIStructure();

            foreach (UIElement child in this.Children)
            {
                this._grid.Children.Add(child);
            }
        }

#if false
        internal override void ManageChildrenChanged(IList oldChildren, IList newChildren)
        {
            if (oldChildren != null)
            {
                foreach (UIElement oldChild in oldChildren)
                {
                    this._grid.Children.Remove(oldChild);
                }
            }
            this.MakeUIStructure();

            if (newChildren != null)
            {
                foreach (UIElement newChild in newChildren)
                {
                    this._grid.Children.Add(newChild);
                }
            }
        }
#endif
#if WORKINPROGRESS
        private static Orientation GetDockOrientation(Dock dock)
        {
            return dock == Dock.Left || dock == Dock.Right ? Orientation.Horizontal : Orientation.Vertical;
        }
        protected override Size MeasureOverride(Size availableSize)
        {
            double remainingWidth = availableSize.Width;
            double remainingHeight = availableSize.Height;

            foreach (UIElement child in Children)
            {
                child.Measure(new Size(remainingWidth, remainingHeight).Max(Size.Zero));

                if (GetDockOrientation(GetDock(child)) == Orientation.Horizontal)
                {
                    remainingWidth -= child.DesiredSize.Width;
                }
                else
                {
                    remainingHeight -= child.DesiredSize.Height;
                }
            }

            double childrenWidth = 0;
            double childrenHeight = 0;

            for (int i = Children.Count - 1; i >= 0; i--)
            {
                UIElement child = Children[i];

                if (GetDockOrientation(GetDock(child)) == Orientation.Horizontal)
                {
                    childrenWidth += child.DesiredSize.Width;
                    childrenHeight = Math.Max(childrenHeight, child.DesiredSize.Height);
                }
                else
                {
                    childrenHeight += child.DesiredSize.Height;
                    childrenWidth = Math.Max(childrenWidth, child.DesiredSize.Width);
                }
            }

            return new Size(childrenWidth, childrenHeight);
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            double remainingWidth = finalSize.Width;
            double remainingHeight = finalSize.Height;

            double left = 0;
            double top = 0;

            for (int i = 0; i < Children.Count; i++)
            {
                UIElement child = Children[i];
                Dock childDock = GetDock(child);
                Orientation childDockOrientation = GetDockOrientation(childDock);

                bool childFill = LastChildFill && i == Children.Count - 1;

                double cellWidth = childDockOrientation == Orientation.Vertical || childFill ? remainingWidth : child.DesiredSize.Width;
                double cellHeight = childDockOrientation == Orientation.Horizontal || childFill ? remainingHeight : child.DesiredSize.Height;

                double cellLeft = childDock == Dock.Right ? remainingWidth - cellWidth : 0;
                double cellTop = childDock == Dock.Bottom ? remainingHeight - cellHeight : 0;

                child.Arrange(new Rect(left + cellLeft, top + cellTop, cellWidth.Max(0), cellHeight.Max(0)));

                if (childDockOrientation == Orientation.Horizontal)
                {
                    remainingWidth -= cellWidth;

                    if (childDock == Dock.Left)
                    {
                        left += cellWidth;
                    }
                }
                else
                {
                    remainingHeight -= cellHeight;

                    if (childDock == Dock.Top)
                    {
                        top += cellHeight;
                    }
                }
            }

            return finalSize;
        }
#endif
    }
}
