using System;
using System.Media;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;


namespace KillKenny
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region ctor
        public MainWindow()
        {         
            InitializeComponent();
            SetUpCommands(); // initialize delegate commands
            SetUpLabel();
            kernelTimer.Tick += new EventHandler(KernelTimer_Tick); // initialize kernel dispatcher
            hitTimer.Tick += new EventHandler(hitTimer_Tick); // initialize kernel intersect dispatcher
            kernelTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            hitTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            hitTimer.Start();
            storyboardEllipse.Completed += StoryboardEllipse_Completed; // subscribe kenny animation
            storyFork.Completed += StoryFork_Completed; // subscribe fork animation
            
        }
     
        #endregion

        #region fields and prop
        static int resultGame = 5;

        DispatcherTimer kernelTimer = new DispatcherTimer();
        DispatcherTimer hitTimer = new DispatcherTimer();

        static int countHitToGoal = 0;

        double screenHeight = SystemParameters.FullPrimaryScreenHeight;

        double screenWidth = SystemParameters.FullPrimaryScreenWidth;

        Kernel ker = null;

        bool checkSound;

        private DelegateCommand fireCommand;
        private DelegateCommand speedUpCommand;
        private DelegateCommand speedDownCommand;
        private DelegateCommand leftRunCommand;
        private DelegateCommand rightRunCommand;
        private DelegateCommand upRunCommand;
        private DelegateCommand downRunCommand;
        private DelegateCommand soundCommand;
        private DelegateCommand restartCommand;

        public double ForkLeft
        {
            get { return (double)GetValue(ForkLeftProperty); }
            set { SetValue(ForkLeftProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForkLeft.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForkLeftProperty =
            DependencyProperty.Register("ForkLeft", typeof(double), typeof(Window), new PropertyMetadata(200.0));

        public double ForkBottom
        {
            get { return (double)GetValue(ForkBottomProperty); }
            set { SetValue(ForkBottomProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ForkBottom.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ForkBottomProperty =
            DependencyProperty.Register("ForkBottom", typeof(double), typeof(Window), new PropertyMetadata(-200.0));

        public double Start
        {
            get { return (double)GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Start.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartProperty =
            DependencyProperty.Register("Start", typeof(double), typeof(MainWindow), new PropertyMetadata(0.0));

        public double TopKenny
        {
            get { return (double)GetValue(TopKennyProperty); }
            set { SetValue(TopKennyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Start.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopKennyProperty =
            DependencyProperty.Register("TopKenny", typeof(double), typeof(MainWindow), new PropertyMetadata(-200.0));


        public DelegateCommand FireCommand
        {
            get { return fireCommand; }
        }
        public DelegateCommand LeftRunCommand
        {
            get { return leftRunCommand; }
        }
        public DelegateCommand RightRunCommand
        {
            get { return rightRunCommand; }
        }
        public DelegateCommand UpRunCommand
        {
            get { return upRunCommand; }
        }
        public DelegateCommand DownRunCommand
        {
            get { return downRunCommand; }
        }
        public DelegateCommand SpeedUpCommand
        {
            get { return speedUpCommand; }
        }

        public DelegateCommand SpeedDownCommand
        {
            get { return speedDownCommand; }
        }

        public DelegateCommand SoundCommand { get => soundCommand; set => soundCommand = value; }
        public DelegateCommand RestartCommand { get => restartCommand; set => restartCommand = value; }
        #endregion

        /// <summary>
        /// Сalculates the intersection with the fork
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hitTimer_Tick(object sender, EventArgs e)
        {
            bool res = false;
            GeneralTransform t1 = forkHit.TransformToVisual(this);
            GeneralTransform t2 = ellForFork.TransformToVisual(this);
            Rect r1 = t1.TransformBounds(new Rect() { X = 0, Y = 0, Width = forkHit.ActualWidth,
                Height = forkHit.ActualHeight });
            Rect r2 = t2.TransformBounds(new Rect() { X = 0, Y = 0, Width = ellForFork.ActualWidth,
                Height = ellForFork.ActualHeight });
            res = r1.IntersectsWith(r2);
            if (res) // if intersected
            {
                // set points in the game
                resultGame = resultGame - ((countHitToGoal <= 0) ? 1 : countHitToGoal - 1);
                lbHit.Foreground = new LinearGradientBrush(Colors.BlueViolet, Colors.Red, 50.0);
                lbHit.Content = "" + resultGame.ToString() + " Points";

            }
            label.Content = "";
            if (resultGame <= 0)
            {
                GameOver();
            }
        }

        /// <summary>
        /// Сalculates a random fork position on the field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StoryFork_Completed(object sender, EventArgs e)
        {
            Random r = new Random();
            double d = (double)r.Next(1, 500);
            ForkLeft = d;
            storyFork.Begin();
        }

        /// <summary>
        /// Sets the random position of the Kenny on the field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StoryboardEllipse_Completed(object sender, EventArgs e)
        {
            ImageBrush ib = new ImageBrush();
            ib.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Kenny.png",
                                             UriKind.RelativeOrAbsolute));
            ell.Fill = ib;
            ell.Effect = null;
            ell.Visibility = Visibility.Visible;
            Random random = new Random();
            int ran = random.Next(10, 600);
            Start = (double)ran;
            TopKenny = -100;
            storyboardEllipse.Begin(this);
        }

        /// <summary>
        /// Set 'label' to center screen
        /// </summary>
        private void SetUpLabel()
        {
            double screeHeight = SystemParameters.FullPrimaryScreenHeight; 
            double screeWidth = SystemParameters.FullPrimaryScreenWidth;
            Canvas.SetTop(label, (screenHeight) / 2 - 100);
            Canvas.SetLeft(label, (screenWidth) / 2 - 100);
        }

        /// <summary>
        /// Setup commands
        /// </summary>
        private void SetUpCommands()
        {
            DataContext = this;
            restartCommand = new DelegateCommand(GameRestart, () => true);
            fireCommand = new DelegateCommand(FireRunCommand_Execute, FireRunCommand_CanExecute);
            SoundCommand = new DelegateCommand(CheckBox_Execute, () => true);
            speedUpCommand = new DelegateCommand(SpeedUpRunCommand_Execute, SpeedUpRunCommand_CanExecute);
            speedDownCommand = new DelegateCommand(SpeedDownRunCommand_Execute, SpeedDownRunCommand_CanExecute);
            leftRunCommand = new DelegateCommand(LeftRunCommand_Executed, LeftRunCommand_CanExecute);
            rightRunCommand = new DelegateCommand(RightRunCommand_Executed, RightRunCommand_CanExecute);
            upRunCommand = new DelegateCommand(UpRunCommand_Executed, UpRunCommand_CanExecute);
            downRunCommand = new DelegateCommand(DownRunCommand_Executed, DownRunCommand_CanExecute);
            fireCommand.GestureKey = Key.F;
            leftRunCommand.GestureKey = Key.Left;
            rightRunCommand.GestureKey = Key.Right;
            upRunCommand.GestureKey = Key.Up;
            downRunCommand.GestureKey = Key.Down;
            speedUpCommand.GestureKey = Key.V;
            speedDownCommand.GestureKey = Key.C;            
            lbShow.Content = "F - FIRE | \xf177  RUN  \xf178 | V - POWER";
        }

        /// <summary>
        /// Calculates 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KernelTimer_Tick(object sender, EventArgs e)
        {
            if (Canvas.GetBottom(ell) < 0) // set kenny image in 'Ellipse'
            {
                ImageBrush ib = new ImageBrush();
                ib.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Images/Kenny.png",
                                                         UriKind.RelativeOrAbsolute));
                ell.Fill = ib;
                ell.Visibility = Visibility.Visible;
            }
            if (Kernel.V >= 10.0) // if power > 10 => reduce power cannon
            {
                Kernel.V = Kernel.V - 0.5;
                pb.Value = pb.Value - 0.5;
                lbShow.Content = "Cannon power: " + Kernel.V.ToString();
            }
            if (ker != null) // removes the kernel if it > one on the field
            {
                if (Canvas.GetLeft(ker) > this.ActualWidth && Canvas.GetTop(ker) > this.ActualHeight)
                {
                    for (int i = 0; i < MainCanvas.Children.Count; i++)
                    {
                        if (MainCanvas.Children[i] is Kernel)
                        {
                            MainCanvas.Children.RemoveAt(i);
                        }
                    }
                }
            }
            if (ell.Fill.ToString().Trim() == "#FFFF0000") // if kenny = 'red' (kenny is killed)
            {
                ell.Effect = null;
                ell.Visibility = Visibility.Hidden;
                storyboardEllipse.Stop();
            }
        }

        /// <summary>
        /// Check cannon power limit
        /// </summary>
        /// <returns></returns>
        private bool SpeedDownRunCommand_CanExecute()
        {
            if (Kernel.V >= 10.0) return true;
            else return false;
        }

        /// <summary>
        /// Reduces the power of the cannon (gun)
        /// </summary>
        private void SpeedDownRunCommand_Execute()
        {
            Kernel.V = Kernel.V - 0.1;
            pb.Value = pb.Value - 0.1;
            lbShow.Content = "Cannon power: " + Kernel.V.ToString();
        }

        /// <summary>
        /// Check cannon power limit
        /// </summary>
        /// <returns></returns>
        private bool SpeedUpRunCommand_CanExecute()
        {
            if (Kernel.V <= 30.0) return true;
            else return false;
        }

        /// <summary>
        /// Increases the power of the cannon
        /// </summary>
        private void SpeedUpRunCommand_Execute()
        {
            Kernel.V = Kernel.V + 0.1;
            pb.Value = pb.Value + 0.1;
        }

        /// <summary>
        /// Cannon fire
        /// </summary>
        private void FireRunCommand_Execute()
        {
            for (int i = 0; i < MainCanvas.Children.Count; i++) // removes all the kernels on the field
            {
                if (MainCanvas.Children[i] is Kernel)
                {
                    MainCanvas.Children.RemoveAt(i);
                }
            }

            kernelTimer.Start(); // start timer
            Vector offset = VisualTreeHelper.GetOffset(gun);
            double x = offset.X + 100;
            double y = offset.Y + 85;
            ker = new Kernel(); // create kernel
            ker.Agr = gun.AngleCannon;
            ker.X = x;
            ker.Y = y;
            ker.Intersect += Ker_Intersect; // enables event (kernel intersecting)
            ker.MoveKernel += Ker_MoveKernel; // enables event (kernel positions)
            Canvas.SetTop(ker, y);
            Canvas.SetLeft(ker, x);
            MainCanvas.Children.Add(ker); // add kernel to the canvas
            ker.Start(); // enable dispatcher (calculate kernel path)
            lbShow.Content = " x: " + x + " y: " + y;
            try
            {
                SoundPlayer sp = new SoundPlayer(); // enable the sound of a shot
                sp.SoundLocation = Environment.CurrentDirectory + @"\Media\gun8.wav";
                sp.Load();
                sp.Play();
            }
            catch (Exception)
            {
                lbShow.Content = "Sound not found";
            }

        }

        /// <summary>
        /// Set behavior when hit by a target
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ker_Intersect(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            bool result = false;
            if (ker != null && Canvas.GetLeft(ker) < this.ActualWidth && Canvas.GetTop(ker) < this.ActualHeight)
            {
                Kernel k = (Kernel)sender;
                GeneralTransform t1 = ker.TransformToVisual(this);
                GeneralTransform t2 = ell.TransformToVisual(this);
                Rect r1 = t1.TransformBounds(new Rect() { X = 0, Y = 0, Width = ker.ActualWidth,
                    Height = ker.ActualHeight });
                Rect r2 = t2.TransformBounds(new Rect() { X = 0, Y = 0, Width = ell.ActualWidth,
                    Height = ell.ActualHeight });
                result = r1.IntersectsWith(r2);
                if (result) // if hit to goal
                {
                    ell.Fill = new SolidColorBrush(Colors.Red);
                    BlurEffect b = new BlurEffect();
                    b.Radius = 150;
                    ell.Effect = b;
                    label.Content = "\uf113";
                    resultGame = resultGame + (countHitToGoal + 1);
                    lbHit.Foreground = new LinearGradientBrush(Colors.BlueViolet, Colors.LightSeaGreen, 50.0);
                    lbHit.Content = "" + resultGame.ToString() + " Points";
                    // enable sound
                    try
                    {
                        SoundPlayer sp = new SoundPlayer();
                        sp.SoundLocation = Environment.CurrentDirectory + @"\Media\crash.wav";
                        sp.Load();
                        sp.Play();
                    }
                    catch (Exception)
                    {
                        label.Content = "not found sound";
                    }
                }
            }
        }

        /// <summary>
        /// Сapture the position of the mouse on the canvas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ker_MoveKernel(object sender, PositionEventArgs e)
        {
            Point position = Mouse.GetPosition(MainCanvas);
            //lb.Content = "X: " + position.X + "\n" + "Y: " + position.Y;
        }

        private bool FireRunCommand_CanExecute()
        {
            return true;
        }

        private void UpRunCommand_Executed()
        {
            --gun.AngleCannon;
        }

        /// <summary>
        /// Sets the gun angle limit
        /// </summary>
        /// <returns></returns>
        private bool UpRunCommand_CanExecute()
        {
            if (gun.AngleCannon >= -55) return true;
            else return false;
        }

        /// <summary>
        /// Increases the angle of the gun
        /// </summary>
        private void DownRunCommand_Executed()
        {
            ++gun.AngleCannon;
        }

        /// <summary>
        /// Sets the gun angle limit
        /// </summary>
        /// <returns></returns>
        private bool DownRunCommand_CanExecute()
        {
            if (gun.AngleCannon <= 30) return true;
            else return false;
        }

        /// <summary>
        /// Move to the right
        /// </summary>
        private void RightRunCommand_Executed()
        {
            RunCartman();
            double x = Canvas.GetLeft(gun);
            double xx = Canvas.GetLeft(ellForFork);
            Canvas.SetLeft(gun, ++x);
            Canvas.SetLeft(ellForFork, ++xx);
        }

        /// <summary>
        /// Cartman`s annimation
        /// </summary>
        private void RunCartman()
        {
            Storyboard storyboard = new Storyboard();
            DoubleAnimation riseAnimation = new DoubleAnimation();
            riseAnimation.From = 60.206;
            riseAnimation.To = 65.206;
            riseAnimation.Duration = TimeSpan.FromSeconds(0.3);
            Storyboard.SetTarget(riseAnimation, gun);
            Storyboard.SetTargetProperty(riseAnimation, new PropertyPath("CartmanRun"));
            storyboard.Children.Add(riseAnimation);
            storyboard.Begin(this);
        }

        /// <summary>
        /// Limit of the movement to the right
        /// </summary>
        /// <returns></returns>
        private bool RightRunCommand_CanExecute()
        {
            if (Canvas.GetLeft(gun) < 400) return true;
            else return false;
        }

        /// <summary>
        /// Limit of the movement to the left
        /// </summary>
        /// <returns></returns>
        private bool LeftRunCommand_CanExecute()
        {
            RunCartman();
            if (Canvas.GetLeft(gun) > 0) return true;
            else return false;
        }

        /// <summary>
        /// Move to the left
        /// </summary>
        private void LeftRunCommand_Executed()
        {
            double x = Canvas.GetLeft(gun);
            double xx = Canvas.GetLeft(ellForFork);
            Canvas.SetLeft(gun, --x);
            Canvas.SetLeft(ellForFork, --xx);
        }

        /// <summary>
        /// Show rules of the game
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btnkeys_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String FormTitle = String.Format("Run - right and left arrows."+
                                                 "\nCannon - up and down arrows."+
                                                 "\nFire - F."+ 
                                                 "\nCannon power - V, C.\n");

                MessageBox.Show(FormTitle, "Keys:");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        /// <summary>
        /// Show game version
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnProgram_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String appName = Assembly.GetEntryAssembly().GetName().Name;
                String version = Assembly.GetEntryAssembly().GetName().Version.ToString();
                String FormTitle = String.Format("Program name: {0}\nVersion: {1} ",
                                                 appName,
                                                 version);
                MessageBox.Show(FormTitle, "About program:");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Enable/disable sound
        /// </summary>
        private void CheckBox_Execute()
        {
            try
            {
                if (checkSound)
                {
                    myMediaElement.Pause();
                    checkSound = false;
                    return;
                }
                myMediaElement.Play();
                checkSound = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
        }

        /// <summary>
        /// Game over
        /// </summary>
        private void GameOver()
        {
            gameOverlb.Content = "GAME \n OVER";          
        }

        /// <summary>
        /// Game restart
        /// </summary>
        private void GameRestart()
        {
            gameOverlb.Content = String.Empty;
            resultGame = 5;
            countHitToGoal = 0;
            lbHit.Content = "5 Points";
        }
    }
}