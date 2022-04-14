﻿// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------
#if __WPF__

namespace Microsoft.Expression.Interactivity.Core
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls.Primitives;
    using System.Windows.Interactivity;

    /// <summary>
    /// An action that will launch a process to open a file or Uri. For files, this action will launch the default program 
    /// for the given file extension. A Uri will open in a web browser.
    /// </summary>
    public class LaunchUriOrFileAction : TriggerAction<DependencyObject>
    {
        public static readonly DependencyProperty PathProperty = DependencyProperty.Register("Path", typeof(string), typeof(LaunchUriOrFileAction));

        public LaunchUriOrFileAction()
        {
        }

        /// <summary>
        /// The file or Uri to open. 
        /// </summary>
        public string Path
        {
            get { return (string)this.GetValue(PathProperty); }
            set { this.SetValue(PathProperty, value); }
        }

        /// <summary>
        /// This method is called when some criteria is met and the action is invoked.
        /// </summary>
        /// <param name="parameter"></param>
        protected override void Invoke(object parameter)
        {
            if (this.AssociatedObject != null && !string.IsNullOrEmpty(this.Path))
            {
                Process.Start(this.Path);
            }
        }
    }
}

#endif
