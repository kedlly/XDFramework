using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Core
{
	using UnityEngine;

	[CreateAssetMenu(menuName = "MySubMenue/Create MyScriptableObject ")]
	public class StateMachineSet : ScriptableObject
	{
		public int someVariable;
	}
}
