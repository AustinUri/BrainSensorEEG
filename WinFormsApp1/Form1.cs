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

    public partial class Form1 : Form
    {
        private Server _server;
        private BarChartAnalyzer _barChartAnalyzer;


        public Form1()
        {
            try
            {
                InitializeComponent();
                this.StartPosition = FormStartPosition.Manual; // Set the start position manually
                this.Location = new Point(0, 0); // Position the form on the top left corner
                _server = new Server(this);
                Task.Run(() => _server.Start()); // Run server on a separate thread
                this._barChartAnalyzer = new BarChartAnalyzer(5,_server);
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
            this._barChartAnalyzer.StartProcessAsync();
        }
    }
}
