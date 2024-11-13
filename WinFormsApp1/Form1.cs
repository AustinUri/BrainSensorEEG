using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;


namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private TcpListener server;
        public Form1()
        {
            InitializeComponent();
        }
    }
}
