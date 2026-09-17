namespace AppUI
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Avalonia;
    using Avalonia.Controls;
    using Avalonia.Interactivity;
    using Avalonia.Media;
  

    using Source;
    using Source.Common;
    using Source.Model;
    using Avalonia.Controls.Shapes;
    public partial class MainWindow : Window
    {
        private readonly GameEngine _engine;

        private readonly List<(string ringName, int direction)> _solution;

        private State _currentState;

        private int _currentStep;

        private bool _isAnimating;

        private bool _isAutoPlaying;

        private readonly Ellipse[] _ballVisuals = new Ellipse[12];

        private const double BallRadius = 25;

        private const double RingRadius = 150;

        
        private readonly Dictionary<string, Point> _ringCenters =
            new()
            {
                ["Red"] = new Point(300, 360),
                ["Yellow"] = new Point(500, 360),
                ["White"] = new Point(700, 360)
            };

        
        private readonly Point[] _positions =
        [
            new Point(225, 489.9),  // 0
            new Point(150, 360),    // 1
            new Point(225, 230.1),  // 2

            new Point(400, 248.2),   // 3
            new Point(400, 471.8),   // 4

            new Point(575, 489.9),   // 5
            new Point(650, 360),     // 6

            new Point(600, 248.2),   // 7

            new Point(600, 471.8),   // 8
            new Point(775, 489.9),   // 9
            new Point(850, 360),     // 10
            new Point(800, 230.1)    // 11
        ];

       
        private readonly Dictionary<string, int[]> _ringPositions =
            new()
            {
                ["Red"] = [0, 1, 2, 3, 4],
                ["Yellow"] = [3, 4, 5, 6, 7],
                ["White"] = [7, 8, 9, 10, 11]
            };

        public MainWindow()
        {
            InitializeComponent();

            
            _engine = new GameEngine();

            
            _solution = _engine.Play();

           
            _currentState = _engine.Context.InputData.StartState;

            _currentStep = 0;

            
            DrawState(_currentState);

            UpdateStepText();
        }

       

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
            DrawRing(
                "Red",
                Brushes.IndianRed);

            DrawRing(
                "Yellow",
                Brushes.Gold);

            DrawRing(
                "White",
                Brushes.LightGray);
        }

        private void DrawRing(
            string name,
            IBrush brush)
        {
            Point center = _ringCenters[name];

            var ring = new Ellipse
            {
                Width = RingRadius * 2,
                Height = RingRadius * 2,

                Fill = Brushes.Transparent,

                Stroke = brush,
                StrokeThickness = 7
            };

            Canvas.SetLeft(
                ring,
                center.X - RingRadius);

            Canvas.SetTop(
                ring,
                center.Y - RingRadius);

            GameCanvas.Children.Add(ring);

           
            var text = new TextBlock
            {
                Text = name,
                FontSize = 18,
                FontWeight = FontWeight.Bold,
                Foreground = brush
            };

            Canvas.SetLeft(
                text,
                center.X - 30);

            Canvas.SetTop(
                text,
                center.Y - RingRadius - 35);

            GameCanvas.Children.Add(text);
        }

        private void DrawBall(
            int position,
            ColoredBall ball)
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

            Canvas.SetLeft(
                ellipse,
                point.X - BallRadius);

            Canvas.SetTop(
                ellipse,
                point.Y - BallRadius);

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

                VerticalAlignment =
                    Avalonia.Layout.VerticalAlignment.Center
            };

            Canvas.SetLeft(
                text,
                point.X - BallRadius);

            Canvas.SetTop(
                text,
                point.Y - BallRadius + 1);

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

                Canvas.SetLeft(
                    text,
                    point.X - 5);

                Canvas.SetTop(
                    text,
                    point.Y + BallRadius + 5);

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

        
        private async void NextButton_OnClick(
            object? sender,
            RoutedEventArgs e)
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

            await AnimateMove(
                move.ringName,
                move.direction);

            
            _currentStep++;

            UpdateStepText();
        }

        
        private async Task AnimateMove(
            string ringName,
            int direction)
        {
            _isAnimating = true;

            
            State oldState = _currentState;

           
            State newState = _engine.ApplyMove(
                oldState,
                ringName,
                direction);

            
            int[] ringPositions =
                _ringPositions[ringName];

            
            const int frames = 30;

            const int delay = 15;

           
            for (int frame = 0; frame <= frames; frame++)
            {
                double progress =
                    (double)frame / frames;

                double smoothProgress =
                    EaseInOut(progress);

                foreach (int sourcePosition in ringPositions)
                {
                    int sourceIndex =
                        Array.IndexOf(
                            ringPositions,
                            sourcePosition);

                    int targetIndex =
                        Mod(
                            sourceIndex + direction,
                            ringPositions.Length);

                    int targetPosition =
                        ringPositions[targetIndex];

                    Point point = GetArcPoint(
                        ringName,
                        sourcePosition,
                        targetPosition,
                        direction,
                        smoothProgress);

                    MoveBallVisual(
                        sourcePosition,
                        point);
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
            Point center =
                _ringCenters[ringName];

            Point source =
                _positions[sourcePosition];

            Point target =
                _positions[targetPosition];

            
            double startAngle =
                Math.Atan2(
                    source.Y - center.Y,
                    source.X - center.X);

            double targetAngle =
                Math.Atan2(
                    target.Y - center.Y,
                    target.X - center.X);

            
            double delta =
                targetAngle - startAngle;

            
            while (delta > Math.PI)
            {
                delta -= 2 * Math.PI;
            }

            while (delta < -Math.PI)
            {
                delta += 2 * Math.PI;
            }

           
            if (direction == 1 && delta < 0)
            {
                delta += 2 * Math.PI;
            }

            if (direction == -1 && delta > 0)
            {
                delta -= 2 * Math.PI;
            }

            double angle =
                startAngle + delta * progress;

            return new Point(
                center.X + RingRadius * Math.Cos(angle),
                center.Y + RingRadius * Math.Sin(angle));
        }

        private void MoveBallVisual(
            int sourcePosition,
            Point point)
        {
            
            Ellipse? ball =
                _ballVisuals[sourcePosition];

            if (ball == null)
            {
                return;
            }

            Canvas.SetLeft(
                ball,
                point.X - BallRadius);

            Canvas.SetTop(
                ball,
                point.Y - BallRadius);
        }

        private double EaseInOut(double value)
        {
            return value * value *
                   (3 - 2 * value);
        }

        private int Mod(int value, int modulus)
        {
            return ((value % modulus) + modulus) % modulus;
        }

       

        private async void PreviousButton_OnClick(
            object? sender,
            RoutedEventArgs e)
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

            
            _currentState =
                _engine.Context.InputData.StartState;

           
            for (int i = 0; i < _currentStep; i++)
            {
                var move = _solution[i];

                _currentState =
                    _engine.ApplyMove(
                        _currentState,
                        move.ringName,
                        move.direction);
            }

            DrawState(_currentState);

            UpdateStepText();

            await Task.CompletedTask;
        }

        

        private async void AutoButton_OnClick(
            object? sender,
            RoutedEventArgs e)
        {
            if (_isAutoPlaying)
            {
                _isAutoPlaying = false;

                return;
            }

            _isAutoPlaying = true;

            while (_isAutoPlaying &&
                   _currentStep < _solution.Count)
            {
                await NextStep();

                await Task.Delay(250);
            }

            _isAutoPlaying = false;
        }

      
        private void UpdateStepText()
        {
            if (_solution.Count == 0)
            {
                StepText.Text =
                    "Решение не найдено";

                return;
            }

            if (_currentStep >= _solution.Count)
            {
                StepText.Text =
                    $"✓ Цель достигнута!    " +
                    $"Шагов: {_solution.Count}";

                return;
            }

            var move =
                _solution[_currentStep];

            string direction =
                move.direction == 1
                    ? "→"
                    : "←";

            StepText.Text =
                $"Шаг {_currentStep} / {_solution.Count}    " +
                $"Следующий: {move.ringName} {direction}";
        }
    }
}