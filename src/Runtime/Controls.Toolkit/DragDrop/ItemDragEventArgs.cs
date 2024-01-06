// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using SW = Microsoft.Windows;
using Resource = OpenSilver.Controls.Toolkit.Resources;

namespace System.Windows.Controls
{
    /// <summary>
    /// Information describing a drag event on a UIElement.
    /// </summary>
    /// <QualityBand>Experimental</QualityBand>
    public sealed class ItemDragEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets a value indicating whether an item drag
        /// operation was handled.
        /// </summary>
        public bool Handled { get; set; }

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
                throw new InvalidOperationException(Resource.ItemDragEventArgs_RemoveDataFromSource_DataHasAlreadyBeenRemovedFromSource);
            }
            else if ((this.AllowedEffects & SW.DragDropEffects.Move) != SW.DragDropEffects.Move)
            {
                throw new InvalidOperationException(Resource.ItemDragEventArgs_RemoveDataFromSource_CannotRemoveDataBecauseMoveIsNotAnAllowedEffect);
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
        /// Gets or sets the effects of the completed drag operation.
        /// </summary>
        public SW.DragDropEffects Effects { get; set; }

        /// <summary>
        /// Gets or sets the control that is the source of the drag.
        /// </summary>
        public DependencyObject DragSource { get; set; }

        /// <summary>
        /// Gets or sets the data associated with the item container being dragged.
        /// </summary>
        public object Data { get; set; }

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
        /// Initializes a new instance of the ItemDragEventArgs class.
        /// </summary>
        internal ItemDragEventArgs()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether to cancel the action.
        /// </summary>
        public bool Cancel { get; set; }
        
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