// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------
namespace Microsoft.Expression.Interactivity
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic", Justification = "This isn't an exception.")]
    interface ITickTimer
    {
#if MIGRATION
        event EventHandler Tick;
#else
        event EventHandler<object> Tick;
#endif
        void Start();
        void Stop();
        TimeSpan Interval { get; set; }
    }
}
