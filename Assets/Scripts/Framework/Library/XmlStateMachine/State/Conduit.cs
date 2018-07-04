using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Framework.Library.XMLStateMachine
{
	internal partial class State<T>
	{
		internal TransitionRule<T> GetConduitTransition(T target)
		{
			TransitionRule<T> result = null;
			if(IsConduit)
			{
				if(target != null)
				{
					foreach(var tran in Transitions)
					{
						if(tran.DynamicConditionInvoke(target))
						{
							result = tran;
							break;
						}
					}
				}
			}
			return result;
		}
	}
}