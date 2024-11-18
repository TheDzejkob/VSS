using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace SnakeGame
{
    public partial class MainWindow : Window
    {
        private const int GridSize = 20;
        private const int Width = 20;  
        private const int Height = 20;
        private List<Rectangle> snake;
        private Rectangle food;      
        private int snakeLength = 1;   
        private int score = 0;           
        private DispatcherTimer timer;   
        private Direction currentDirection = Direction.Right; 
        private Direction nextDirection = Direction.Right;   

        public MainWindow()
        {
            InitializeComponent();
            snake = new List<Rectangle>();


            snake.Add(CreateSnakePart(Width / 2, Height / 2));

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100) 
            };
            timer.Tick += Timer_Tick;
            timer.Start();

            GenerateFood();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            MoveSnake();

            if (IsGameOver())
            {
                timer.Stop();
                MessageBox.Show("Game Over! Final Score: " + score);
                Application.Current.Shutdown();
            }

            if (SnakeEatsFood())
            {
                GrowSnake();
                GenerateFood(); 
                score++;
            }
        }

        private void MoveSnake()
        {
            Point headPosition = GetSnakeHeadPosition();
            Point newHeadPosition = headPosition;

            switch (nextDirection)
            {
                case Direction.Up:
                    newHeadPosition.Y -= 1;
                    break;
                case Direction.Down:
                    newHeadPosition.Y += 1;
                    break;
                case Direction.Left:
                    newHeadPosition.X -= 1;
                    break;
                case Direction.Right:
                    newHeadPosition.X += 1;
                    break;
            }

            Rectangle newHead = CreateSnakePart((int)newHeadPosition.X, (int)newHeadPosition.Y);
            snake.Insert(0, newHead);

            if (snake.Count > snakeLength)
            {
                Rectangle lastPart = snake[snake.Count - 1];
                GameCanvas.Children.Remove(lastPart);
                snake.RemoveAt(snake.Count - 1);
            }

            currentDirection = nextDirection;
        }

        private bool SnakeEatsFood()
        {
            
            Point headPosition = GetSnakeHeadPosition();
            return headPosition.X == Canvas.GetLeft(food) / GridSize && headPosition.Y == Canvas.GetTop(food) / GridSize;
        }

        private void GrowSnake()
        {   //aksually pridá dílek
            snakeLength++; 
        }

        private void GenerateFood()
        {
            Random rand = new Random();
            int foodX = rand.Next(0, Width) * GridSize;
            int foodY = rand.Next(0, Height) * GridSize;

            food = new Rectangle
            {
                Width = GridSize,
                Height = GridSize,
                Fill = Brushes.Red
            };
            

            Canvas.SetLeft(food, foodX);
            Canvas.SetTop(food, foodY);
            GameCanvas.Children.Add(food);
        }

        private bool IsFoodOnSnake(int x, int y)
        {
            foreach (var part in snake)
            {
                if (Canvas.GetLeft(part) / GridSize == x && Canvas.GetTop(part) / GridSize == y)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsGameOver()
        {
            Point headPosition = GetSnakeHeadPosition();

            
            if (headPosition.X < 0 || headPosition.X >= Width || headPosition.Y < 0 || headPosition.Y >= Height)
                return true;

            
            for (int i = 1; i < snake.Count; i++)
            {
                if (Canvas.GetLeft(snake[i]) == headPosition.X && Canvas.GetTop(snake[i]) == headPosition.Y)
                    return true;
            }

            return false;
        }

        private Point GetSnakeHeadPosition()
        {
            return new Point(Canvas.GetLeft(snake[0]) / GridSize, Canvas.GetTop(snake[0]) / GridSize);
        }

        //vytvoření dalšího článku
        private Rectangle CreateSnakePart(int x, int y)
        {
            Rectangle snakePart = new Rectangle
            {
                Width = GridSize,
                Height = GridSize,
                Fill = Brushes.Green
            };
            Canvas.SetLeft(snakePart, x * GridSize);
            Canvas.SetTop(snakePart, y * GridSize);
            GameCanvas.Children.Add(snakePart);
            return snakePart;
        }

        // změna směru
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up && currentDirection != Direction.Down)
                nextDirection = Direction.Up;
            if (e.Key == Key.Down && currentDirection != Direction.Up)
                nextDirection = Direction.Down;
            if (e.Key == Key.Left && currentDirection != Direction.Right)
                nextDirection = Direction.Left;
            if (e.Key == Key.Right && currentDirection != Direction.Left)
                nextDirection = Direction.Right;
        }
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }
}
