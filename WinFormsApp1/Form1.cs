using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using ServerF;


namespace WinFormsApp1
{

    public partial class Form1 : Form
    {
        private Server server;
        public Form1()
        {
            InitializeComponent();
            server = new Server(this);
        }

        internal void updateLabels(Server.DroneData data)
        {
            label_Dbattery.Invoke((MethodInvoker)(() => label_Dbattery.Text = $"Battery: {data.Battery}%"));
            label_Dheight.Invoke((MethodInvoker)(() => label_Dheight.Text = $"Height: {data.Height} cm"));
            label_Dstatus.Invoke((MethodInvoker)(() => label_Dstatus.Text = "Status: Connected"));
        }
    }
}
