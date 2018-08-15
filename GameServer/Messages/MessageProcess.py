#coding:utf8
from Messages_pb2 import *
from Bussiness.Singleton import Singleton
from MessageMap import Serialize

from Bussiness.Delay import Delay
from Bussiness.PlayerManager import *

def verifyLogin(username, password):
	return True

class MessageProcessor(object):
	
	__metaclass__ = Singleton
	
	def __init__(self):
		self.__processors = {}
		self.__processors[Request_LoginAuth] = self.onLogin
		self.__processors[Request_Moving] = self.onMovement
	
	def onReceived(self, link, data):
		datatype = type(data)
		if self.__processors.has_key(datatype):
			self.__processors[datatype](link, data)
			
	def onLogout(self, link, data):
		if data is None:
			msg = Respond_Logout()
			if link.player is None:
				return
			msg.pid = link.player.ID
			for player in PlayerManager().Players:
				player.link.transport.write(Serialize(msg))
	
	def onLogin(self, link, data):
		print data.username, data.password
		print ("user : %s, request to login ." % data.username)
		result = verifyLogin(data.username, data.password)
		rpa = Respond_PlayerAppeared()
		if result:
			r_la = Respond_LoginAuth()
			link.createPlayer()
			link.player.Name = data.username
			# r_la.player.pid = link.player.ID
			# r_la.player.name = data.username
			# r_la.player.state.position.CopyFrom(PyVector3.default().toProtoVector3())
			r_la.player.CopyFrom(link.player.toProtoPlayer())
			rn = rpa.neighborhood.add()
			rn.CopyFrom(r_la.player)
			r_la.neighborhood.extend([v.toProtoPlayer() for v in link.player.getNeighborhoods()])
			r_la.token = "accessToken=%d" % link.player.ID
			r_la.sceneId = 0
			#r_la.ClearField("neighborhood")
			link.transport.write(Serialize(r_la))
		for p in PlayerManager().Players:
			if p.ID != link.player.ID:
				p.link.transport.write(Serialize(rpa))
			
	def onMovement(self, link, data):
		if link.player is not None:
			link.player.state.movement.position = PyVector3.fromProtoVector3(data.movement.position)
			link.player.state.movement.velocity = PyVector3.fromProtoVector3(data.movement.velocity)
			print link.player.ID, "movement updated.", link.player.state.movement.position
			msg = Respond_Moving()
			md = msg.movementList.add()
			md.pid = link.player.ID
			md.movement.CopyFrom(data.movement)
			for player in PlayerManager().Players:
				player.link.transport.write(Serialize(msg))
		