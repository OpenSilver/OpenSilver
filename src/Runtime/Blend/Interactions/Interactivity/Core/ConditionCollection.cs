// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System.Windows;

namespace Microsoft.Expression.Interactivity.Core
{
    public class ConditionCollection :
#if __WPF__
        FreezableCollection<ComparisonCondition>
#else
        DependencyObjectCollection<ComparisonCondition>
#endif
    {
    }
}