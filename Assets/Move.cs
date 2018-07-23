using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Library.Log;
using Assets;
using Framework.Core.Runtime.ECS;
using System;

[Serializable]
public class MoveEntity : IEntity
{
	public float speed;
}


class Move : EntityComponentWapper<MoveEntity, MoveSystem>
{

}


public class MoveSystem : ISystem
{
	public void Initialize()
	{
		throw new System.NotImplementedException();
	}

	public void Release()
	{
		throw new System.NotImplementedException();
	}

	public void Tick()
	{
// 		var behaviours = EntityManager.Instance.Get<MoveEntity>();
// 		foreach(var b in behaviours)
// 		{
// 			//b.transform.position = b.transform.position + new Vector3(b.)
// 		}
	}
}