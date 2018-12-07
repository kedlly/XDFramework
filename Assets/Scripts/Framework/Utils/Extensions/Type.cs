
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Framework.Utils.Extensions
{

	/// <summary>
	/// 通过编写方法并且添加属性可以做到转换至String 如：
	/// 
	/// <example>
	/// [ToStringable]
	/// public static string ConvertToString(TestObj obj)
	/// </example>
	///
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class ToStringableAttribute : Attribute { }

	/// <summary>
	/// 通过编写方法并且添加属性可以做到转换至String 如：
	/// 
	/// <example>
	/// [FromStringable]
	/// public static TestObj ConvertFromString(string str)
	/// </example>
	///
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class FromStringableAttribute : Attribute { }

	public static partial class ExtensionUtils
	{
		#region  Type 类型扩展方法

		/// <summary>
		/// 判断当前类型是否实现某个特定接口
		/// </summary>
		/// <returns><c>true</c>如果实现对应接口<c>false</c> 否则 </returns>
		/// <param name="type">Type.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static bool ImplementsInterface<T>(this Type type)
		{
			return !type.IsInterface && type.GetInterfaces().Contains(typeof(T));
		}

		/// <summary>
		/// 判断当前对象是否实现某个特定接口
		/// </summary>
		/// <returns><c>true</c>, 如果实现对应接口, <c>false</c> 否则.</returns>
		/// <param name="type">Type.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static bool ImplementsInterface<T>(this object obj)
		{
			return obj.GetType().ImplementsInterface<T>();
		}

		public static bool ImplementsGenericInterface(this Type type, Type GenericType)
		{
			return type.GetGenericInterfaces().Where(_type => _type.GetGenericTypeDefinition() == GenericType).Count() > 0;
		}

		public static IEnumerable<Type> GetGenericInterfaces(this Type type)
		{
			return type.IsInterface ? null : type.GetInterfaces().Where(_type => _type.IsGenericType);
		}

		public static Type CreateGenericClassType(this Type unboundGenericType, params Type[] genericArgs)
		{
			if (unboundGenericType.IsGenericTypeDefinition)
			{
				return unboundGenericType.MakeGenericType(genericArgs);
			}
			return unboundGenericType;
		}


		//可转换类型列表
		public static readonly List<Type> convertableTypes = new List<Type>
		{
			typeof(int),
			typeof(string),
			typeof(float),
			typeof(double),
			typeof(byte),
			typeof(long),
			typeof(bool),
			typeof(short),
			typeof(uint),
			typeof(ulong),
			typeof(ushort),
			typeof(sbyte),
			typeof(Dictionary<,>),
			typeof(KeyValuePair<,>),
			typeof(List<>),
			typeof(Enum),
			typeof(Array)
		};

		/// <summary>
		/// 是否为可转换字符串的类型
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsConvertableType(this Type type)
		{
			return CanConvertFromString(type) && CanConvertToString(type) || convertableTypes.Contains(type);
		}

		/// <summary>
		/// 是否可以从String中转换出来
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool CanConvertFromString(this Type type)
		{
			var methodInfos = type.GetMethods();
			return methodInfos.Select(method => method.GetCustomAttributes(false)).Any(attrs => attrs.OfType<FromStringableAttribute>().Any());
		}

		/// <summary>
		/// 是否可以转换为String
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool CanConvertToString(this Type type)
		{
			var methodInfos = type.GetMethods();
			return methodInfos.SelectMany(method => method.GetCustomAttributes(false)).OfType<ToStringableAttribute>().Any();
		}

		public static FieldInfo GetPublicStaticFieldInfo(this Type type, string name)
		{
			var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public).Where(it => it.Name == name);
			if (fields.Count() > 0)
			{
				return fields.Single();
			}
			return null;
		}

		public static T GetPublicStaticField<T>(this Type clsType, string name) where T : class
		{
			var fi = clsType.GetPublicStaticFieldInfo(name);
			if (fi == null)
			{
				return null;
			}
			return fi.GetValue(null) as T;
		}

		#endregion
		#region AppDomain 扩展

		private static Assembly[] CurrentAppDomain { get; set; }


		static ExtensionUtils()
		{
			CurrentAppDomain = AppDomain.CurrentDomain.LegalAssemblies().ToArray();
		}

		private static IEnumerable<Assembly> LegalAssemblies(this AppDomain aAppDomain)
		{
			return aAppDomain.GetAssemblies().Where(assembly =>
						//-------------------------Base Assembly-----------------------
						(assembly.FullName.StartsWith("Mono.")) ||
						(assembly.FullName.StartsWith("UnityScript")) ||
						(assembly.FullName.StartsWith("Boo.Lang")) ||
						(assembly.FullName.StartsWith("System")) ||
						(assembly.FullName.StartsWith("I18N")) ||
						(assembly.FullName.StartsWith("UnityEngine")) ||
						(assembly.FullName.StartsWith("UnityEditor")) ||
						(assembly.FullName.StartsWith("mscorlib")) ||
						(assembly.FullName.StartsWith("Unity.")) ||
						(assembly.FullName.StartsWith("Accessibility")) ||
						(assembly.FullName.StartsWith("ExCSS.")) ||
						(assembly.FullName.StartsWith("nunit.")) ||
						//-------------------------Plugins-----------------------------
						(assembly.FullName.StartsWith("Newtonsoft.")) ||
						(assembly.FullName.StartsWith("protobuf")) ||
						(assembly.FullName.StartsWith("SyntaxTree.")) ? false : true
					);
		}

		public static IEnumerable<Type> GetAllTypesDerivedFrom(this AppDomain aAppDomain, Type aType)
		{
			var itor = AppDomain.CurrentDomain == aAppDomain ? CurrentAppDomain : aAppDomain.LegalAssemblies();
			return itor.SelectMany(it => it.GetTypes()).Where(it => it.IsSubclassOf(aType));
		}

		public static IEnumerable<Type> GetAllTypesDerivedFrom<T>(this AppDomain aAppDomain)
		{
			return aAppDomain.GetAllTypesDerivedFrom(typeof(T));
		}

		public static IEnumerable<Type> GetAllTypesImplementsInterface<T>(this AppDomain aAppDomain, bool ifContainsAbstract = false)
		{
			var itor = AppDomain.CurrentDomain == aAppDomain ? CurrentAppDomain : aAppDomain.LegalAssemblies();
			return itor.SelectMany(it => it.GetTypes()).Where(
				it => it.ImplementsInterface<T>() && (ifContainsAbstract || !it.IsAbstract));
		}

		#endregion
		#region  Attribute 扩展
		public static T[] GetCustomAttributes<T>(this Type type, bool inherit = false)
		{
			return type.GetCustomAttributes(typeof(T), inherit) as T[];
		}
#endregion
	}
}