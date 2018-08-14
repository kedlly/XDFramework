
using UnityEngine;
using Framework.Core.Attributes;

namespace Framework.Core.Infomation
{
    public interface IUserInfo
    {
        int Id{get;} 
        string Name {get;}
    }

    public enum MovementStyle
    {
        Freedom, Climb, Fly
    }

    [RealTimeUpdateInfo]
    public interface IMovementInfo
    {
        MovementStyle Style {get; set; }
        Vector3 Velocity {get; set; }
		Vector3 Posiotion { get; set; }
		float MaxSpeed { get; set; }
    }

	[RealTimeUpdateInfo]
	public interface IRotationInfo
	{
		Vector3 CurrentDirection { get; set; }
		Vector3 TargetDirection { get; set; }
		float Speed { get; set; }
	}
}