#coding:utf8
from Servers.PMSProxyServer import *

from Configure import PMS_PROXY_SERVER_IP, PMS_PROXY_SERVER_PORT

if __name__ == "__main__":
	ipAddress = PMS_PROXY_SERVER_IP
	port = PMS_PROXY_SERVER_PORT
	print "PMS Proxy Server listening: %s, port : %d" % (ipAddress, port)
	Server.TCPServer(port).start()