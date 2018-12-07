using System;

namespace Framework.Core.Attributes
{
	public class DescriptionAttribute : Attribute
	{
		public string Description { get; private set; }
		public DescriptionAttribute(string description)
		{
			this.Description = description;
		}
	}

	public static class DescriptionHelper
	{
		/// <summary>
		/// 返回枚举项的描述信息。
		/// </summary>
		/// <param name="value">要获取描述信息的枚举项。</param>
		/// <returns>枚举想的描述信息。</returns>
		public static string GetDescription(this Enum value, bool isTop = false)
		{
			Type enumType = value.GetType();
			DescriptionAttribute attr = null;
			if (isTop)
			{
				attr = (DescriptionAttribute)Attribute.GetCustomAttribute(enumType, typeof(DescriptionAttribute));
			}
			else
			{
				// 获取枚举常数名称。
				string name = Enum.GetName(enumType, value);
				if (name != null)
				{
					// 获取枚举字段。
					System.Reflection.FieldInfo fieldInfo = enumType.GetField(name);
					if (fieldInfo != null)
					{
						// 获取描述的属性。
						attr = Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute), false) as DescriptionAttribute;
					}
				}
			}

			if (attr != null && !string.IsNullOrEmpty(attr.Description))
				return attr.Description;
			else
				return string.Empty;

		}
	}
}
