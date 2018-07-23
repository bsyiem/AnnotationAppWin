import socket


def main():
    IP = "localhost"
    PORT = 4000
    sock = socket.socket(socket.AF_INET,
                         socket.SOCK_DGRAM)
    sock.setblocking(0)
    while(1):
        print ("sending")
        sock.sendto("hello".encode('utf-8'), (IP, PORT))

        try:
            data, addr = sock.recvfrom(4096)
            print(data)
        except:
            print ("no data yet")



if __name__ == '__main__':
    main()
