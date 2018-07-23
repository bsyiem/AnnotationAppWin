import zmq
import msgpack 
import sys
import json
import jsonpickle
import socket

#establish connection to device with pupil labs
def establishConnToPupil(topic):
	
	context = zmq.Context()
	
	addr = "127.0.0.1"
	
	req_port = "50020"
	req = context.socket(zmq.REQ)
	req.connect("tcp://"+addr+":"+req_port)
	req.send("SUB_PORT")
	
	sub_port = req.recv()
	sub = context.socket(zmq.SUB)
	sub.connect("tcp://"+addr+":"+sub_port)
	sub.setsockopt(zmq.SUBSCRIBE,topic)

	return sub

# gets the msgs from the established connection
def getmsg(sub):
	topic,msg = sub.recv_multipart()
	msg = msgpack.loads(msg)

	return msg

#gets the coordinates
def getGazePoint(msg):
	gazePoint = msg.get('norm_pos')
	return gazePoint
	
	
#gets the base from the msg
def getMsgBase(msg):
	msgBase = msg.get('base_data')[0] #because it is a list, we need to get the first element i.e, 0
	return msgBase
	
#gets raw gaze data
def getRawGazePoint(msgBase):
	rawGazePoint = msgBase.get('norm_pos')
	return rawGazePoint
	
#gets the confidence of detection
def getConf(msgBase):
	conf = msgBase.get('confidence')
	return conf
	
#Initialize UDP Ports
def initializeSocket():

	sock = socket.socket(socket.AF_INET,socket.SOCK_DGRAM)
	return sock


#send the gaze point
def sendUncalibrated(sub,sock,ip,port):
	while (True):
		msg = getmsg(sub)
		msgBase = getMsgBase(msg)
		
		conf = getConf(msgBase)			
		xy_coord = getRawGazePoint(msgBase)
		
		if(float(conf)>=0.98):
			print(str(xy_coord[0]+","+xy_coord[1]))
			xy_toSend = msgpack.dumps(xy_coord)
			sock.sendto(xy_toSend,(ip,port))		

#the main function
def main():
	UDP_IP = "127.0.0.1"
	UDP_PORT = 12345
	sock = initializeSocket()
		
	sub = establishConnToPupil("gaze")
	
	sendUncalibrated(sub,sock,UDP_IP,UDP_PORT)
		

if __name__ == "__main__":
	main()
