
using UnityEngine;
using System;

namespace Framework.Core.Attributes
{
	[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Field
		, Inherited = true
		, AllowMultiple = false)]
	public sealed class RealTimeSyncInfoAttribute : Attribute
	{
		public RealTimeSyncInfoAttribute(int frequency)
		{
			
		}
	}
}
