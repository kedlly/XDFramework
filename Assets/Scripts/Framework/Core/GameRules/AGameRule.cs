using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.Core.GameRule
{

	public enum EGameRuleEvent { }

	public abstract class AGameEventProcessor<T> : IGameEventProcessorContainer<T>
	{
		private Dictionary<T, Delegate> dictGameRuleProcessors = new Dictionary<T, Delegate>();

		bool IGameEventProcessorContainer<T>.IsExistProcessor(T env)
		{
			return dictGameRuleProcessors.ContainsKey(env);
		}
		void IGameEventProcessorContainer<T>.Process(T env, params object[] args)
		{
			if (!((IGameEventProcessorContainer<T>)this).IsExistProcessor(env))
			{
				return;
			}
			if (args == null)
			{
				args = new object[1] { null };
			}
			if (dictGameRuleProcessors.ContainsKey(env) && dictGameRuleProcessors[env] != null)
			{
				dictGameRuleProcessors[env].DynamicInvoke(args);
			}
			else
			{
				Debug.Log(
						string.Format(
							"事件: {0}, 无处理例程"
							, env
							)
						);
			}
		}
		bool IGameEventProcessorContainer<T>.RegisterEvent(T env, Delegate delegateFunc)
		{
			bool result = true;
			if (dictGameRuleProcessors.ContainsKey(env))
			{
				try
				{
					if (dictGameRuleProcessors[env] != null)
					{
						dictGameRuleProcessors[env] = Delegate.RemoveAll(dictGameRuleProcessors[env], delegateFunc);
					}
					dictGameRuleProcessors[env] = Delegate.Combine(dictGameRuleProcessors[env], delegateFunc);
				}
				catch (Exception ex)
				{
					Debug.Log(
						string.Format(
							"注册规则必须遵循相同事件处理例程必须有相同的参数类型的规则. 异常事件 : {0}, 异常信息: {1}"
							, env
							, ex.Message
							)
						);
					result = false;
				}
				
			}
			else
			{
				dictGameRuleProcessors[env] = delegateFunc;
			}
			return result;
		}
		Delegate IGameEventProcessorContainer<T>.UnregisterEvent(T env)
		{
			Delegate result = null;
			if (dictGameRuleProcessors.ContainsKey(env))
			{
				result = dictGameRuleProcessors[env];
				dictGameRuleProcessors.Remove(env);
			}
			return null;
		}
		protected void AddRule(T env, Delegate delegateFunc)
		{
			if(!((IGameEventProcessorContainer<T>)this).RegisterEvent(env, delegateFunc))
			{
				UnityEngine.Debug.Log(string.Format("游戏规则 {0} 添加失败.", env));
			}
		}
		protected Delegate RemoveRule(T env)
		{
			return ((IGameEventProcessorContainer<T>)this).UnregisterEvent(env);
		}

		void IGameEventProcessorContainer<T>.UnregisterEvent(T env, Delegate delegateFunc)
		{
			if(dictGameRuleProcessors.ContainsKey(env))
			{
				var result = dictGameRuleProcessors[env];
				if(dictGameRuleProcessors[env] != null)
				{
					dictGameRuleProcessors[env] = Delegate.Remove(dictGameRuleProcessors[env], delegateFunc);
				}
			}
		}

		void IGameEventProcessorContainer<T>.UnregisterEventAll(T env, Delegate delegateFunc)
		{
			if(dictGameRuleProcessors.ContainsKey(env))
			{
				var result = dictGameRuleProcessors[env];
				if(dictGameRuleProcessors[env] != null)
				{
					dictGameRuleProcessors[env] = Delegate.RemoveAll(dictGameRuleProcessors[env], delegateFunc);
				}
			}
		}
	}



	public abstract class AGameRule : AGameEventProcessor<EGameRuleEvent>
	{

	}

}
