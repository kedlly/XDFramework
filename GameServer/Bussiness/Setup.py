


import datetime,time

__timeSinceStart = datetime.datetime.now()
def getElapsedTime():
	global __timeSinceStart
	return datetime.datetime.now() - __timeSinceStart


FRAME_STEP = 100

__timeStart = time.clock()
def frameProcess(caller):
	global __timeStart
	__now = time.clock()
	delta = __now - __timeStart
	if delta > FRAME_STEP:
		__timeStart = __now
		caller()

def initialize(framestep):
	global FRAME_STEP
	FRAME_STEP = framestep

def run(caller):
	while 1:
		frameProcess(caller)