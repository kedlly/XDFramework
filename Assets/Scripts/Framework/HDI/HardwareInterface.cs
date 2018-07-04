

using System.Linq;
using UnityEngine;

namespace Framework.HDI
{
	/// <summary>
	/// 硬件相关信息
	/// </summary>
	public static class DeviceUtil
	{
		/// <summary>
		/// 启动设备震动: 未实现
		/// </summary>
		public static void Vibrate()
		{
#if UNITY_IOS
			
#endif
		}

		/// <summary>
		/// 是否运行于Apple产品上
		/// iPhone/iPad/OS X
		/// </summary>
		/// <returns></returns>
		public static bool IsApplePlatform()
		{
			return RuntimePlatform.IPhonePlayer == Application.platform
				   || RuntimePlatform.OSXEditor == Application.platform
				   || RuntimePlatform.OSXPlayer == Application.platform;
		}

		/// <summary>
		/// 查询当前是否运行于Android设备上
		/// </summary>
		/// <returns>
		/// 是 / 否
		/// </returns>
		public static bool IsAndroidPlatform()
		{
			return RuntimePlatform.Android == Application.platform;
		}

		/*
		iPhone = 1,
		iPhone3G = 2,
		iPhone3GS = 3,
														iPodTouch1Gen = 4,
														iPodTouch2Gen = 5,
														iPodTouch3Gen = 6,
								iPad1Gen = 7,
		iPhone4 = 8,
														iPodTouch4Gen = 9,
								iPad2Gen = 10,
		iPhone4S = 11,
								iPad3Gen = 12,
		iPhone5 = 13,
														iPodTouch5Gen = 14,
								iPadMini1Gen = 15,
								iPad4Gen = 0x10,
		iPhone5C = 0x11,
		iPhone5S = 0x12,
								iPadAir1 = 0x13,
								iPadMini2Gen = 20,
		iPhone6 = 0x15,
		iPhone6Plus = 0x16,
								iPadMini3Gen = 0x17,
								iPadAir2 = 0x18,
		iPhone6S = 0x19,
		iPhone6SPlus = 0x1a,	
								iPadPro1Gen = 0x1b,
								iPadMini4Gen = 0x1c,
		iPhoneSE1Gen = 0x1d,
								iPadPro10Inch1Gen = 30,
		iPhone7 = 0x1f,
		iPhone7Plus = 0x20,
														iPodTouch6Gen = 0x21,
								iPad5Gen = 0x22,
								iPadPro2Gen = 0x23,
								iPadPro10Inch2Gen = 0x24,
		iPhone8 = 0x25,
		iPhone8Plus = 0x26,
		iPhoneX = 0x27,
								iPhoneUnknown = 0x2711,
								iPadUnknown = 0x2712,
								iPodTouchUnknown = 0x2713
	*/

		public static int[] IPhoneIDs =
		{
			1, 2, 3, 8, 10, 11, 13, 0x11, 0x12, 0x15, 0x16, 0x19, 0x1a, 0x1d, 0x1f, 0x20, 0x25, 0x26, 0x27
		};

		public static int[] IPadIDs =
		{
			7, 10, 12, 15, 0x10, 0x13, 20, 0x17, 0x18, 0x1b, 0x1c, 30, 0x22, 0x23, 0x24,
		};

		public static int[] IPodIDs =
		{
			4, 5, 6, 9, 14, 0x21,
		};

		/// <summary>
		/// 判断是否允许在编辑器模式下
		/// </summary>
		/// <returns>是/否</returns>
		public static bool IsEditor()
		{
			return RuntimePlatform.OSXEditor == Application.platform
				   || RuntimePlatform.WindowsEditor == Application.platform;
		}

		#region refactor to ResolutionHelper
		/// <summary>
		/// iPhone 5s height/width 1.78 1280x720
		/// </summary>
		/// <returns></returns>
		public static bool PhoneResolution()
		{
			float aspect = Screen.height > Screen.width ? (float)Screen.height / Screen.width : (float)Screen.width / Screen.height;
			return aspect > (16.0f / 9 - 0.05) && aspect < (16.0f / 9 + 0.05);
		}

		/// <summary>
		/// height/width = 1.67  iPhone 4/4s 960x640
		/// </summary>
		/// <returns></returns>
		public static bool Phone167Resolution()
		{
			float aspect = Screen.height > Screen.width ? (float)Screen.height / Screen.width : (float)Screen.width / Screen.height;
			return aspect > (1920.0f / 1152 - 0.05) && aspect < (1920.0f / 1152 + 0.05);
		}

		/// <summary>
		/// height/width = 1.60  Huawei Pad?  2560 / 1600
		/// </summary>
		/// <returns></returns>
		public static bool Phone160Resolution()
		{
			float aspect = Screen.height > Screen.width ? (float)Screen.height / Screen.width : (float)Screen.width / Screen.height;
			return aspect > (2560.0f / 1600 - 0.05) && aspect < (2560.0f / 1600 + 0.05);
		}


		/// <summary>
		/// Pad resolution  iPad   2048 / 1536
		/// </summary>
		/// <returns></returns>
		public static bool PadResolution()
		{
			float aspect = Screen.height > Screen.width ? (float)Screen.height / Screen.width : (float)Screen.width / Screen.height;
			return aspect > (4.0f / 3 - 0.05) && aspect < (4.0f / 3 + 0.05);
		}
		#endregion

		/// <summary>
		/// 判断是否运行于iphone上
		/// </summary>
		/// <returns></returns>
		public static bool IsIPhone()
		{
			var deviceID = GetDeviceID();
			return IPhoneIDs.Contains(deviceID);
		}

		public static bool IsIPod()
		{
			var deviceId = GetDeviceID();
			return IPadIDs.Contains(deviceId);
		}

		public static int GetDeviceID()
		{
#if UNITY_EDITOR || UNITY_IOS
			return (int)UnityEngine.iOS.Device.generation;
			
#else
			return -1;
#endif
		}

		/// <summary>
		/// 判断是否运行于ipad设备上
		/// </summary>
		/// <returns></returns>
		public static bool IsIPad()
		{
			var deviceID = GetDeviceID();
			return IPadIDs.Contains(deviceID);
		}

		public static string GetOSSuffix()
		{
			string suffix = "Undefined";
			switch(Application.platform)
			{
				case RuntimePlatform.WindowsEditor:
					suffix = "Windows_Editor";
					break;
				case RuntimePlatform.LinuxEditor:
					suffix = "Linux_Editor";
					break;
				case RuntimePlatform.OSXEditor:
					suffix = "OSX_Editor";
					break;
				case RuntimePlatform.OSXPlayer:
					suffix = "OSX_Player";
					break;
				case RuntimePlatform.WindowsPlayer:
					suffix = "Windows_Player";
					break;
				case RuntimePlatform.LinuxPlayer:
					suffix = "Linux_Player";
					break;
				case RuntimePlatform.IPhonePlayer:
					suffix = "iOS";
					break;
				case RuntimePlatform.Android:
					suffix = "Android";
					break;
				default:
					break;
			}
			return suffix;
		}

		public static bool IsAndroidPad()
		{
			return IsAndroidPlatform() && PadResolution();
		}

		public static bool IsAndroidPhone()
		{
			return IsAndroidPlatform() && PhoneResolution();
		}

		//-- 奇葩的魅族分辨率
		public static bool IsAndroidPhone167()
		{
			return IsAndroidPlatform() && Phone167Resolution();
		}

		public static bool IsAndroidPhone160()
		{
			return IsAndroidPlatform() && Phone160Resolution();
		}
	}
}