using Framework.Core.DataStruct;
using Framework.Library.Singleton;
using Framework.Library.StateMachine.Template;
using Framework.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Framework.Library.StateMachine
{
	class FSMCache : ToSingleton<FSMCache>
	{
		private FSMCache() { }
		protected override void OnSingletonInit() { }

		Dictionary<string, FSMTemplate> templateCache = new Dictionary<string, FSMTemplate>();
		public bool IsExistTemplate(string name)
		{
			return templateCache.ContainsKey(name);
		}
		public FSMTemplate LoadTemplate(string name, string xml = null)
		{
			if (xml == null)
			{
				throw new Exception("need xml fsm file.");
			}
			FSMTemplate template = Parser.FSMXMLParser.LoadFromXML(xml);
			templateCache[name] = template;
			return template;
		}

		public FSMTemplate GetTemplate(string name)
		{
			if (IsExistTemplate(name))
			{
				return templateCache[name];
			}
			return null;
		}

		Dictionary<Tuple<string, Type>, Action<object>> actionCache = new Dictionary<Tuple<string, Type>, Action<object>>();

		public Action<object> GetEventAction<T>(string methodName) where T : class
		{
			if (methodName.IsNotNullAndEmpty())
			{
				Type targetType = typeof(T);
				var tupleKey = Tuple.Create(methodName, targetType);
				if (!actionCache.ContainsKey(tupleKey))
				{
					var mi = targetType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
					if (mi == null)
					{
						return null;
					}
					var d = (Action<T>)Delegate.CreateDelegate(typeof(Action<T>), null, mi);
					actionCache[tupleKey] = o =>
					{
						d.Invoke(o as T);
					};
				}
				return actionCache[tupleKey];
			}
			return null;
		}


		Dictionary<Tuple<string, bool, Type, string[]>, Func<object, bool>> transitionCache = new Dictionary<Tuple<string, bool, Type, string[]>, Func<object, bool>>();

		public Func<object, bool> GetTransitionCondition<T>(string condition, bool isInvert, string[] paramaters) where T : class
		{
			if (condition.IsNotNullAndEmpty())
			{
				Type targetType = typeof(T);
				var tupleKey = Tuple.Create(condition, isInvert, targetType, paramaters);
				if (transitionCache.ContainsKey(tupleKey))
				{
					return transitionCache[tupleKey];
				}
				MemberInfo[] mis = targetType.GetMember(condition
												, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static
											);
				if (mis.Length > 1)
				{
					throw new Exception("Transition Condition must be unique field, property or method. " + condition);
				}
				if (mis.Length == 0)
				{
					return null;
				}
				MemberInfo DynamicConditionFunc = mis[0];
				Func<object, bool> result = null;
				switch (DynamicConditionFunc.MemberType)
				{
					case MemberTypes.Field:
					{
						FieldInfo fi = DynamicConditionFunc as FieldInfo;
						if (fi.FieldType == typeof(bool))
						{
							result = obj => { return (bool)fi.GetValue(obj); };
						}
						break;
					}
					case MemberTypes.Property:
					{
						PropertyInfo pi = DynamicConditionFunc as PropertyInfo;
						if (pi.PropertyType == typeof(bool))
						{
							if (!pi.CanRead)
							{
								throw new InvalidOperationException("property name {0} is not \"CanRead\"".FormatEx(condition));
							}
							var mi = pi.GetGetMethod(true);
							var d = (Func<T, bool>)Delegate.CreateDelegate(typeof(Func<T, bool>), null, mi);
							result = o => { return d.Invoke(o as T); };
						}
						break;
					}
					case MemberTypes.Method:
					{
						MethodInfo mi = DynamicConditionFunc as MethodInfo;
						if (mi.ReturnType == typeof(bool))
						{
							var allParams = mi.GetParameters();
							if (allParams.Length > paramaters.Length)
							{
								throw new Exception("must more params in condition :" + condition);
							}

							if (allParams.Length == 0)
							{
								var d = (Func<T, bool>)Delegate.CreateDelegate(typeof(Func<T, bool>), null, mi);
								result = o => { return d(o as T); };
							}
							else if (allParams.Length == 1)
							{
								ParameterInfo pInfo = allParams[0];
								result = GenerateDelegateOneParamStyle<T>(paramaters, pInfo.ParameterType, mi);
							}
							else
							{
								if (allParams.Length > paramaters.Length)
								{
									throw new Exception("Transition Condition method does not have enough args in Params : " + condition);
								}
								if (allParams.Any(it => it.ParameterType.IsArray))
								{
									throw new Exception("Transition Condition method must be NOT have ARRAY args : " + condition);
								}
								object[] args = allParams.Select((it, index) => ValueParam.getValue(it.ParameterType, paramaters[index])).ToArray();
								result = o =>
								{
									return (bool)mi.Invoke(o, args);
								};
							}
						}
						else
						{
							throw new Exception("Transition Condition method must be return boolean :" + condition);
						}
						if (result == null)
						{
							throw new Exception("Transition Condition method must be return boolean and have params that can parse it :" + condition);
						}
						break;
					}
					default:
						break;
				}
				if (isInvert)
				{
					Func<object, bool> target = result;
					result = obj => { return !target(obj); };
				}
				this.transitionCache[tupleKey] = result;
				return result;
			}
			return null;
		}


		internal static Func<object, bool> GenerateDelegateOneParamStyle<T>(string[] args, Type ParameterType, MethodInfo mi) where T : class
		{
			Func<object, bool> result = null;
			if (ParameterType.IsArray)
			{
				if (ParameterType == typeof(string[]))
				{
					var d = (Func<T, string[], bool>)Delegate.CreateDelegate(typeof(Func<T, string[], bool>), mi);
					result = o =>
					{
						return d.Invoke(o as T, args);
					};
				}
				else if (ParameterType == typeof(int[]))
				{
					var d = (Func<T, int[], bool>)Delegate.CreateDelegate(typeof(Func<T, int[], bool>), mi);
					var valueList = args.Select(it => it.ToInt()).ToArray();
					result = o =>
					{
						return d.Invoke(o as T, valueList);
					};
				}
				else if (ParameterType == typeof(float[]))
				{
					var d = (Func<T, float[], bool>)Delegate.CreateDelegate(typeof(Func<T, float[], bool>), mi);
					var valueList = args.Select(it => it.ToFloat()).ToArray();
					result = o =>
					{
						return d.Invoke(o as T, valueList);
					};
				}
			}
			else
			{
				string parameterObject = args.Length > 0 ? args[0] : null;
				if (ParameterType == typeof(string))
				{
					var d = (Func<T, string, bool>)Delegate.CreateDelegate(typeof(Func<T, string, bool>), mi);
					result = o =>
					{
						return d.Invoke(o as T, parameterObject);
					};
				}
				else if (ParameterType == typeof(int))
				{
					var d = (Func<T, int, bool>)Delegate.CreateDelegate(typeof(Func<T, int, bool>), mi);
					int value = 0;
					if (!int.TryParse(parameterObject, out value))
					{
						throw new Exception("cant parse Param : \"" + parameterObject + "\" as int.");
					}
					result = o =>
					{
						return d.Invoke(o as T, value);
					};
				}
				else if (ParameterType == typeof(float))
				{
					var d = (Func<T, float, bool>)Delegate.CreateDelegate(typeof(Func<T, float, bool>), mi);
					float value = 0;
					if (!float.TryParse(parameterObject, out value))
					{
						throw new Exception("cant parse Param : \"" + parameterObject + "\" as float.");
					}
					result = o =>
					{
						return d.Invoke(o as T, value);
					};
				}
				else if (ParameterType == typeof(double))
				{
					var d = (Func<T, double, bool>)Delegate.CreateDelegate(typeof(Func<T, double, bool>), mi);
					double value = 0;
					if (!double.TryParse(parameterObject, out value))
					{
						throw new Exception("cant parse Param : \"" + parameterObject + "\" as double.");
					}
					result = o =>
					{
						return d.Invoke(o as T, value);
					};
				}
				else if (ParameterType == typeof(long))
				{
					var d = (Func<T, long, bool>)Delegate.CreateDelegate(typeof(Func<T, long, bool>), mi);
					long value = 0;
					if (!long.TryParse(parameterObject, out value))
					{
						throw new Exception("cant parse Param : \"" + parameterObject + "\" as long.");
					}
					result = o =>
					{
						return d.Invoke(o as T, value);
					};
				}
			}

			return result;
		}

		Dictionary<Tuple<FSMTemplate, Type>, StateMachine> stateMachineCache = new Dictionary<Tuple<FSMTemplate, Type>, StateMachine>();

		public StateMachine GetStateMachine<T>(FSMTemplate template) where T : class
		{
			Type targetType = typeof(T);
			var tupleKey = Tuple.Create(template, targetType);
			if (!stateMachineCache.ContainsKey(tupleKey))
			{
				stateMachineCache[tupleKey] = template.Create<T>();
			}
			return stateMachineCache[tupleKey];
		}

		public void ClearTemplateCache(string name = null)
		{
			if (name.IsNullOrEmpty())
			{
				templateCache.Clear();
			}
			else
			{
				templateCache.Remove(name);
			}
		}

		public void ClearEventActionCache(Type type = null)
		{
			if (type == null)
			{
				actionCache.Clear();
			}
			else
			{
				actionCache.RemoveAll(it => it.Key.Item2 == type);
			}
		}

		public void ClearEventActionCache<T>()
		{
			ClearEventActionCache(typeof(T));
		}

		public void ClearTrasitionCache(Type type = null)
		{
			if (type == null)
			{
				transitionCache.Clear();
			}
			else
			{
				transitionCache.RemoveAll(it => it.Key.Item3 == type);
			}
		}

		public void ClearTrasitionCache<T>()
		{
			ClearTrasitionCache(typeof(T));
		}
	}

	internal static class ValueParam
	{
		public static object getValue(Type type, string strValue)
		{
			if (type == null || strValue == null)
			{
				return null;
			}
			if (type == typeof(int))
			{
				int result = 0;
				if (!int.TryParse(strValue, out result)) { }
				return result;
			}
			else if (type == typeof(float))
			{
				float result = 0.0f;
				if (!float.TryParse(strValue, out result)) { }
				return result;
			}
			else if (type == typeof(double))
			{
				double result = 0.0;
				if (!double.TryParse(strValue, out result)) { }
				return result;
			}
			else if (type == typeof(bool))
			{
				bool result = false;
				if (!bool.TryParse(strValue, out result)) { }
				return result;
			}
			else if (type == typeof(string))
			{
				return strValue;
			}
			return null;
		}
	}
}
