from Messages_pb2 import *


def createMap():
	keys =  EMessageType.keys()
	values = EMessageType.values()
	m_E_CLS = {}
	m_CLS_E = {}
	gl = globals()
	for val in values:
		enumName = EMessageType.Name(val)
		className = enumName[1:]
		cls = gl[className]
		m_E_CLS[val] = cls
		m_CLS_E[cls] = val
	return m_E_CLS, m_CLS_E
		
MAP_E_CLS, MAP_CLS_E = createMap()


def createMessage(dataMessage):
	dataType = type(dataMessage)
	if not MAP_CLS_E.has_key(dataType):
		raise "Illegal dataMessage type"
	msg = Message()
	msg.type = MAP_CLS_E[dataType]
	msg.data = dataMessage.SerializeToString()
	return msg

def parseMessage(message):
	msgType = message.type
	if not MAP_E_CLS.has_key(msgType):
		raise "Illegal Message data"
	cls = MAP_E_CLS[msgType]
	obj = cls()
	obj.ParseFromString(message.data)
	return obj

def Serialize(requestOrResponseMessage):
	try:
		msg = createMessage(requestOrResponseMessage)
		return msg.SerializeToString()
	except Exception as e:
		print e, "while Serialize "
		return None

def Deserialize(binData):
	try:
		msg = Message()
		msg.ParseFromString(binData)
		return parseMessage(msg)
	except:
		return None