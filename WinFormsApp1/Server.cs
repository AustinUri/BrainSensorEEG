using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WinFormsApp1;
using NeuroTech.Analysis;

namespace ServerF
{
    // Enum for message codes from the drone
    public enum DroneMessageCode : byte
    {
        Connected = 1,
        Disconnected = 2,
        LowBattery = 3,
        ConnectionError = 4,
        DataError = 5
    }

    // Enum for commands to control the drone
    public enum DroneCommand : byte
    {
        Takeoff = 0b000,    // 0 in decimal
        Land = 0b001,       // 1 in decimal
        TurnLeft = 0b010,   // 2 in decimal
        TurnRight = 0b011,  // 3 in decimal
        MoveLeft = 0b100,   // 4 in decimal
        MoveRight = 0b101,  // 5 in decimal
        MoveUp = 0b110,     // 6 in decimal
        MoveDown = 0b111    // 7 in decimal
    }

    public class DroneData
    {
        public int? Battery { get; set; }
        public int? Height { get; set; }
        public bool Connected { get; set; }
    }

    public class Server
    {
        private readonly int port = 5000;
        private TcpListener listener;
        private Form1 form1;

        // Constructor to initialize server with a reference to the main form
        public Server(Form1 mainForm)
        {
            listener = new TcpListener(IPAddress.Loopback, port);
            this.form1 = mainForm;
        }

        // Start the server and handle incoming client connections
        public void Start()
        {
            listener.Start();
            Console.WriteLine($"Server started on port {port}. Waiting for connection...");

            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Client connected!");

            NetworkStream stream = client.GetStream();
            ReceiveAndProcessMessages(stream);

            client.Close();
            listener.Stop();
        }



        // Main loop for handling message processing
        private void ReceiveAndProcessMessages(NetworkStream stream)
        {
            try
            {
                // Process commands in batches (5 captures per command)
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {


                        // TODO: Implement logic for averaging and sending to Python
                        // TODO: Receive command back from Python based on analysis
                        System.Threading.Thread.Sleep(10000);

                    }

                    // After processing 5 captures, receive and handle commands
                    ProcessReceivedMessage(stream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during message processing: {ex.Message}");
            }
        }

        // Process a single message from the client
        private void ProcessReceivedMessage(NetworkStream stream)
        {
            try
            {
                // 1. Read the message code (1 byte)
                byte[] msgCodeBuffer = new byte[1];
                stream.Read(msgCodeBuffer, 0, 1);
                DroneMessageCode code = (DroneMessageCode)msgCodeBuffer[0];
                Console.WriteLine($"Received message code: {code}");

                // 2. Read the message length (4 bytes)
                byte[] lengthBuffer = new byte[4];
                stream.Read(lengthBuffer, 0, 4);
                int msgLength = BitConverter.ToInt32(lengthBuffer, 0);
                Console.WriteLine($"Message length: {msgLength}");


                if (DroneMessageCode.Connected == code)
                {
                    // 3. Read and deserialize the JSON payload
                    byte[] jsonBuffer = new byte[msgLength];
                    stream.Read(jsonBuffer, 0, msgLength);
                    string jsonData = Encoding.UTF8.GetString(jsonBuffer);
                    Console.WriteLine($"Received JSON: {jsonData}");
                    DroneData data = JsonConvert.DeserializeObject<DroneData>(jsonData);
                    HandleDroneMessage(code, data);
                }
                else
                {
                    DroneData data = new DroneData();
                    data.Connected = false;
                    HandleDroneMessage(code,data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving message: {ex.Message}");
            }
        }

        // Handle the received drone message based on its code
        private void HandleDroneMessage(DroneMessageCode code, DroneData data)
        {
            switch (code)
            {
                case DroneMessageCode.Connected:
                case DroneMessageCode.Disconnected:
                case DroneMessageCode.LowBattery:
                case DroneMessageCode.ConnectionError:
                case DroneMessageCode.DataError:
                    UpdateUIBasedOnCode(code, data);
                    break;
                default:
                    Console.WriteLine("Unknown command received.");
                    break;
            }
        }



        private void UpdateUIBasedOnCode(DroneMessageCode code, DroneData data)
        {
            string message = code switch
            {
                DroneMessageCode.Connected => "Drone Connected. All systems go!",
                DroneMessageCode.Disconnected => "Connection lost. Please look for the drone.",
                DroneMessageCode.LowBattery => "Low battery! Please look for the drone in the air and step away until it lands safely.",
                DroneMessageCode.ConnectionError => "Could not establish connection with the drone.",
                DroneMessageCode.DataError => "Data error. Something is wrong. Please be cautious and check the drone.",
                _ => "Unhandled code."
            };

            form1.Invoke((MethodInvoker)(() =>
            {
                form1.updateLabels(data); // Assuming updateLabels handles all types of updates
                form1.ShowAlert(message, code.ToString());
            }));
        }
    }
}
