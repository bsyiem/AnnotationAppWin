import socket

def main():
    IP = "127.0.0.1"
    PORT = 4000
    sock = socket.socket(socket.AF_INET,socket.SOCK_DGRAM)
    sock.bind((IP,PORT))

    while(1):
        data,addr = sock.recvfrom(4096)
        if data:
            print("data ="+str(data))

if __name__ == '__main__':
    main()
