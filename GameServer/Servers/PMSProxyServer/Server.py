# coding:utf8

from twisted.internet import protocol, reactor
from twisted.internet import task

from Servers.Core.NetworkTransport import CommunicationProtocol, ServerFactory


class PMSProxyProtocol(CommunicationProtocol):
	
	def _onConnectionLost(self, reason):
		pass
	
	def _onDataReceived(self, protoData):
		pass
	
	def _onConnectionMade(self):
		pass


class PMSProxyFactory(ServerFactory):
	
	protocol = PMSProxyProtocol
	
	def __init__(self):
		ServerFactory.__init__(self, None)


class TCPServer(object):
	
	def __init__(self, tcpPort):
		self.port = tcpPort
	
	def start(self):
		# service = Service()
		factory = PMSProxyFactory()
		reactor.listenTCP(self.port, factory)
		# l = task.LoopingCall(service.update)
		# l.start(Service.UpdateFrequency)
		reactor.run()
		pass