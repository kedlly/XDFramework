
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
        MovementStyle Style {get;}
        Vector3 Velocity {get;}
    }
}