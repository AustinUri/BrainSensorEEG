using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WinFormsApp1;


namespace ServerF
{


    public enum DroneMessageCode : byte
    {
        Connected = 0b0001,
        Disconnected = 0b0010,
        LowBattery = 0b0100,
        ConnectionError = 0b0111,
        DataError = 0b1000
    }

    public class Server
    {
        private readonly int port = 5000;
        private TcpListener listener;
        private Form1 form1;


        public class DroneData
        {
            public int? Battery { get; set; }
            public int? Height { get; set; }
            public bool Connected { get; set; }
        }

        public Server(Form form1)
        {
            listener = new TcpListener(IPAddress.Loopback, port);
            this.form1 = form1; 
        }

        public void Start()
        {
            listener.Start();
            Console.WriteLine($"Server started on port {port}. Waiting for connection...");

            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Client connected!");

            NetworkStream stream = client.GetStream();
            ReceiveMessage(stream);

            client.Close();
            listener.Stop();
        }

        private void ReceiveMessage(NetworkStream stream)
        {
            try
            {
                // 1. Read the message code (1 byte)
                byte[] msgCodeBuffer = new byte[1];
                stream.Read(msgCodeBuffer, 0, 1);
                byte msgCode = msgCodeBuffer[0];
                DroneMessageCode code = (DroneMessageCode)msgCode;
                Console.WriteLine($"Received message code: {code}");

                // 2. Read the message length (4 bytes)
                byte[] lengthBuffer = new byte[4];
                stream.Read(lengthBuffer, 0, 4);
                int msgLength = BitConverter.ToInt32(lengthBuffer, 0);
                Console.WriteLine($"Message length: {msgLength}");

                // 3. Read the JSON payload
                byte[] jsonBuffer = new byte[msgLength];
                stream.Read(jsonBuffer, 0, msgLength);
                string jsonData = Encoding.UTF8.GetString(jsonBuffer);
                Console.WriteLine($"Received JSON: {jsonData}");

                // 4. Deserialize the JSON payload
                DroneData data = JsonConvert.DeserializeObject<DroneData>(jsonData);
                Console.WriteLine($"Battery: {data.Battery}, Height: {data.Height}, Connected: {data.Connected}");



                switch (code)
                {
                    case DroneMessageCode.Connected:
                        form1.updateLabels(data);  // Call updateLabels on MainForm
                        break;

                    case DroneMessageCode.Disconnected:
                        form1.ShowAlert("Connection lost. Please look for the drone.", "Drone Disconnected");
                        break;

                    case DroneMessageCode.LowBattery:
                        form1.ShowAlert("Low battery! Please look for the drone in the air and step away until it lands safely.", "Low Battery Warning");
                        break;

                    case DroneMessageCode.ConnectionError:
                        form1.ShowAlert("Could not establish connection with the drone.", "Connection Error");
                        break;

                    case DroneMessageCode.DataError:
                        form1.ShowAlert("Data error. Something is wrong. Please be cautious and check the drone.", "Data Error");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving message: {ex.Message}");
            }
        }
    }
}