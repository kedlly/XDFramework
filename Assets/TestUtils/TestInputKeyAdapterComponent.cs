using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.TestUtils
{
	[Serializable]
	public class Z_ConditionBinding
	{
		public KeyCode keyCode;
		public string[] functions;
	}

	public class TestInputKeyAdapterComponent : MonoBehaviour
	{
		public Z_ConditionBinding[] bindingArray;

		private void Update()
		{
			if(bindingArray != null && bindingArray.Length > 0)
			{
				foreach(var data in bindingArray)
				{
					if(Input.GetKeyDown(data.keyCode))
					{
						if(data.functions != null && data.functions.Length > 0)
						{
							foreach(var element in data.functions)
							{
								SendMessage(element);
							}
						}

					}
				}
			}
		}
	}
}
