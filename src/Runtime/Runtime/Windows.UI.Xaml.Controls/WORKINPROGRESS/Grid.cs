#if WORKINPROGRESS

#if MIGRATION
using System.Collections.Generic;
using System.Linq;

namespace System.Windows.Controls
#else
namespace Windows.UI.Xaml.Controls
#endif
{
    public partial class Grid
    {
        [OpenSilver.NotImplemented]
        public static readonly DependencyProperty ShowGridLinesProperty = DependencyProperty.Register("ShowGridLines", typeof(bool), typeof(Grid), new PropertyMetadata(false));

        private RowDefinition[] defaultRowDefinitions = new[] { new RowDefinition() };
        private ColumnDefinition[] defaultColumnDefinitions = new[] { new ColumnDefinition() };

        [OpenSilver.NotImplemented]
        public bool ShowGridLines
        {
            get { return (bool)this.GetValue(ShowGridLinesProperty); }
            set { this.SetValue(ShowGridLinesProperty, value); }
        }

        private Size MeasureSingleCell(Size availableSize, GridLength width, GridLength height)
        {
            Size desiredSize = Size.Zero;
            availableSize = new Size(width.IsAbsolute ? width.Value : availableSize.Width, height.IsAbsolute ? height.Value : availableSize.Height);

            foreach (FrameworkElement child in Children)
            {
                child.Measure(availableSize);
                desiredSize = desiredSize.Max(child.DesiredSize);
            }

            return desiredSize;
        }

        private static void GetChildPosition(UIElement child, int rowsCount, int columnsCount, out int row, out int column, out int rowSpan, out int columnSpan)
        {
            row = GetRow(child).Bounds(0, rowsCount - 1);
            column = GetColumn(child).Bounds(0, columnsCount - 1);
            rowSpan = GetRowSpan(child).Bounds(1, rowsCount - row);
            columnSpan = GetColumnSpan(child).Bounds(1, columnsCount - column);
        }

        private static double GetMeasureLength(IDefinitionBase[] definitionBases, double availableLength, double start, double span)
        {
            double remainingLength = availableLength;
            double absoluteLength = 0;
            bool allAbsolute = true;

            for (int i = 0; i < definitionBases.Length; i++)
            {
                if (i >= start && i < start + span)
                {
                    if (definitionBases[i].Length.IsAbsolute)
                    {
                        absoluteLength += (double)definitionBases[i].Length.Value;
                    }
                    else
                    {
                        allAbsolute = false;
                    }
                }
                else if (definitionBases[i].Length.IsAbsolute)
                {
                    remainingLength -= (double)definitionBases[i].Length.Value;
                }
            }

            return allAbsolute ? absoluteLength : Math.Max(0, remainingLength);
        }

        private static void SetBoundedValues(IDefinitionBase[] definitionBases, ref double[] lengths)
        {
            for (int i = 0; i < lengths.Length; i++)
            {
                lengths[i] = lengths[i].Bounds(definitionBases[i].MinLength, definitionBases[i].MaxLength);
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            IDefinitionBase[] currentRowDefinitions = RowDefinitions.Count == 0 ? defaultRowDefinitions : RowDefinitions.ToArray();
            IDefinitionBase[] currentColumnDefinitions = ColumnDefinitions.Count == 0 ? defaultColumnDefinitions : ColumnDefinitions.ToArray();
            
            if (currentRowDefinitions.Length == 1 && currentColumnDefinitions.Length == 1)
            {
                // optimization
                return MeasureSingleCell(availableSize, currentColumnDefinitions[0].Length, currentRowDefinitions[0].Length);
            }

            double[] rowsLength = currentRowDefinitions.Select(definitionBase => definitionBase.Length.IsAbsolute ? definitionBase.Length.Value : 0).ToArray();
            double[] columnsLength = currentColumnDefinitions.Select(definitionBase => definitionBase.Length.IsAbsolute ? definitionBase.Length.Value : 0).ToArray();

            int row;
            int column;
            int rowSpan;
            int columnSpan;

            foreach (UIElement child in Children)
            {
                GetChildPosition(child, currentRowDefinitions.Length, currentColumnDefinitions.Length, out row, out column, out rowSpan, out columnSpan);

                child.Measure(new Size(
                    GetMeasureLength(currentColumnDefinitions, availableSize.Width, column, columnSpan),
                    GetMeasureLength(currentRowDefinitions, availableSize.Height, row, rowSpan)));

                if (rowSpan == 1 && (currentRowDefinitions[row].Length.IsAuto || currentRowDefinitions[row].Length.IsStar))
                {
                    rowsLength[row] = Math.Max(rowsLength[row], child.DesiredSize.Height);
                }

                if (columnSpan == 1 && (currentColumnDefinitions[column].Length.IsAuto || currentColumnDefinitions[column].Length.IsStar))
                {
                    columnsLength[column] = Math.Max(columnsLength[column], child.DesiredSize.Width);
                }
            }

            SetBoundedValues(currentRowDefinitions, ref rowsLength);
            SetBoundedValues(currentColumnDefinitions, ref columnsLength);

            return new Size(columnsLength.Sum(), rowsLength.Sum());
        }
        private Size ArrangeSingleCell(Size finalSize, IDefinitionBase columnDefinition, IDefinitionBase rowDefinition)
        {
            double finalWidth = columnDefinition.Length.IsAbsolute ? columnDefinition.Length.Value : finalSize.Width;
            double finalHeight = rowDefinition.Length.IsAbsolute ? rowDefinition.Length.Value : finalSize.Height;

            Rect finalRect = new Rect(new Size(finalWidth, finalHeight));

            foreach (FrameworkElement child in Children)
            {
                child.Arrange(finalRect);
            }

            return finalSize;
        }

        private static double GetStarLength(IDefinitionBase[] definitionBases, double totalStarsLength)
        {
            // find a starLength where:
            // definitionBases.Sum(axis => axis.GetStarredLength(starLength)) == totalStarsLength
            IEnumerable<IDefinitionBase> starredAxis = definitionBases.Where(axis => axis.Length.IsStar);

            if (starredAxis.Count() == 0 || totalStarsLength <= 0)
            {
                return 0;
            }

            if (starredAxis.Count() == 1)
            {
                return totalStarsLength;
            }

            double[] bounds = starredAxis.Select(axis => axis.MinLength / axis.Length.Value).Union(starredAxis.Select(axis => axis.MaxLength / axis.Length.Value)).ToArray();

            double smallerBound = bounds.Where(vertex => starredAxis.Sum(axis => GetStarredAxisLength(axis, vertex)) <= totalStarsLength).DefaultIfEmpty(Double.NaN).Max();
            double largerBound = bounds.Where(vertex => starredAxis.Sum(axis => GetStarredAxisLength(axis, vertex)) >= totalStarsLength).DefaultIfEmpty(Double.NaN).Min();

            if (!Double.IsNaN(smallerBound) && !Double.IsNaN(largerBound))
            {
                if (smallerBound == largerBound)
                {
                    return smallerBound;
                }

                if (Double.IsInfinity(largerBound))
                {
                    double totalSmallerStarsLength = starredAxis.Where(axis => axis.MaxLength <= axis.Length.Value * smallerBound).Sum(axis => GetStarredAxisLength(axis, smallerBound));
                    double totalLargerStars = starredAxis.Where(axis => axis.MaxLength > axis.Length.Value * smallerBound).Sum(axis => axis.Length.Value);

                    return (totalStarsLength - totalSmallerStarsLength) / totalLargerStars;
                }

                double smallerBoundTotalLength = starredAxis.Sum(axis => GetStarredAxisLength(axis, smallerBound));
                double largerBoundTotalLength = starredAxis.Sum(axis => GetStarredAxisLength(axis, largerBound));

                return smallerBound + (largerBound - smallerBound) * (totalStarsLength - smallerBoundTotalLength) / (largerBoundTotalLength - smallerBoundTotalLength);
            }

            return smallerBound.DefaultIfNaN(largerBound).DefaultIfNaN(0);
        }
        private static double GetStarredAxisLength(IDefinitionBase starredAxis, double starLength)
        {
            return (starredAxis.Length.Value * starLength).Bounds(starredAxis.MinLength, starredAxis.MaxLength);
        }
        
        private static void SetStarLengths(IDefinitionBase[] definitionBases, double starLength, ref double[] lengths)
        {
            for (int i = 0; i < definitionBases.Length; i++)
            {
                if (definitionBases[i].Length.IsStar)
                {
                    lengths[i] = definitionBases[i].Length.Value * starLength;
                }
            }
        }
        private static void SetActualLength(IDefinitionBase[] definitionBases, double[] actualLengths)
        {
            for (int i = 0; i < definitionBases.Length; i++)
            {
                // TODO read only
                //Console.WriteLine($"SetRowActualLength {i} {actualLengths[i]}");
                //definitionBases[i].ActualHeight = actualLengths[i];
            }
        }
        
        protected override Size ArrangeOverride(Size finalSize)
        {
            RowDefinition[] currentRowDefinitions = RowDefinitions.Count == 0 ? defaultRowDefinitions : RowDefinitions.ToArray();
            ColumnDefinition[] currentColumnDefinitions = ColumnDefinitions.Count == 0 ? defaultColumnDefinitions : ColumnDefinitions.ToArray();

            if (currentRowDefinitions.Length == 1 && currentColumnDefinitions.Length == 1)
            {
                // optimization
                return ArrangeSingleCell(finalSize, currentColumnDefinitions[0], currentRowDefinitions[0]);
            }

            double[] rowsLength = currentRowDefinitions.Select(definitionBase => definitionBase.Height.IsAbsolute ? definitionBase.Height.Value : 0).ToArray();
            double[] columnsLength = currentColumnDefinitions.Select(definitionBase => definitionBase.Width.IsAbsolute ? definitionBase.Width.Value : 0).ToArray();

            int row;
            int column;
            int rowSpan;
            int columnSpan;

            foreach (FrameworkElement child in Children)
            {
                GetChildPosition(child, currentRowDefinitions.Length, currentColumnDefinitions.Length, out row, out column, out rowSpan, out columnSpan);

                if (rowSpan == 1 && currentRowDefinitions[row].Height.IsAuto)
                {
                    rowsLength[row] = Math.Max(rowsLength[row], child.DesiredSize.Height);
                }

                if (columnSpan == 1 && currentColumnDefinitions[column].Width.IsAuto)
                {
                    columnsLength[column] = Math.Max(columnsLength[column], child.DesiredSize.Width);
                }
            }

            double rowStarLength = GetStarLength(currentRowDefinitions, finalSize.Height - rowsLength.Sum());
            double columnStarLength = GetStarLength(currentColumnDefinitions, finalSize.Width - columnsLength.Sum());

            SetStarLengths(currentRowDefinitions, rowStarLength, ref rowsLength);
            SetStarLengths(currentColumnDefinitions, columnStarLength, ref columnsLength);

            SetBoundedValues(currentRowDefinitions, ref rowsLength);
            SetBoundedValues(currentColumnDefinitions, ref columnsLength);

            SetActualLength(currentRowDefinitions, rowsLength);
            SetActualLength(currentColumnDefinitions, columnsLength);

            foreach (FrameworkElement child in Children)
            {
                GetChildPosition(child, currentRowDefinitions.Length, currentColumnDefinitions.Length, out row, out column, out rowSpan, out columnSpan);

                child.Arrange(new Rect(
                    columnsLength.Take(column).Sum(),
                    rowsLength.Take(row).Sum(),
                    columnsLength.Skip(column).Take(columnSpan).Sum(),
                    rowsLength.Skip(row).Take(rowSpan).Sum()));
            }

            return finalSize;
        }
    }
}

#endif