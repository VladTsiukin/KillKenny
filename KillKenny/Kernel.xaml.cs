using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;


namespace KillKenny
{   
    /// <summary>
    /// Interaction logic for Karnel.xaml
    /// </summary>
    public partial class Kernel : UserControl
    {

        #region ctor
        public Kernel()
        {
            InitializeComponent();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(TimeSpan.TicksPerMillisecond * 15);
            kernel.MouseMove += Kernel_MouseMove;
        }
        #endregion

        #region fields and properties
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public event EventHandler<PositionEventArgs> MoveKernel;

        public static RoutedEvent IntersectEvent;

        // time
        private double t = 0;

        // speed
        public static double V = 10;

        private static bool check = true;

        public double X
        {
            get { return (double)GetValue(XProperty); }
            set { SetValue(XProperty, value); }
        }

        // Using a DependencyProperty as the backing store for X.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("X", typeof(double), typeof(Kernel),
                new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnIntersect)));

        public double Y
        {
            get { return (double)GetValue(YProperty); }
            set { SetValue(YProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Y.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("Y", typeof(double), typeof(Kernel), new PropertyMetadata(0.0));
        
        public double Agr
        {
            get { return (double)GetValue(AgrProperty); }
            set { SetValue(AgrProperty, value); }
        }
        
        // Using a DependencyProperty as the backing store for Agr.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AgrProperty =
            DependencyProperty.Register("Agr", typeof(double), typeof(Kernel), new PropertyMetadata(0.0));
        #endregion

        /// <summary>
        /// Event intersect with kernel
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> Intersect
        {
            add { AddHandler(IntersectEvent, value); }
            remove { RemoveHandler(IntersectEvent, value); }
        }

        /// <summary>
        /// Event handling mouse position
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Kernel_MouseMove(object sender, MouseEventArgs e)
        {
            double x = e.GetPosition(this).X;
            double y = e.GetPosition(this).Y;
            this.OnMoveKernel(x, y);
        }

        /// <summary>
        /// Event handling position kernel on canvas
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected virtual void OnMoveKernel(double x, double y)
        {
            if (MoveKernel != null)
            {
                MoveKernel.Invoke(this, new PositionEventArgs(x, y));
            }
        }

        /// <summary>
        /// Event handling intersect with kernel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnIntersect(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            Kernel k = (Kernel)sender;
            double c = (double)e.NewValue;
            double o = (double)e.OldValue;

            if (check)
            {
                IntersectEvent = EventManager.RegisterRoutedEvent("Intersect", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<double>), typeof(Kernel));
                check = false;
            }

            RoutedPropertyChangedEventArgs<double> args = new RoutedPropertyChangedEventArgs<double>(o, c);
            args.RoutedEvent = Kernel.IntersectEvent;
            k.RaiseEvent(args);
        }

        /// <summary>
        /// Calculate path kernel (the flight body thrown at an angle to the horizontal)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            Canvas.SetTop(this, Y);
            Canvas.SetLeft(this, X);
            X = X + V * t * Math.Cos(this.AngleGunRadian);
            Y = Y + V * t * Math.Sin(this.AngleGunRadian) + 1.0 / 2 * 9.80665 * Math.Pow(t, 2);
            t = t + 0.02;

            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// Calculate cannon angle 
        /// </summary>
        private double AngleGunRadian
        {
            get { return this.Agr / 180.0 * Math.PI; }
        }

        /// <summary>
        /// Start dispatcher timer
        /// </summary>
        public void Start()
        {
            t = 0;
            dispatcherTimer.Start();          
        }
    }
}