using System;
using System.Collections.Generic;
using System.IO;
using ProjectProtocal;

namespace Protocals
{
	public static class ProtocalWrapper
    {
		static Dictionary<Type, EMessageType> dict_msg2id = new Dictionary<Type, EMessageType>();
		static Dictionary<EMessageType, Type> dict_id2msg = new Dictionary<EMessageType, Type>();
		static ProtocalWrapper()
		{
			Type enumType = typeof(EMessageType);
			string[] msgIdNames = Enum.GetNames(enumType);
			string msgIdNamespace = enumType.Namespace;
			foreach (var enumName in msgIdNames)
			{
				var msgDataTypeNames = enumName.Substring(1);
				EMessageType enumValue = (EMessageType)Enum.Parse(typeof(EMessageType), enumName);
				Type msgType = Type.GetType(msgIdNamespace + "." + msgDataTypeNames);
				dict_msg2id.Add(msgType, enumValue);
				dict_id2msg.Add(enumValue, msgType);
			}
		}

		public static ProtoBuf.IExtensible DeserializeMessage(byte[] data)
		{
			ProtoBuf.IExtensible retObject = null;
			using (MemoryStream ms = new MemoryStream(data))
			{
				Message msg = ProtoBuf.Serializer.Deserialize<Message>(ms);
				EMessageType type = msg.type;
				Type msgType = dict_id2msg[type];
				var content = Activator.CreateInstance(msgType);
				using (MemoryStream stream = new MemoryStream(msg.data))
				{
					retObject = ProtoBuf.Serializer.Deserialize(msgType, stream) as ProtoBuf.IExtensible;
				}
			}
			return retObject;
		}

		public static byte[] SerializeMessage(ProtoBuf.IExtensible data)
		{
			Type t = data.GetType();
			int id = -1;
			byte[] msgBytes = null;
			if (dict_msg2id.ContainsKey(t))
			{
				id = (int)dict_msg2id[t];
			}
			if (id != -1)
			{
				Message msg = new Message();
				msg.type = (EMessageType)id;
				using (MemoryStream stream = new MemoryStream())
				{
					ProtoBuf.Serializer.Serialize(stream, data);
					msg.data = stream.ToArray();
				}
				using (MemoryStream stream = new MemoryStream())
				{
					ProtoBuf.Serializer.Serialize(stream, msg);
					msgBytes = stream.ToArray();
				}
			}
			return msgBytes;
		}

		public static byte[] SerializeByteArray(this ProtoBuf.IExtensible obj)
		{
			return SerializeMessage(obj);
		}

		public static ProtoBuf.IExtensible DeserializeProtoObject(this byte[] data)
		{
			return DeserializeMessage(data);
		}

		public delegate void MessageHandle(ProtoBuf.IExtensible target);

		public static ProjectProtocal.Vector3 toPV(this UnityEngine.Vector3 vec)
		{
			var pv = new ProjectProtocal.Vector3();
			pv.x = vec.x;
			pv.y = vec.y;
			pv.z = vec.z;
			return pv;
		}

		public static UnityEngine.Vector3 toUV(this ProjectProtocal.Vector3 vec)
		{
			var uv = new UnityEngine.Vector3();
			uv.x = vec.x;
			uv.y = vec.y;
			uv.z = vec.z;
			return uv;
		}
	}
}
