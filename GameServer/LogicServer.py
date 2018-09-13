#coding:utf8
from Servers.LogicServer import *

from Configure import LOGIC_SERVER_IP, LOGIC_SERVER_PORT

if __name__ == "__main__":
	ipAddress = LOGIC_SERVER_IP
	port = LOGIC_SERVER_PORT
	print "Game Logic Server listening: %s, port : %d" % (ipAddress, port)
	
	Server.TCPServer(port).start()