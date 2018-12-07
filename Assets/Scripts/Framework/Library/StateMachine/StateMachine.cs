
using Framework.Library.StateMachine.Template;
using UnityEngine;

namespace Framework.Library.StateMachine
{
	public static class Utils
	{
		public static FSMTemplate GetTemplate(TextAsset textAsset)
		{
			if (textAsset != null && !FSMCache.Instance.IsExistTemplate(textAsset.name))
			{
				FSMCache.Instance.LoadTemplate(textAsset.name, textAsset.text);
			}
			return FSMCache.Instance.GetTemplate(textAsset.name);
		}

		public static FSMExecutor<T> GetStateMachineExecutor<T>(T obj, TextAsset textAsset) where T : class
		{
			var template = GetTemplate(textAsset);
			return new FSMExecutor<T>(template, obj);
		}
	}
}
