using UnityEngine;
using System.Collections.Generic;
using Framework.Library.Singleton;
using UnityEngine.Profiling;
using Framework.Library.Log;
using Framework.Utils.Extensions;
using System;

namespace Framework.Core
{

	/// <summary>
	/// 内存检测器，目前只是输出Profiler信息
	/// </summary>
	class MemoryDetector
	{
		private const string TotalAllocMemroyFormation = "Alloc Memory : {0}M";
		private const string TotalReservedMemoryFormation = "Reserved Memory : {0}M";
		private const string TotalUnusedReservedMemoryFormation = "Unused Reserved: {0}M";
		private const string MonoHeapFormation = "Mono Heap : {0}M";
		private const string MonoUsedFormation = "Mono Used : {0}M";
		// 字节到兆
		private float ByteToM = 0.000001f;

		private Rect allocMemoryRect;
		private Rect reservedMemoryRect;
		private Rect unusedReservedMemoryRect;
		private Rect monoHeapRect;
		private Rect monoUsedRect;

		private int x = 0;
		private int y = 0;
		private int w = 0;
		private int h = 0;

		public MemoryDetector(GameConsole console)
		{
			this.x = 60;
			this.y = 60;
			this.w = 200;
			this.h = 20;

			this.allocMemoryRect = new Rect(x, y, w, h);
			this.reservedMemoryRect = new Rect(x, y + h, w, h);
			this.unusedReservedMemoryRect = new Rect(x, y + 2 * h, w, h);
			this.monoHeapRect = new Rect(x, y + 3 * h, w, h);
			this.monoUsedRect = new Rect(x, y + 4 * h, w, h);

			console.onGUICallback += OnGUI;
		}

		public void Init()
		{

		}

		void OnGUI()
		{
			GUI.contentColor = Color.white;
			GUI.Label(this.allocMemoryRect,
				string.Format(TotalAllocMemroyFormation, Profiler.GetTotalAllocatedMemoryLong() * ByteToM));
			GUI.Label(this.reservedMemoryRect,
				string.Format(TotalReservedMemoryFormation, Profiler.GetTotalAllocatedMemoryLong() * ByteToM));
			GUI.Label(this.unusedReservedMemoryRect,
				string.Format(TotalUnusedReservedMemoryFormation, Profiler.GetTotalUnusedReservedMemoryLong() * ByteToM));
			GUI.Label(this.monoHeapRect,
				string.Format(MonoHeapFormation, Profiler.GetMonoHeapSizeLong() * ByteToM));
			GUI.Label(this.monoUsedRect,
				string.Format(MonoUsedFormation, Profiler.GetMonoHeapSizeLong() * ByteToM));
		}
	}

	/// <summary>
	/// 帧率计算器
	/// </summary>
	class FPSCounter
	{
		// 帧率计算频率
		private const float calcRate = 0.5f;
		// 本次计算频率下帧数
		private int frameCount = 0;
		// 频率时长
		private float rateDuration = 0f;
		// 显示帧率
		private int fps = 0;

		public FPSCounter(GameConsole console)
		{
			console.onUpdateCallback += Update;
			console.onGUICallback += OnGUI;
		}

		public void Init()
		{
			this.frameCount = 0;
			this.rateDuration = 0f;
			this.fps = 0;
		}

		void Update()
		{
			++this.frameCount;
			this.rateDuration += Time.deltaTime;
			if(this.rateDuration > calcRate)
			{
				// 计算帧率
				this.fps = (int)(this.frameCount / this.rateDuration);
				this.frameCount = 0;
				this.rateDuration = 0f;
			}
		}

		void OnGUI()
		{
			GUI.color = Color.white;
			GUI.Label(new Rect(80, 20, 120, 20), "fps:" + this.fps.ToString());
		}
	}

	/// <summary>
	/// 控制台GUI输出类
	/// 包括FPS，内存使用情况，日志GUI输出
	/// </summary>
	/// 
	[PathInHierarchy("/[Game]/Console"), DisallowMultipleComponent]
	public class GameConsole : ToSingletonBehavior<GameConsole>
	{

		/// <summary>
		/// Update回调
		/// </summary>
		public delegate void OnUpdateCallback();
		/// <summary>
		/// OnGUI回调
		/// </summary>
		public delegate void OnGUICallback();

		public OnUpdateCallback onUpdateCallback = null;
		public OnGUICallback onGUICallback = null;
		/// <summary>
		/// FPS计数器
		/// </summary>
		private FPSCounter fpsCounter = null;
		/// <summary>
		/// 内存监视器
		/// </summary>
		private MemoryDetector memoryDetector = null;
		private bool showGUI = false;
		List<LogData> entries = new List<LogData>();
		Vector2 scrollPos;
		bool scrollToBottom = true;
		bool collapse;
#if UNITY_IOS
		bool mTouching = false;
#endif

		const int margin = 20;
		Rect windowRect = new Rect(margin + Screen.width * 0.5f, margin, Screen.width * 0.5f - (2 * margin), Screen.height - (2 * margin));

		GUIContent clearLabel    = new GUIContent("Clear",    "Clear the contents of the console.");
		GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");
		GUIContent scrollToBottomLabel = new GUIContent("ScrollToBottom", "Scroll bar always at bottom");


		public event Action<string> OnCommand; 

		private void Awake()
		{
			this.fpsCounter = new FPSCounter(this);
			this.memoryDetector = new MemoryDetector(this);
		}

		private void Start()
		{
			this.fpsCounter.Init();
			this.memoryDetector.Init();
		}

		public void Initlize()
		{
			var logHelper = LogUtil.GetLogHelper();
			if (logHelper != null)
			{
				logHelper.handleLog += HandleLog;
			}
		}



		void Update()
		{
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
			if (Input.GetKeyUp(KeyCode.F1))
				this.showGUI = !this.showGUI;
#elif UNITY_ANDROID
			if (Input.GetKeyUp(KeyCode.Escape))
				this.showGUI = !this.showGUI;
#elif UNITY_IOS
			if (!mTouching && Input.touchCount == 4)
			{
				mTouching = true;
				this.showGUI = !this.showGUI;
			} else if (Input.touchCount == 0){
				mTouching = false;
			}
#endif

			if (this.onUpdateCallback != null)
				this.onUpdateCallback();
		}

		const string BtnText = "Clear PlayerPrefs Data And Exit Game";
		const string windowText = "Console";

		void OnGUI()
		{
			if (!this.showGUI)
				return;

			if (this.onGUICallback != null)
				this.onGUICallback ();
			GUI.contentColor = Color.white;
			if (GUI.Button (new Rect (100, 200, 240, 20), BtnText))
			{
				PlayerPrefs.DeleteAll ();
#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
#else
				Application.Quit();
#endif
			}
			windowRect = GUILayout.Window(10100, windowRect, ConsoleWindow, windowText);
		}

		private string commandText = "";

		/// <summary>
		/// A window displaying the logged messages.
		/// </summary>
		void ConsoleWindow (int windowID)
		{
			if (scrollToBottom) {
				GUILayout.BeginScrollView (Vector2.up * entries.Count * 100.0f);
			}
			else {
				scrollPos = GUILayout.BeginScrollView (scrollPos);
			}
			// Go through each logged entry
			for (int i = 0; i < entries.Count; i++) {
				LogData entry = entries[i];
				// If this message is the same as the last one and the collapse feature is chosen, skip it
				if (collapse && i > 0 && entry.Message == entries[i - 1].Message) {
					continue;
				}
				// Change the text colour according to the log type
				switch (entry.Level) {
					case LogType.Error:
					case LogType.Exception:
						GUI.contentColor = Color.red;
						break;
					case LogType.Warning:
						GUI.contentColor = Color.yellow;
						break;
					default:
						GUI.contentColor = Color.white;
						break;
				}
				if (entry.Level == LogType.Exception)
				{
					GUILayout.Label(entry.Message + " || " + entry.StackTrace);
				} else {
					GUILayout.Label(entry.Message);
				}
			}
			GUI.contentColor = Color.white;
			GUILayout.EndScrollView();
			GUILayout.BeginHorizontal();
			// Clear button
			if (GUILayout.Button(clearLabel)) {
				entries.Clear();
			}
			// Collapse toggle
			collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));
			scrollToBottom = GUILayout.Toggle (scrollToBottom, scrollToBottomLabel, GUILayout.ExpandWidth (false));
			GUILayout.EndHorizontal();
			if (commandText.IsNotNullAndEmpty() && Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
			{
				if (OnCommand != null)
				{
					OnCommand(commandText);
					commandText = "";
				}
				Event.current.Use();
			}
			GUI.SetNextControlName("CommandLine");
			commandText = GUILayout.TextField(commandText);
			
			//GUI.FocusControl("CommandLine");
			// Set the window to be draggable by the top title bar
			GUI.DragWindow(new Rect(0, 0, 10000, 20));
		}

		void HandleLog (LogData logData)
		{
			entries.Add(logData);
		}
	}
}