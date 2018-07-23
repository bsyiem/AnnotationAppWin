import zmq
import msgpack 
import sys
import json
import jsonpickle
import socket

#establish connection to device with pupil labs
def establishConnToPupil():
	
	context = zmq.Context()
	
	addr = "127.0.0.1"
	
	req_port = "50020"
	req = context.socket(zmq.REQ)
	req.connect("tcp://"+addr+":"+req_port)
	req.send("SUB_PORT")
	
	sub_port = req.recv()
	sub = context.socket(zmq.SUB)
	sub.connect("tcp://"+addr+":"+sub_port)

	return sub

# gets the msgs from the established connection
def getmsg(topic):
	
	sub = establishConnToPupil()
	sub.setsockopt(zmq.SUBSCRIBE,topic)
	topic,msg = sub.recv_multipart()
	msg = msgpack.loads(msg)

	return msg

#gets the coordinates
def getGazePoint(msg):
	gazePoint = msg.get('norm_pos')
	return gazePoint
	
#Initialize UDP Ports
def initializeSocket():

	sock = socket.socket(socket.AF_INET,socket.SOCK_DGRAM)
	return sock

#the main function
def main():
	UDP_IP = "127.0.0.1"
	UDP_PORT = 12345
	sock = initializeSocket()
	
	while (True):
		msg = getmsg("gaze")

		xy_coord = getGazePoint(msg)
		x=float(xy_coord[0])
		y=float(xy_coord[1])
		#if((x>=0 and x<=1) and (y>=0 and y<=1)):
		if(y>=-5): # the values are not sent when X = 0.674988214924 and Y = -6.66013818644 as these are the default values when no pupil is detected
			print(str(xy_coord[0]+","+xy_coord[1]))
			xy_toSend = msgpack.dumps(xy_coord)
			sock.sendto(xy_toSend,(UDP_IP,UDP_PORT))		


if __name__ == "__main__":
	main()