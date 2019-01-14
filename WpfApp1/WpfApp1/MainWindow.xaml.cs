using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Random random = new Random();
        DispatcherTimer enemyTimer = new DispatcherTimer();
        DispatcherTimer targetTimer = new DispatcherTimer();
        
        private void  startButton_Click(object sender,RoutedEventArgs e)
        {
            StartGame();
            human.IsHitTestVisible = true;
            humanCaptured = false;
            progressBar.Value = 0;
            startButton.Visibility = Visibility.Collapsed;
            playArea.Children.Add(target);
            playArea.Children.Add(human);
            enemyTimer.Start();
            targetTimer.Start();

        }

        private void StartGame()
        {
            throw new NotImplementedException();
        }
        private  void human_MouseDown(object sender,MouseButtonEventArgs e)
        {
            if (enemyTimer.IsEnabled)
            {
                humanCaptured = true;
                human.IsHitTestVisible =false;
            }
        }

        private void Rectangle_MouseEnter(object sender, MouseEventArgs e)
        {
            if (targetTimer.IsEnabled && humanCaptured)
            {
                progressBar.Value = 0;
                Canvas.SetLeft(target, random.Next(100, (int)playArena.ActualWidth - 100));
                Canvas.SetTop(target, random.Next(100, (int)playArena.ActualHeight - 100));
                Canvas.SetLeft(human, random.Next(100, (int)playArena.ActualWidth - 100));
                Canvas.SetTop(human, random.Next(100, (int)playArena.ActualHeight - 100));
                humanCaptured = false;
                human.IsHitTestVisible = true;


            }
        }

        private void playArena_MouseMove(object sender, MouseEventArgs e)
        {
            if (humanCaptured)
            {
                Point pointerPosition = e.GetPosition(null);
                Point relativePosition = grid.TransformToVisual(playArena).Transform(pointerPosition);
                if ((Math.Abs(relativePosition.X - Canvas.GetLeft(human)) > human.ActualWidth * 3)
                || (Math.Abs(relativePosition.Y - Canvas.GetTop(human)) > human.ActualHeight * 3))
                {
                    humanCaptured = false;
                    human.IsHitTestVisible = true;
                }
                else
                {
                    Canvas.SetLeft(human, relativePosition.X - human.ActualWidth / 2);
                    Canvas.SetTop(human, relativePosition.Y - human.ActualHeight / 2);
                }
            }
        }

        private void playArena_MouseLeave(object sender, MouseEventArgs e)
        {
            if (humanCaptured)
            {
                EndTheGame();
            }
        }

        ContentControl enemy = new ContentControl();
        private void AddEnemy()
        {
            enemy.Template = Resources["EnemyTemplate"] as ControlTemplate;

            AnimateEnemy(enemy, 0, playArea.ActualWidth - 100, "(Canvas.Left)");
            AnimateEnemy(enemy, random.Next((int)playArea.ActualHeight - 100),
                random.Next((int)playArea.ActualHeight - 100), "(Canvas.Top)");
            playArea.Children.Add(enemy);
            enemy.MouseEnter += Enemy_MouseEnter;
        }

        private void Enemy_MouseEnter(object sender, MouseEventArgs e)
        {
            if (humanCaptured)
            {
                EndTheGame();
            }
        }

        private void AnimateEnemy(ContentControl enemy, double from, double to, string propertyToAnimate)
        {
            Storyboard storyboard = new Storyboard(){ AutoReverse = true,RepeatBehavior = RepeatBehavior.Forever};
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(random.Next(4, 6))),
           };
            Storyboard.SetTarget(animation, enemy);
            Storyboard.SetTargetProperty(animation, new PropertyPath(propertyToAnimate));
            storyboard.Children.Add(animation);
            storyboard.Begin();

           
        }

        public MainWindow()
        {
            
            InitializeComponent();
            enemyTimer.Tick += enemyTimer_Tick;
            enemyTimer.Interval = TimeSpan.FromSeconds(2);

            targetTimer.Tick += targetTimer_Tick;
            targetTimer.Interval = TimeSpan.FromSeconds(.1);

        }

         void targetTimer_Tick(object sender, EventArgs e)
        {
            ProgressBar.Value += 1;
            if (ProgressBar.Value >= ProgressBar.Maximum)
                EndTheGame();
        }

        private void EndTheGame()
        {
            if (!playArea.Children.Contains(gameOverText))
            {
                enemyTimer.Stop();
                targetTimer.Stop();
                humanCatptured = false;
                StartButton.Visibility = Visibility.Visible;
                palyArea.Children.Add(EndTheGame);
            }
        }

        void enemyTimer_Tick(object sender, EventArgs e)
        {
            AddEnemy();
        }
    }

  
}
