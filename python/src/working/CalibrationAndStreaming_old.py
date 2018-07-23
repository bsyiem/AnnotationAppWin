import zmq
import msgpack 
import sys
import json
import jsonpickle
import socket
import time
import cv2
import numpy as np

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
#def getmsg(topic):
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

#calibrate to get the homography matrix
def calculate_homography(msgData):
	dest_points = np.array([(0.1,0.9),(0.5,0.9),(0.9,0.9),(0.1,0.5),(0.5,0.5),(0.9,0.5),(0.1,0.1),(0.5,0.1),(0.9,0.1)]);
	
	# extract source_points from data
	src_points = np.array(msgData)

	homography_matrix,mask = cv2.findHomography(src_points,dest_points,cv2.RANSAC)
	return (homography_matrix)

#returns the gazePoints from the streamer with confidence level >= conf
def getGazePointWithConf(sub,conf):
	msg = getmsg(sub)
	msgBase = getMsgBase(msg)

	confidence = getConf(msgBase)
	xy_coord = getRawGazePoint(msgBase)
	if (float(confidence) >= conf):
		return xy_coord

	return [-1,-1]

#send uncalibrated points and wait for the src_points
def send_uncalibrated(sub,sock,ip,port):
	while (True):
		xy_coord = getGazePointWithConf(sub,0.98)

		if (xy_coord[0] != -1 and xy_coord[1] != -1):
			print(str(xy_coord[0]) + "," + str(xy_coord[1]))
			xy_toSend = msgpack.dumps(xy_coord)
			sock.sendto(xy_toSend, (ip, port))
		try:
			data, addr = sock.recvfrom(1024)
			msgData = msgpack.loads(data)
			break
		except:
			'''no data yet'''

	return msgData

#calculate the homography matrix from the received src_points
def send_calibrated(sub,sock,ip,port,homography_matrix):
	while (True):
		xy_coord = getGazePointWithConf(sub,0.98)

		if (xy_coord[0] != -1 and xy_coord[1] != -1):
			mat_point = np.array([xy_coord[0], xy_coord[1], 1])
			calibrated_point_mat = np.dot(homography_matrix, mat_point)
			calibrated_point = [calibrated_point_mat[0] / calibrated_point_mat[2],
								calibrated_point_mat[1] / calibrated_point_mat[2]]
			xy_toSend = msgpack.dumps(calibrated_point)
			sock.sendto(xy_toSend, (ip, port))

#the main function
def main():
	UDP_IP = sys.argv[1]
	UDP_PORT = int(sys.argv[2])

	sock = initializeSocket()
	sock.setblocking(0)
	
	sub = establishConnToPupil("gaze")


	srcPoints_msgpack = send_uncalibrated(sub,sock,UDP_IP,UDP_PORT)

	homography_matrix = calculate_homography(srcPoints_msgpack)
	

#start sending calibrated points
	send_calibrated(sub,sock,UDP_IP,UDP_PORT,homography_matrix)


if __name__ == "__main__":
	main()

