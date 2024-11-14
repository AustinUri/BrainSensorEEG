"""
This file is a POC created by Uri Austin!
This POC demonstrates a Python application that acts as both a server and client.
Server: Sends height and other drone data to a C# client.
Client: Controls the Tello drone.
"""

import socket
import json
from djitellopy import Tello
from enum import Enum

# Enum for drone message codes to communicate with C#
class DroneMessageCode(Enum):
    CONNECTED = 0b0001
    DISCONNECTED = 0b0010
    LOW_BATTERY = 0b0100  # Indicates low battery, emergency landing required
    CONNECTION_ERROR = 0b0111  # Initial connection failed
    DATA_ERROR = 0b1000  # Data retrieval failed (e.g., battery or height data)

# Socket configuration
HOST = 'localhost'
PORT = 5000

def initialize_socket():
    """
    Initialize a socket connection to the C# server.
    """
    try:
        client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        client_socket.connect((HOST, PORT))
        print("Connected to C# server successfully.")
        return client_socket
    except (socket.error, socket.timeout) as e:
        print("Failed to connect to C# server:", e)
        return None

def send_message(client_socket, code, data):
    """
    Sends a message in the format: [msgCode (1 byte)][length (4 bytes)][JSON message].
    """
    if client_socket:
        # Convert the code (enum value) to 1 byte and data to JSON string
        msg_code = code.value.to_bytes(1, byteorder='big')  # Use code.value to get the integer
        json_data = json.dumps(data)
        json_bytes = json_data.encode('utf-8')
        
        # Combine code, length, and JSON data into one message
        msg_len = len(json_bytes).to_bytes(4, byteorder='big')
        message = msg_code + msg_len + json_bytes
        client_socket.sendall(message)
    else:
        print("No socket initialized!")


def get_drone_data(tello):
    """
    Retrieve battery and height data from the Tello drone, including connection status.
    """
    try:
        if not tello.get_battery():
            print("Failed to retrieve Tello drone data.")
            return None, DroneMessageCode.CONNECTION_ERROR
        battery = tello.get_battery()
        height = tello.get_height()
        
        # Check for low battery and return the appropriate code
        code = DroneMessageCode.LOW_BATTERY if battery < 20 else DroneMessageCode.CONNECTED
        return {"battery": battery, "height": height, "connected": True}, code
    except Exception as e:
        print(f"Error retrieving Tello data: {e}")
        return None, DroneMessageCode.DATA_ERROR

def main():
    # Initialize the socket and check connection
    client_socket = initialize_socket()
    if not client_socket:
        exit(-1)

    # Create Tello object and attempt connection
    tello = Tello()
    try:
        tello.connect()
        print("Tello drone connected successfully.")
    except Exception as e:
        print("An error occurred while connecting to Tello:", e)
        send_message(client_socket, DroneMessageCode.CONNECTION_ERROR, {"error": str(e)})
        client_socket.close()
        exit(-1)

    # Retrieve and send drone data
    data, code = get_drone_data(tello)
    if data:
        send_message(client_socket, code.value, data)
    else:
        send_message(client_socket, code.value, {"connected": False})

    # Close the socket after use
    client_socket.close()

if __name__ == "__main__":
    main()
