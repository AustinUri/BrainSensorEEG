using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timer = System.Windows.Forms.Timer;


namespace NeuroTech.Analysis
{
    internal class BarChartAnalyzer
    {

        private ScreenshotTimer _ScreenshotTimer;
        private int _times;

        public BarChartAnalyzer(int i) {
        
               this._times = i;
        
        }


        
        



    }


    public class ScreenshotTimer
    {
        private Timer _timer;
        private int _count = 0;
        private ScreenCapture _screenCapture;
        int _times;

        public ScreenshotTimer(int times)
        {
            _screenCapture = new ScreenCapture();
            _timer = new Timer();
            _timer.Interval = 1000; // Interval set to 1000 milliseconds (1 second)
            _timer.Tick += Timer_Tick;
            _times = times;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_count < 5)
            {
                Bitmap screenshot = _screenCapture.CaptureScreen();
                screenshot.Save($"screenshot{this._count*this._times}.png", ImageFormat.Png); // Save screenshot to disk
                _count++;
            }
            else
            {
                this._timer.Stop(); // Stop the timer after 5 screenshots
                this._count = 0; // Reset the count if you need to restart the process later
            }
        }

        public void Start()
        {
            _timer.Start();
        }
    }

    public class ScreenCapture
    {
        public Bitmap CaptureScreen()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap screenshot = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
            Graphics graphic = Graphics.FromImage(screenshot);
            graphic.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);
            return screenshot;
        }


        public (int R, int G, int B) CapturePixel(Point position)
        {
            using (Bitmap bmp = new Bitmap(1, 1))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.CopyFromScreen(position.X, position.Y, 0, 0, new Size(1, 1));
                }
                Color color = bmp.GetPixel(0, 0);
                return (color.R, color.G, color.B); // Return RGB as a tuple
            }
        }
    }


}
