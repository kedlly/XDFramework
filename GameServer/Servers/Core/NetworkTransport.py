from twisted.internet import protocol

from Core.DataReader import DataReceivedFSM

class CommunicationProtocol(protocol.Protocol):
	
	def __init__(self):
		self._receiveProcess = DataReceivedFSM(self.__onPackageDataReceived)
	
	def connectionMade(self):
		try:
			self._onConnectionMade()
		except Exception as e:
			print e
	
	def connectionLost(self, reason):
		try:
			self._onConnectionLost(reason)
		except Exception as e:
			print e
		print reason
	
	def dataReceived(self, data):
		self._onDataReceived(data)
		# print data, self.transport.getTcpKeepAlive(), self.transport.getTcpNoDelay()
		if data == '1\n':
			self.transport.abortConnection()
		# #self.transport.write(data)
		# print data
		try:
			self._receiveProcess.receiveData(data)
		except Exception as e:
			print e
	
	def _onConnectionMade(self):
		pass
	
	def _onConnectionLost(self, reason):
		pass
	
	def __onPackageDataReceived(self, packageData):
		from Core.MessageMap import Deserialize
		request = Deserialize(packageData)
		self._onDataReceived(request)
	
	def _onDataReceived(self, protoData):
		pass

class ServerFactory(protocol.ServerFactory):
	
	def __init__(self, service):
		self._service = service


# def Setup(protocolClass, factoryClass):
# 	if factoryClass is None:
# 		factoryClass = type(protocolClass, superclasses, attributes_dict)
# 	factoryClass.protocol = protocolClass