#coding:utf8

from Singleton import Singleton


class PyVector3(object):
	
	def __init__(self, *args, **kwargs):
		
		self._x = 0
		self._y = 0
		self._z = 0
		
		if len(args) > 0:
			if len(args) == 1:
				self._x,  = args
			elif len(args) == 2:
				self._x, self._y = args
			else:
				self._x, self._y, self._z = args
				
		if len (kwargs) > 0 :
			self._x = kwargs.get("x", self._x)
			self._y = kwargs.get("y", self._y)
			self._z = kwargs.get("z", self._z)
	
	@property
	def x(self):
		return self._x
	
	@x.setter
	def x(self, value):
		self._x = value
		
	@property
	def y(self):
		return self._y
	
	@y.setter
	def y(self, value):
		self._y = value
	
	@property
	def z(self):
		return self._z
	
	@z.setter
	def z(self, value):
		self._z = value
	
	def __add__(self, other):
		return PyVector3(self.x + other.x, self.y + other.y, self.z + other.z)
	
	def __mul__(self, other):
		otherType = type(other)
		if otherType == PyVector3:
			return self.x * other.x + self.y * other.y + self.z * other.z
		elif otherType == float or otherType == int:
			return PyVector3(other * self.x, other * self.y, other * self.z)
		else:
			print self, "error multiply for", other
	
	def __rmul__(self, other):
		return self * other
	
	def __str__(self):
		return "PyVector3: x=%f, y=%f, z=%f"%(self.x, self.y, self.z)
	
	@staticmethod
	def default():
		return PyVector3(-0.02612079, -1.65, 3.404987)
	
	def toProtoVector3(self):
		from Messages.RawData.InternalData_pb2 import Vector3
		v = Vector3()
		v.x, v.y, v.z = self.x, self.y, self.z
		return v
	
	@staticmethod
	def fromProtoVector3(protoVector3):
		return PyVector3(protoVector3.x, protoVector3.y, protoVector3.z)
	
class Movement(object):
	
	def __init__(self):
		self._position = PyVector3.default()
		self._velocity = PyVector3()
	
	@property
	def position(self):
		return self._position
	
	@position.setter
	def position(self, value):
		if type(value) == PyVector3:
			self._position = value
		else:
			print "position must be PyVector3 object"
	
	@property
	def velocity(self):
		return self._velocity
	
	@velocity.setter
	def velocity(self, value):
		if type(value) == PyVector3:
			self._velocity = value
		else:
			print "velocity must be PyVector3 object"
			
class PlayerInfo(object):
	
	def __init__(self):
		self._movement = Movement()
	
	@property
	def movement(self):
		return self._movement

class Player(object):
	
	def __init__(self):
		self.__id = -1
		self._link = None
		self._state = PlayerInfo()
		self._name = ""
		self._time = 0
	
	@property
	def ID(self):
		return self.__id
	
	@ID.setter
	def ID(self, value):
		self.__id = value
	
	@property
	def Name(self):
		return self._name
	
	@Name.setter
	def Name(self, value):
		self._name = value
	
	@property
	def link(self):
		return self._link
	
	@link.setter
	def link(self, value):
		self._link = value
	
	@property
	def state(self):
		return self._state
	
	@staticmethod
	def create(id = -1):
		return PlayerManager().GetPlayer(id)
	
	def getNeighborhoods(self):
		return (player for player in PlayerManager().Players if player.ID != self.ID)
	
	def toProtoPlayer(self):
		import Messages.RawData.InternalData_pb2 as protoRawData
		pi = protoRawData.PlayerInfo()
		pi.pid = self.ID
		pi.name = self.Name
		snu = self.Name.lower()[0:3]
		pi.playerType = protoRawData.EPT_UAV if snu == "uav" else protoRawData.EPT_ROBOT if snu == 'rob' else protoRawData.EPT_HUMAN
		pi.movement.position.CopyFrom(self._state.movement.position.toProtoVector3())
		pi.movement.velocity.CopyFrom(self._state.movement.velocity.toProtoVector3())
		return pi
	
	def update(self):
		from Service import Service
		self._time += Service.UpdateFrequency
		self.state.movement.position += self.state.movement.velocity * Service.UpdateFrequency
		

class PlayerManager(object):
	
	__metaclass__ = Singleton
	
	def __init__(self):
		self.__players = {}
	
	def addPlayer(self, player):
		if not self.__players.has_key(player.ID):
			self.__players[player.ID] = player
		
	def __getNewId(self):
		import time
		newId = int(time.time())
		while 1:
			if not self.__players.has_key(newId):
				break
			newId = int(time.time())
		return newId
		
	def GetPlayer(self, id = -1):
		id = self.__getNewId() if id == -1 else id
		if self.__players.has_key(id):
			return self.__players[id]
		player = Player()
		player.ID = id
		self.__players[id] = player
		return player
	
	@property
	def Players(self):
		return self.__players.itervalues()
	
	def update(self):
		for player in self.__players.itervalues():
			player.update()
	
	def remove(self, *players):
		for player in players:
			self.removePlayer(player)
	
	def removePlayer(self, player):
		self.__players.pop(player.ID, None)
		
	@property
	def counts(self):
		return len(self.__players)
	