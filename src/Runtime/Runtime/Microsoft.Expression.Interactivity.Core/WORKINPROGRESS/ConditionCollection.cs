#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace Microsoft.Expression.Interactivity.Core
{
    public class ConditionCollection : DependencyObjectCollection<ComparisonCondition>
    {
    }
}