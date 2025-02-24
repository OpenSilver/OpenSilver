// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Description: common base class and contract for data source provider objects

using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;

namespace System.Windows.Data
{
    /// <summary>
    /// Common base class and contract for <see cref="DataSourceProvider"/> objects, which are factories that execute 
    /// some queries to produce a single object or a list of objects that you can use as binding source objects.
    /// </summary>
    public abstract class DataSourceProvider : INotifyPropertyChanged, ISupportInitialize
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataSourceProvider"/> class.
        /// </summary>
        protected DataSourceProvider()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        /// <summary>
        /// Starts the initial query to the underlying data model. The result is returned on the <see cref="Data"/> property.
        /// </summary>
        public void InitialLoad()
        {
            // ignore call if IsInitialLoadEnabled == false or already started initialization
            if (!IsInitialLoadEnabled || _initialLoadCalled)
                return;

            _initialLoadCalled = true;
            BeginQuery();
        }

        /// <summary>
        /// Initiates a refresh operation to the underlying data model. The result is returned on the <see cref="Data"/> property.
        /// </summary>
        public void Refresh()
        {
            _initialLoadCalled = true;
            BeginQuery();
        }

        /// <summary>
        /// Gets or sets a value that indicates whether to prevent or delay the automatic loading of data.
        /// </summary>
        /// <returns>
        /// false to prevent or delay the automatic loading of data; otherwise, true. The default value is true.
        /// </returns>
        [DefaultValue(true)]
        public bool IsInitialLoadEnabled
        {
            get { return _isInitialLoadEnabled; }
            set
            {
                _isInitialLoadEnabled = value;
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsInitialLoadEnabled)));
            }
        }

        /// <summary>
        /// Gets the underlying data object.
        /// </summary>
        /// <returns>
        /// A value of type System.Object that is the underlying data object.
        /// </returns>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object Data
        {
            get { return _data; }
        }

        /// <summary>
        /// Occurs when the <see cref="Data"/> property has a new value.
        /// </summary>
        public event EventHandler DataChanged;

        /// <summary>
        /// Gets the error of the last query operation.
        /// </summary>
        /// <returns>
        /// A value of type System.Exception that is the error of the last query operation, or null if there was no error.
        /// </returns>
        public Exception Error
        {
            get { return _error; }
        }

        /// <summary>
        /// Enters a defer cycle that you can use to change properties of the provider and delay automatic refresh.
        /// </summary>
        /// <returns>
        /// An <see cref="IDisposable"/> object that you can use to dispose of the calling object.
        /// </returns>
        public virtual IDisposable DeferRefresh()
        {
            ++_deferLevel;
            return new DeferHelper(this);
        }

        /// <summary>
        /// Initialization of this element is about to begin
        /// </summary>
        void ISupportInitialize.BeginInit()
        {
            BeginInit();
        }

        /// <summary>
        /// Initialization of this element has completed
        /// </summary>
        void ISupportInitialize.EndInit()
        {
            EndInit();
        }

        /// <summary>
        /// PropertyChanged event (per <see cref="INotifyPropertyChanged" />).
        /// </summary>
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                PropertyChanged += value;
            }
            remove
            {
                PropertyChanged -= value;
            }
        }

        /// <summary>
        /// Gets a value that indicates whether there is an outstanding <see cref="DeferRefresh"/> is use.
        /// </summary>
        /// <returns>
        /// true if there is an outstanding <see cref="DeferRefresh"/> in use; otherwise, false.
        /// </returns>
        protected bool IsRefreshDeferred
        {
            get
            {
                return _deferLevel > 0 || (!IsInitialLoadEnabled && !_initialLoadCalled);
            }
        }

        /// <summary>
        /// Gets or sets the current <see cref="Threading.Dispatcher"/> object to the UI thread to use.
        /// </summary>
        /// <returns>
        /// The current <see cref="Threading.Dispatcher"/> object to the UI thread to use. By default, this is the 
        /// <see cref="Threading.Dispatcher"/> object that is associated with the thread on which this instance was created.
        /// </returns>
        protected Dispatcher Dispatcher
        {
            get { return _dispatcher; }
            set
            {
                if (_dispatcher != value)
                {
                    _dispatcher = value;
                }
            }
        }

        /// <summary>
        /// When overridden in a derived class, this base class calls this method when <see cref="InitialLoad"/> or 
        /// <see cref="Refresh"/> has been called. The base class delays the call if refresh is deferred or initial 
        /// load is disabled.
        /// </summary>
        protected virtual void BeginQuery()
        {
        }

        /// <summary>
        /// Derived classes call this method to indicate that a query has finished.
        /// </summary>
        /// <param name="newData">
        /// The data that is the result of the query.
        /// </param>
        protected void OnQueryFinished(object newData)
        {
            OnQueryFinished(newData, null, null, null);
        }

        /// <summary>
        /// Derived classes call this method to indicate that a query has finished.
        /// </summary>
        /// <param name="newData">
        /// The data that is the result of the query.
        /// </param>
        /// <param name="error">
        /// The error that occurred while running the query. This value is null if there is no error.
        /// </param>
        /// <param name="completionWork">
        /// Optional delegate that is used to execute completion work on the UI thread, for example, to set 
        /// additional properties.
        /// </param>
        /// <param name="callbackArguments">
        /// Optional arguments to send as a parameter with the completionWork delegate.
        /// </param>
        protected virtual void OnQueryFinished(object newData, Exception error, DispatcherOperationCallback completionWork, object callbackArguments)
        {
            Debug.Assert(Dispatcher != null);
            // check if we're already on the dispatcher thread
            if (Dispatcher.CheckAccess())
            {
                // already on UI thread
                UpdateWithNewResult(error, newData, completionWork, callbackArguments);
            }
            else
            {
                // marshal the result back to the main thread
                Dispatcher.BeginInvoke(
                    DispatcherPriority.Normal, UpdateWithNewResultCallback,
                    new object[] { this, error, newData, completionWork, callbackArguments });
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        protected virtual event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event with the provided arguments.
        /// </summary>
        /// <param name="e">
        /// Arguments of the event being raised.
        /// </param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Indicates that initialization of this object is about to begin; no implicit <see cref="Refresh"/>
        /// occurs until the matched <see cref="EndInit"/> method is called.
        /// </summary>
        protected virtual void BeginInit()
        {
            ++_deferLevel;
        }

        /// <summary>
        /// Indicates that the initialization of this object has completed; this causes a <see cref="Refresh"/> 
        /// if no other <see cref="DeferRefresh"/> is outstanding.
        /// </summary>
        protected virtual void EndInit()
        {
            EndDefer();
        }

        private void EndDefer()
        {
            --_deferLevel;

            if (_deferLevel == 0)
            {
                Refresh();
            }
        }

        private static object UpdateWithNewResult(object arg)
        {
            object[] args = (object[])arg;
            Debug.Assert(args.Length == 5);
            DataSourceProvider provider = (DataSourceProvider)args[0];
            Exception error = (Exception)args[1];
            object newData = args[2];
            DispatcherOperationCallback completionWork = (DispatcherOperationCallback)args[3];
            object callbackArgs = args[4];

            provider.UpdateWithNewResult(error, newData, completionWork, callbackArgs);
            return null;
        }

        private void UpdateWithNewResult(Exception error, object newData, DispatcherOperationCallback completionWork, object callbackArgs)
        {
            bool errorChanged = _error != error;
            _error = error;
            if (error != null)
            {
                newData = null;
                _initialLoadCalled = false; // allow again InitialLoad after an error
            }

            _data = newData;

            completionWork?.Invoke(callbackArgs);

            // notify any listeners
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Data)));
            DataChanged?.Invoke(this, EventArgs.Empty);
            if (errorChanged)
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Error)));
        }

        private sealed class DeferHelper : IDisposable
        {
            public DeferHelper(DataSourceProvider provider)
            {
                _provider = provider;
            }

            public void Dispose()
            {
                GC.SuppressFinalize(this);
                if (_provider != null)
                {
                    _provider.EndDefer();
                    _provider = null;
                }
            }

            private DataSourceProvider _provider;
        }

        private bool _isInitialLoadEnabled = true;
        private bool _initialLoadCalled;
        private int _deferLevel;
        private object _data;
        private Exception _error;
        private Dispatcher _dispatcher;

        private static readonly DispatcherOperationCallback UpdateWithNewResultCallback = new DispatcherOperationCallback(UpdateWithNewResult);
    }
}