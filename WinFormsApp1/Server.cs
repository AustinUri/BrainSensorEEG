﻿using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using WinFormsApp1;

namespace ServerF
{
    // Enum for message codes from the drone
    public enum DroneMessageCode : byte
    {
        Connected = 0b0001,
        Disconnected = 0b0010,
        LowBattery = 0b0100,
        ConnectionError = 0b0111,
        DataError = 0b1000
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

        // Capture the RGB color at a specific screen pixel position
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
                        Point position = new Point(1600, 633);  // Example point to capture
                        var (r, g, b) = CapturePixel(position);
                        Console.WriteLine($"Capture {j + 1} - R: {r}, G: {g}, B: {b}");

                        // TODO: Implement logic for averaging and sending to Python
                        // TODO: Receive command back from Python based on analysis
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

                // 3. Read and deserialize the JSON payload
                byte[] jsonBuffer = new byte[msgLength];
                stream.Read(jsonBuffer, 0, msgLength);
                string jsonData = Encoding.UTF8.GetString(jsonBuffer);
                Console.WriteLine($"Received JSON: {jsonData}");
                if (DroneMessageCode.Connected == code )
                {
                    DroneData data = JsonConvert.DeserializeObject<DroneData>(jsonData);
                    HandleDroneMessage(code, data);
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
                    form1.Invoke((MethodInvoker)(() => form1.updateLabels(data)));  // Use Invoke to call updateLabels on the UI thread
                    break;

                case DroneMessageCode.Disconnected:
                    form1.Invoke((MethodInvoker)(() => form1.ShowAlert("Connection lost. Please look for the drone.", "Drone Disconnected")));
                    break;

                case DroneMessageCode.LowBattery:
                    form1.Invoke((MethodInvoker)(() => form1.ShowAlert("Low battery! Please look for the drone in the air and step away until it lands safely.", "Low Battery Warning")));
                    break;

                case DroneMessageCode.ConnectionError:
                    form1.Invoke((MethodInvoker)(() => form1.ShowAlert("Could not establish connection with the drone.", "Connection Error")));
                    break;

                case DroneMessageCode.DataError:
                    form1.Invoke((MethodInvoker)(() => form1.ShowAlert("Data error. Something is wrong. Please be cautious and check the drone.", "Data Error")));
                    break;

                default:
                    Console.WriteLine("Unknown command received.");
                    break;
            }
        }

    }
}