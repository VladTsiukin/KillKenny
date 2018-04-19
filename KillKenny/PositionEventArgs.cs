using System;


namespace KillKenny
{
    /// <summary>
    /// For calculating position kernel on canvas
    /// </summary>
    public class PositionEventArgs : EventArgs
    {
        public PositionEventArgs(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
        public double X { get; set; }
        public double Y { get; set; }
    }
}