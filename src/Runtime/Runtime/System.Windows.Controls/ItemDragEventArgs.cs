
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

using SW = Microsoft.Windows;
using System.Collections.ObjectModel;

namespace System.Windows.Controls
{
    /// <summary>
    /// Information describing a drag event on a UIElement.
    /// </summary>
    public sealed class ItemDragEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the ItemDragEventArgs class.
        /// </summary>
        internal ItemDragEventArgs(SelectionCollection data)
        {
            this.Data = data;
        }

        /// <summary>
        /// Gets or sets a value indicating whether an item drag
        /// operation was handled.
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        /// Gets or sets the effects of the completed drag operation.
        /// </summary>
        public SW.DragDropEffects Effects { get; set; }

        /// <summary>
        /// Gets or sets the data associated with the item container being dragged.
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to cancel the action.
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Gets a value indicating whether removing data
        /// from the source is handled by the target.
        /// </summary>
        public bool DataRemovedFromDragSource { get; private set; }

        /// <summary>
        /// Gets or sets an action that removes data from the drag source.
        /// </summary>
        internal Action<ItemDragEventArgs> RemoveDataFromDragSourceAction { get; set; }

        /// <summary>
        /// This method removes the data from the drag source.
        /// </summary>
        public void RemoveDataFromDragSource()
        {
            if (DataRemovedFromDragSource)
            {
                throw new InvalidOperationException("Properties.Resources.ItemDragEventArgs_RemoveDataFromSource_DataHasAlreadyBeenRemovedFromSource");
            }
            else if ((this.AllowedEffects & SW.DragDropEffects.Move) != SW.DragDropEffects.Move)
            {
                throw new InvalidOperationException("Properties.Resources.ItemDragEventArgs_RemoveDataFromSource_CannotRemoveDataBecauseMoveIsNotAnAllowedEffect");
            }
            else
            {
                RemoveDataFromDragSourceAction(this);
                DataRemovedFromDragSource = true;
            }
        }

        /// <summary>
        /// Gets the key states.
        /// </summary>
        public SW.DragDropKeyStates KeyStates { get; internal set; }

        /// <summary>
        /// Gets or sets the allowed effects.
        /// </summary>
        public SW.DragDropEffects AllowedEffects { get; set; }

        /// <summary>
        /// Gets or sets the control that is the source of the drag.
        /// </summary>
        public DependencyObject DragSource { get; set; }

        /// <summary>
        /// Gets or sets the mouse offset from the item being dragged at the 
        /// beginning of the drag operation.
        /// </summary>
        public Point DragDecoratorContentMouseOffset { get; set; }

        /// <summary>
        /// Gets or sets the content to insert into the DragDecorator.
        /// </summary>
        public object DragDecoratorContent { get; set; }

        /// <summary>
        /// Initializes a new instance of the ItemDragEventArgs class using an
        /// existing instance.
        /// </summary>
        /// <param name="args">The instance to use as the template when creating
        /// the new instance.</param>
        internal ItemDragEventArgs(ItemDragEventArgs args)
        {
            this.AllowedEffects = args.AllowedEffects;
            this.Effects = args.Effects;
            this.Data = args.Data;
            this.DragSource = args.DragSource;
            this.KeyStates = args.KeyStates;
            this.DragDecoratorContent = args.DragDecoratorContent;
            this.DragDecoratorContentMouseOffset = args.DragDecoratorContentMouseOffset;
            this.RemoveDataFromDragSourceAction = args.RemoveDataFromDragSourceAction;
            this.DataRemovedFromDragSource = args.DataRemovedFromDragSource;
        }
    }
}
