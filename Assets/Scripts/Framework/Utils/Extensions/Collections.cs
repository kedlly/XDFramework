﻿
using System;
using System.Collections.Generic;
using System.Linq;


namespace Framework.Utils.Extensions
{
	public static partial class ExtensionUtils
	{
		#region Array 扩展方法
		/// <summary>
		/// Fors the each.
		/// </summary>
		/// <returns>The each.</returns>
		/// <param name="selfArray">Self array.</param>
		/// <param name="action">Action.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T[] ForEach<T>(this T[] selfArray, Action<T> action)
		{
			Array.ForEach(selfArray, action);
			return selfArray;
		}

		/// <summary>
		/// Fors the each.
		/// </summary>
		/// <returns>The each.</returns>
		/// <param name="selfArray">Self array.</param>
		/// <param name="action">Action.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static IEnumerable<T> ForEach<T>(this IEnumerable<T> selfArray, Action<T> action)
		{
			if(action == null)
				throw new ArgumentException();
			foreach(var item in selfArray)
			{
				action(item);
			}

			return selfArray;
		}

		public static T Find<T>(this T[] array, Predicate<T> match, T expect)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}

			if (match == null)
			{
				throw new ArgumentNullException("match");
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (match(array[i]))
				{
					return array[i];
				}
			}
			return expect;
		}

		public static int FindIndex<T>(this T[] array, Predicate<T> p)
		{
			return Array.FindIndex(array, p);
		}

		public static int FindLastIndex<T>(this T[] array, Predicate<T> p)
		{
			return Array.FindLastIndex(array, p);
		}

		#endregion

		#region List 扩展方法

		/// <summary>
		/// Fors the each reverse.
		/// </summary>
		/// <returns>The each reverse.</returns>
		/// <param name="selfList">Self list.</param>
		/// <param name="action">Action.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static List<T> ForEachReverse<T>(this List<T> selfList, Action<T> action)
		{
			if(action == null)
				throw new ArgumentException();

			for(var i = selfList.Count - 1; i >= 0; --i)
				action(selfList[i]);

			return selfList;
		}

		/// <summary>
		/// 遍历列表
		/// </summary>
		/// <typeparam name="T">列表类型</typeparam>
		/// <param name="list">目标表</param>
		/// <param name="action">行为</param>
		public static void ForEach<T>(this List<T> list, Action<int, T> action)
		{
			for(var i = 0; i < list.Count; i++)
			{
				action(i, list[i]);
			}
		}

		/// <summary>
		/// 获得随机列表中元素
		/// </summary>
		/// <typeparam name="T">元素类型</typeparam>
		/// <param name="list">列表</param>
		/// <returns></returns>
		public static T GetRandomItem<T>(this List<T> list)
		{
			return list[UnityEngine.Random.Range(0, list.Count - 1)];
		}

		/// <summary>
		/// 根据权值来获取索引
		/// </summary>
		/// <param name="powers"></param>
		/// <returns></returns>
		public static int GetRandomWithPower(this List<int> powers)
		{
			var sum = 0;
			foreach(var power in powers)
			{
				sum += power;
			}

			var randomNum = UnityEngine.Random.Range(0, sum);
			var currentSum = 0;
			for(var i = 0; i < powers.Count; i++)
			{
				var nextSum = currentSum + powers[i];
				if(randomNum >= currentSum && randomNum <= nextSum)
				{
					return i;
				}

				currentSum = nextSum;
			}
			return -1;
		}

		/// <summary>
		/// 拷贝到
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="from"></param>
		/// <param name="to"></param>
		/// <param name="begin"></param>
		/// <param name="end"></param>
		public static void CopyTo<T>(this List<T> from, List<T> to, int begin = 0, int end = -1)
		{
			if(begin < 0)
			{
				begin = 0;
			}

			var endIndex = (from.Count < to.Count ? from.Count : to.Count) - 1;

			if(end != -1 && end < endIndex)
			{
				endIndex = end;
			}

			for(var i = begin; i < end; i++)
			{
				to[i] = from[i];
			}
		}

		/// <summary>
		/// 将List转为Array
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="selfList"></param>
		/// <returns></returns>
		public static T[] ToArraySavely<T>(this List<T> selfList)
		{
			var res = new T[selfList.Count];

			for(var i = 0; i < selfList.Count; i++)
			{
				res[i] = selfList[i];
			}

			return res;
		}

		/// <summary>
		/// 尝试获取，如果没有该数则返回null
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="selfList"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static T TryGet<T>(this List<T> selfList, int index)
		{
			return selfList.Count > index ? selfList[index] : default(T);
		}

		#endregion

		#region Dictionary 扩展方法

		/// <summary>
		/// 合并2个Dictionary
		/// </summary>
		/// <returns>合并后的 Dictionary.</returns>
		/// <param name="dictionary">Dictionary.</param>
		/// <param name="dictionaries">Dictionaries.</param>
		/// <typeparam name="TKey">The 1st type parameter.</typeparam>
		/// <typeparam name="TValue">The 2nd type parameter.</typeparam>
		public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
			params Dictionary<TKey, TValue>[] dictionaries)
		{
			return dictionaries.Aggregate(dictionary,
				(current, dict) => current.Union(dict).ToDictionary(kv => kv.Key, kv => kv.Value));
		}

		/// <summary>
		/// 根据权值获取值，Key为值，Value为权值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="powersDict"></param>
		/// <returns></returns>
		public static T GetRandomWithPower<T>(this Dictionary<T, int> powersDict)
		{
			var keys = new List<T>();
			var values = new List<int>();

			foreach(var key in powersDict.Keys)
			{
				keys.Add(key);
				values.Add(powersDict[key]);
			}

			var finalKeyIndex = values.GetRandomWithPower();
			return keys[finalKeyIndex];
		}

		/// <summary>
		/// 遍历
		/// </summary>
		/// <typeparam name="K"></typeparam>
		/// <typeparam name="V"></typeparam>
		/// <param name="dict"></param>
		/// <param name="action"></param>
		public static void ForEach<K, V>(this Dictionary<K, V> dict, Action<K, V> action)
		{
			var dictE = dict.GetEnumerator();

			while(dictE.MoveNext())
			{
				var current = dictE.Current;
				action(current.Key, current.Value);
			}

			dictE.Dispose();
		}

		/// <summary>
		/// 向其中添加新的词典
		/// </summary>
		/// <typeparam name="K"></typeparam>
		/// <typeparam name="V"></typeparam>
		/// <param name="dict"></param>
		/// <param name="addInDict"></param>
		/// <param name="isOverride"></param>
		public static void AddRange<K, V>(this Dictionary<K, V> dict, Dictionary<K, V> addInDict,
			bool isOverride = false)
		{
			var dictE = addInDict.GetEnumerator();

			while(dictE.MoveNext())
			{
				var current = dictE.Current;
				if(dict.ContainsKey(current.Key))
				{
					if(isOverride)
						dict[current.Key] = current.Value;
					continue;
				}

				dict.Add(current.Key, current.Value);
			}

			dictE.Dispose();
		}


		public static void RemoveAll<TKey, TValue>(this Dictionary<TKey, TValue> dict, Func<KeyValuePair<TKey, TValue>, bool> predicate)
		{ 
			dict.Where(it => predicate(it)).Select(it => it.Key).ToArray().ForEach(key => dict.Remove(key));
		}
		
		
		#endregion

	}
}