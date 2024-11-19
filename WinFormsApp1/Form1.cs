using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NeuroTech.Analysis;
using Newtonsoft.Json.Linq;
using ServerF;


namespace WinFormsApp1
{


    public enum DroneCommand : byte
    {
        Takeoff = 0b000,
        Land = 0b001,
        TurnLeft = 0b010,
        TurnRight = 0b011,
        MoveLeft = 0b100,
        MoveRight = 0b101,
        MoveUp = 0b110,
        MoveDown = 0b111,
        Hover = 0b1000
    }

    public partial class Form1 : Form
    {
        private Server _server;


        public Form1()
        {
            try
            {
                InitializeComponent();
                this.StartPosition = FormStartPosition.Manual; // Set the start position manually
                this.Location = new Point(0, 0); // Position the form on the top left corner
                _server = new Server(this);
                Task.Run(() => _server.Start()); // Run server on a separate thread
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize Form1: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal void updateLabels(ServerF.DroneData data)
        {
            label_Dbattery.Invoke((MethodInvoker)(() => label_Dbattery.Text = $"Battery: {data.Battery}%"));
            label_Dheight.Invoke((MethodInvoker)(() => label_Dheight.Text = $"Height: {data.Height} cm"));
            label_Dstatus.Invoke((MethodInvoker)(() => label_Dstatus.Text = "Status: Connected"));
        }

        private void button_StartProccess_Click(object sender, EventArgs e)
        {
            try
            {

                for (int i = 0; i < 5; i++)
                {
                    ScreenshotTimer screenshotTimer = new ScreenshotTimer(5); // Initialize ScreenshotTimer
                    BarChartAnalyzer analyzer = new BarChartAnalyzer(screenshotTimer); // Pass it to BarChartAnalyzer

                    DroneCommand command = await analyzer.StartAnalysisAsync(); // Analyze and get the command

                    Console.WriteLine($"Drone Command: {command}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

        }
    }
}
