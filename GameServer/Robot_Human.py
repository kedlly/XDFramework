#coding:utf8

from twisted.internet import reactor, task
from twisted.internet.protocol import ClientFactory, Protocol
from Messages.MessageMap import Serialize, Deserialize
from Messages.RequestMessages_pb2 import *
from Messages.RespondMessages_pb2 import *
from Bussiness.Delay import Delay
from Bussiness.PlayerManager import PyVector3

import optparse

def parse_args():
	usage = """usage: %prog [options] [hostname]:port ...
This is the Get Poetry Now! client, Twisted version 8.0
Run it like this:
  python Robot.py xform-port port1 port2 ...
If you are in the base directory of the twisted-intro package,
you could run it like this:
  python Robot.py 10001 10002 10003
to grab poetry from servers on ports 10002, and 10003 and transform
it using the server on port 10001.
Of course, there need to be appropriate servers listening on those
ports for that to work.
"""

	parser = optparse.OptionParser(usage)

	parser.add_option("-t", "--type", action = 'store', type = 'string', default = '', dest = 'playerType')
	parser.add_option('-n', '--name', action = 'store', type = 'string', default = '' ,dest = 'name')
	parser.add_option('-p', '--password', action = 'store', type = 'string', default = '',dest = 'pwd')
	#parser.add_option('-p', '--password', action='store', type='string', dest='pwd')
	options, addresses = parser.parse_args()

	if len(addresses) < 2:
		#print parser.format_help()
		#parser.exit()
		addresses = "127.0.0.1:8192"

	def parse_address(addr):
		if ':' not in addr:
			host = '127.0.0.1'
			port = addr
		else:
			host, port = addr.split(':', 1)

		if not port.isdigit():
			parser.error('Ports must be integers.')

		return host, int(port)

	return options, parse_address(addresses)


class RobotProtocol(Protocol):

	def connectionMade(self):
		print "request login ...."
		self.transport.write(login())
		global transport
		transport = self.transport
		
	def dataReceived(self, data):
		respond = Deserialize(data)
		global processors
		if processors.has_key(type(respond)):
			processors[type(respond)](respond)

	def connectionLost(self, reason):
		print "----lost connection :", reason

		
	
class RobotClientFactory(ClientFactory):
	protocol = RobotProtocol
	
	def startedConnecting(self, connector):
		print('Started to connect.')
		print connector
	
	def clientConnectionLost(self, connector, reason):
		print('Lost connection.  Reason:', reason)
		if reactor.running:
			reactor.stop()
	
	def clientConnectionFailed(self, connector, reason):
		print('Connection failed. Reason:', reason)
		if reactor.running:
			reactor.stop()

transport = None

def login():
	from Messages.RequestMessages_pb2 import Request_LoginAuth
	import os
	username = _NAME_PREFIX + str(os.getpid())
	pwd = "password"
	login_request = Request_LoginAuth()
	login_request.username = username
	login_request.password = pwd
	return Serialize(login_request)

__velocity = PyVector3()
__position = PyVector3()

def onLoginRespond(data):
	if data.player.pid == -1:
		print "login failed, username or password error"
		Delay().schedule(5, reactor.stop)
	else:
		print "login succeed, pid = " + str(data.player.pid) + " accessToken = " + data.token
		global __velocity, __position
		__velocity = PyVector3.fromProtoVector3(data.player.movement.velocity)
		__position = PyVector3.fromProtoVector3(data.player.movement.position)
		Delay().schedule(240, transport.write, "1\n")
		Delay().schedule(1, setDir, PyVector3(0,0,-3.5))
		Delay().schedule(0.75, reportMovement)
		
		

def reportMovement():
	from Messages.RawData.InternalData_pb2 import MovementData
	try:
		md = MovementData()
		md.position.CopyFrom(__position.toProtoVector3())
		md.velocity.CopyFrom(__velocity.toProtoVector3())
		rm = Request_Moving()
		rm.movement.CopyFrom(md)
		transport.write(Serialize(rm))
	except Exception as e:
		print e
	Delay().schedule(0.75, reportMovement)

def setDir(dir):
	global __velocity
	__velocity = dir
	Delay().schedule(8, setDir, dir * -1)
	

processors = {}
processors[Respond_LoginAuth] = onLoginRespond

#----------------------------------------------------------------------------------------------------------------------------


			
#----------------------------------------------------------------------------------------------------------------------------

UpdateFrequency = 0.25

def walk():
	global __velocity, __position
	__position += __velocity * UpdateFrequency

def update():
	Delay().update(UpdateFrequency)
	walk()
	pass

#__opitions, addr_port =parse_args()

_NAME_PREFIX = ''

if __name__ == "__main__":
	factory = RobotClientFactory()
	reactor.connectTCP("127.0.0.1", 8192, factory)
	l = task.LoopingCall(update)
	l.start(UpdateFrequency)
	reactor.run()

