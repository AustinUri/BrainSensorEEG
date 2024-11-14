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
        public BarChartAnalyzer() {
        
               
        
        
        }


        
        



    }


    public class ScreenshotTimer
    {
        private Timer _timer;
        private int _count = 0;
        private ScreenCapture _screenCapture;

        public ScreenshotTimer()
        {
            _screenCapture = new ScreenCapture();
            _timer = new Timer();
            _timer.Interval = 1000; // Interval set to 1000 milliseconds (1 second)
            _timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_count < 5)
            {
                Bitmap screenshot = _screenCapture.CaptureScreen();
                screenshot.Save($"screenshot{this._count}.png", ImageFormat.Png); // Save screenshot to disk
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
    }


}
