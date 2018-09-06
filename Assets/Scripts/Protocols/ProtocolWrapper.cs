using System;
using System.Collections.Generic;
using System.IO;
using Protocol.Transport;
using UnityEngine;

namespace Protocol
{
	struct DataPackageTypeInfo : IEquatable<DataPackageTypeInfo>
	{
		public const char SplitChar = '_';
		public readonly static string MessageNameSpace = typeof(ProtocolWrapper).Namespace;
		public MessageMajorType MajorType;
		public MessageMinorType MinorType;
		public string MSG_TYPE_NAME
		{
			get
			{
				return GetMessageTypeName(MajorType, MinorType);
			}
		}

		public bool Equals(DataPackageTypeInfo other)
		{
			return this.MajorType == other.MajorType && this.MinorType == other.MinorType;
		}

		public bool IsValid
		{
			get
			{
				return MajorType != MessageMajorType.Unknow && MinorType != MessageMinorType.Undefined;
			}
		}

		public static string GetMessageTypeName(MessageMajorType majorType, MessageMinorType minorType)
		{
			var prefix = Enum.GetName(typeof(MessageMajorType), majorType);
			var ns_prefix = prefix;
			return MessageNameSpace + '.' + ns_prefix + '.' + prefix + SplitChar + Enum.GetName(typeof(MessageMinorType), minorType);
		}

		public static DataPackageTypeInfo ParseFromType<T>() where T : class
		{
			return ParseFromType(typeof(T));
		}

		public static DataPackageTypeInfo ParseFromType(Type type)
		{
			var target = new DataPackageTypeInfo() { MajorType = MessageMajorType.Unknow, MinorType = MessageMinorType.Undefined };
			string typeName = type.Name;
			var items = typeName.Split(SplitChar);
			if (items.Length == 2)
			{
				target.MajorType = (MessageMajorType) Enum.Parse(typeof(MessageMajorType), items[0]);
				target.MinorType = (MessageMinorType) Enum.Parse(typeof(MessageMinorType), items[1]);
				Debug.Assert(target.MSG_TYPE_NAME == type.FullName);
			}
			return target;
		}
	}

	public static class ProtocolHelper
	{
		public static ProtoBuf.IExtensible Unpack(this DataPackage dataPackage)
		{
			return ProtocolWrapper.DataPackage_Unpack(dataPackage);
		}

		public static DataPackage Pack(this ProtoBuf.IExtensible data)
		{
			return ProtocolWrapper.DataPackage_Pack(data);
		}

		public static byte[] Serialize(this DataPackage data)
		{
			byte[] msgBytes = null;
			if (data != null)
			{
				using (MemoryStream stream = new MemoryStream())
				{
					ProtoBuf.Serializer.Serialize(stream, data);
					msgBytes = stream.ToArray();
				}
			}
			return msgBytes;
		}

		public static ArraySegment<byte> Serialize(this DataPackage data, ArraySegment<byte> segment)
		{
			ArraySegment<byte> result = default(ArraySegment<byte>);
			if (data != null)
			{
				using (MemoryStream stream = new MemoryStream(segment.Array, segment.Offset, segment.Count))
				{
					ProtoBuf.Serializer.Serialize(stream, data);
					result = new ArraySegment<byte>(segment.Array, segment.Offset, segment.Offset + (int)stream.Position);
				}
			}
			return result;
		}

		public static DataPackage Deserialize(this byte[] data)
		{
			DataPackage dataPackage = null;
			if (data != null)
			{
				using (MemoryStream ms = new MemoryStream(data))
				{
					dataPackage = ProtoBuf.Serializer.Deserialize<DataPackage>(ms);
				}
			}
			return dataPackage;
		}

		public static DataPackage Deserialize(this ArraySegment<byte> data)
		{
			DataPackage dataPackage = null;
			using (MemoryStream ms = new MemoryStream(data.Array, data.Offset, data.Count))
			{
				dataPackage = ProtoBuf.Serializer.Deserialize<DataPackage>(ms);
			}
			return dataPackage;
		}

		public static RawData.Vector3 ToPV(this Vector3 vec)
		{
			var pv = new RawData.Vector3();
			pv.x = vec.x;
			pv.y = vec.y;
			pv.z = vec.z;
			return pv;
		}

		public static Vector3 ToUV(this RawData.Vector3 vec)
		{
			var uv = new Vector3();
			uv.x = vec.x;
			uv.y = vec.y;
			uv.z = vec.z;
			return uv;
		}

		public static void Process<T>(this ProtoBuf.IExtensible data, Action<T> action) where T : class, ProtoBuf.IExtensible
		{
			T obj = data as T;
			if (obj != null)
			{
				if (action != null)
				{
					action(obj);
				}
			}
			else
			{
				Debug.LogErrorFormat("Error Message Convertion , Type: {0}, Data Type: {1}", typeof(T).FullName, data.GetType().FullName);
			}
		}
	}

	public static class ProtocolWrapper
    {
		static Dictionary<Type, DataPackageTypeInfo> dict_msg2id = new Dictionary<Type, DataPackageTypeInfo>();
		static Dictionary<DataPackageTypeInfo, Type> dict_id2msg = new Dictionary<DataPackageTypeInfo, Type>();

		public static ProtoBuf.IExtensible DeserializeMessage(byte[] data)
		{
			ProtoBuf.IExtensible retObject = null;
			using (MemoryStream ms = new MemoryStream(data))
			{
				DataPackage dataPackage = ProtoBuf.Serializer.Deserialize<DataPackage>(ms);
				retObject = dataPackage.Unpack();
			}
			return retObject;
		}

		internal static ProtoBuf.IExtensible DataPackage_Unpack(DataPackage dataPackage)
		{
			ProtoBuf.IExtensible retObject = null;
			if (dataPackage != null)
			{
				DataPackageTypeInfo mti = new DataPackageTypeInfo() { MajorType = dataPackage.majorType, MinorType = dataPackage.minorType };
				if (!dict_id2msg.ContainsKey(mti))
				{
					dict_id2msg.Add(mti, Type.GetType(mti.MSG_TYPE_NAME));
				}
				Type msgType = dict_id2msg[mti];
				//var content = Activator.CreateInstance(msgType);
				using (MemoryStream stream = new MemoryStream(dataPackage.data))
				{
					retObject = ProtoBuf.Serializer.Deserialize(msgType, stream) as ProtoBuf.IExtensible;
				}
			}
			return retObject;
		}

		internal static DataPackage DataPackage_Pack(ProtoBuf.IExtensible data)
		{
			Type dataType = data.GetType();
			DataPackageTypeInfo mti = DataPackageTypeInfo.ParseFromType(dataType);
			if (!dict_msg2id.ContainsKey(dataType))
			{
				dict_msg2id.Add(dataType, mti);
			}
			mti = dict_msg2id[dataType];
			if (!mti.IsValid)
			{
				return null;
			}
			DataPackage msg = new DataPackage();
			msg.majorType = mti.MajorType;
			msg.minorType = mti.MinorType;
			using (MemoryStream stream = new MemoryStream())
			{
				ProtoBuf.Serializer.Serialize(stream, data);
				msg.data = stream.ToArray();
			}
			return msg;
		}

		public static byte[] SerializeMessage(ProtoBuf.IExtensible data)
		{
			DataPackage dataPackage = data.Pack();
			byte[] msgBytes = null;
			if (dataPackage != null)
			{
				using (MemoryStream stream = new MemoryStream())
				{
					ProtoBuf.Serializer.Serialize(stream, dataPackage);
					msgBytes = stream.ToArray();
				}
			}
			return msgBytes;
		}

		

		public delegate void MessageHandle(ProtoBuf.IExtensible target);

		public delegate void MessageHandle<T>(T data) where T : ProtoBuf.IExtensible;

		public delegate void MessageHanle();
	}
}
