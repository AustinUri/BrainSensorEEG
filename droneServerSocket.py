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
    HOVER = 8

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

    for x in range(0,5):
        receive_command(client_socket)


    #security 
    if tello.get_height() is not 0:
        tello.land()

    # Close the socket after use
    client_socket.close()



 

def receive_command(client_socket):
    try:
        # Step 1: Read the message code (1 byte)
        message_code = client_socket.recv(1)
        if not message_code:
            raise ValueError("No message code received.")
        message_code = int.from_bytes(message_code, 'big')
        print(f"Message Code: {message_code}")

        # Step 2: Read the message length (4 bytes)
        length_buffer = client_socket.recv(4)
        if len(length_buffer) < 4:
            raise ValueError("Invalid message length received.")
        message_length = int.from_bytes(length_buffer, 'big')
        print(f"Message Length: {message_length}")

        # Step 3: Read the JSON payload
        json_payload = client_socket.recv(message_length).decode('utf-8')
        print(f"JSON Payload: {json_payload}")

        # Step 4: Parse the JSON payload
        data = json.loads(json_payload)

        # Step 5: Extract the command and process it
        command = data.get("command")
        if command is None:
            raise ValueError("No command found in the received data.")
        print(f"Command: {command}")

        if command == "Takeoff":
            handle_command(DroneCommand.TAKEOFF)        
        elif command == "Land":
            handle_command(DroneCommand.LAND) 
        elif command == "MoveUp" :
            handle_command(DroneCommand.MOVE_UP)

                   
        else:
            print("ffs")


    except Exception as e:
        print(f"Error receiving command: {e}")


def handle_command(command : DroneCommand):
    """
    Process the command based on its type.
    :param command: The DroneCommand received.
    :param payload: Additional data sent with the command. :not implemented yet
    """
    if command == DroneCommand.TAKEOFF:
        print("Executing Takeoff...")
        tello.takeoff()
    elif command == DroneCommand.LAND:
        print("Executing Land...")
        tello.land()
    elif command == DroneCommand.TURN_LEFT:
        print("Rotating counter clockwise 15 degrees")
        tello.rotate_counter_clockwise(15)
    elif command == DroneCommand.MOVE_LEFT:
        print("Moving Left by 10 cm")
        tello.move_left(10)
    elif command == DroneCommand.MOVE_RIGHT:
        print("Moving Right by 10 cm")
        tello.move_right(10)
    elif command == DroneCommand.MOVE_UP:
        if tello.get_height() == 0:
            tello.takeoff()
        else:
            tello.move_up(10)
    elif command == DroneCommand.MOVE_DOWN:
        if tello.get_height() == 10 :
            print("Executing Land...")
            tello.land()
        elif tello.get_height() < 10 :
            print("Executing Land...")
            tello.land()
        else :
            print("Moving Down by 10 cm...")
            tello.move_down(10)
    else:
        print(f"Unknown command: {command.name}")


if __name__ == "__main__":
    main()
