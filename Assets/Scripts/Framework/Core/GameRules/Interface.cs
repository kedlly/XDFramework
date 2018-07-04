using System;

namespace Framework.Core.GameRule
{
	
	/// <summary>
	/// 游戏事件处理集合接口
	/// 使用原则:
	///		1. 对应的 GameEvent 必须有统一的 类型
	/// </summary>
	public interface IGameEventProcessorContainer<EventType>
	{
		/// <summary>
		/// 注册游戏规则
		/// </summary>
		/// <param name="option"> 游戏中出现的影响游戏规则的事件类型 </param>
		/// <param name="delegateFunc">该事件对应的处理方法(必须是 Action<T, K, ..> 类型, 对于相同事件,必须注册相同的参数类型的处理方法)</param>
		/// <returns>注册成功返回true 否则false</returns>
		bool RegisterEvent(EventType env, Delegate delegateFunc);
		/// <summary>
		/// 取消对应事件的所有相关游戏规则
		/// </summary>
		/// <param name="option">事件类型</param>
		/// <returns>返回被取消的所有调用信息</returns>
		Delegate UnregisterEvent(EventType env);
		/// <summary>
		/// 取消对应事件的指定的相关游戏规则
		/// </summary>
		/// <param name="env"></param>
		/// <param name="delegateFunc"></param>
		void UnregisterEvent(EventType env, Delegate delegateFunc);

		/// <summary>
		/// 取消对应事件的指定的所有相关游戏规则
		/// </summary>
		/// <param name="env"></param>
		/// <param name="delegateFunc"></param>
		void UnregisterEventAll(EventType env, Delegate delegateFunc);
		/// <summary>
		/// 游戏中对应事件发生时的处理例程
		/// </summary>
		/// <param name="option">事件类型</param>
		/// <param name="args">参数列表 (该参数列表必须 与 事件注册时使用的参数列表一致, 如果已注册的事件参数列表不敷使用, 需要先反注册 当前事件,重新添加对应事件的处理, 消除歧义)</param>
		void Process(EventType env, params object[] args);
		/// <summary>
		/// 判断目前是否已经有对应规则的处理例程
		/// </summary>
		/// <param name="option">事件类型</param>
		/// <returns>有返回true,否则false</returns>
		bool IsExistProcessor(EventType env);

	}
}
