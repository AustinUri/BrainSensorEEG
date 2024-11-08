using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace Screen_Capture
{
    public partial class frmPrintScreen : Form
    {
        // Declare captureBitmap at the class level to make it accessible throughout the form.
        private Bitmap captureBitmap;

        public frmPrintScreen()
        {
            InitializeComponent();
        }

        private void frmPrintScreen_Load(object sender, EventArgs e)
        {
            // Additional initialization, if necessary.
        }

        private void butCapture_Click(object sender, EventArgs e)
        {
            // Define the area of the screen to capture
            Rectangle captureRectangle = new Rectangle(0, 700, 200, 800);

            // Capture the specified area of the screen
            captureBitmap = new Bitmap(captureRectangle.Width, captureRectangle.Height, PixelFormat.Format32bppArgb);
            using (Graphics captureGraphics = Graphics.FromImage(captureBitmap))
            {
                captureGraphics.CopyFromScreen(captureRectangle.Left, captureRectangle.Top, 0, 0, captureRectangle.Size);
            }

            // Save the captured image to a MemoryStream
            using (MemoryStream memoryStream = new MemoryStream())
            {
                captureBitmap.Save(memoryStream, ImageFormat.Png);
                memoryStream.Position = 0; // Rewind the stream

                // Save the MemoryStream to a file
                using (FileStream fileStream = File.Open("CapturedScreen.png", FileMode.Create))
                {
                    memoryStream.CopyTo(fileStream);
                }

                // Display the captured image in the PictureBox
                picScreen.Image = Image.FromStream(memoryStream);
            }
        }

        private void picScreen_MouseDown(object sender, MouseEventArgs e)
        {
            // Ensure captureBitmap is not null and contains an image.
            if (captureBitmap != null)
            {
                int x = e.X;
                int y = e.Y;
                txtX.Text = x.ToString();
                txtY.Text = y.ToString();

                // Ensure the coordinates are within the bounds of the image.
                if (x >= 0 && x < captureBitmap.Width && y >= 0 && y < captureBitmap.Height)
                {
                    Color middlePixelColor = captureBitmap.GetPixel(x, y);

                    int R = middlePixelColor.R;
                    int G = middlePixelColor.G;
                    int B = middlePixelColor.B;
                    txtR.Text = R.ToString();
                    txtG.Text = G.ToString();
                    txtB.Text = B.ToString();
                }
            }
        }
    }
}