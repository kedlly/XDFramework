# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: Message.proto

import sys
_b=sys.version_info[0]<3 and (lambda x:x) or (lambda x:x.encode('latin1'))
from google.protobuf.internal import enum_type_wrapper
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from google.protobuf import reflection as _reflection
from google.protobuf import symbol_database as _symbol_database
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()




DESCRIPTOR = _descriptor.FileDescriptor(
  name='Message.proto',
  package='Protocal.Transport',
  syntax='proto2',
  serialized_options=None,
  serialized_pb=_b('\n\rMessage.proto\x12\x12Protocal.Transport\"\x8d\x01\n\x0b\x44\x61taPackage\x12\x37\n\tmajorType\x18\x01 \x02(\x0e\x32$.Protocal.Transport.MessageMajorType\x12\x37\n\tminorType\x18\x02 \x02(\x0e\x32$.Protocal.Transport.MessageMinorType\x12\x0c\n\x04\x64\x61ta\x18\x03 \x01(\x0c*8\n\x10MessageMajorType\x12\n\n\x06Unknow\x10\x00\x12\x0b\n\x07Request\x10\x01\x12\x0b\n\x07Respond\x10\x02*\x80\x01\n\x10MessageMinorType\x12\r\n\tUndefined\x10\x00\x12\r\n\tLoginAuth\x10\x01\x12\n\n\x06Moving\x10\x02\x12\x12\n\x0ePlayerAppeared\x10\x03\x12\x15\n\x11PlayerDisappeared\x10\x04\x12\x0b\n\x07Rotated\x10\x05\x12\n\n\x06Logout\x10\x06')
)

_MESSAGEMAJORTYPE = _descriptor.EnumDescriptor(
  name='MessageMajorType',
  full_name='Protocal.Transport.MessageMajorType',
  filename=None,
  file=DESCRIPTOR,
  values=[
    _descriptor.EnumValueDescriptor(
      name='Unknow', index=0, number=0,
      serialized_options=None,
      type=None),
    _descriptor.EnumValueDescriptor(
      name='Request', index=1, number=1,
      serialized_options=None,
      type=None),
    _descriptor.EnumValueDescriptor(
      name='Respond', index=2, number=2,
      serialized_options=None,
      type=None),
  ],
  containing_type=None,
  serialized_options=None,
  serialized_start=181,
  serialized_end=237,
)
_sym_db.RegisterEnumDescriptor(_MESSAGEMAJORTYPE)

MessageMajorType = enum_type_wrapper.EnumTypeWrapper(_MESSAGEMAJORTYPE)
_MESSAGEMINORTYPE = _descriptor.EnumDescriptor(
  name='MessageMinorType',
  full_name='Protocal.Transport.MessageMinorType',
  filename=None,
  file=DESCRIPTOR,
  values=[
    _descriptor.EnumValueDescriptor(
      name='Undefined', index=0, number=0,
      serialized_options=None,
      type=None),
    _descriptor.EnumValueDescriptor(
      name='LoginAuth', index=1, number=1,
      serialized_options=None,
      type=None),
    _descriptor.EnumValueDescriptor(
      name='Moving', index=2, number=2,
      serialized_options=None,
      type=None),
    _descriptor.EnumValueDescriptor(
      name='PlayerAppeared', index=3, number=3,
      serialized_options=None,
      type=None),
    _descriptor.EnumValueDescriptor(
      name='PlayerDisappeared', index=4, number=4,
      serialized_options=None,
      type=None),
    _descriptor.EnumValueDescriptor(
      name='Rotated', index=5, number=5,
      serialized_options=None,
      type=None),
    _descriptor.EnumValueDescriptor(
      name='Logout', index=6, number=6,
      serialized_options=None,
      type=None),
  ],
  containing_type=None,
  serialized_options=None,
  serialized_start=240,
  serialized_end=368,
)
_sym_db.RegisterEnumDescriptor(_MESSAGEMINORTYPE)

MessageMinorType = enum_type_wrapper.EnumTypeWrapper(_MESSAGEMINORTYPE)
Unknow = 0
Request = 1
Respond = 2
Undefined = 0
LoginAuth = 1
Moving = 2
PlayerAppeared = 3
PlayerDisappeared = 4
Rotated = 5
Logout = 6



_DATAPACKAGE = _descriptor.Descriptor(
  name='DataPackage',
  full_name='Protocal.Transport.DataPackage',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='majorType', full_name='Protocal.Transport.DataPackage.majorType', index=0,
      number=1, type=14, cpp_type=8, label=2,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='minorType', full_name='Protocal.Transport.DataPackage.minorType', index=1,
      number=2, type=14, cpp_type=8, label=2,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='data', full_name='Protocal.Transport.DataPackage.data', index=2,
      number=3, type=12, cpp_type=9, label=1,
      has_default_value=False, default_value=_b(""),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto2',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=38,
  serialized_end=179,
)

_DATAPACKAGE.fields_by_name['majorType'].enum_type = _MESSAGEMAJORTYPE
_DATAPACKAGE.fields_by_name['minorType'].enum_type = _MESSAGEMINORTYPE
DESCRIPTOR.message_types_by_name['DataPackage'] = _DATAPACKAGE
DESCRIPTOR.enum_types_by_name['MessageMajorType'] = _MESSAGEMAJORTYPE
DESCRIPTOR.enum_types_by_name['MessageMinorType'] = _MESSAGEMINORTYPE
_sym_db.RegisterFileDescriptor(DESCRIPTOR)

DataPackage = _reflection.GeneratedProtocolMessageType('DataPackage', (_message.Message,), dict(
  DESCRIPTOR = _DATAPACKAGE,
  __module__ = 'Message_pb2'
  # @@protoc_insertion_point(class_scope:Protocal.Transport.DataPackage)
  ))
_sym_db.RegisterMessage(DataPackage)


# @@protoc_insertion_point(module_scope)