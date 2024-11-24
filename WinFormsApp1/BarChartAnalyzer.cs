using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsApp1;
using ServerF;

namespace NeuroTech.Analysis
{


    public class ScreenCapture
    {
        public Bitmap CaptureScreen()
        {
            try
            {
                Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
                Bitmap screenshot = new Bitmap(screenBounds.Width, screenBounds.Height);
                using (Graphics g = Graphics.FromImage(screenshot))
                {
                    g.CopyFromScreen(screenBounds.Location, Point.Empty, screenBounds.Size);
                }
                return screenshot;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error capturing screen: {ex.Message}");
                throw;
            }
        }
    }

    public class ScreenshotTimer
    {
        private int _interval; // Interval between screenshots in milliseconds
        private int _numberOfScreenshots;
        private ScreenCapture _screenCapture;
        private string _saveDirectory;

        public ScreenshotTimer(int numberOfScreenshots, int intervalInSeconds = 1)
        {
            _numberOfScreenshots = numberOfScreenshots;
            _interval = intervalInSeconds * 1000; // Convert seconds to milliseconds
            _screenCapture = new ScreenCapture();
            _saveDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        public List<Bitmap> CaptureScreenshots()
        {
            List<Bitmap> screenshots = new List<Bitmap>();

            try
            {
                for (int i = 0; i < _numberOfScreenshots; i++)
                {
                    Console.WriteLine($"Capturing screenshot {i + 1}...");
                    Bitmap screenshot = _screenCapture.CaptureScreen();
                    screenshots.Add(screenshot);
                    string filePath = Path.Combine(_saveDirectory, $"screenshot_{i + 1}.png");
                    screenshot.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

                    if (i < _numberOfScreenshots - 1) // Sleep only if more screenshots are to be taken
                    {
                        Thread.Sleep(_interval); // Wait for the specified interval
                    }
                }

                Console.WriteLine("All screenshots captured successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error capturing screenshots: {ex.Message}");
            }

            return screenshots;
        }
    }

    public class BarAnalysisResult
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int PositionX { get; set; }
        public Bitmap CroppedBarImage { get; set; }

        public string SavePath { get; set; }  // Add this

    }

    public class BarChartAnalyzer
    {
        private ScreenshotTimer _screenshotTimer;

        public BarChartAnalyzer(ScreenshotTimer screenshotTimer)
        {
            _screenshotTimer = screenshotTimer;
        }

        public async Task<DroneCommand> StartAnalysisAsync(Form1 mainfrom)
        {
            try
            {
                Console.WriteLine("Starting screenshot capture...");

                // Step 1: Use ScreenshotTimer to capture 5 screenshots
                List<Bitmap> screenshots = _screenshotTimer.CaptureScreenshots();

                if (screenshots.Count == 0)
                {
                    throw new Exception("No screenshots captured.");
                }

                Console.WriteLine("Screenshots captured successfully. Starting analysis...");

                // Step 2: Analyze each screenshot for bar graphs
                List<List<BarAnalysisResult>> allResults = new List<List<BarAnalysisResult>>();
                for (int i = 0; i < screenshots.Count; i++)
                {
                    Console.WriteLine($"Analyzing screenshot {i + 1}...");
                    List<BarAnalysisResult> results = AnalyzeChart(screenshots[i]);
                    allResults.Add(results);
                }

                Console.WriteLine("All screenshots analyzed. Comparing results...");

                // Step 3: Compare bar results across screenshots and decide a command
                DroneCommand command = CompareChartsAndDecideCommand(allResults);
                mainfrom.setCommandLabelText(command);

                Console.WriteLine($"Command decided: {command}");
                
                return command;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during analysis: {ex.Message}");
                return DroneCommand.Land; // Default fallback command
            }
        }

        private List<BarAnalysisResult> AnalyzeChart(Bitmap screenshot)
        {
            List<BarAnalysisResult> barResults = new List<BarAnalysisResult>();

            try
            {
                // Step 1: Define the Region of Interest (ROI)
                Rectangle roi = new Rectangle(0, screenshot.Height / 2, screenshot.Width, screenshot.Height / 2);
                Bitmap croppedImage = CropRectangleFromBitmap(screenshot, roi);

                // Step 2: Filter bars based on color
                Bitmap filteredImage = FilterBarColors(croppedImage);

                // Step 3: Detect and analyze bar rectangles
                List<Rectangle> barRectangles = FindBarRectangles(filteredImage);
                foreach (Rectangle rect in barRectangles)
                {
                    int barHeight = rect.Height;
                    int barWidth = rect.Width;
                    int barPositionX = rect.X;

                    Bitmap croppedBar = CropRectangleFromBitmap(croppedImage, rect);
                    string savePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"cropped_bar_{barPositionX}.png");
                    croppedBar.Save(savePath, ImageFormat.Png);
                    barResults.Add(new BarAnalysisResult
                    {
                        Height = barHeight,
                        Width = barWidth,
                        PositionX = barPositionX,
                        CroppedBarImage = croppedBar
                    });

                    Console.WriteLine($"Bar detected: Height={barHeight}, Width={barWidth}, X-Position={barPositionX}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error analyzing chart: {ex.Message}");
            }

            return barResults;
        }

        private DroneCommand CompareChartsAndDecideCommand(List<List<BarAnalysisResult>> allResults)
        {
            // Assume all screenshots have the same number of bars
            int numBars = allResults[0].Count;

            for (int barIndex = 0; barIndex < numBars; barIndex++)
            {
                // Collect heights of this bar across all screenshots
                List<int> barHeights = new List<int>();
                foreach (var result in allResults)
                {
                    barHeights.Add(result[barIndex].Height);
                }

                // Analyze trends for this bar
                string trend = AnalyzeTrends(barHeights);
                Console.WriteLine($"Bar {barIndex + 1}: {trend}");

                // Example logic to decide command based on trends
                if (trend == "Increasing")
                {
                    return DroneCommand.MoveUp;
                }
                else if (trend == "Decreasing")
                {
                    return DroneCommand.MoveDown;
                }
            }

            // Default fallback command
            return DroneCommand.Land;
        }

        private string AnalyzeTrends(List<int> barHeights)
        {
            bool increasing = true, decreasing = true;

            for (int i = 1; i < barHeights.Count; i++)
            {
                if (barHeights[i] < barHeights[i - 1]) increasing = false;
                if (barHeights[i] > barHeights[i - 1]) decreasing = false;
            }

            if (increasing) return "Increasing";
            if (decreasing) return "Decreasing";
            return "Stable";
        }

        private Bitmap FilterBarColors(Bitmap image)
        {
            Bitmap filteredImage = new Bitmap(image.Width, image.Height);

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);

                    // Detect black bars (low RGB values) and ignore white background
                    if (pixel.R < 50 && pixel.G < 50 && pixel.B < 50) // Black bars
                    {
                        filteredImage.SetPixel(x, y, Color.Black);
                    }
                    else
                    {
                        filteredImage.SetPixel(x, y, Color.White); // Set everything else to white
                    }
                }
            }

            return filteredImage;
        }

        private List<Rectangle> FindBarRectangles(Bitmap binaryImage)
        {
            List<Rectangle> barRectangles = new List<Rectangle>();

            int width = binaryImage.Width;
            int height = binaryImage.Height;

            bool inBar = false;
            int barStartX = 0;

            for (int x = 0; x < width; x++)
            {
                bool columnHasBlack = false;

                for (int y = 0; y < height; y++)
                {
                    if (binaryImage.GetPixel(x, y).ToArgb() == Color.Black.ToArgb())
                    {
                        columnHasBlack = true;
                        break;
                    }
                }

                if (columnHasBlack && !inBar)
                {
                    // Starting a new bar
                    inBar = true;
                    barStartX = x;
                }
                else if (!columnHasBlack && inBar)
                {
                    // End of the current bar
                    inBar = false;
                    int barWidth = x - barStartX;

                    // Calculate bar rectangle
                    Rectangle barRect = new Rectangle(barStartX, 0, barWidth, height);
                    barRectangles.Add(barRect);
                }
            }

            return barRectangles;
        }

        private Bitmap CropRectangleFromBitmap(Bitmap source, Rectangle rect)
        {
            Bitmap cropped = new Bitmap(rect.Width, rect.Height);
            using (Graphics g = Graphics.FromImage(cropped))
            {
                g.DrawImage(source, new Rectangle(0, 0, rect.Width, rect.Height), rect, GraphicsUnit.Pixel);
            }
            return cropped;
        }
    }
}
