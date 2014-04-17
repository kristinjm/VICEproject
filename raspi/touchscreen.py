#!usr/bin/python -u
#***************************************************************************************#
# FILENAME: 	      touchscreen.py 													#
# LOCAL DEPENDENCIES: sendMessage.c, sendMessage.h 										#
# COMPILATION:		  no compilation, compile sendMessage through Makefile				#
# EXECUTION:		  sudo python ./touchscreen.py 										#
# 																						#
# AUTHORS:      Thomas Gerstenberg, Chad Higgins, Kristin Makowski,                     #
#               Mark Parangue, Robert Paulino                                           #
# INSTITUTIONS: SDSU and SDG&E 															#
# SEMESTER: 	SPRING 2014 															#
# PROJECT: 		VICE VFD Controller and Monitor 										#
#																						#
# This is the touchscreen and UI handler for the VFD Controller.  It allows the user to #
# change the flow rate and state of the VFD, as well as lock out the website from		#
# changing these values.  It also houses settings to change the server IP address and 	#
# restart the receive thread listening to the server. 									#
#***************************************************************************************#

import fnmatch
import os
import pygame
from pygame.locals import *
import subprocess 
from time import sleep


# UI classes ---------------------------------------------------------------

# Icon is a very simple bitmap class, just associates a name and a pygame
# image (PNG loaded from icons directory) for each.
# There isn't a globally-declared fixed list of Icons.  Instead, the list
# is populated at runtime from the contents of the 'icons' directory.
class Icon:
	def __init__(self, name):
		self.name = name
		try:
			self.bitmap = pygame.image.load(iconPath + '/' + name + '.png')
		except:
			pass

# Button is a simple tappable screen region.  Each has:
#  - bounding rect ((X,Y,W,H) in pixels)
#  - optional background color and/or Icon (or None), always centered
#  - optional foreground Icon, always centered
#  - optional single callback function
#  - optional single value passed to callback
# Occasionally Buttons are used as a convenience for positioning Icons
# but the taps are ignored.  Stacking order is important; when Buttons
# overlap, lowest/first Button in list takes precedence when processing
# input, and highest/last Button is drawn atop prior Button(s).  This is
# used, for example, to center an Icon by creating a passive Button the
# width of the full screen, but with other buttons left or right that
# may take input precedence (e.g. the Effect labels & buttons).
# After Icons are loaded at runtime, a pass is made through the global
# buttons[] list to assign the Icon objects (from names) to each Button.
class Button:
	def __init__(self, rect, **kwargs):
		self.rect     = rect # Bounds
		self.name 	  = None
		self.color    = None # Background fill color, if any
		self.iconBg   = None # Background Icon (atop color fill)
		self.iconFg   = None # Foreground Icon (atop background)
		self.bg       = None # Background Icon name
		self.fg       = None # Foreground Icon name
		self.callback = None # Callback function
		self.value    = None # Value passed to callback
		for key, value in kwargs.iteritems():
			if   key == 'color': self.color    = value
			elif key == 'bg'   : 
				self.bg     = value
				self.name 	= value
			elif key == 'fg'   : self.fg       = value
			elif key == 'cb'   : self.callback = value
			elif key == 'value': self.value    = value

	def selected(self, pos):
		x1 = self.rect[0]
		y1 = self.rect[1]
		x2 = x1 + self.rect[2] - 1
		y2 = y1 + self.rect[3] - 1
		if ((pos[0] >= x1) and (pos[0] <= x2) and
				(pos[1] >= y1) and (pos[1] <= y2)):
			if self.callback:
				if self.value is None: self.callback()
				else:                  self.callback(self.value)
			return True
		return False

	def draw(self, screen):
		if self.color:
			screen.fill(self.color, self.rect)
		if self.iconBg:
			screen.blit(self.iconBg.bitmap,
				(self.rect[0]+(self.rect[2]-self.iconBg.bitmap.get_width())/2,
				 self.rect[1]+(self.rect[3]-self.iconBg.bitmap.get_height())/2))
		if self.iconFg:
			screen.blit(self.iconFg.bitmap,
				(self.rect[0]+(self.rect[2]-self.iconFg.bitmap.get_width())/2,
				 self.rect[1]+(self.rect[3]-self.iconFg.bitmap.get_height())/2))

	def setBg(self, name):
		if name is None:
			self.iconBg = None
		else:
			for i in icons:
				if name == i.name:
					self.iconBg = i
					break


##################################### UI Callbacks #####################################

# Handler for the numeric input screen
# Enhancement: Add a lock screen to the settings
def numericCallback(n):
	global screenMode, updateScreen, returnScreen
	global lockSettings
	global displayString
	global serverIP
	if n < 10:
		displayString = displayString + str(n)
	elif n == 10:
		displayString = displayString[:-1]
	elif n == 11:
		screenMode = returnScreen
	elif n == 12:
		if pwScreen == 0:
			screenMode = returnScreen
			serverIP = displayString
			saveSettings()
		else:
			if displayString == passcode:
				lockSettings = (lockSettings == 0) # Toggle lockSettings
				updateLockIcons()
				screenMode = returnScreen
			else:
				displayString = ''
	elif n == 13:
		displayString = displayString + '.'


	updateScreen = 1

# Handler for the settings screen
def settingsCallback(n): # Pass 1 (next setting) or -1 (prev setting)
	global screenMode
	global returnScreen
	global displayString
	global serverIP
	global updateScreen
	global webLockState
	if n == -1:			#Previous Screen
		screenMode = 0
		saveSettings()
	if n == 1:			#Setting 1: Server Ip
		screenMode = 2
		returnScreen = 1
		displayString = serverIP
	elif n == 2:		#Setting2: Lock/unlock the web state
		webLockState = (webLockState+1)%2 # toggle the webLockstate
		sendLockMessage()	
		updateScreen = 1	
	elif n == 3:		#Setting3: Restart Services
			returnScreen = 1

def screenCallback(n):
	if lockSettings:
		return
	global screenMode
	if n is 0:   #Settings
		screenMode = 1

def lockScreen(n):
	global screenMode, returnScreen, pwScreen
	global displayString
	pwScreen = 1
	screenMode = 2
	returnScreen = 0
	displayString = ''
	return 1

def updateLockIcons():
	global icons, buttons
	global lockSettings
	if lockSettings:
		addButton(0, Button(( 10,180, 48, 48), bg='lockoverlay'))
		addButton(0, Button((220, 60, 48, 48), bg='lockoverlay'))
	else:
		removeButton(0, 'lockoverlay')
		removeButton(0, 'lockoverlay')
		

# This function toggles the on/off setting and button for the main screen
# TODO: Implement another screen to confirm on off setting
def onOffCallback(n):
	global buttons
	global updateScreen
	global currentState
	global lockSettings

	if lockSettings:
		return
	# If currentState is ON, set the currentState to OFF, and add the ON button to the list
	if currentState == 1: #ON
		removeButton(0, 'off')
		addButton(0, Button((180, 60,128, 48), bg='on',  cb=onOffCallback, value=1))
		currentState = 0 #OFF
	# If currentState is OFF, set the currentState to ON, and add the OFF button to the list
	else:
		removeButton(0, 'on')	
		addButton(0, Button((180, 60,128, 48), bg='off', cb=onOffCallback, value=0))	
		currentState = 1 #ON
		
	sendStateToServer();
	updateScreen = 1
	#send off message
	
# Updates the screen with the new flow
def flowCallback(updown):
	global flow
	global updateScreen
	if updown == 1:
		if flow != 3.5:
			flow += 0.25
	else:
		if flow != 0:
			flow -= 0.25
	updateScreen = 1

def removeButton(screen, name):
	global buttons, updateScreen
	i = 0
	for b in buttons[screen]:
		if b.name == name:
			buttons[0].pop(i)
		i = i+1
	updateScreen = 1

def addButton(screen, button):
	global buttons, icons
	name = button.name
	for i in icons:
		if i.name == name:
			button.iconBg = i
			button.bg = None
	buttons[screen].append(button)



################################# CONTROLLER FUNCTIONS #################################
def restartServices(n): # working
	#Send signal to restart recv handler in socket
	print 'restarting services'
	subprocess.call(['sudo', 'pkill', '-USR1', 'controller'])

# Currently unimplemented
def stopServices():
	# send kill message to stop the controller
	print 'stopping services'
	subprocess.call(['sudo', 'pkill', 'controller'])

# Currently unimplemented
def startServices():
	print 'starting services'
	subprocess.call(['sudo', './controller', '&'])

# Sends the new flow rate to server
def sendFlowToServer(n):
	global flow, currentState
	message = '<,DF=' + str(flow) + ',><EOF>'
	length = len(message)
	subprocess.call(['./sendMessage', message, str(length)])
# Sends the web lock state to server
def sendLockMessage():
	global webLockState
	message = '<,L=' + str(webLockState) + ',><EOF>'
	length = len(message)
	subprocess.call(['./sendMessage', message, str(length)])

# Sends the new state
def sendStateToServer():
	global currentState
	message = '<,VS=' + str(currentState) + ',><EOF>' 
	length = len(message)
	subprocess.call(['./sendMessage', message, str(length)])


################################### GLOBAL VARIABLES ###################################
flow = 0
currentState = 1 #ON
prevflow = flow
serverIP = '192.168.144.1'
lockSettings = 0
webLockState = 1

passcode = '7343'
pwScreen = 0;

ipLocation  	= '/etc/vice/ip'
valuesLocation 	= '/etc/vice/flow'	# Currently unimplemented

threadExited    = False
updateScreen 	= 1 	  # Flag used to indicate that UI needs to be updated
screenMode      =  0      # Current screen mode; default (0): home screen
screenModePrior = -1      # Prior screen mode (for detecting changes)
iconPath        = 'icons' # Subdirectory containing UI bitmaps (PNG format)
displayString	= ''
returnScreen    = 0

icons = [] # This list gets populated at startup

# Used for the numeric input screen
bHgt = 48
col = [0+8, 1*64+8, 2*64+8, 3*64, 4*64, 5*64]
row = [0, 1*bHgt, 2*bHgt, 3*bHgt, 4*bHgt]
# Arrays of each button on each screen and their callback values
buttons = [

	# Screen mode 0 is main screen of current status
	[Button(( 10,180, 48, 48), bg='cog',   			cb=screenCallback, 	value=0),
	 Button(( 90,180, 48, 48), bg='lock',			cb=lockScreen,		value=0),
	 Button((180,180,128, 48), bg='set',			cb=sendFlowToServer,value=0),
	 Button((200,125, 48, 48), bg='up',				cb=flowCallback,	value=1),
	 Button((260,125, 48, 48), bg='down',			cb=flowCallback, 	value=0),
	 Button((180, 60,128, 48), bg='off', 			cb=onOffCallback, 	value=0),
	 Button((180, 60,128, 48), bg='on',  			cb=onOffCallback, 	value=1)],

	# Screen 1 for changing settings
	[Button((260, 10, 48, 48), bg='cog',   			cb=settingsCallback, 	value=1),
	 Button((260, 70, 48, 48), bg='cog',   			cb=settingsCallback, 	value=2),
	 Button((260,130, 48, 48), bg='cog',   			cb=restartServices, 	value=1),
	 Button(( 96,180,128, 48), bg='ok',    			cb=settingsCallback, 	value=-1)],

	# Screen 2 for numeric input
	[Button((col[0],row[0],320, 48), bg='box'),
	 # Row 1
	 Button((col[0],row[1], 48, 48), bg='1',     	cb=numericCallback, value= 1),
	 Button((col[1],row[1], 48, 48), bg='2',     	cb=numericCallback, value= 2),
	 Button((col[2],row[1], 48, 48), bg='3',     	cb=numericCallback, value= 3),
	 Button((col[3],row[1], 128, 48), bg='ok',    	cb=numericCallback, value=12),
	 # Row 2
	 Button((col[0],row[2], 48, 48), bg='4',     	cb=numericCallback, value= 4),
	 Button((col[1],row[2], 48, 48), bg='5',     	cb=numericCallback, value= 5),
	 Button((col[2],row[2], 48, 48), bg='6',     	cb=numericCallback, value= 6),
	 # Row 3
	 Button((col[0],row[3], 48, 48), bg='7',     	cb=numericCallback, value= 7),
	 Button((col[1],row[3], 48, 48), bg='8',     	cb=numericCallback, value= 8),
	 Button((col[2],row[3], 48, 48), bg='9',    	cb=numericCallback, value= 9),
	 # Row 4
	 Button((col[0],row[4], 48, 48), bg='dot',		cb=numericCallback, value=13),
	 Button((col[1],row[4], 48, 48), bg='0',     	cb=numericCallback, value= 0),
	 Button((col[2],row[4], 48, 48), bg='del',   	cb=numericCallback, value=10),
	 Button((col[3],row[4], 128, 48), bg='cancel',	cb=numericCallback, value=11)]
]


################ UTILITY FUNCTIONS ################
def saveSettings():
	global v 
	try:
		ipfile = open(ipLocation, 'w')
		ipfile.write(serverIP)
		ipfile.close()
	except:
		pass

def loadSettings():
	global v
	global serverIP
	try:
		ipfile = open(ipLocation, 'r')
		serverIP = ipfile.readline()
		ipfile.close()
	except:
		pass


##################################### INITIALIZTION #####################################
# Init framebuffer/touchscreen environment variables
os.putenv('SDL_VIDEODRIVER', 'fbcon')
os.putenv('SDL_FBDEV'      , '/dev/fb1')
os.putenv('SDL_MOUSEDRV'   , 'TSLIB')
os.putenv('SDL_MOUSEDEV'   , '/dev/input/touchscreen')

# Init pygame and screen
print "Initting..."
pygame.init()
print "Setting Mouse invisible..."
pygame.mouse.set_visible(False)
print "Setting fullscreen..."
modes = pygame.display.list_modes(16)
screen = pygame.display.set_mode([320,240])

print "Loading Icons..."
# Load all icons at startup.
for file in os.listdir(iconPath):
	if fnmatch.fnmatch(file, '*.png'):
		icons.append(Icon(file.split('.')[0]))
# Assign Icons to Buttons, now that they're loaded
print"Assigning Buttons"
for s in buttons:        # For each screenful of buttons...
	for b in s:            #  For each button on screen...
		for i in icons:      #   For each icon...
			if b.bg == i.name: #    Compare names; match?
				b.iconBg = i     #     Assign Icon to Button
				b.bg     = None  #     Name no longer used; allow garbage collection
			if b.fg == i.name:
				b.iconFg = i
				b.fg     = None
buttons[0].pop() #Remove the ON button from main screen to start

loadSettings() # Loads IP address from file
print "loading background.."
img = pygame.image.load("icons/vice.png")
if img is None or img.get_height() < 240: # Letterbox, clear background
	screen.fill(0)
if img:
	screen.blit(img,
		((320 - img.get_width() ) / 2,
		 (240 - img.get_height()) / 2))
pygame.display.update()
sleep(1)

########################################## SUPERLOOP ##########################################
print "In Superloop.."
while(True):

	# Process touchscreen input
	while True:
		for event in pygame.event.get():
			if(event.type is MOUSEBUTTONDOWN):
				pos = pygame.mouse.get_pos()
				for b in buttons[screenMode]:
					if b.selected(pos): break
		# Stay in this while loop as long as there is no screen updates
		if updateScreen == 1 or screenMode != screenModePrior: break
		sleep(0.1)


	if img is None or img.get_height() < 240: # Letterbox, clear background
		screen.fill(0)
	if img:
		screen.blit(img,
			((320 - img.get_width() ) / 2,
			 (240 - img.get_height()) / 2))

	# Overlay buttons on display and update
	for i,b in enumerate(buttons[screenMode]):
		b.draw(screen)

	################## Draw Fonts for Screen 2 ##################
	if screenMode == 2:
		myfont = pygame.font.SysFont("Arial", 42, True)
		label = myfont.render(displayString, 1, (200,200,200))
		screen.blit(label, (10, 2))

	################## Draw Fonts for Screen 1 ##################
	elif screenMode == 1:
		myfont = pygame.font.SysFont("Arial", 16, True)
		label = myfont.render("Server IP:" , 1, (200,200,200))
		screen.blit(label, (10, 20))
		label = myfont.render("Lock State:" , 1, (200,200,200))
		screen.blit(label, (10, 80))
		label = myfont.render("Restart Services:" , 1, (200,200,200))
		screen.blit(label, (10, 140))

		label = myfont.render(serverIP, 1, (200,200,200))
		screen.blit(label, (100, 20))

		if webLockState == 0:
			label = myfont.render("Locked" , 1, (191,0,0))
		else:
			label = myfont.render("Unlocked",1, (0,191,0))
		screen.blit(label, (100, 80))

	################## Draw Fonts for Screen 0 ##################
	elif screenMode == 0:

		myfont = pygame.font.SysFont("Arial", 38, True)
		label = myfont.render("VFD Controller" , 1, (200,200,200))
		screen.blit(label, (35, 2))
		myfont = pygame.font.SysFont("Arial", 24, True)
		label = myfont.render("Pump is" , 1, (200,200,200))
		screen.blit(label, (10, 65))
		label = myfont.render("Rate:" , 1, (200,200,200))
		screen.blit(label, (10,130))

		if currentState == 0: #OFF
			label = myfont.render("OFF", 1, (191,0,0))
		else:
			label = myfont.render("ON", 1, (0,191,0))

		screen.blit(label, (125, 65))

		label = myfont.render( str(flow) + ' gal/min' , 1, (200,200,200))
		screen.blit(label, (75, 130))

	pygame.display.update()
	updateScreen = 0
	screenModePrior = screenMode

