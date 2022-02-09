// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------
namespace Microsoft.Expression.Interactivity.Core
{
#if MIGRATION
    using System.Windows;
#else
    using Windows.UI.Xaml;
#endif
    public class ConditionCollection :
#if __WPF__
        FreezableCollection<ComparisonCondition>
#else
        DependencyObjectCollection<ComparisonCondition>
#endif
    {
    }
}