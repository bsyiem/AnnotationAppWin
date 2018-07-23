import sys
import socket

def main():
	
	UDP_PORT = 12345
	sock = socket.socket(socket.AF_INET,socket.SOCK_DGRAM)
#	UDP_IP = "172.16.1.168"
	UDP_IP = "localhost"
	msg = "Hello World"
	encoded_msg = msg.encode('utf-8')
	sock.sendto(encoded_msg,(UDP_IP,UDP_PORT))
	print ("sent")

if __name__ == "__main__":
	main()