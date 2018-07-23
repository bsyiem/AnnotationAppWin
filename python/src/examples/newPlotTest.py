import pygame
import sys


def returnKeyPressed():
	events = pygame.event.get()
	for event in events:
		if event.type == pygame.KEYDOWN:
			return event
		else:
			return None

def checkQuit(event):
	if event == None:
		return False
	else:
		if event.key == pygame.K_ESCAPE:
			return True
		else:
			return False

def toggleFullscreen(event,screen):
	if event == None:
		return None
	if event.key == pygame.K_f:
		if screen.get_flags() & pygame.FULLSCREEN:
       		 pygame.display.set_mode((800,640))
   		else:
       		 pygame.display.set_mode((0,0), pygame.FULLSCREEN)

def drawCircle(x,y,screen):
	pygame.draw.circle(screen,(255,0,0),(x,y),40,0)


def main():
	pygame.init()

	screen = pygame.display.set_mode((800,640))
	while (True):	
		
		drawCircle(200,200,screen)
		pygame.display.update()
		
		event = returnKeyPressed()
		if (checkQuit(event)):
			break
		else:
			toggleFullscreen(event,screen)

	sys.exit()

if __name__ == "__main__":
	main()



    		