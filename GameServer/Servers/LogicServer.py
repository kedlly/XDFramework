# coding:utf8

from twisted.internet import protocol, reactor
from twisted.internet import task

from Bussiness.Service import Service, Link
from Messages.MessageProcess import MessageProcessor


class LogicServerProxyProtocol(protocol.Protocol):
	
	def connectionMade(self):
		#self.deferred = self.factory.service.getDeferred()
		#self.deferred.addCallback(self.transport.write)
		#self.deferred.addBoth(lambda r: self.transport.loseConnection())
		self.transport.setTcpKeepAlive(True)
		self.transport.setTcpNoDelay(True)
		self.link = Link(self.transport)
		self.factory.service.addLink(self.link)
		print "new link in ..."
	
	def connectionLost(self, reason):
		try:
			self.factory.service.removeLink(self.link)
		except Exception as e:
			print e
		print reason
	
	def dataReceived(self, data):
		#print data, self.transport.getTcpKeepAlive(), self.transport.getTcpNoDelay()
		if data == '1\n':
			self.transport.abortConnection()
		# #self.transport.write(data)
		#print data
		try:
			from Messages.MessageMap import Deserialize
			request = Deserialize(data)
			MessageProcessor().onReceived(self.link, request)
		except Exception as e:
			print e


class LogicServerFactory(protocol.ServerFactory):
	
	protocol = LogicServerProxyProtocol
	
	def __init__(self, service):
		self.service = service
		print self.service

class LogicServer(object):
	
	def __init__(self, tcpPort):
		self.port = tcpPort
		
	def start(self):
		service = Service()
		factory = LogicServerFactory(service)
		reactor.listenTCP(self.port, factory)
		l = task.LoopingCall(service.update)
		l.start(Service.UpdateFrequency)
		reactor.run()