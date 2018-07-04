
using System.Reflection;

namespace Framework.Library.XMLStateMachine
{

	internal enum TransitionRuleStyle
	{
		Normal, Conditional, Absoulte
	}

	internal partial class  TransitionRule<T>
	{
		public State<T> Target { get; private set; }

		public string Event { get; private set; }

		public TransitionRuleStyle Style { get; set; }

		public MemberInfo DynamicConditionFunc { get; private set; }


		public State<T> OwnerState { get; private set; }
		public TransitionRule(State<T> owner, string strEvent, string condition, State<T> target)
		{
			OwnerState = owner;
			Style = TransitionRuleStyle.Normal;
			SetDynamicCondition(condition);
			Event = strEvent;
			if (string.IsNullOrEmpty(strEvent))
			{
				Style = string.IsNullOrEmpty(condition) ? TransitionRuleStyle.Absoulte : TransitionRuleStyle.Conditional;
			}
			Target = target;
		}

		internal bool DynamicConditionInvoke(T instance)
		{
			bool result = true;
			if (DynamicConditionFunc != null)
			{
				result = false;
				switch(DynamicConditionFunc.MemberType)
				{
					case MemberTypes.Field:
					{
						FieldInfo fi = DynamicConditionFunc as FieldInfo;
						if(fi.FieldType == typeof(bool))
						{
							result = (bool)fi.GetValue(instance);	
						}
						break;
					}
					case MemberTypes.Property:
					{
						PropertyInfo pi = DynamicConditionFunc as PropertyInfo;
						if(pi.PropertyType == typeof(bool))
						{
							result = (bool)pi.GetValue(instance, null);
						}
						break;
					}
					case MemberTypes.Method:
					{
						MethodInfo mi = DynamicConditionFunc as MethodInfo;
						if (mi.ReturnType == typeof(bool))
						{
							result = (bool)mi.Invoke(instance, dynamicMethodArgs);
						}
						break;
					}
					default:
						break;
				}
			}
			return result;
		}

		private object[] dynamicMethodArgs = State<T>.ParamDescNoArgs;

		public void SetDynamicParams(params object [] args)
		{
			dynamicMethodArgs = args;
		}

		public void SetDynamicCondition(string condition)
		{
			if(!string.IsNullOrEmpty(condition))
			{
				MemberInfo[] mis = typeof(T).GetMember(condition, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
				if(mis.Length > 0)
				{
					DynamicConditionFunc = mis[0];
				}
			}
		}
	}
}