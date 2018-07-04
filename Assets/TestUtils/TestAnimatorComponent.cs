using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.TestUtils
{
	[Serializable]
	public class AnimatorCondition
	{
		public enum ConditionType
		{
			INT,FLOAT,TRIGGER, BOOL
		}
		public string conditionName;
		public ConditionType type;
		public string value;
	}

	[Serializable]
	public class AnimatorConditionBinding
	{
		public KeyCode keyCode;
		public AnimatorCondition[] condition;
	}


	[RequireComponent(typeof(Animator))]
    public class TestAnimatorComponent : MonoBehaviour
	{
		Animator animator = null;
		public AnimatorConditionBinding[] bindingArray;
		private void Awake()
		{
			animator = transform.GetComponent<Animator>();
		}
		private void Start()
		{
			
		}

		private void Update()
		{
			if (bindingArray != null && bindingArray.Length > 0)
			{
				foreach (var data in bindingArray)
				{
					if (Input.GetKeyDown(data.keyCode))
					{
						if (data.condition != null && data.condition.Length > 0)
						{
							foreach (var element in data.condition)
							{
								switch(element.type)
								{
									case AnimatorCondition.ConditionType.BOOL:
									{
										bool value = bool.Parse(element.value);
										animator.SetBool(element.conditionName, value);
									}
									break;
									case AnimatorCondition.ConditionType.INT:
									{
										int value = int.Parse(element.value);
										animator.SetInteger(element.conditionName, value);
									}
									break;
									case AnimatorCondition.ConditionType.FLOAT:
									{
										float value = float.Parse(element.value);
										animator.SetFloat(element.conditionName, value);
									}
									break;
									case AnimatorCondition.ConditionType.TRIGGER:
									{
										//bool value = bool.Parse(data.value);
										animator.SetTrigger(element.conditionName);
									}
									break;
									default:
										break;
								}
							}
						}
						
					}
				}
			}
		}
	}
}
