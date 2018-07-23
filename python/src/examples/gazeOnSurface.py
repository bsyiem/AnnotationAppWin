import zmq
import msgpack 
import sys
import json
import jsonpickle
import pygame

#checks if a key is pressed on keyboard
def returnKeyPressed():
	events = pygame.event.get()
	for event in events:
		if event.type == pygame.KEYDOWN:
			return event
		else:
			return None
#checks if escape is pressed in the keyboard
def checkQuit(event):
	if event == None:
		return False
	else:
		if event.key == pygame.K_ESCAPE:
			return True
		else:
			return False

#functionality to toggle fullscreen
def toggleFullscreen(event,screen):
	if event == None:
		return None
	if event.key == pygame.K_f:
		if screen.get_flags() & pygame.FULLSCREEN:
       		 pygame.display.set_mode((1280,720))
   		else:
       		 pygame.display.set_mode((0,0), pygame.FULLSCREEN)

#draws a circle on the screen
def drawCircle(x,y,screen):
	pygame.draw.circle(screen,(255,0,0),(x,y),40,0)

#establish connection to device with pupil labs
def establishConnToPupil():
	
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
def getmsg(topic):
	
	sub = establishConnToPupil()
	sub.setsockopt(zmq.SUBSCRIBE,topic)
	topic,msg = sub.recv_multipart()
	msg = msgpack.loads(msg)

	return msg

#gets the gaze position on the surface
def getPosOnSurface(msg):
	gazeOnSurface = msg.get('gaze_on_srf')
	# print gazeOnSurface
	json_msg = json.loads(json.dumps(gazeOnSurface[0]))
	return json_msg["norm_pos"]

#check if gazepoint is on the surface 
def getOnSurf(msg):
	gazeOnSurface = msg.get('gaze_on_srf')
	json_msg = json.loads(json.dumps(gazeOnSurface[0]))
	return bool(json_msg["on_srf"])

			

#the main function
def main():
	pygame.init()
	screen = pygame.display.set_mode((1280,720))
	
	while (True):
		msg = getmsg("surface")

		xy_coord = getPosOnSurface(msg)


		#map the normalized coordinate used by pupil to the coordinate system used by pygame
		actual_x = (xy_coord[0]) * screen.get_size()[0]
		actual_y = 720 - (xy_coord[1] * screen.get_size()[1])


		if (getOnSurf(msg)):
			screen.fill((0,0,0))
			drawCircle(int(actual_x),int(actual_y),screen)
		else:
			screen.fill((0,0,0))

		pygame.display.update()		
		event = returnKeyPressed()
		if (checkQuit(event)):
			break
		else:
			toggleFullscreen(event,screen)

	sys.exit()
	
if __name__ == "__main__":
	main()