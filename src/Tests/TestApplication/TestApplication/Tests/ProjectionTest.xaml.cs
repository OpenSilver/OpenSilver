using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace TestApplication.Tests
{
    public partial class ProjectionTest : Page
    {
        private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

        private static readonly ParameterSpec[] PlaygroundParameters =
        [
            new ParameterSpec(PlaneProjection.RotationXProperty, -180, 180, 0, "0"),
            new ParameterSpec(PlaneProjection.RotationYProperty, -180, 180, 0, "0"),
            new ParameterSpec(PlaneProjection.RotationZProperty, -180, 180, 0, "0"),
            new ParameterSpec(PlaneProjection.CenterOfRotationXProperty, -1, 2, 0.5, "0.00"),
            new ParameterSpec(PlaneProjection.CenterOfRotationYProperty, -1, 2, 0.5, "0.00"),
            new ParameterSpec(PlaneProjection.CenterOfRotationZProperty, -300, 300, 0, "0"),
            new ParameterSpec(PlaneProjection.LocalOffsetXProperty, -200, 200, 0, "0"),
            new ParameterSpec(PlaneProjection.LocalOffsetYProperty, -200, 200, 0, "0"),
            new ParameterSpec(PlaneProjection.LocalOffsetZProperty, -300, 300, 0, "0"),
            new ParameterSpec(PlaneProjection.GlobalOffsetXProperty, -200, 200, 0, "0"),
            new ParameterSpec(PlaneProjection.GlobalOffsetYProperty, -200, 200, 0, "0"),
            new ParameterSpec(PlaneProjection.GlobalOffsetZProperty, -1500, 900, 0, "0"),
        ];

        private static readonly Preset[] PlaygroundPresets =
        [
            new Preset("Tilt", (PlaneProjection.RotationXProperty, -35), (PlaneProjection.RotationYProperty, 25)),
            new Preset("Hinge", (PlaneProjection.RotationYProperty, -65), (PlaneProjection.CenterOfRotationXProperty, 0)),
            new Preset("Spin", (PlaneProjection.RotationXProperty, 40), (PlaneProjection.RotationZProperty, 45)),
            new Preset("Back", (PlaneProjection.RotationYProperty, 180)),
            new Preset("Edge", (PlaneProjection.RotationYProperty, 90)),
            new Preset("Closer", (PlaneProjection.GlobalOffsetZProperty, 500)),
            new Preset("Farther", (PlaneProjection.GlobalOffsetZProperty, -1200)),
            new Preset("Slide", (PlaneProjection.RotationXProperty, -55), (PlaneProjection.LocalOffsetYProperty, -120)),
            new Preset("Reset"),
        ];

        private static readonly Color[] Palette =
        [
            Color.FromArgb(255, 0x4A, 0x90, 0xD9),
            Color.FromArgb(255, 0x27, 0xAE, 0x60),
            Color.FromArgb(255, 0xE6, 0x7E, 0x22),
            Color.FromArgb(255, 0x8E, 0x44, 0xAD),
            Color.FromArgb(255, 0xC0, 0x39, 0x2B),
            Color.FromArgb(255, 0x16, 0xA0, 0x85),
            Color.FromArgb(255, 0xF3, 0x9C, 0x12),
            Color.FromArgb(255, 0x2C, 0x3E, 0x50),
        ];

        private const double CoverFlowTileSize = 150;

        private readonly Dictionary<DependencyProperty, ParameterSlider> _playgroundSliders = [];
        private readonly Animator _playgroundAnimator = new();
        private readonly List<Border> _coverFlowTiles = [];
        private readonly Animator _coverFlowAnimator = new();

        private bool _isInitialized;
        private bool _isPlaygroundMatrixUpdatePending;
        private int _playgroundClicks;
        private int _nestedClicks;
        private int _selectedCoverFlowIndex = 6;

        public ProjectionTest()
        {
            InitializeComponent();

            BuildPlayground();
            BuildFlipCards();
            BuildCoverFlow();

            _isInitialized = true;
            UpdateMatrixProjection();

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            UpdatePlaygroundMatrixLater();
            OnStartAnimationsClick(null, null);
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            OnStopAnimationsClick(null, null);
            ProjectedPopupCheckBox.IsChecked = false;
        }

        #region 1. Playground

        private void BuildPlayground()
        {
            foreach (ParameterSpec spec in PlaygroundParameters)
            {
                var item = new ParameterSlider
                {
                    Spec = spec,
                    Label = new TextBlock { FontSize = 11 },
                    Slider = new Slider { Minimum = spec.Minimum, Maximum = spec.Maximum, Value = spec.DefaultValue, Margin = new Thickness(0, 0, 0, 2) },
                };
                item.UpdateLabel();
                item.Slider.ValueChanged += (s, e) =>
                {
                    item.UpdateLabel();
                    PlaygroundProjection.SetValue(item.Spec.Property, item.Slider.Value);
                    UpdatePlaygroundMatrixLater();
                };

                PlaygroundSlidersPanel.Children.Add(item.Label);
                PlaygroundSlidersPanel.Children.Add(item.Slider);
                _playgroundSliders[spec.Property] = item;
            }

            StackPanel row = null;
            for (int i = 0; i < PlaygroundPresets.Length; i++)
            {
                if (i % 4 == 0)
                {
                    row = new StackPanel { Orientation = Orientation.Horizontal };
                    PlaygroundPresetsPanel.Children.Add(row);
                }

                var button = new Button
                {
                    Content = PlaygroundPresets[i].Name,
                    Tag = PlaygroundPresets[i].Values,
                    Margin = new Thickness(0, 0, 4, 4),
                    Padding = new Thickness(6, 2, 6, 2),
                };
                button.Click += OnPlaygroundPresetClick;
                row.Children.Add(button);
            }
        }

        private void OnPlaygroundPresetClick(object sender, RoutedEventArgs e)
        {
            var targets = new Dictionary<DependencyProperty, double>();
            foreach (ParameterSlider slider in _playgroundSliders.Values)
            {
                targets[slider.Spec.Property] = slider.Spec.DefaultValue;
            }

            var presetValues = ((DependencyProperty Property, double Value)[])((Button)sender).Tag;
            foreach ((DependencyProperty dp, double value) in presetValues)
            {
                targets[dp] = value;
            }

            // The sliders are animated, so that they show the values as they change.
            var easing = new CubicEase { EasingMode = EasingMode.EaseInOut };
            var values = new List<AnimatedValue>();
            foreach (ParameterSlider slider in _playgroundSliders.Values)
            {
                values.Add(new AnimatedValue(slider.Slider, RangeBase.ValueProperty, targets[slider.Spec.Property], 0, 0.7, easing));
            }

            _playgroundAnimator.Run(values);
        }

        private void UpdatePlaygroundMatrixLater()
        {
            if (_isPlaygroundMatrixUpdatePending)
            {
                return;
            }

            _isPlaygroundMatrixUpdatePending = true;
            Dispatcher.BeginInvoke(() =>
            {
                _isPlaygroundMatrixUpdatePending = false;
                PlaygroundMatrixText.Text = FormatMatrix(PlaygroundProjection.ProjectionMatrix);
            });
        }

        private void OnPlaygroundMouseMove(object sender, MouseEventArgs e)
        {
            Point position = e.GetPosition(PlaygroundCard);
            PlaygroundMouseText.Text = FormatPoint(position);

            // The dot is in the child of the border, which starts after its BorderThickness.
            Thickness border = PlaygroundCard.BorderThickness;
            Canvas.SetLeft(PlaygroundMouseDot, position.X - border.Left - PlaygroundMouseDot.Width / 2);
            Canvas.SetTop(PlaygroundMouseDot, position.Y - border.Top - PlaygroundMouseDot.Height / 2);
            PlaygroundMouseDot.Visibility = Visibility.Visible;
        }

        private void OnPlaygroundMouseLeave(object sender, MouseEventArgs e)
        {
            PlaygroundMouseText.Text = "-";
            PlaygroundMouseDot.Visibility = Visibility.Collapsed;
        }

        private void OnPlaygroundButtonClick(object sender, RoutedEventArgs e)
        {
            _playgroundClicks++;
            PlaygroundClicksText.Text = _playgroundClicks.ToString(Invariant);
        }

        #endregion

        #region 2. Flip cards

        // A face is visible when all its animated properties are 0, and hidden when they have their
        // hidden values, which must leave it edge-on (a RotationX or RotationY of ±90).
        private sealed class FlipCard
        {
            public readonly PlaneProjection Front = new PlaneProjection();
            public readonly PlaneProjection Back = new PlaneProjection();
            public readonly Animator Animator = new Animator();
            public bool IsFlipped;

            public DependencyProperty[] Properties;
            public double[] HiddenFrontValues;
            public double[] HiddenBackValues;
            public double LeaveDuration;
            public double ArriveDuration;
            public IEasingFunction LeaveEasing;
            public IEasingFunction ArriveEasing;
        }

        private void BuildFlipCards()
        {
            FlipCardsPanel.Children.Add(CreateFlipCard("A", "♥", Palette[4], "Horizontal", new FlipCard
            {
                Properties = [PlaneProjection.RotationYProperty],
                HiddenFrontValues = [90.0],
                HiddenBackValues = [-90.0],
                LeaveDuration = 0.25,
                ArriveDuration = 0.25,
            }));

            FlipCardsPanel.Children.Add(CreateFlipCard("K", "♠", Palette[7], "Vertical, with a bounce", new FlipCard
            {
                Properties = [PlaneProjection.RotationXProperty],
                HiddenFrontValues = [90.0],
                HiddenBackValues = [-90.0],
                LeaveDuration = 0.2,
                ArriveDuration = 0.8,
                LeaveEasing = new CubicEase { EasingMode = EasingMode.EaseIn },
                ArriveEasing = new ElasticEase { EasingMode = EasingMode.EaseOut, Oscillations = 2, Springiness = 4 },
            }));

            FlipCardsPanel.Children.Add(CreateFlipCard("Q", "♦", Palette[2], "Jump toward the viewer", new FlipCard
            {
                Properties = [PlaneProjection.RotationYProperty, PlaneProjection.GlobalOffsetZProperty],
                HiddenFrontValues = [90.0, 350],
                HiddenBackValues = [-90.0, 350],
                LeaveDuration = 0.35,
                ArriveDuration = 0.35,
                LeaveEasing = new CubicEase { EasingMode = EasingMode.EaseOut },
                ArriveEasing = new CubicEase { EasingMode = EasingMode.EaseIn },
            }));

            FlipCardsPanel.Children.Add(CreateFlipCard("J", "♣", Palette[1], "Twirl", new FlipCard
            {
                Properties = [PlaneProjection.RotationYProperty, PlaneProjection.RotationZProperty],
                HiddenFrontValues = [90.0, 180],
                HiddenBackValues = [-90.0, -180],
                LeaveDuration = 0.4,
                ArriveDuration = 0.4,
                LeaveEasing = new SineEase { EasingMode = EasingMode.EaseIn },
                ArriveEasing = new SineEase { EasingMode = EasingMode.EaseOut },
            }));
        }

        private UIElement CreateFlipCard(string rank, string suit, Color color, string caption, FlipCard card)
        {
            for (int i = 0; i < card.Properties.Length; i++)
            {
                card.Back.SetValue(card.Properties[i], card.HiddenBackValues[i]);
            }

            var back = new Border
            {
                Background = new SolidColorBrush(Palette[0]),
                BorderBrush = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(6),
                CornerRadius = new CornerRadius(12),
                Projection = card.Back,
                Child = new TextBlock
                {
                    Text = "OpenSilver",
                    Foreground = new SolidColorBrush(Colors.White),
                    FontSize = 20,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                },
            };

            var faceText = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
            faceText.Children.Add(new TextBlock { Text = rank, FontSize = 44, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(color), HorizontalAlignment = HorizontalAlignment.Center });
            faceText.Children.Add(new TextBlock { Text = suit, FontSize = 52, Foreground = new SolidColorBrush(color), HorizontalAlignment = HorizontalAlignment.Center });

            var front = new Border
            {
                Background = new SolidColorBrush(Colors.White),
                BorderBrush = new SolidColorBrush(color),
                BorderThickness = new Thickness(3),
                CornerRadius = new CornerRadius(12),
                Projection = card.Front,
                Child = faceText,
            };

            var faces = new Grid { Width = 150, Height = 210, Cursor = Cursors.Hand };
            faces.Children.Add(back);
            faces.Children.Add(front);
            faces.MouseLeftButtonUp += (s, e) => Flip(card);

            var root = new StackPanel { Margin = new Thickness(0, 0, 25, 0) };
            root.Children.Add(faces);
            root.Children.Add(new TextBlock { Text = caption, Foreground = new SolidColorBrush(Color.FromArgb(255, 0x55, 0x55, 0x55)), HorizontalAlignment = HorizontalAlignment.Center, Margin = new Thickness(0, 6, 0, 0) });
            return root;
        }

        private static void Flip(FlipCard card)
        {
            if (card.Animator.IsRunning)
            {
                return;
            }

            card.IsFlipped = !card.IsFlipped;

            // The visible face turns until it is seen edge-on (and disappears), then the other face
            // arrives from the opposite edge.
            PlaneProjection leaving = card.IsFlipped ? card.Front : card.Back;
            PlaneProjection arriving = card.IsFlipped ? card.Back : card.Front;
            double[] hiddenValues = card.IsFlipped ? card.HiddenFrontValues : card.HiddenBackValues;

            var values = new List<AnimatedValue>();
            for (int i = 0; i < card.Properties.Length; i++)
            {
                values.Add(new AnimatedValue(leaving, card.Properties[i], hiddenValues[i], 0, card.LeaveDuration, card.LeaveEasing));
                values.Add(new AnimatedValue(arriving, card.Properties[i], 0, card.LeaveDuration, card.ArriveDuration, card.ArriveEasing));
            }

            card.Animator.Run(values);
        }

        #endregion

        #region 3. Cover flow

        private void BuildCoverFlow()
        {
            for (int i = 0; i < 13; i++)
            {
                Color color = Palette[i % Palette.Length];
                var content = new StackPanel { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center };
                content.Children.Add(new TextBlock { Text = (i + 1).ToString(Invariant), Foreground = new SolidColorBrush(Colors.White), FontSize = 48, FontWeight = FontWeights.Bold, HorizontalAlignment = HorizontalAlignment.Center });
                content.Children.Add(new TextBlock { Text = "Album " + (i + 1).ToString(Invariant), Foreground = new SolidColorBrush(Colors.White), HorizontalAlignment = HorizontalAlignment.Center });

                var tile = new Border
                {
                    Width = CoverFlowTileSize,
                    Height = CoverFlowTileSize,
                    CornerRadius = new CornerRadius(4),
                    BorderBrush = new SolidColorBrush(Colors.White),
                    BorderThickness = new Thickness(2),
                    Background = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 1),
                        GradientStops =
                        {
                            new GradientStop { Color = color, Offset = 0 },
                            new GradientStop { Color = Colors.Black, Offset = 1.6 },
                        },
                    },
                    Projection = new PlaneProjection(),
                    Cursor = Cursors.Hand,
                    Child = content,
                };

                int index = i;
                tile.MouseLeftButtonUp += (s, e) => SelectCoverFlowTile(index);
                Canvas.SetTop(tile, (CoverFlowCanvas.Height - CoverFlowTileSize) / 2);

                _coverFlowTiles.Add(tile);
                CoverFlowCanvas.Children.Add(tile);
            }

            UpdateCoverFlow(false);
        }

        private void OnCoverFlowPreviousClick(object sender, RoutedEventArgs e)
        {
            SelectCoverFlowTile(_selectedCoverFlowIndex - 1);
        }

        private void OnCoverFlowNextClick(object sender, RoutedEventArgs e)
        {
            SelectCoverFlowTile(_selectedCoverFlowIndex + 1);
        }

        private void SelectCoverFlowTile(int index)
        {
            index = Math.Max(0, Math.Min(_coverFlowTiles.Count - 1, index));
            if (index != _selectedCoverFlowIndex)
            {
                _selectedCoverFlowIndex = index;
                UpdateCoverFlow(true);
            }
        }

        private void UpdateCoverFlow(bool animate)
        {
            double center = CoverFlowCanvas.Width / 2;
            var easing = new CubicEase { EasingMode = EasingMode.EaseOut };
            var values = new List<AnimatedValue>();

            for (int i = 0; i < _coverFlowTiles.Count; i++)
            {
                Border tile = _coverFlowTiles[i];
                int distance = i - _selectedCoverFlowIndex;
                int side = Math.Sign(distance);

                double x = distance == 0 ? center : center + side * (115 + (Math.Abs(distance) - 1) * 50);
                double left = x - CoverFlowTileSize / 2;
                double rotationY = -side * 60;
                double offsetZ = distance == 0 ? 80 : -150;

                Canvas.SetZIndex(tile, 100 - Math.Abs(distance));

                if (animate)
                {
                    values.Add(new AnimatedValue(tile, Canvas.LeftProperty, left, 0, 0.45, easing));
                    values.Add(new AnimatedValue(tile.Projection, PlaneProjection.RotationYProperty, rotationY, 0, 0.45, easing));
                    values.Add(new AnimatedValue(tile.Projection, PlaneProjection.GlobalOffsetZProperty, offsetZ, 0, 0.45, easing));
                }
                else
                {
                    Canvas.SetLeft(tile, left);
                    var projection = (PlaneProjection)tile.Projection;
                    projection.RotationY = rotationY;
                    projection.GlobalOffsetZ = offsetZ;
                }
            }

            if (animate)
            {
                _coverFlowAnimator.Run(values);
            }
        }

        #endregion

        #region 4. Animations

        private void OnStartAnimationsClick(object sender, RoutedEventArgs e)
        {
            ((Storyboard)Resources["CrawlStoryboard"]).Stop();
            UpdateCrawlAnimation();

            foreach (Storyboard storyboard in EnumerateStoryboards())
            {
                storyboard.Begin();
            }
        }

        private IEnumerable<Storyboard> EnumerateStoryboards()
        {
            yield return (Storyboard)Resources["SpinStoryboard"];
            yield return (Storyboard)Resources["DoorStoryboard"];
            yield return (Storyboard)Resources["ZoomStoryboard"];
            yield return (Storyboard)Resources["CrawlStoryboard"];
        }

        // The text starts with its top at the bottom of the viewport, and ends when its bottom
        // reaches the top, so that the viewport is never empty.
        private void UpdateCrawlAnimation()
        {
            const double speed = 40;

            double height = CrawlPanel.ActualHeight;
            double viewportHeight = ((FrameworkElement)CrawlPanel.Parent).ActualHeight;
            if (height == 0 || viewportHeight == 0)
            {
                return;
            }

            double from = GetCrawlPlaneDistance(viewportHeight - height / 2) + height / 2;
            double to = GetCrawlPlaneDistance(-height / 2) - height / 2;

            var animation = (DoubleAnimation)((Storyboard)Resources["CrawlStoryboard"]).Children[0];
            animation.From = from;
            animation.To = to;
            animation.Duration = TimeSpan.FromSeconds((from - to) / speed);
        }

        // Returns the distance, along the tilted plane from the center of the text, of the point
        // that appears at the vertical distance screenDistance from that center. The camera of
        // PlaneProjection is at distance 999.
        private double GetCrawlPlaneDistance(double screenDistance)
        {
            double angle = -CrawlProjection.RotationX * Math.PI / 180;
            return 999 * screenDistance / (999 * Math.Cos(angle) + screenDistance * Math.Sin(angle));
        }

        private void OnStopAnimationsClick(object sender, RoutedEventArgs e)
        {
            foreach (Storyboard storyboard in EnumerateStoryboards())
            {
                storyboard.Stop();
            }
        }

        #endregion

        #region 5. Matrix3DProjection and nested projections

        private void OnMatrixAngleChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateMatrixProjection();
        }

        private void UpdateMatrixProjection()
        {
            if (!_isInitialized)
            {
                return;
            }

            double angle = MatrixAngleSlider.Value;
            Matrix3D matrix = CreateMsdnStyleMatrix(MatrixTile.Width, MatrixTile.Height, angle);

            ((Matrix3DProjection)MatrixTile.Projection).ProjectionMatrix = matrix;
            MatrixAngleText.Text = "Angle = " + angle.ToString("0", Invariant) + "°";
            MatrixText.Text = FormatMatrix(matrix);
        }

        // Built like the Matrix3DProjection sample of the Silverlight documentation: the element is
        // centered at the origin, rotated, moved away from the camera, projected and mapped back to
        // its own pixel space. The distance is chosen so that the unrotated element keeps its size.
        private static Matrix3D CreateMsdnStyleMatrix(double width, double height, double rotationYDegrees)
        {
            double fieldOfViewY = Math.PI / 3;
            double distance = height / 2 / Math.Tan(fieldOfViewY / 2);
            double a = rotationYDegrees * Math.PI / 180;

            Matrix3D center = CreateTranslation(-width / 2, -height / 2, 0);
            var invertY = new Matrix3D(1, 0, 0, 0, 0, -1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
            var rotateY = new Matrix3D(Math.Cos(a), 0, -Math.Sin(a), 0, 0, 1, 0, 0, Math.Sin(a), 0, Math.Cos(a), 0, 0, 0, 0, 1);
            Matrix3D moveAway = CreateTranslation(0, 0, -distance);

            double yScale = 1 / Math.Tan(fieldOfViewY / 2);
            double xScale = yScale / (width / height);
            double near = 1;
            double far = 1000;
            var perspective = new Matrix3D(
                xScale, 0, 0, 0,
                0, yScale, 0, 0,
                0, 0, far / (near - far), -1,
                0, 0, near * far / (near - far), 0);
            var viewport = new Matrix3D(width / 2, 0, 0, 0, 0, -height / 2, 0, 0, 0, 0, 1, 0, width / 2, height / 2, 0, 1);

            return center * invertY * rotateY * moveAway * perspective * viewport;
        }

        private static Matrix3D CreateTranslation(double x, double y, double z)
        {
            return new Matrix3D(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, x, y, z, 1);
        }

        private void OnNestedButtonClick(object sender, RoutedEventArgs e)
        {
            _nestedClicks++;
            NestedButton.Content = "Click me (" + _nestedClicks.ToString(Invariant) + ")";
        }

        #endregion

        #region 6. Controls and popups in a projected plane

        private void OnProjectedPopupChanged(object sender, RoutedEventArgs e)
        {
            ProjectedPopup.IsOpen = ProjectedPopupCheckBox.IsChecked == true;
        }

        private void OnCloseProjectedPopupClick(object sender, RoutedEventArgs e)
        {
            ProjectedPopupCheckBox.IsChecked = false;
        }

        private void OnProjectedPopupDragDelta(object sender, DragDeltaEventArgs e)
        {
            ProjectedPopup.HorizontalOffset += e.HorizontalChange;
            ProjectedPopup.VerticalOffset += e.VerticalChange;
        }

        #endregion

        #region Helpers

        private sealed class ParameterSpec
        {
            public ParameterSpec(DependencyProperty dp, double minimum, double maximum, double defaultValue, string format)
            {
                Property = dp;
                Minimum = minimum;
                Maximum = maximum;
                DefaultValue = defaultValue;
                Format = format;
            }

            public DependencyProperty Property { get; private set; }
            public double Minimum { get; private set; }
            public double Maximum { get; private set; }
            public double DefaultValue { get; private set; }
            public string Format { get; private set; }
        }

        private sealed class ParameterSlider
        {
            public ParameterSpec Spec;
            public Slider Slider;
            public TextBlock Label;

            public void UpdateLabel()
            {
                Label.Text = Spec.Property.Name + " = " + Slider.Value.ToString(Spec.Format, Invariant);
            }
        }

        private sealed class Preset
        {
            public Preset(string name, params (DependencyProperty Property, double Value)[] values)
            {
                Name = name;
                Values = values;
            }

            public string Name { get; }
            public (DependencyProperty Property, double Value)[] Values { get; }
        }

        private sealed class AnimatedValue
        {
            public AnimatedValue(DependencyObject target, DependencyProperty property, double to, double beginTime, double duration, IEasingFunction easing)
            {
                Target = target;
                Property = property;
                To = to;
                BeginTime = beginTime;
                Duration = duration;
                Easing = easing;
            }

            public DependencyObject Target { get; private set; }
            public DependencyProperty Property { get; private set; }
            public double To { get; private set; }
            public double BeginTime { get; private set; }
            public double Duration { get; private set; }
            public IEasingFunction Easing { get; private set; }
        }

        // Runs one storyboard at a time. When it completes, the final values become the local values,
        // so that they can be changed again afterwards (for instance by dragging a slider). When it is
        // interrupted, the current values are kept, so that the next animation starts from them.
        private sealed class Animator
        {
            private Storyboard _storyboard;
            private List<AnimatedValue> _values;

            public bool IsRunning
            {
                get { return _storyboard != null; }
            }

            public void Run(List<AnimatedValue> values)
            {
                Cancel();

                var storyboard = new Storyboard();
                foreach (AnimatedValue value in values)
                {
                    var animation = new DoubleAnimation
                    {
                        To = value.To,
                        Duration = TimeSpan.FromSeconds(value.Duration),
                        BeginTime = TimeSpan.FromSeconds(value.BeginTime),
                        EasingFunction = value.Easing,
                    };
                    Storyboard.SetTarget(animation, value.Target);
                    Storyboard.SetTargetProperty(animation, new PropertyPath(value.Property));
                    storyboard.Children.Add(animation);
                }

                storyboard.Completed += (s, e) =>
                {
                    if (_storyboard == storyboard)
                    {
                        Finish();
                    }
                };

                _storyboard = storyboard;
                _values = values;
                storyboard.Begin();
            }

            public void Cancel()
            {
                if (_storyboard == null)
                {
                    return;
                }

                foreach (AnimatedValue value in _values)
                {
                    value.Target.SetValue(value.Property, value.Target.GetValue(value.Property));
                }

                Stop();
            }

            private void Finish()
            {
                foreach (AnimatedValue value in _values)
                {
                    value.Target.SetValue(value.Property, value.To);
                }

                Stop();
            }

            private void Stop()
            {
                Storyboard storyboard = _storyboard;
                _storyboard = null;
                _values = null;
                storyboard.Stop();
            }
        }

        private static string FormatMatrix(Matrix3D m)
        {
            double[] values =
            {
                m.M11, m.M12, m.M13, m.M14,
                m.M21, m.M22, m.M23, m.M24,
                m.M31, m.M32, m.M33, m.M34,
                m.OffsetX, m.OffsetY, m.OffsetZ, m.M44,
            };

            var text = new StringBuilder();
            for (int i = 0; i < 16; i++)
            {
                text.Append(values[i].ToString("0.000", Invariant).PadLeft(10));
                if (i % 4 == 3 && i < 15)
                {
                    text.Append('\n');
                }
            }
            return text.ToString();
        }

        private static string FormatPoint(Point p)
        {
            return p.X.ToString("0.0", Invariant) + ", " + p.Y.ToString("0.0", Invariant);
        }

        #endregion
    }
}
