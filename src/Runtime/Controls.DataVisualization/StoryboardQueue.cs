using System.Collections.Generic;
using System.Windows.Media.Animation;

namespace System.Windows.Controls.DataVisualization
{
    /// <summary>
    /// Represents a storyboard queue that plays storyboards in sequence.
    /// </summary>
    internal class StoryboardQueue
    {
        /// <summary>A queue of the storyboards.</summary>
        private Queue<Storyboard> _storyBoards = new Queue<Storyboard>();

        /// <summary>Accepts a new storyboard to play in sequence.</summary>
        /// <param name="storyBoard">The storyboard to play.</param>
        /// <param name="completedAction">An action to execute when the
        /// storyboard completes.</param>
        public void Enqueue(Storyboard storyBoard, EventHandler completedAction)
        {
            storyBoard.Completed += (EventHandler)((sender, args) =>
            {
                if (completedAction != null)
                    completedAction(sender, args);
                this._storyBoards.Dequeue();
                this.Dequeue();
            });
            this._storyBoards.Enqueue(storyBoard);
            if (this._storyBoards.Count != 1)
                return;
            this.Dequeue();
        }

        /// <summary>
        /// Removes the next storyboard in the queue and plays it.
        /// </summary>
        private void Dequeue()
        {
            if (this._storyBoards.Count <= 0)
                return;
            this._storyBoards.Peek().Begin();
        }
    }
}