using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
namespace Framework.Core.Runtime
{
	[ProtoContract]
	public class Player : GameActor
	{
		public int Id;
		public string Nickname;
		//public string
	}
}
