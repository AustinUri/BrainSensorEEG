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
        private TcpClient client;
        private NetworkStream stream;
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

            client = listener.AcceptTcpClient();
            Console.WriteLine("Client connected!");

            stream = client.GetStream();
            ReceiveAndProcessMessages();

            client.Close();
            listener.Stop();
        }



        // Main loop for handling message processing
        private void ReceiveAndProcessMessages()
        {
            try
            {
                // Process commands in batches (5 captures per command)
                //for (int i = 0; i < 5; i++)
                //{
                //for (int j = 0; j < 5; j++)
                //{

                SendCommand(stream, DroneCommand.Takeoff, null);
                        // TODO: Implement logic for averaging and sending to Python
                        // TODO: Receive command back from Python based on analysis
                System.Threading.Thread.Sleep(10000);
                ProcessReceivedMessage(stream);
                //}
                SendCommand(stream, DroneCommand.Land, null);
                System.Threading.Thread.Sleep(50000);

                // After processing 5 captures, receive and handle commands
                ProcessReceivedMessage(stream);

                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during message processing: {ex.Message}");
            }
        }

        // Process a single message from the client
        private void ReadExact(NetworkStream stream, byte[] buffer, int offset, int count)
        {
            int bytesRead = 0;
            while (bytesRead < count)
            {
                int read = stream.Read(buffer, offset + bytesRead, count - bytesRead);
                if (read == 0)
                    throw new Exception("Stream closed unexpectedly.");
                bytesRead += read;
            }
        }


        private void SendCommand(NetworkStream stream, DroneCommand command, object commandData = null)
        {
            try
            {
                // 1. Convert the command to a single byte
                byte[] commandBuffer = new byte[] { (byte)command };

                // 2. Serialize the command data to JSON, if any, and encode to bytes
                string jsonData = commandData != null ? JsonConvert.SerializeObject(commandData) : string.Empty;
                byte[] jsonBuffer = Encoding.UTF8.GetBytes(jsonData);

                // 3. Get the length of the JSON data as a 4-byte array (Big Endian)
                byte[] lengthBuffer = BitConverter.GetBytes(jsonBuffer.Length);
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(lengthBuffer); // Convert to Big Endian if system uses Little Endian
                }

                // 4. Combine the command code, length, and JSON payload
                byte[] message = new byte[commandBuffer.Length + lengthBuffer.Length + jsonBuffer.Length];
                Buffer.BlockCopy(commandBuffer, 0, message, 0, commandBuffer.Length);
                Buffer.BlockCopy(lengthBuffer, 0, message, commandBuffer.Length, lengthBuffer.Length);
                Buffer.BlockCopy(jsonBuffer, 0, message, commandBuffer.Length + lengthBuffer.Length, jsonBuffer.Length);

                // 5. Send the complete message to the client
                stream.Write(message, 0, message.Length);
                Console.WriteLine($"Sent command: {command}");
                Console.WriteLine($"Command length: {jsonBuffer.Length}");
                Console.WriteLine($"Command JSON: {jsonData}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending command: {ex.Message}");
            }
        }


        private void ProcessReceivedMessage(NetworkStream stream)
        {
            try
            {
                // 1. Read the message code (1 byte)
                byte[] msgCodeBuffer = new byte[1];
                ReadExact(stream, msgCodeBuffer, 0, 1);
                DroneMessageCode code = (DroneMessageCode)msgCodeBuffer[0];
                Console.WriteLine($"Received message code: {code}");
                Console.WriteLine("Raw Message Code Buffer: " + BitConverter.ToString(msgCodeBuffer));

                if (DroneMessageCode.Connected == code)
                {
                    // 2. Read the message length (4 bytes)
                    byte[] lengthBuffer = new byte[4];
                    ReadExact(stream, lengthBuffer, 0, 4);
                    int msgLength = BitConverter.ToInt32(lengthBuffer.Reverse().ToArray(), 0); // Ensure correct endianness
                    Console.WriteLine($"Message length: {msgLength}");
                    Console.WriteLine("Raw Length Buffer: " + BitConverter.ToString(lengthBuffer));

                // 3. Read and deserialize the JSON payload if the message code is 'Connected'
                    byte[] jsonBuffer = new byte[msgLength];
                    ReadExact(stream, jsonBuffer, 0, msgLength);
                    string jsonData = Encoding.UTF8.GetString(jsonBuffer);
                    Console.WriteLine($"Received JSON: {jsonData}");
                    DroneData data = JsonConvert.DeserializeObject<DroneData>(jsonData);
                    HandleDroneMessage(code, data);
                }
                else
                {
                    // Handle other codes
                    DroneData data = new DroneData { Connected = false };
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
