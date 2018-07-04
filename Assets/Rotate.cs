using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Library.Log;
using Assets;
using Framework.Core.Runtime;
using System;

[Serializable]
class RotateEntity2 : IEntity
{
	public float Speed = 0;
}


class Rotate : EntityWapper<RotateEntity2>
{

}