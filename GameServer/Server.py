#coding:utf8
from Servers.LogicServer import LogicServer

import Configure

if __name__ == "__main__":
	config = Configure.getConfigure("config.ini")
	ipAddress = config['server'][0]
	port = int(config['server'][1])
	print "listening: %s, port : %d" % (ipAddress, port)
	LogicServer(port).start()