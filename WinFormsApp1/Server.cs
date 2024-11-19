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

        public NetworkStream GetStream()
        {
            return stream;
        }



        // Main loop for handling message processing
        private void ReceiveAndProcessMessages()
        {
            try
            {
                Console.WriteLine("Listening for messages from Python client...");

                while (true) // Continuous loop to handle incoming messages
                {
                    if (stream.DataAvailable)
                    {
                        // Process the received message
                        ProcessReceivedMessage(stream);
                    }
                    else
                    {
                        // Optional: Wait briefly to avoid busy waiting
                        System.Threading.Thread.Sleep(100);
                    }
                }
            }
            catch (IOException ioEx)
            {
                Console.WriteLine($"Connection lost: {ioEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during message processing: {ex.Message}");
            }
        }

        // Process a single message from the client
        private void ReadExact(byte[] buffer, int offset, int count)
        {
            int bytesRead = 0;
            while (bytesRead < count)
            {
                int read = this.stream.Read(buffer, offset + bytesRead, count - bytesRead);
                if (read == 0)
                    throw new Exception("Stream closed unexpectedly.");
                bytesRead += read;
            }
        }


        public void SendCommand(NetworkStream stream, DroneCommand command, string trend)
        {
            try
            {
                // Step 1: Define the message code for DroneCommand (e.g., 0x01)
                byte messageCode = 0x01; // Example: 0x01 for command messages

                // Step 2: Create the JSON response with command and details
                var response = new
                {
                    command = command.ToString(),
                    details = new
                    {
                        timestamp = DateTime.UtcNow.ToString("o"), // ISO 8601 format
                        trend = trend // Additional context
                    }
                };

                string jsonResponse = JsonConvert.SerializeObject(response);
                byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonResponse);

                // Step 3: Combine message code and JSON data
                byte[] fullMessage = new byte[1 + jsonBytes.Length];
                fullMessage[0] = messageCode; // First byte: Message code
                Array.Copy(jsonBytes, 0, fullMessage, 1, jsonBytes.Length); // Remaining bytes: JSON payload

                // Step 4: Send the combined message
                stream.Write(fullMessage, 0, fullMessage.Length);
                Console.WriteLine($"Message sent: Code={messageCode}, JSON={jsonResponse}");
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
                ReadExact(msgCodeBuffer, 0, 1);
                DroneMessageCode code = (DroneMessageCode)msgCodeBuffer[0];
                Console.WriteLine($"Received message code: {code}");
                Console.WriteLine("Raw Message Code Buffer: " + BitConverter.ToString(msgCodeBuffer));

                if (DroneMessageCode.Connected == code)
                {
                    // 2. Read the message length (4 bytes)
                    byte[] lengthBuffer = new byte[4];
                    ReadExact(lengthBuffer, 0, 4);
                    int msgLength = BitConverter.ToInt32(lengthBuffer.Reverse().ToArray(), 0); // Ensure correct endianness
                    Console.WriteLine($"Message length: {msgLength}");
                    Console.WriteLine("Raw Length Buffer: " + BitConverter.ToString(lengthBuffer));

                    // 3. Read and deserialize the JSON payload if the message code is 'Connected'
                    byte[] jsonBuffer = new byte[msgLength];
                    ReadExact(jsonBuffer, 0, msgLength);
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
