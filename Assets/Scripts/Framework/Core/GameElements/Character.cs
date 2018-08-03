using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework.Core
{ 
	public class Character : GameElement
	{
		public interface IMovement
		{
			Vector3 CurrentPosition { get; set; }
			Vector3 Velocity { get; set; }
		}
		public Character() : base(GameElementTypes.Character)
		{

		}

		
	}
}
