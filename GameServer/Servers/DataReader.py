#coding:utf8

import struct
from zope.interface import implementer, Attribute, Interface

class IState(Interface):
	
	length = Attribute('length')
	data = Attribute('data')
	remainData = Attribute('remain data')
	
	def read(data):
		pass
	
	def toNext():
		pass

@implementer(IState)
class State_ReadLength(object):
	
	def __init__(self):
		self._length = 0
		self._data = None
		
	@property
	def length(self):
		return self._length
	
	@property
	def data(self):
		return None
	
	def read(self, data):
		if len(data) < 4:
			return
		head = data[0:4]
		u = struct.unpack('!i', head)
		self._length = u[0]
		self._data = data[4:]
	
	@property
	def remainData(self):
		return self._data if len(self._data) > 0 else None
	
	def toNext(self):
		return State_ReadData(self.length) if self.length > 0 else self
		

@implementer(IState)
class State_ReadData(object):
	
	def __init__(self, exceptLength = -1):
		self._length = 0
		self._data = b''
		self._exceptLength = exceptLength
		self._remian = None
	
	@property
	def length(self):
		return self._length
	
	@property
	def data(self):
		return self._exceptLength == 0 and self._data or None
	
	@property
	def remainData(self):
		return self._remian
	
	def read(self, data):
		lengthOfData = len(data)
		if lengthOfData > self._exceptLength:
			self._data += data[0:self._exceptLength]
			self._remian = data[self._exceptLength:]
			self._exceptLength = 0
		else:
			self._data += data[0:]
			self._exceptLength -= lengthOfData
			self._remian = ''
		if len(self._remian) == 0:
			self._remian = None
	
	def toNext(self):
		return State_ReadLength() if self._remian is None else self
	

class DataReceivedFSM(object):

	
	def __init__(self, dataReceivedCallback = None):
		self._state = State_ReadLength()
		self._callback = dataReceivedCallback
	
	def receiveData(self, data):
		remain = data
		while remain is not None:
			self._state.read(remain)
			if self._callback is not None and self._state.data is not None:
				self._callback(self._state.data)
			remain = self._state.remainData
			self._state = self._state.toNext()
	

def toPackageHead(head):
	import binascii
	import struct
	print str(binascii.b2a_hex(head))
	print map(ord, head);
	u = struct.unpack('<i', head)
	print u
	return u[0];