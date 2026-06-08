using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace TestApplication.Tests
{
    public partial class DoubleAnimationUsingPathTest : Page, INotifyPropertyChanged
    {
        private readonly PathGeometry _pathGeometry = new();
        private readonly AnimationHelper _animations;

        private bool _animateX = true;
        private bool _animateY = true;
        private bool _animateAngle = true;
        private bool _isAdditive;
        private bool _isCumulative;
        private double _spiralWidth = 700;
        private double _spiralHeight = 350;

        public DoubleAnimationUsingPathTest()
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

        public bool AnimateX
        {
            get => _animateX;
            set
            {
                if (SetProperty(ref _animateX, value))
                {
                    _animations.AnimateX();
                }
            }
        }

        public bool AnimateY
        {
            get => _animateY;
            set
            {
                if (SetProperty(ref _animateY, value))
                {
                    _animations.AnimateY();
                }
            }
        }

        public bool AnimateAngle
        {
            get => _animateAngle;
            set
            {
                if (SetProperty(ref _animateAngle, value))
                {
                    _animations.AnimateAngle();
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
            var startPoint = new Point(center.X - SpiralWidth / 2, center.Y);

            var figure = new PathFigure
            {
                IsClosed = false,
                StartPoint = startPoint,
            };

            const int points = 420;
            const double turns = 3.5;
            const double endWidth = 8;
            const double endHeight = 8;

            for (int i = 1; i <= points; i++)
            {
                double progress = (double)i / points;

                double width = SpiralWidth + (endWidth - SpiralWidth) * progress;
                double height = SpiralHeight + (endHeight - SpiralHeight) * progress;

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
            private readonly DoubleAnimationUsingPathTest _owner;

            private bool _isCumulative;
            private bool _isAdditive;

            private Storyboard _xStoryboard;
            private Storyboard _yStoryboard;
            private Storyboard _angleStoryboard;

            public AnimationHelper(DoubleAnimationUsingPathTest owner)
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

                if (_xStoryboard is not null)
                {
                    ((DoubleAnimationUsingPath)_xStoryboard.Children[0]).IsCumulative = _isCumulative;
                }

                if (_yStoryboard is not null)
                {
                    ((DoubleAnimationUsingPath)_yStoryboard.Children[0]).IsCumulative = _isCumulative;
                }

                if (_angleStoryboard is not null)
                {
                    ((DoubleAnimationUsingPath)_angleStoryboard.Children[0]).IsCumulative = _isCumulative;
                }
            }

            public void UpdateIsAdditive()
            {
                if (_isAdditive == _owner.IsAdditive)
                {
                    return;
                }

                _isAdditive = _owner.IsAdditive;

                if (_xStoryboard is not null)
                {
                    ((DoubleAnimationUsingPath)_xStoryboard.Children[0]).IsAdditive = _isAdditive;
                }

                if (_yStoryboard is not null)
                {
                    ((DoubleAnimationUsingPath)_yStoryboard.Children[0]).IsAdditive = _isAdditive;
                }

                if (_angleStoryboard is not null)
                {
                    ((DoubleAnimationUsingPath)_angleStoryboard.Children[0]).IsAdditive = _isAdditive;
                }
            }

            public TimeSpan GetCurrentTime()
            {
                if (_xStoryboard is not null && _xStoryboard.GetCurrentState() == ClockState.Active)
                {
                    return _xStoryboard.GetCurrentTime();
                }

                if (_yStoryboard is not null && _yStoryboard.GetCurrentState() == ClockState.Active)
                {
                    return _yStoryboard.GetCurrentTime();
                }

                if (_angleStoryboard is not null && _angleStoryboard.GetCurrentState() == ClockState.Active)
                {
                    return _angleStoryboard.GetCurrentTime();
                }

                return TimeSpan.Zero;
            }

            public void Animate()
            {
                AnimateX();
                AnimateY();
                AnimateAngle();
            }

            public void Stop()
            {
                StopXAnimation();
                StopYAnimation();
                StopAngleAnimation();
            }

            public void AnimateX()
            {
                if (_owner.AnimateX)
                {
                    StartXAnimation();
                }
                else
                {
                    StopXAnimation();
                }
            }

            public void AnimateY()
            {
                if (_owner.AnimateY)
                {
                    StartYAnimation();
                }
                else
                {
                    StopYAnimation();
                }
            }

            public void AnimateAngle()
            {
                if (_owner.AnimateAngle)
                {
                    StartAngleAnimation();
                }
                else
                {
                    StopAngleAnimation();
                }
            }

            private void StartXAnimation()
            {
                StopXAnimation();

                TimeSpan currentTime = GetCurrentTime();

                var animation = CreatePathAnimation(PathAnimationSource.X);

                Storyboard.SetTarget(animation, _owner.RocketTranslate);
                Storyboard.SetTargetProperty(animation, new PropertyPath(TranslateTransform.XProperty));

                _xStoryboard = new Storyboard();
                _xStoryboard.Children.Add(animation);
                _xStoryboard.Begin();

                if (currentTime > TimeSpan.Zero)
                {
                    _xStoryboard.Seek(currentTime);
                }

                _owner.RocketPosition.X = -_owner.Rocket.Width / 2;
            }

            private void StopXAnimation()
            {
                _xStoryboard?.Remove();
                _xStoryboard = null;

                _owner.RocketPosition.X = _owner._pathGeometry.Figures[0].StartPoint.X - _owner.Rocket.Width / 2;
            }

            private void StartYAnimation()
            {
                StopYAnimation();

                TimeSpan currentTime = GetCurrentTime();

                var animation = CreatePathAnimation(PathAnimationSource.Y);

                Storyboard.SetTarget(animation, _owner.RocketTranslate);
                Storyboard.SetTargetProperty(animation, new PropertyPath(TranslateTransform.YProperty));

                _yStoryboard = new Storyboard();
                _yStoryboard.Children.Add(animation);
                _yStoryboard.Begin();

                if (currentTime > TimeSpan.Zero)
                {
                    _yStoryboard.Seek(currentTime);
                }

                _owner.RocketPosition.Y = -_owner.Rocket.Height / 2;
            }

            private void StopYAnimation()
            {
                _yStoryboard?.Remove();
                _yStoryboard = null;

                _owner.RocketPosition.Y = _owner._pathGeometry.Figures[0].StartPoint.Y - _owner.Rocket.Height / 2;
            }

            private void StartAngleAnimation()
            {
                StopAngleAnimation();

                TimeSpan currentTime = GetCurrentTime();

                var animation = CreatePathAnimation(PathAnimationSource.Angle);

                Storyboard.SetTarget(animation, _owner.RocketRotate);
                Storyboard.SetTargetProperty(animation, new PropertyPath(RotateTransform.AngleProperty));

                _angleStoryboard = new Storyboard();
                _angleStoryboard.Children.Add(animation);
                _angleStoryboard.Begin();

                if (currentTime > TimeSpan.Zero)
                {
                    _angleStoryboard.Seek(currentTime);
                }
            }

            private void StopAngleAnimation()
            {
                _angleStoryboard?.Remove();
                _angleStoryboard = null;
            }

            private DoubleAnimationUsingPath CreatePathAnimation(PathAnimationSource source)
            {
                return new DoubleAnimationUsingPath
                {
                    PathGeometry = _owner._pathGeometry,
                    Source = source,
                    Duration = TimeSpan.FromSeconds(6),
                    RepeatBehavior = RepeatBehavior.Forever,
                    IsCumulative = _isCumulative,
                    IsAdditive = _isAdditive,
                };
            }
        }
    }
}
