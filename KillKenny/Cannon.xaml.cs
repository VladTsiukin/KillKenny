using System.Windows;
using System.Windows.Controls;


namespace KillKenny
{
    /// <summary>
    /// Interaction logic for Cannon.xaml
    /// </summary>
    public partial class Cannon : UserControl
    {
        public Cannon()
        {
            InitializeComponent();
        }

        /// <summary>
        /// For cannon angle
        /// </summary>
        public int AngleCannon
        {
            get { return (int)GetValue(AngleCannonProperty); }
            set { SetValue(AngleCannonProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AngleCannon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AngleCannonProperty =
            DependencyProperty.Register("AngleCannon", typeof(int), typeof(Cannon), new PropertyMetadata(0));

        /// <summary>
        /// For animation cartman
        /// </summary>
        public double CartmanRun
        {
            get { return (double)GetValue(CartmanRunProperty); }
            set { SetValue(CartmanRunProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CartmanRun.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CartmanRunProperty =
            DependencyProperty.Register("CartmanRun", typeof(double), typeof(Cannon), new PropertyMetadata(60.206));

    }
}