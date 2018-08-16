from Messages.Message_pb2 import *
from Messages.RequestMessages_pb2 import *
from Messages.RespondMessages_pb2 import *

def Serialize(requestOrResponseMessage):
	try:
		msg = pack(requestOrResponseMessage)
		return msg.SerializeToString()
	except Exception as e:
		print e, "Error : Serialize "
		return None

def Deserialize(binData):
	try:
		dataPackage = DataPackage()
		dataPackage.ParseFromString(binData)
		return unpack(dataPackage)
	except:
		return None

def getType(obj):
	objType = type(obj)
	return objType, str(objType).split("'")[1].split(".")[-1]

def getMessageTypePair(typeName, splitChar='_'):
	_major, _minor = typeName.split(splitChar)
	if _major in MessageMajorType.keys():
		_major = MessageMajorType.Value(_major)
	else:
		raise Exception("unsupported major message type. for message : %s" % typeName)
	if _minor in MessageMinorType.keys():
		_minor = MessageMinorType.Value(_minor)
	else:
		raise Exception("unsupported minor message type. for message : %s" % typeName)
	return _major, _minor

def getMessageTypeSN(pair, splitChar='_'):
	_major, _minor = pair
	if _major in MessageMajorType.values():
		_major = MessageMajorType.Name(_major)
	else:
		raise Exception("unsupported major message value. value is : %d" % _major)
	if _minor in MessageMinorType.values():
		_minor = MessageMinorType.Name(_minor)
	else:
		raise Exception("unsupported minor message value. value is : %d" % _minor)
	return _major + splitChar + _minor

EnPair_To_Type = {}
Type_To_EnPair = {}

def CreateMessageMap():
	gl = globals()
	for i in MessageMajorType.values():
		for j in MessageMinorType.values():
			_tuple = (i, j)
			if i == Unknow or j == Undefined:
				continue
			_mtsn = getMessageTypeSN(_tuple)
			if gl.has_key(_mtsn):
				EnPair_To_Type[_tuple] = gl[_mtsn]
				Type_To_EnPair[gl[_mtsn]] = _tuple
			else:
				print "No used Message Type : %s" % _mtsn

def pack(dataMessage):
	''' pack Request/Respond/..etc (dataMessage) message to Message object '''
	_msgType = type(dataMessage)
	if not Type_To_EnPair.has_key(_msgType):
		raise Exception("unsupported data message. type is : %s" % str(_msgType))
	_major, _minor = Type_To_EnPair[_msgType]
	msg = DataPackage()
	msg.majorType = _major
	msg.minorType = _minor
	msg.data = dataMessage.SerializeToString()
	return msg

def unpack(dataPackage):
	''' unpack Message object (msg) to Request/Respond or other message'''
	_msgType = type(dataPackage)
	if _msgType is not DataPackage:
		raise Exception("msg must be instance of Type : Message, but : %s" % str(_msgType))
	_typeTuple = (dataPackage.majorType, dataPackage.minorType)
	if not EnPair_To_Type.has_key(_typeTuple):
		raise Exception("unsupported message. Major & Minor Type : %s & %s" % (
		MessageMajorType.Name(dataPackage.majorType), MessageMinorType.Name(dataPackage.minorType)))
	_cls = EnPair_To_Type[_typeTuple]
	_object = _cls()
	_object.ParseFromString(dataPackage.data)
	return _object

#--------------------------------------------------------------------------------------------------
CreateMessageMap()