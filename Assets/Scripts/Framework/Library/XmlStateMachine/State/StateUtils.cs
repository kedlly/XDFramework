

using UnityEngine;

namespace Framework.Library.XMLStateMachine
{
	internal partial class State<T>
	{
		/// <summary>
		/// 状态切换
		/// </summary>
		/// <param name="target"></param>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <returns></returns>
		public static State<T> GotoNext(T target, State<T> from, State<T> to)
		{
			if (from.IsExit)
			{
				Debug.LogError("cant change state from ExitState to another.");
			}
			
			if (from == to && !from.IsReenterable)
			{
				return from;
			}

			State<T> outerState = ExitState(target, from);
			return  outerState == null ? EnterState(target, to) : EnterState(target, outerState);
		}

		/// <summary>
		/// 进入状态 若状态标记 isEntry 被设置, 递归进入状态, 并返回递归结束时的状态
		/// </summary>
		/// <param name="target">状态持有者</param>
		/// <param name="state">状态对象</param>
		/// <returns>递归结束时的状态</returns>
		public static State<T> EnterState(T target, State<T> state)
		{
			if (target == null || state == null)
			{
				Debug.LogError("can not enter null state");
				return null;
			}
			State<T> actualState = state;
			state.onEnter(target);
			if(state.IsExit && state.ParentAbsoluteRuleState != null) // 有exit状态则返回 绝对跳转状态
			{
				actualState = EnterState(target, state.ParentAbsoluteRuleState);
			}
			else if (state.IsEntry && state.AbsoluteRuleState != null)
			{
				actualState = EnterState(target, state.AbsoluteRuleState);
			}
			else if(state.IsConduit)
			{
				var activedTran = state.GetConduitTransition(target);
				if (activedTran == null || activedTran.Target == null)
				{
					Debug.LogError(string.Format("Conduit :{0} not actived or target error.", state.FullName));
				}
				var targetState = activedTran == null ? null : activedTran.Target;
				actualState = EnterState(target, targetState);
			}
			else if (state.SubStateMap != null) //有子状态则返回子状态中的入口状态
			{
				if (state.SubStateMap.Entry != null)
				{
					actualState = EnterState(target, state.SubStateMap.Entry);
				}
				else
				{
					Debug.LogError(string.Format("state name : {0} must be set a entry state.", state.FullName));
				}
			}
			
			else
			{

			}
			
			return actualState;
		}

		/// <summary>
		/// 更新当前状态
		/// </summary>
		/// <param name="target"></param>
		/// <param name="state"></param>
		public static void UpdateState(T target, State<T> state)
		{
			if(target == null || state == null)
			{
				return;
			}
			state.onUpdate(target);
		}

		/// <summary>
		/// 退出状态，并返回下一个目标状态
		/// </summary>
		/// <param name="target">状态持有者</param>
		/// <param name="state">状态对象</param>
		/// <returns>注：若状态标记 isExit 被设置，将返回 绝对跳转 制定的状态 否则返回null</returns>
		public static State<T> ExitState(T target, State<T> state)
		{
			if(target == null || state == null)
			{
				return null;
			}
			State<T> nextState = null;
			state.onExit(target);
			if(state.IsExit && state.Parent != null && state.Parent.AbsoluteRuleState != null)
			{
				nextState = state.Parent.AbsoluteRuleState;
			}
			return nextState;
		}
	}

}