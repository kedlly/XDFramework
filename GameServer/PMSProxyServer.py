#coding:utf8
from Servers.PMSProxyServer import *

from Configure import PMS_PROXY_SERVER_IP, PMS_PROXY_SERVER_PORT

from Servers.PMSProxyServer.Database.Tables import *
from Servers.PMSProxyServer.DBSession import DBSession

db = DBSession()
dataset = db.query(WorkTicket_Class_One)
for k in db.query(MachineAccount_Performance) : print k.ID
for k in db.query(MachineAccount_BaseInfo)  : print k.ID
for k in db.query(MachineAccount_Breaker_TechnicalParameter)  : print k.ID
for k in db.query(MachineAccount_Defect)  : print k.ID
for k in db.query(MachineAccount_Operation)  : print k.ID
for k in dataset:
	print k.CZ_NAME, k.WORK_WRITE_ID

if __name__ == "__main__":
	ipAddress = PMS_PROXY_SERVER_IP
	port = PMS_PROXY_SERVER_PORT
	print "PMS Proxy Server listening: %s, port : %d" % (ipAddress, port)
	Server.TCPServer(port).start()