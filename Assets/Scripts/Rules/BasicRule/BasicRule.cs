using System;
using System.Reflection;
using UnityEngine;
using Framework.Core.GameRule;

namespace XDDQFrameWork
{
	public class BasicGameRule : AGameRule
	{
		void sfs(int u, ref string d)
		{ }

		public BasicGameRule()
		{
			var p = new EGameRuleEvent();
			AddRule(p
					, new Action ( () => {
						Debug.LogError("QQQQ"); } )
				);

			AddRule(p
					, new Action(() =>
					{
						Debug.LogError("AAA");
					})
				);
			AddRule(p
					, new Action(() =>
					{
						Debug.LogError("ZZZ");
					})
				);
			AddRule(p
					, new Action(() =>
					{
						Debug.LogError("CCC");
					})
				);

			(this as IGameEventProcessorContainer<EGameRuleEvent>).Process(p);

		}
	}
}
