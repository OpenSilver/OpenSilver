using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TestApplication.Tests
{
    public partial class PointAnimationUsingPathTest : Page, INotifyPropertyChanged
    {
        private readonly PathGeometry _pathGeometry = new();
        private readonly AnimationHelper _animations;

        private bool _isAdditive;
        private bool _isCumulative;
        private double _waveWidth = 700;
        private double _waveHeight = 350;

        public PointAnimationUsingPathTest()
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

        public bool IsCumulative
        {
            get => _isCumulative;
            set
            {
                if (SetProperty(ref _isCumulative, value))
                {
                    _animations.UpdateIsCumulative();
                }
            }
        }

        public double WaveWidth
        {
            get => _waveWidth;
            set
            {
                if (SetProperty(ref _waveWidth, value))
                {
                    RebuildPath();
                }
            }
        }

        public double WaveHeight
        {
            get => _waveHeight;
            set
            {
                if (SetProperty(ref _waveHeight, value))
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

            const int points = 420;
            const double waves = 3.0;

            var startPoint = new Point(80, 270);

            var figure = new PathFigure
            {
                IsClosed = false,
                StartPoint = startPoint,
            };

            for (int i = 1; i <= points; i++)
            {
                double progress = (double)i / points;

                double x = startPoint.X + WaveWidth * progress;
                double y = startPoint.Y + Math.Sin(progress * waves * 2 * Math.PI) * WaveHeight / 2;

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
            private readonly PointAnimationUsingPathTest _owner;

            private bool _isCumulative;
            private bool _isAdditive;

            private Storyboard _storyboard;

            public AnimationHelper(PointAnimationUsingPathTest owner)
            {
                _owner = owner;
            }

            public void UpdateIsCumulative()
            {
                if (_isCumulative == _owner.IsCumulative)
                {
                    return;
                }

                _isCumulative = _owner.IsCumulative;

                if (_storyboard is not null)
                {
                    ((PointAnimationUsingPath)_storyboard.Children[0]).IsCumulative = _isCumulative;
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
                    ((PointAnimationUsingPath)_storyboard.Children[0]).IsAdditive = _isAdditive;
                }
            }

            public void Animate()
            {
                Stop();

                var animation = CreatePathAnimation();

                Storyboard.SetTarget(animation, _owner.Dot);
                Storyboard.SetTargetProperty(animation, new PropertyPath(EllipseGeometry.CenterProperty));

                _storyboard = new Storyboard();
                _storyboard.Children.Add(animation);
                _storyboard.Begin();
            }

            public void Stop()
            {
                _storyboard?.Remove();
                _storyboard = null;
            }

            private PointAnimationUsingPath CreatePathAnimation()
            {
                return new PointAnimationUsingPath
                {
                    PathGeometry = _owner._pathGeometry,
                    Duration = TimeSpan.FromSeconds(6),
                    RepeatBehavior = RepeatBehavior.Forever,
                    IsCumulative = _isCumulative,
                    IsAdditive = _isAdditive,
                };
            }
        }
    }
}
