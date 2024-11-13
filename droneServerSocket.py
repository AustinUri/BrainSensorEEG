"""
This file is a POC created by Uri Austin!
This POC is meant to prove that I can create a code that serves both as a server and both and a client
Server = sends hight to client 
Client = client of Tello drone
"""
import socket
import json
from djitellopy import Tello


#create enum of binary codes so the c# and the python can communicate


#-1 = socket error

# Socket configuration
HOST = 'localhost'
PORT = 5000

def send_json_message(client_socket, code, data):
    if client_socket:  # Only attempt to send if the socket is valid
        message = {
            "code": code,
            "data": data
        }
        jsonObj = json.loads(message)
        client_socket.sendall(jsonObj)
    else:
        print("No socket initiallized!")
        exit()




def main():
     # Initialize socket
    client_socket = None
    try:
        client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        client_socket.connect((HOST, PORT))
        print("Connected to C# server successfully.")
    except (socket.error, socket.timeout) as e:
        print("Failed to connect to C# server:", e)
        # Socket is not connected, so we skip sending a JSON message and exit
        if client_socket:
            client_socket.close()
        exit(-1)

    # Create Tello object and connect
    tello = Tello()
    
    try:
        tello.connect()
        print("Tello drone connected successfully.")
        
        # Check connection status
        response = tello.get_battery()
        if response is None:
            print("Failed to connect to Tello drone.")
            send_json_message(client_socket, 0, {"error": "Failed to connect to Tello drone"})
            client_socket.close()
            exit(-1)
        else:
            print("Tello drone battery level:", response)
            send_json_message(client_socket, 1, {"battery": response})
    except Exception as e:
        print("An error occurred while connecting to Tello:", e)
        send_json_message(client_socket, 0, {"error": str(e)})
        client_socket.close()
        exit(-1)

    # Continue with further drone operations here





    # Close the socket after use
    client_socket.close()










if __name__ == "__main__" :
    main()