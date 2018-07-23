using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Library.Log;
using Assets;
using Framework.Core.Runtime.ECS;
using System;

[Serializable]
class RotateEntity : IEntity
{
	[SerializeField]
	[Header("name")]
	public int a;
	//public int b;
}


// class NewBehaviourScript : EntityWapper<RotateEntity, NewBehaviourScript>
// {

//}