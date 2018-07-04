using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Framework.Utils.Extensions
{

	public static partial class ExtensionUtils
	{

		public static string FormatEx(this string str, params object[] objs)
		{
			return string.Format(str, objs);
		}

		/// <summary>
		/// 检查 字符串 是否为 null 或者空串
		/// </summary>
		/// <param name="testStr"></param>
		/// <returns></returns>
		public static bool IsNullOrEmpty(this string testStr)
		{
			return string.IsNullOrEmpty(testStr);
		}

		/// <summary>
		/// 判断字符串非null并且非空串
		/// </summary>
		/// <param name="testStr"></param>
		/// <returns></returns>
		public static bool IsNotNullAndEmpty(this string testStr)
		{
			return !string.IsNullOrEmpty(testStr);
		}

		/// <summary>
		/// 测试字符串移除前后空白符后不为null且不是空串
		/// </summary>
		/// <param name="testStr"></param>
		/// <returns></returns>
		public static bool IsTrimNotNullAndEmpty(this string testStr)
		{
			return !string.IsNullOrEmpty(testStr.Trim());
		}

		/// <summary>
		/// 字符串首字母大写
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string UppercaseFirst(this string str)
		{
			return char.ToUpper(str[0]) + str.Substring(1);
		}

		/// <summary>
		/// 字符串首字母小写
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string LowercaseFirst(this string str)
		{
			return char.ToLower(str[0]) + str.Substring(1);
		}

		/// <summary>
		/// 将字符串中windows 类型换行符 \r\n 换成 Unix 类型的 \n
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ToUnixLineEndings(this string str)
		{
			return str.Replace("\r\n", "\n").Replace("\r", "\n");
		}

		/// <summary>
		/// 将字符串数组转换成 ', ' 分割的字符串 的 CSV 格式
		/// </summary>
		/// <param name="values"></param>
		/// <returns>目标字符串</returns>
		public static string ToCSV(this string[] values)
		{
			return string.Join(", ", values
				.Where(value => !string.IsNullOrEmpty(value))
				.Select(value => value.Trim())
				.ToArray()
			);
		}

		/// <summary>
		/// 将 当前 CSV 格式 字符串 解析成 字符串数组形式
		/// </summary>
		/// <param name="values"></param>
		/// <returns>目标字符串数组 </returns>
		public static string[] ArrayFromCSV(this string values)
		{
			return values
				.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(value => value.Trim())
				.ToArray();
		}

		/// <summary>
		/// 在当前字符串后追加字符串
		/// </summary>
		/// <param name="testStr"></param>
		/// <param name="toAppend">追加的字符串</param>
		/// <returns>StringBuilder</returns>
		public static StringBuilder Append(this string testStr, string toAppend)
		{
			return new StringBuilder(testStr).Append(toAppend);
		}

		/// <summary>
		/// 在当前字符串前添加前缀
		/// </summary>
		/// <param name="testStr"></param>
		/// <param name="toPrefix"></param>
		/// <returns></returns>
		public static string AddPrefix(this string testStr, string toPrefix)
		{
			return new StringBuilder(toPrefix).Append(testStr).ToString();
		}

		/// <summary>
		/// 将当前字符串前后添加指定字符
		/// </summary>
		/// <param name="strToFormat"></param>
		/// <param name="cPaddingChar">前后扩展字符</param>
		/// <param name="iTotalWidth">总字符宽度</param>
		/// <returns></returns>
		public static string PaddingChar(this string strToFormat, char cPaddingChar = '*', int iTotalWidth = 80)
		{
			//auto added space
			strToFormat = " " + strToFormat + " "; //" [4] Valid: B0009IQZFM "

			//1. padding left
			int iPaddingLen = (iTotalWidth - strToFormat.Length) / 2;
			int iLefTotalLen = iPaddingLen + strToFormat.Length;
			string strLefPadded = strToFormat.PadLeft(iLefTotalLen, cPaddingChar); //"============================ [4] Valid: B0009IQZFM "
																				   //2. padding right
			string strFormatted = strLefPadded.PadRight(iTotalWidth, cPaddingChar); //"============================ [4] Valid: B0009IQZFM ============================="

			return strFormatted;
		}

		/// <summary>
		/// 在当前字符串后追加格式化字符串
		/// </summary>
		/// <param name="testStr"></param>
		/// <param name="toAppend"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		public static StringBuilder AppendFormat(this string testStr, string toAppend, params object[] args)
		{
			return new StringBuilder(testStr).AppendFormat(toAppend, args);
		}

		/// <summary>
		/// 尝试转换成int类型，若失败，返回默认值或者指定值
		/// </summary>
		/// <param name="testStr"></param>
		/// <param name="defaulValue"></param>
		/// <returns></returns>
		public static int ToInt(this string testStr, int defaulValue = 0)
		{
			var retValue = defaulValue;
			return int.TryParse(testStr, out retValue) ? retValue : defaulValue;
		}

		/// <summary>
		/// 尝试转换成float类型，若失败，返回默认值或者指定值
		/// </summary>
		/// <param name="testStr"></param>
		/// <param name="defaulValue"></param>
		/// <returns></returns>
		public static float ToFloat(this string testStr, float defaulValue = 0)
		{
			var retValue = defaulValue;
			return float.TryParse(testStr, out retValue) ? retValue : defaulValue;
		}


		/// <summary>
		/// 将指定字符分割的字符串去除前后空白符后组合成链表
		/// </summary>
		/// <param name="strList"></param>
		/// <param name="listSpriter"></param>
		/// <returns></returns>
		public static List<string> ParseList(this string strList, char listSpriter = ',')
		{
			var list = new List<string>();
			if(!string.IsNullOrEmpty(strList))
			{
				var str = strList.Trim();
				if(string.IsNullOrEmpty(strList))
				{
					return list;
				}

				var strArray = str.Split(listSpriter);
				list.AddRange(from str2 in strArray where !string.IsNullOrEmpty(str2) select str2.Trim());
			}

			return list;
		}

		/// <summary>
		/// 将指定字符分割的字符串去除前后空白符后组合成词典
		/// </summary>
		/// <param name="strMap"></param>
		/// <param name="keyValueSpriter"> 键值对分割符 </param>
		/// <param name="mapSpriter">词典元素分割符</param>
		/// <returns></returns>
		public static Dictionary<string, string> ParseMap(this string strMap, char keyValueSpriter = ':',
			char mapSpriter = ',')
		{
			var dictionary = new Dictionary<string, string>();
			if(!string.IsNullOrEmpty(strMap))
			{
				var strArray = strMap.Split(mapSpriter);
				foreach(var str in strArray)
				{
					if(!string.IsNullOrEmpty(str))
					{
						var strArray2 = str.Split(keyValueSpriter);
						if((strArray2.Length == 2) && !dictionary.ContainsKey(strArray2[0]))
						{
							dictionary.Add(strArray2[0].Trim(), strArray2[1].Trim());
						}
					}
				}
			}

			return dictionary;
		}

		/// <summary>
		/// 从“？~？”的字符串中获取随机数
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		/*
		public static float GetRandom(this string str)
		{
			var strs = str.Split('~');
			var num1 = strs[0].GetValue<float>();
			var num2 = strs[1].GetValue<float>();
			return str.Length == 1 ? num1 : Random.Range(Mathf.Min(num1, num2), Mathf.Max(num1, num2));
		}
		*/
		



		

		/// <summary>
		/// 替换第一个匹配值
		/// </summary>
		/// <param name="input"></param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		/// <param name="startAt"></param>
		/// <returns></returns>
		public static string ReplaceFirst(this string input, string oldValue, string newValue, int startAt = 0)
		{
			var index = input.IndexOf(oldValue, startAt);
			if(index < 0)
			{
				return input;
			}

			return (input.Substring(0, index) + newValue + input.Substring(index + oldValue.Length));
		}

		/// <summary>
		/// 是否存在中文字符
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool HasChinese(this string input)
		{
			return Regex.IsMatch(input, @"[\u4e00-\u9fa5]");
		}

		/// <summary>
		/// 是否存在空格
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static bool HasSpace(this string input)
		{
			return input.Contains(" ");
		}

		/// <summary>
		/// 删除特定字符
		/// </summary>
		/// <param name="str"></param>
		/// <param name="target"></param>
		/// <returns></returns>
		public static string RemoveString(this string str, params string[] targets)
		{
			return targets.Aggregate(str, (current, t) => current.Replace(t, string.Empty));
		}

		/// <summary>
		/// 拆分并去除空格
		/// </summary>
		/// <param name="str"></param>
		/// <param name="separator"></param>
		/// <returns></returns>
		public static string[] SplitAndTrim(this string str, params char[] separator)
		{
			var res = str.Split(separator);
			for(var i = 0; i < res.Length; i++)
			{
				res[i] = res[i].Trim();
			}

			return res;
		}

		/// <summary>
		/// 查找在两个字符串中间的字符串
		/// </summary>
		/// <param name="str"></param>
		/// <param name="front"></param>
		/// <param name="behined"></param>
		/// <returns></returns>
		public static string FindBetween(this string str, string front, string behined)
		{
			var startIndex = str.IndexOf(front) + front.Length;
			var endIndex = str.IndexOf(behined);
			if(startIndex < 0 || endIndex < 0)
				return str;
			return str.Substring(startIndex, endIndex - startIndex);
		}

		/// <summary>
		/// 查找在字符后面的字符串
		/// </summary>
		/// <param name="str"></param>
		/// <param name="front"></param>
		/// <returns></returns>
		public static string FindAfter(this string str, string front)
		{
			var startIndex = str.IndexOf(front) + front.Length;
			return startIndex < 0 ? str : str.Substring(startIndex);
		}

		public static string PathScale(this string path)
		{
			var systemPathSeparatorChar = System.IO.Path.DirectorySeparatorChar;
			var pathInPlatform = path.Replace('/', systemPathSeparatorChar);

			var dirList = pathInPlatform.Split(systemPathSeparatorChar).Where(it => it.IsNotNullAndEmpty()).ToArray();

			Stack<string> stack = new Stack<string>();

			var systemSepStr = systemPathSeparatorChar + "";

			if (pathInPlatform.StartsWith(systemSepStr))
			{
				stack.Push("");
			}
			else if (dirList[0] == ".")
			{
				stack.Push(".");
			}
			else
			{
			}
			foreach (var d in dirList)
			{
				if (d == ".")
				{
					continue;
				}
				if (d != "..")
				{
					stack.Push(d);
				}
				else
				{
					if (stack.Count == 1)
					{
						stack.Push(d);
					}
					else
					{
						var lastItem = stack.Pop();
						if (lastItem == "..")
						{
							stack.Push(lastItem);
							stack.Push(d);
						}
					}
					
				}
			}

			return string.Join(systemSepStr, stack.Reverse().ToArray());
		}
	}
}