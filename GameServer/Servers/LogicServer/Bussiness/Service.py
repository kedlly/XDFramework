#coding:utf8

from Core.Singleton import Singleton
from PlayerManager import Player, PlayerManager

class Link(object):
	
	def __init__(self, transport):
		self._transport = transport
		#self.players = set()
		self._timeElapse = 0
		self._player = None
		if self._transport is None:
			raise "Error transport"
	
	@property
	def transport(self):
		return self._transport
	
	@property
	def player(self):
		return self._player
		
	def createPlayer(self):
		player = Player.create()
		player.link = self
		#self.players.add(player)
		self._player = player
	
	def update(self):
		# if len(self.players) == 0:
		if self.player is None:
			self._timeElapse += Service.UpdateFrequency
			if self._timeElapse > 30:
				if self.transport is not None:
					self.transport.loseConnection()
					
	
	
class Service(object):
	
	__metaclass__ = Singleton
	
	UpdateFrequency = 0.25
	
	def __init__(self):
		self.__linkpool = set()
		
	def addLink(self, link):
		self.__linkpool.add(link)
	
	def removeLink(self, link):
		try:
			self.__linkpool.remove(link)
			if link.player is not None:
				PlayerManager().remove(link.player)
				from Servers.LogicServer.Bussiness.MessageProcess import MessageProcessor
				MessageProcessor().onLogout(link, None)
			if link.transport.connected:
				link.transport.abortConnection()
		except Exception as e:
			print e
	
	@property
	def counts(self):
		return len(self.__linkpool)
	
	def update(self):
		try:
			for v in self.__linkpool:
				v.update()
			PlayerManager().update()
			if self.counts > 0 :
				print self.counts, PlayerManager().counts
		except Exception as e:
			print e
		
		
	