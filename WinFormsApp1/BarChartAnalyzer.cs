using ServerF;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsApp1;


namespace NeuroTech.Analysis
{
    internal class BarChartAnalyzer
    {

        private ScreenshotTimer _screenshotTimer;
        private int _iterations;
        private Server _server;
        private Form1 Form1;


        public BarChartAnalyzer(int i,Server server,Form1 mainForm) {
            this._server = server;
            _iterations = i;
            Form1 = mainForm;
        }

        public async Task StartProcessAsync()
        {
            try
            {
                for (int i = 0; i < _iterations; i++)
                {
                    Console.WriteLine($"Starting process iteration {i + 1}/{_iterations}");

                    // Step 1: Take 5 screenshots
                    var screenshots = _screenshotTimer.TakeScreenshots(5);

                    // Step 2: Analyze screenshots to determine the command
                    DroneCommand command = AnalyzeScreenshots(screenshots);

                    // Step 3: Send the determined command to the Python server
                    _server.SendCommand(_server.GetStream(), command);

                    Console.WriteLine($"Command sent: {command}");

                    // Wait 5 seconds before the next iteration
                    await Task.Delay(5000);
                }

                Console.WriteLine("Process completed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during process: {ex.Message}");
            }
        }

        private DroneCommand AnalyzeScreenshots(List<Bitmap> screenshots)
        {
            // Placeholder logic for analyzing screenshots
            // Implement your actual analysis here
            Console.WriteLine("Analyzing screenshots...");
            return DroneCommand.MoveUp; // Example command
        }





    }


    public class ScreenshotTimer
    {
        private ScreenCapture _screenCapture;

        public ScreenshotTimer()
        {
            _screenCapture = new ScreenCapture();
        }

        public List<Bitmap> TakeScreenshots(int count)
        {
            List<Bitmap> screenshots = new List<Bitmap>();

            for (int i = 0; i < count; i++)
            {
                Bitmap screenshot = _screenCapture.CaptureScreen();
                screenshots.Add(screenshot);

                // Save the screenshot for debugging purposes
                screenshot.Save($"screenshot_{i}.png", ImageFormat.Png);

                // Wait 1 second
                Thread.Sleep(1000);
            }

            return screenshots;
        }
    }



    public class ScreenCapture
    {
        public Bitmap CaptureScreen()
        {
            Rectangle bounds = Screen.PrimaryScreen.Bounds;
            Bitmap screenshot = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);
            Graphics graphics = Graphics.FromImage(screenshot);
            graphics.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy);
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
