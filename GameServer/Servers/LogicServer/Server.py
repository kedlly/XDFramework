# coding:utf8

from twisted.internet import reactor
from twisted.internet import task

from Bussiness.Service import Service, Link
from Servers.LogicServer.Bussiness.MessageProcess import MessageProcessor
from Servers.Core.NetworkTransport import CommunicationProtocol, ServerFactory

class LogicServerProxyProtocol(CommunicationProtocol):
	
	def _onConnectionMade(self):
		#self.deferred = self.factory.service.getDeferred()
		#self.deferred.addCallback(self.transport.write)
		#self.deferred.addBoth(lambda r: self.transport.loseConnection())
		self.transport.setTcpKeepAlive(True)
		self.transport.setTcpNoDelay(True)
		self.link = Link(self.transport)
		self.factory._service.addLink(self.link)
		print "new link in ..."
	
	def _onConnectionLost(self, reason):
		self.factory._service.removeLink(self.link)
	
	def _onDataReceived(self, data):
		MessageProcessor().onReceived(self.link, data)
		

class LogicServerFactory(ServerFactory):
	
	protocol = LogicServerProxyProtocol
	
	def __init__(self, service):
		ServerFactory.__init__(self, service)


class TCPServer(object):
	
	def __init__(self, tcpPort):
		self.port = tcpPort
		
	def start(self):
		service = Service()
		factory = LogicServerFactory(service)
		reactor.listenTCP(self.port, factory)
		l = task.LoopingCall(service.update)
		l.start(Service.UpdateFrequency)
		reactor.run()
