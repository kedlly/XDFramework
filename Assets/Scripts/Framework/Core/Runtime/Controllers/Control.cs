using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework.Core
{

	public interface IMovementMessage
	{
		float MaxSpeed { get; }
		Vector3 Velocity { get; }
		Vector3 Position { get; }
	}

	public interface IRotateMessage
	{
		//public 
	}
}
