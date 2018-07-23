import sys
import socket
def main():
	
	UDP_PORT = 12345
	sock = socket.socket(socket.AF_INET,socket.SOCK_DGRAM)
	UDP_IP = ""
	sock.bind((UDP_IP, UDP_PORT))
	
	while True:
		data, addr = sock.recvfrom(1024)
		print ("recv:"+data.decode("utf-8"))

if __name__ == "__main__":
	main()