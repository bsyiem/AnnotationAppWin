import zmq
import msgpack 
import sys
import json
# used to print the msg received from pupil
def printmsg (msg):
	print msg


def printSelected(msg,parameter):
	for i in range (0,len(parameter)):
		print msg.get(parameter[i])

#establishes a connection to the device that sends pupil labs data
def establishConn():
	
	context = zmq.Context()
	
	addr = str(sys.argv[1])
	
	req_port = str(sys.argv[2])
	req = context.socket(zmq.REQ)
	req.connect("tcp://"+addr+":"+req_port)
	req.send("SUB_PORT")
	
	sub_port = req.recv()
	sub = context.socket(zmq.SUB)
	sub.connect("tcp://"+addr+":"+sub_port)

	return sub

# gets the msgs from the established connection
def getmsg(sub,topic):
	
	#sub = establishConn()
	sub.setsockopt(zmq.SUBSCRIBE,topic)
	topic,msg = sub.recv_multipart()
	msg = msgpack.loads(msg)

	print topic 
	return msg
	

if __name__  == "__main__":

	iterations = ""
	while (str(iterations).upper().strip(' ') != 'Y' and str(iterations).upper().strip(' ') != 'N'):
		iterations = raw_input("do you wish to receive just one read [Y/N]: ")
	
	topic = ""
#	while (str(topic).lower().strip(' ') != 'pupil' and str(topic).lower().strip(' ') != 'gaze'):
	topic = raw_input("Enter what topic you want to subscribe to (pupil/gaze/surface):")
	
	parameter = raw_input("Enter the parameter you wish to view: ")
	
	print parameter.split(' ')

	sub=establishConn()

	while True:
		msg = getmsg(sub,str(topic))
		if(parameter == None or parameter == ''):
			printmsg(msg)
		else:
			printSelected(msg,parameter.split(' '))

		if (str(iterations).upper().strip(' ') == 'Y'):
			break