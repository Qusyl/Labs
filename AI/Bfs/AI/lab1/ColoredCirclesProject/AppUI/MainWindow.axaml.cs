namespace AppUI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Controls.Shapes;
    using Avalonia.Interactivity;
    using Avalonia.Media;

    using Source;
    using Source.Analyse;
    using Source.Common;
    using Source.Model;

    public partial class MainWindow : Window
    {
        private const double BallRadius = 25;

        private readonly GameEngine _engine;

        private readonly List<(string ringName, int direction)> _solution;

        // Топология: имя кольца -> позиции в порядке обхода (из Inputes json)
        private readonly Dictionary<string, int[]> _ringPositions;

        // Данные для рисования: центр, радиус, цвет, направление
        private readonly Dictionary<string, RingView> _rings;

        // Координаты позиций, считаются автоматически
        private readonly Point[] _positions;

        private readonly Ellipse?[] _ballVisuals;

        private State _currentState;

        private int _currentStep;

        private bool _isAnimating;

        private bool _isAutoPlaying;

        public MainWindow()
        {
            InitializeComponent();

            _engine = new GameEngine();

            _solution = _engine.Play();

            var sb = new StringBuilder();

            foreach (var sol in _solution)
            {
                string rotate = sol.direction == 1 ? "право ->" : " <- лево";
                sb.Append($"{sol.ringName} повернулась на {rotate}\n");
            }

            File.WriteAllText("founded_path.txt", sb.ToString());
            AnalyzeSaver.SaveAnalyse(_engine.GetReport());

            _currentState = _engine.Context.InputData.StartState;

         
            var rings = _engine.Context.Geometry.Rings;

            _ringPositions = rings.ToDictionary(r => r.Name, r => r.Positions);

            _rings = rings.ToDictionary(
                r => r.Name,
                r => new RingView(
                    new Point(r.X, r.Y),
                    r.Radius,
                    new SolidColorBrush(Color.Parse(r.Color)),
                    r.Clockwise));

            _positions = BoardGeometry.Build(
                _ringPositions,
                _rings,
                _currentState.Balls.Length);

            _ballVisuals = new Ellipse?[_currentState.Balls.Length];

            _currentStep = 0;

            DrawState(_currentState);

            UpdateStepText();
        }

        // ---------- Отрисовка ----------

        private void DrawState(State state)
        {
            GameCanvas.Children.Clear();

            DrawRings();

            for (int i = 0; i < state.Balls.Length; i++)
            {
                DrawBall(i, state.Balls[i]);
            }

            DrawPositionNumbers();
        }

        private void DrawRings()
        {
            foreach (var (name, ring) in _rings)
            {
                DrawRing(name, ring);
            }
        }

        private void DrawRing(string name, RingView ring)
        {
            var ellipse = new Ellipse
            {
                Width = ring.Radius * 2,
                Height = ring.Radius * 2,

                Fill = Brushes.Transparent,

                Stroke = ring.Brush,
                StrokeThickness = 7
            };

            Canvas.SetLeft(ellipse, ring.Center.X - ring.Radius);
            Canvas.SetTop(ellipse, ring.Center.Y - ring.Radius);

            GameCanvas.Children.Add(ellipse);

            var text = new TextBlock
            {
                Text = name,
                FontSize = 18,
                FontWeight = FontWeight.Bold,
                Foreground = ring.Brush
            };

            Canvas.SetLeft(text, ring.Center.X - 30);
            Canvas.SetTop(text, ring.Center.Y - ring.Radius - 35);

            GameCanvas.Children.Add(text);
        }

        private void DrawBall(int position, ColoredBall ball)
        {
            Point point = _positions[position];

            var ellipse = new Ellipse
            {
                Width = BallRadius * 2,
                Height = BallRadius * 2,

                Fill = GetBallBrush(ball.Color),

                Stroke = Brushes.Black,
                StrokeThickness = 3
            };

            Canvas.SetLeft(ellipse, point.X - BallRadius);
            Canvas.SetTop(ellipse, point.Y - BallRadius);

            GameCanvas.Children.Add(ellipse);

            _ballVisuals[position] = ellipse;

            var text = new TextBlock
            {
                Text = GetColorSymbol(ball.Color),

                Width = BallRadius * 2,
                Height = BallRadius * 2,

                FontSize = 16,
                FontWeight = FontWeight.Bold,

                Foreground = GetTextBrush(ball.Color),

                TextAlignment = TextAlignment.Center,

                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };

            Canvas.SetLeft(text, point.X - BallRadius);
            Canvas.SetTop(text, point.Y - BallRadius + 1);

            GameCanvas.Children.Add(text);
        }

        private void DrawPositionNumbers()
        {
            for (int i = 0; i < _positions.Length; i++)
            {
                Point point = _positions[i];

                var text = new TextBlock
                {
                    Text = i.ToString(),
                    FontSize = 12,
                    Foreground = Brushes.Gray
                };

                Canvas.SetLeft(text, point.X - 5);
                Canvas.SetTop(text, point.Y + BallRadius + 5);

                GameCanvas.Children.Add(text);
            }
        }

        private IBrush GetBallBrush(AppColors color)
        {
            return color switch
            {
                AppColors.Red => Brushes.Red,
                AppColors.Green => Brushes.Green,
                AppColors.Blue => Brushes.Blue,
                AppColors.Purple => Brushes.MediumPurple,
                AppColors.Yellow => Brushes.Gold,
                AppColors.Cyan => Brushes.Cyan,
                AppColors.White => Brushes.White,

                _ => Brushes.Gray
            };
        }

        private IBrush GetTextBrush(AppColors color)
        {
            return color switch
            {
                AppColors.White => Brushes.Black,
                AppColors.Yellow => Brushes.Black,
                AppColors.Cyan => Brushes.Black,

                _ => Brushes.White
            };
        }

        private string GetColorSymbol(AppColors color)
        {
            return color switch
            {
                AppColors.Purple => "P",
                AppColors.Cyan => "C",
                AppColors.Red => "R",
                AppColors.Green => "G",
                AppColors.Blue => "B",
                AppColors.Yellow => "Y",
                AppColors.White => "W",

                _ => "?"
            };
        }

        // ---------- Шаги и анимация ----------

        private async void NextButton_OnClick(object? sender, RoutedEventArgs e)
        {
            await NextStep();
        }

        private async Task NextStep()
        {
            if (_isAnimating)
            {
                return;
            }

            if (_currentStep >= _solution.Count)
            {
                return;
            }

            var move = _solution[_currentStep];

            await AnimateMove(move.ringName, move.direction);

            _currentStep++;

            UpdateStepText();
        }

        private async Task AnimateMove(string ringName, int direction)
        {
            _isAnimating = true;

            State oldState = _currentState;

            State newState = _engine.ApplyMove(oldState, ringName, direction);

            int[] ringPositions = _ringPositions[ringName];

            const int frames = 30;
            const int delay = 1;

            for (int frame = 0; frame <= frames; frame++)
            {
                double progress = (double)frame / frames;

                double smoothProgress = EaseInOut(progress);

                for (int i = 0; i < ringPositions.Length; i++)
                {
                    int sourcePosition = ringPositions[i];

                    int targetPosition =
                        ringPositions[Mod(i + direction, ringPositions.Length)];

                    Point point = GetArcPoint(
                        ringName,
                        sourcePosition,
                        targetPosition,
                        direction,
                        smoothProgress);

                    MoveBallVisual(sourcePosition, point);
                }

                await Task.Delay(delay);
            }

            _currentState = newState;

            DrawState(_currentState);

            _isAnimating = false;
        }

        private Point GetArcPoint(
            string ringName,
            int sourcePosition,
            int targetPosition,
            int direction,
            double progress)
        {
            RingView ring = _rings[ringName];

            double a0 = BoardGeometry.Angle(ring.Center, _positions[sourcePosition]);
            double a1 = BoardGeometry.Angle(ring.Center, _positions[targetPosition]);

            // direction = +1: к следующей позиции в списке кольца
            double sign = (ring.Clockwise ? 1 : -1) * direction;

            double delta = sign * BoardGeometry.Norm(sign * (a1 - a0));

            double angle = a0 + delta * progress;

            return new Point(
                ring.Center.X + ring.Radius * Math.Cos(angle),
                ring.Center.Y + ring.Radius * Math.Sin(angle));
        }

        private void MoveBallVisual(int sourcePosition, Point point)
        {
            Ellipse? ball = _ballVisuals[sourcePosition];

            if (ball == null)
            {
                return;
            }

            Canvas.SetLeft(ball, point.X - BallRadius);
            Canvas.SetTop(ball, point.Y - BallRadius);
        }

        private double EaseInOut(double value)
        {
            return value * value * (3 - 2 * value);
        }

        private int Mod(int value, int modulus)
        {
            return ((value % modulus) + modulus) % modulus;
        }

        // ---------- Кнопки ----------

        private async void PreviousButton_OnClick(object? sender, RoutedEventArgs e)
        {
            if (_isAnimating)
            {
                return;
            }

            if (_currentStep <= 0)
            {
                return;
            }

            _currentStep--;

            _currentState = _engine.Context.InputData.StartState;

            for (int i = 0; i < _currentStep; i++)
            {
                var move = _solution[i];

                _currentState = _engine.ApplyMove(
                    _currentState,
                    move.ringName,
                    move.direction);
            }

            DrawState(_currentState);

            UpdateStepText();

            await Task.CompletedTask;
        }

        private async void AutoButton_OnClick(object? sender, RoutedEventArgs e)
        {
            if (_isAutoPlaying)
            {
                _isAutoPlaying = false;

                return;
            }

            _isAutoPlaying = true;

            while (_isAutoPlaying && _currentStep < _solution.Count)
            {
                await NextStep();
            }

            _isAutoPlaying = false;
        }

        private void UpdateStepText()
        {
            if (_solution.Count == 0)
            {
                StepText.Text = "Решение не найдено";

                return;
            }

            if (_currentStep >= _solution.Count)
            {
                StepText.Text =
                    $"✓ Цель достигнута!    Шагов: {_solution.Count}";

                return;
            }

            var move = _solution[_currentStep];

            string direction = move.direction == 1 ? "→" : "←";

            StepText.Text =
                $"Шаг {_currentStep} / {_solution.Count}    " +
                $"Следующий: {move.ringName} {direction}";
        }
    }
}