// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// Description: Analogous to DispatcherOperation - one unit of cross-thread work.
//

using System.Windows.Threading;

namespace OpenSilver.Internal.Data;

internal sealed class DataBindOperation
{
    public DataBindOperation(DispatcherOperationCallback method, object arg, int cost = 1)
    {
        _method = method;
        _arg = arg;
        Cost = cost;
    }

    public int Cost { get; set; }

    public void Invoke() => _method(_arg);

    private readonly DispatcherOperationCallback _method;
    private readonly object _arg;
}