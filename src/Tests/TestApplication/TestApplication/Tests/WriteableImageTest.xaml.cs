using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace TestApplication.Tests
{
    public partial class WriteableImageTest : Page
    {
        private const int PixelWidth = 800;
        private const int PixelHeight = 600;
        private const int StripeA = unchecked((int)0xFF2A2018);
        private const int StripeB = unchecked((int)0xFF3A2C22);
        private const int WaveColor = unchecked((int)0xFFD2C14A);
        private const int WaveColor2 = unchecked((int)0xFFC45AC8);
        private const int ScanColor = unchecked((int)0xFF88F0FF);
        private const int NeedleColor = unchecked((int)0xFFE8F0FF);

        private static readonly Ball[] Balls =
        [
            new(210, 130, 0.10, 0.40, 28, unchecked((int)0xFF3DCC55)),
            new(160, 220, 1.20, 0.15, 22, unchecked((int)0xFF2A2AFF)),
            new(280, 90, 0.55, 1.80, 18, unchecked((int)0xFF33CCFF)),
            new(120, 170, 2.10, 0.90, 32, unchecked((int)0xFFFF66AA)),
            new(190, 250, 0.80, 2.40, 16, unchecked((int)0xFF66FF88)),
            new(240, 140, 1.70, 1.10, 24, unchecked((int)0xFF4488FF)),
            new(150, 200, 2.80, 0.60, 20, unchecked((int)0xFFFFAA44)),
        ];

        private readonly DispatcherTimer _timer = new();
        private readonly DateTime _origin = DateTime.UtcNow;

        private bool _running;
        private bool _useDisplayRefresh = true;
        private double _refreshIntervalMs;
        private DateTime _fpsWindowStart;
        private int _frames;

        public WriteableImageTest()
        {
            InitializeComponent();

            Board.Resize(PixelWidth, PixelHeight);
            _timer.Tick += (_, _) => OnFrame();
            Loaded += (_, _) => StartLoop();
            Unloaded += (_, _) => StopLoop();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            StopLoop();
            base.OnNavigatedFrom(e);
        }

        private void RefreshRateBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (RefreshRateBox.SelectedIndex)
            {
                case 0:
                    _useDisplayRefresh = false;
                    _refreshIntervalMs = 1000.0 / 8;
                    break;
                case 1:
                    _useDisplayRefresh = false;
                    _refreshIntervalMs = 1000.0 / 30;
                    break;
                case 2:
                    _useDisplayRefresh = false;
                    _refreshIntervalMs = 1000.0 / 60;
                    break;
                case 3:
                    _useDisplayRefresh = false;
                    _refreshIntervalMs = 1000.0 / 120;
                    break;
                default:
                    _useDisplayRefresh = true;
                    _refreshIntervalMs = 0;
                    break;
            }

            if (_running)
            {
                StartLoop();
            }
        }

        private void StartLoop()
        {
            StopLoop();
            _running = true;
            _fpsWindowStart = DateTime.UtcNow;
            _frames = 0;
            Draw();

            if (_useDisplayRefresh)
            {
                CompositionTarget.Rendering += OnRendering;
            }
            else
            {
                _timer.Interval = TimeSpan.FromMilliseconds(_refreshIntervalMs);
                _timer.Start();
            }
        }

        private void StopLoop()
        {
            _running = false;
            _timer.Stop();
            CompositionTarget.Rendering -= OnRendering;
        }

        private void OnRendering(object sender, EventArgs e) => OnFrame();

        private void OnFrame()
        {
            Draw();
            UpdateFps(DateTime.UtcNow);
        }

        private void UpdateFps(DateTime now)
        {
            _frames++;
            double windowMs = (now - _fpsWindowStart).TotalMilliseconds;
            if (windowMs < 500)
            {
                return;
            }

            double fps = _frames * 1000.0 / windowMs;
            string target = _useDisplayRefresh ? "display" : $"{1000.0 / _refreshIntervalMs:0} Hz";
            FpsText.Text = $"{fps:0} fps ({target})";
            _frames = 0;
            _fpsWindowStart = now;
        }

        private void Draw()
        {
            int[] pixels = Board.Pixels;
            double t = (DateTime.UtcNow - _origin).TotalSeconds;

            DrawStripes(pixels, t);
            DrawWaves(pixels, t);
            DrawScan(pixels, t);
            DrawNeedle(pixels, t);
            DrawBalls(pixels, t);

            Board.Invalidate();
        }

        private static void DrawStripes(int[] pixels, double t)
        {
            int shift = (int)(t * 180.0);
            for (int y = 0; y < PixelHeight; y++)
            {
                int row = y * PixelWidth;
                for (int x = 0; x < PixelWidth; x++)
                {
                    pixels[row + x] = (((x + y + shift) >> 4) & 1) == 0 ? StripeA : StripeB;
                }
            }
        }

        private static void DrawWaves(int[] pixels, double t)
        {
            for (int x = 0; x < PixelWidth; x++)
            {
                int y1 = (int)(PixelHeight * 0.42 + Math.Sin(x * 0.018 + t * 3.2) * 70);
                int y2 = (int)(PixelHeight * 0.58 + Math.Sin(x * 0.027 - t * 2.4) * 55);
                FillRect(pixels, x, y1 - 2, 1, 5, WaveColor);
                FillRect(pixels, x, y2 - 2, 1, 5, WaveColor2);
            }
        }

        private static void DrawScan(int[] pixels, double t)
        {
            int x = (int)PingPong(t * 220, PixelWidth - 6);
            FillRect(pixels, x, 0, 5, PixelHeight, ScanColor);
        }

        private static void DrawNeedle(int[] pixels, double t)
        {
            double angle = t * 1.7;
            int cx = PixelWidth / 2;
            int cy = PixelHeight / 2;
            int x1 = cx + (int)(Math.Cos(angle) * 220);
            int y1 = cy + (int)(Math.Sin(angle) * 220);
            DrawLine(pixels, cx, cy, x1, y1, NeedleColor);
            FillCircle(pixels, cx, cy, 6, NeedleColor);
        }

        private static void DrawBalls(int[] pixels, double t)
        {
            for (int i = 0; i < Balls.Length; i++)
            {
                Ball ball = Balls[i];
                int x = ball.Radius + (int)PingPong((t + ball.PhaseX) * ball.SpeedX, PixelWidth - 2 * ball.Radius);
                int y = ball.Radius + (int)PingPong((t + ball.PhaseY) * ball.SpeedY, PixelHeight - 2 * ball.Radius);
                FillCircle(pixels, x, y, ball.Radius, ball.Color);
                FillCircle(pixels, x - ball.Radius / 3, y - ball.Radius / 3, Math.Max(3, ball.Radius / 4), NeedleColor);
            }
        }

        private static double PingPong(double time, double length)
        {
            if (length <= 0)
            {
                return 0;
            }

            double cycle = length * 2;
            double m = time % cycle;
            if (m < 0)
            {
                m += cycle;
            }

            return m < length ? m : cycle - m;
        }

        private static void FillCircle(int[] pixels, int cx, int cy, int radius, int color)
        {
            int r2 = radius * radius;
            int y0 = Math.Max(0, cy - radius);
            int y1 = Math.Min(PixelHeight - 1, cy + radius);
            int x0 = Math.Max(0, cx - radius);
            int x1 = Math.Min(PixelWidth - 1, cx + radius);

            for (int y = y0; y <= y1; y++)
            {
                int dy = y - cy;
                int row = y * PixelWidth;
                for (int x = x0; x <= x1; x++)
                {
                    int dx = x - cx;
                    if (dx * dx + dy * dy <= r2)
                    {
                        pixels[row + x] = color;
                    }
                }
            }
        }

        private static void FillRect(int[] pixels, int x, int y, int width, int height, int color)
        {
            int x1 = Math.Min(PixelWidth, x + width);
            int y1 = Math.Min(PixelHeight, y + height);
            int x0 = Math.Max(0, x);
            int y0 = Math.Max(0, y);

            for (int py = y0; py < y1; py++)
            {
                int row = py * PixelWidth;
                for (int px = x0; px < x1; px++)
                {
                    pixels[row + px] = color;
                }
            }
        }

        private static void DrawLine(int[] pixels, int x0, int y0, int x1, int y1, int color)
        {
            int dx = Math.Abs(x1 - x0);
            int sx = x0 < x1 ? 1 : -1;
            int dy = -Math.Abs(y1 - y0);
            int sy = y0 < y1 ? 1 : -1;
            int err = dx + dy;

            while (true)
            {
                SetPixel(pixels, x0, y0, color);
                SetPixel(pixels, x0 + 1, y0, color);
                SetPixel(pixels, x0, y0 + 1, color);
                if (x0 == x1 && y0 == y1)
                {
                    break;
                }

                int e2 = 2 * err;
                if (e2 >= dy)
                {
                    err += dy;
                    x0 += sx;
                }

                if (e2 <= dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }
        }

        private static void SetPixel(int[] pixels, int x, int y, int color)
        {
            if ((uint)x < PixelWidth && (uint)y < PixelHeight)
            {
                pixels[y * PixelWidth + x] = color;
            }
        }

        private readonly struct Ball(double speedX, double speedY, double phaseX, double phaseY, int radius, int color)
        {
            public double SpeedX { get; } = speedX;
            public double SpeedY { get; } = speedY;
            public double PhaseX { get; } = phaseX;
            public double PhaseY { get; } = phaseY;
            public int Radius { get; } = radius;
            public int Color { get; } = color;
        }
    }
}
