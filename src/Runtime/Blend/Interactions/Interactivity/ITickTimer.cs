// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System;

namespace Microsoft.Expression.Interactivity
{
    interface ITickTimer
    {
        event EventHandler Tick;
        void Start();
        void Stop();
        TimeSpan Interval { get; set; }
    }
}
