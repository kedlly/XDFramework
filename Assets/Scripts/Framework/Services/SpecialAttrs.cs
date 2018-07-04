using System;

namespace Framework.Services
{
	[AttributeUsageAttribute(AttributeTargets.Method)]
	public class ExternServiceAPIAttribute: Attribute
	{
		public ExternServiceAPIAttribute()	{}
	}
}
