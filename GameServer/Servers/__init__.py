from twisted.python.runtime import platform

def _getInstallFunction():
	try:
		if platform.isLinux():
			try:
				from twisted.internet.epollreactor import install
			except ImportError:
				from twisted.internet.pollreactor import install
		elif platform.getType() == 'posix' and not platform.isMacOSX():
			from twisted.internet.pollreactor import install
		elif platform.isWindows():
			from twisted.internet.iocpreactor import install
		else:
			from twisted.internet.selectreactor import install
	except ImportError:
		from twisted.internet.selectreactor import install
	return install


install = _getInstallFunction()
install()