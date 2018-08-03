using UnityEngine;
using Framework.Core.Attributes;

namespace Framework.Core
{
	[ManagerTemplate("internalMovementSystem", AutoRegister = true)]
	public sealed class MovementSystem : AMananger
	{
		
	}

	[ManagerTemplate("internalRotationSystem", AutoRegister = false)]
	public sealed class RotationSystem : AMananger
	{
		
	}
}