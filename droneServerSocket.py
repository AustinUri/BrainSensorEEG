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
import time

# Enum for drone message codes to communicate with C#
class DroneMessageCode(Enum):
    CONNECTED = 1
    DISCONNECTED = 2
    LOW_BATTERY = 3  # Indicates low battery, emergency landing required
    CONNECTION_ERROR = 4  # Initial connection failed
    DATA_ERROR = 5  # Data retrieval failed (e.g., battery or height data)

# Create Tello object and attempt connection
tello = Tello()

class DroneCommand(Enum):
    TAKEOFF = 0
    LAND = 1
    TURN_LEFT = 2
    TURN_RIGHT = 3
    MOVE_LEFT = 4
    MOVE_RIGHT = 5
    MOVE_UP = 6
    MOVE_DOWN = 7

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
        msg_code = code.to_bytes(1, byteorder='big')  # Use code.value to get the integer
        json_data = json.dumps(data)
        json_bytes = json_data.encode('utf-8')
        
        # Combine code, length, and JSON data into one message
        msg_len = len(json_bytes).to_bytes(4, byteorder='big')
        message = msg_code + msg_len + json_bytes
        print('len code',len(msg_code),'len len',len(msg_len),'len json',len(json_bytes))
        print(message)
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

    
    receive_command(client_socket)

    # Retrieve and send drone data
    data, code = get_drone_data(tello)
    if data:
        send_message(client_socket, code.value, data)
    else:
        send_message(client_socket, code.value, {"connected": False})
    
    
    receive_command(client_socket)

    # Close the socket after use
    client_socket.close()



 

def receive_command(client_socket):
    try:
        # 1. Read the command code (1 byte)
        command_code = client_socket.recv(1)
        if not command_code:
            print("No data received. Closing connection.")
            return None
        command = DroneCommand(int.from_bytes(command_code, byteorder="big"))
        print(f"Received command: {command.name}")

        # 2. Read the payload length (4 bytes, Big Endian)
        length_buffer = client_socket.recv(4)
        if len(length_buffer) < 4:
            raise ValueError("Failed to receive the payload length.")
        payload_length = int.from_bytes(length_buffer, byteorder="big")
        print(f"Payload length: {payload_length}")

        # 3. Read the JSON payload (if any)
        if payload_length > 0:
            json_buffer = client_socket.recv(payload_length)
            if len(json_buffer) < payload_length:
                raise ValueError("Failed to receive the full payload.")
            payload = json.loads(json_buffer.decode("utf-8"))
            print(f"Payload data: {payload}")
        else:
            payload = None
            print("No payload provided.")

        # 4. Handle the command
        handle_command(command, payload)

    except Exception as e:
        print(f"Error receiving command: {e}")

def handle_command(command, payload):
    """
    Process the command based on its type.
    :param command: The DroneCommand received.
    :param payload: Additional data sent with the command.
    """
    if command == DroneCommand.TAKEOFF:
        print("Executing Takeoff...")
        tello.takeoff()
        # Add your drone's takeoff logic here
    elif command == DroneCommand.LAND:
        print("Executing Land...")
        tello.land()
    elif command == DroneCommand.TURN_LEFT:
        distance = payload.get("Distance", 0) if payload else 0
        print(f"Turning Left by {distance} units...")
        tello.rotate_counter_clockwise(90)
        # Add turn left logic here
    elif command == DroneCommand.MOVE_LEFT:
        distance = payload.get("Distance", 0) if payload else 0
        print(f"Moving Left by {distance} units...")
        tello.move_left()
    elif command == DroneCommand.MOVE_RIGHT:
        distance = payload.get("Distance", 0) if payload else 0
        print(f"Moving Right by {distance} units...")
        tello.move_right()
    elif command == DroneCommand.MOVE_UP:
        tello.move_up()
    elif command == DroneCommand.MOVE_DOWN:
        distance = payload.get("Distance", 0) if payload else 0
        print(f"Moving Down by {distance} units...")
        tello.move_down
    else:
        print(f"Unknown command: {command.name}")


if __name__ == "__main__":
    main()
