using Microsoft.AspNetCore.Components;

namespace OpenSilver.Simulator.BlazorSupport
{
    /// <summary>
    /// A <see cref="Dispatcher"/> implementation that adapts a WPF <see cref="System.Windows.Threading.Dispatcher"/>
    /// for use in a Blazor application.
    /// </summary>
    internal sealed class BlazorDispatcher : Dispatcher
    {
        private readonly System.Windows.Threading.Dispatcher _originalDispatcher;

        public BlazorDispatcher(System.Windows.Threading.Dispatcher dispatcher)
        {
            _originalDispatcher = dispatcher
                ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        /// <inheritdoc />
        public override bool CheckAccess()
            => _originalDispatcher.CheckAccess();

        /// <inheritdoc />
        public override async Task InvokeAsync(Action workItem)
        {
            if (CheckAccess())
            {
                workItem();
            }
            else
            {
                await _originalDispatcher.InvokeAsync(workItem).Task.ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public override async Task InvokeAsync(Func<Task> workItem)
        {
            if (CheckAccess())
            {
                await workItem().ConfigureAwait(false);
            }
            else
            {
                await _originalDispatcher.InvokeAsync(workItem).Task.Unwrap().ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public override async Task<TResult> InvokeAsync<TResult>(Func<TResult> workItem)
        {
            if (CheckAccess())
            {
                return workItem();
            }
            else
            {
                return await _originalDispatcher.InvokeAsync(workItem).Task.ConfigureAwait(false);
            }
        }

        /// <inheritdoc />
        public override async Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> workItem)
        {
            if (CheckAccess())
            {
                return await workItem().ConfigureAwait(false);
            }
            else
            {
                return await _originalDispatcher.InvokeAsync(workItem).Task.Unwrap().ConfigureAwait(false);
            }
        }
    }
}