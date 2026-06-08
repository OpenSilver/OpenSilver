using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TestApplication.Tests
{
    public partial class MatrixAnimationUsingPathTest : Page, INotifyPropertyChanged
    {
        private readonly PathGeometry _pathGeometry = new();
        private readonly AnimationHelper _animations;

        private bool _isAdditive;
        private bool _doesRotateWithTangent;
        private bool _isAngleCumulative;
        private bool _isOffsetCumulative;
        private double _spiralWidth = 700;
        private double _spiralHeight = 350;

        public MatrixAnimationUsingPathTest()
        {
            InitializeComponent();

            _animations = new AnimationHelper(this);

            DataContext = this;

            Loaded += (_, _) =>
            {
                RebuildPath();
                _animations.Animate();
            };
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool DoesRotateWithTangent
        {
            get => _doesRotateWithTangent;
            set
            {
                if (SetProperty(ref _doesRotateWithTangent, value))
                {
                    _animations.UpdateDoesRotateWithTangent();
                }
            }
        }

        public bool IsAngleCumulative
        {
            get => _isAngleCumulative;
            set
            {
                if (SetProperty(ref _isAngleCumulative, value))
                {
                    _animations.UpdateIsAngleCumulative();
                }
            }
        }

        public bool IsOffsetCumulative
        {
            get => _isOffsetCumulative;
            set
            {
                if (SetProperty(ref _isOffsetCumulative, value))
                {
                    _animations.UpdateIsOffsetCumulative();
                }
            }
        }

        public bool IsAdditive
        {
            get => _isAdditive;
            set
            {
                if (SetProperty(ref _isAdditive, value))
                {
                    _animations.UpdateIsAdditive();
                }
            }
        }

        public double SpiralWidth
        {
            get => _spiralWidth;
            set
            {
                if (SetProperty(ref _spiralWidth, value))
                {
                    RebuildPath();
                }
            }
        }

        public double SpiralHeight
        {
            get => _spiralHeight;
            set
            {
                if (SetProperty(ref _spiralHeight, value))
                {
                    RebuildPath();
                }
            }
        }

        private void RebuildPath()
        {
            VisiblePath.Data = null;
            PathGlow.Data = null;

            _pathGeometry.Figures.Clear();

            var center = new Point(430, 270);

            var figure = new PathFigure
            {
                IsClosed = false,
                IsFilled = false,
                StartPoint = center,
            };

            const int points = 420;
            const double turns = 3.5;
            const double startWidth = 8;
            const double startHeight = 8;

            for (int i = 1; i <= points; i++)
            {
                double progress = (double)i / points;

                double width = startWidth + (SpiralWidth - startWidth) * progress;
                double height = startHeight + (SpiralHeight - startHeight) * progress;

                double angle = Math.PI + progress * turns * 2 * Math.PI;

                double x = center.X + Math.Cos(angle) * width / 2;
                double y = center.Y + Math.Sin(angle) * height / 2;

                figure.Segments.Add(new LineSegment(new Point(x, y), true));
            }

            _pathGeometry.Figures.Add(figure);

            VisiblePath.Data = _pathGeometry;
            PathGlow.Data = _pathGeometry;
        }

        private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
            {
                return false;
            }

            field = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnStartAnimationClick(object sender, RoutedEventArgs e)
        {
            _animations.Stop();
            _animations.Animate();
        }

        private void OnStopAnimationClick(object sender, RoutedEventArgs e)
        {
            _animations.Stop();
        }

        private sealed class AnimationHelper
        {
            private readonly MatrixAnimationUsingPathTest _owner;

            private bool _doesRotateWithTangent;
            private bool _isAngleCumulative;
            private bool _isOffsetCumulative;
            private bool _isAdditive;

            private Storyboard _storyboard;

            public AnimationHelper(MatrixAnimationUsingPathTest owner)
            {
                _owner = owner;
            }

            public void UpdateDoesRotateWithTangent()
            {
                if (_doesRotateWithTangent == _owner.DoesRotateWithTangent)
                {
                    return;
                }

                _doesRotateWithTangent = _owner.DoesRotateWithTangent;

                if (_storyboard is not null)
                {
                    ((MatrixAnimationUsingPath)_storyboard.Children[0]).DoesRotateWithTangent = _doesRotateWithTangent;
                }
            }

            public void UpdateIsAngleCumulative()
            {
                if (_isAngleCumulative == _owner.IsAngleCumulative)
                {
                    return;
                }

                _isAngleCumulative = _owner.IsAngleCumulative;

                if (_storyboard is not null)
                {
                    ((MatrixAnimationUsingPath)_storyboard.Children[0]).IsAngleCumulative = _isAngleCumulative;
                }
            }

            public void UpdateIsOffsetCumulative()
            {
                if (_isOffsetCumulative == _owner.IsOffsetCumulative)
                {
                    return;
                }

                _isOffsetCumulative = _owner.IsOffsetCumulative;

                if (_storyboard is not null)
                {
                    ((MatrixAnimationUsingPath)_storyboard.Children[0]).IsOffsetCumulative = _isOffsetCumulative;
                }
            }

            public void UpdateIsAdditive()
            {
                if (_isAdditive == _owner.IsAdditive)
                {
                    return;
                }

                _isAdditive = _owner.IsAdditive;

                if (_storyboard is not null)
                {
                    ((MatrixAnimationUsingPath)_storyboard.Children[0]).IsAdditive = _isAdditive;
                }
            }

            public void Animate()
            {
                Stop();

                var animation = CreatePathAnimation();

                Storyboard.SetTarget(animation, _owner.RocketTransform);
                Storyboard.SetTargetProperty(animation, new PropertyPath(MatrixTransform.MatrixProperty));

                _storyboard = new Storyboard();
                _storyboard.Children.Add(animation);
                _storyboard.Begin();
            }

            public void Stop()
            {
                _storyboard?.Remove();
                _storyboard = null;
            }

            private MatrixAnimationUsingPath CreatePathAnimation()
            {
                return new MatrixAnimationUsingPath
                {
                    PathGeometry = _owner._pathGeometry,
                    Duration = TimeSpan.FromSeconds(6),
                    RepeatBehavior = RepeatBehavior.Forever,
                    DoesRotateWithTangent = _doesRotateWithTangent,
                    IsAngleCumulative = _isAngleCumulative,
                    IsOffsetCumulative = _isOffsetCumulative,
                    IsAdditive = _isAdditive,
                };
            }
        }
    }
}
