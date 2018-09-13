from Core.Singleton import Singleton

class Delay(object):
	__metaclass__ = Singleton
	
	def __init__(self):
		self._defer = set()
		self._add = set()
		self._remove = set()
	
	def schedule(self, delay, func, *args, **kwargs):
		class _task(object):
			def __init__(self, delay, func, *args, **kwargs):
				self.delay = delay
				self.func = func
				self.args = args
				self.kwargs = kwargs
				self.currTime = 0
				self.called = False
			
			def __call__(self, *args, **kwargs):
				if self.func is not None and not self.called:
					try:
						self.func(*self.args, **self.kwargs)
					except Exception as e:
						print e
					self.called = True
			
			def update(self, delta):
				self.currTime += delta
				if self.currTime > self.delay:
					self()
		self._add.add(_task(delay, func, *args, **kwargs))
	
	def update(self, delta):
		self._defer.update(self._add)
		self._add.clear()
		for v in self._defer:
			v.update(delta)
			if v.called:
				self._remove.add(v)
		self._defer.difference_update(self._remove)
		self._remove.clear()