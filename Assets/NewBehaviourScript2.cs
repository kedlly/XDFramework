
using UnityEngine;
using System;
using System.Reflection;
using Framework.Library.XMLStateMachine;
using Framework.Core.FlowControl;
using Framework.Utils.Extensions;

public class NewBehaviourScript2 : MonoBehaviour {

	// Use this for initialization

	

	//private string info;//显示的信息/  
	private int frameTime;//记录按下的时间/  

	private string test1 = "";
	private int test2 = 0;

	private string test3 = "";
	private bool test4 = true;
	private bool get { get; set; }

	private Timeline tl = new Timeline(5);

	public Action AAAAA;

	

	bool test5()
	{
		return false;
	}

	bool test10(string a, string b)
	{
		return a.CompareTo(b) < 0;
	}

	void hello()
	{
		Debug.Log("Enter hello");
	}

	void world()
	{
		Debug.Log("Enter world");
	}

	void showme()
	{
		Debug.Log("Enter showme");
	}

	FlipFlopNormal f = new FlipFlopNormal(5);

	public AnimationCurve cur;

	Delay d = new Delay(3);

	FSMComponent fSM;

	void Awake()
	{
		
		Type t = typeof(NewBehaviourScript2);
		fSM = GetComponent<FSMComponent>();
		fSM.LoadFSM(this);
		//var mi = t.GetMember("test5", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		FieldInfo fi = t.GetField("test4", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
		Debug.Log(fi);
		f.SetAction(0, () => Debug.Log(f.ActionIndex + "  First"));
		f.SetAction(0, () => Debug.Log(f.ActionIndex + "  OOOSD"));
		f.SetAction(1, () => Debug.Log(f.ActionIndex + "  Second"));
		f.SetAction(4, () => Debug.Log(f.ActionIndex + "  Third"));
		//tl.OnUpdate += () => Debug.Log("onUpdate timeline" + tl.CurrenTime);
		tl.OnFinished += ()=> Debug.LogError("onFinished timeline" + tl.CurrenTime);
		d.OnExit += () => Debug.Log("Delay :" + d.DelayTime);

		tl.AddEvent(1, () => Debug.Log("Event :" + tl.CurrenTime + " / " + 1));

		tl.AddEvent(3, () => Debug.Log("Event :" + tl.CurrenTime + " / " + 3));

		tl.AddEvent(5, () => Debug.Log("Event :" + tl.CurrenTime + " / " + 5));

		var file = new Framework.Library.Configure.IniFile();
		file.Write("Hello", "world", "test");

		
	}

	void Start () {
		
	}

	// Update is called once per frame
	bool kf = false;
	void Update () {
		bool last = kf;
		kf = tl.IsPlaying;
		if (kf != last)
		{
			Debug.Log(tl.IsPlaying);
		}
		
		
	}

	void OnGUI()
	{
		GUI.Label(new Rect(50, 10, 200, 20), fSM.StateName);
		GUI.Label(new Rect(50, 30, 200, 20), test1 + '/' + test2 + "|||" + test3 + '/' + test4 );
		if (GUI.Button(new Rect(50, 50, 200, 20), "Play"))
		{
			fSM.PushEvent("watch.start"); 
			//tl.Play();
			//d.Execute();
		
		}
		if (GUI.Button(new Rect(50, 100, 200, 20), "PlayFromStart"))
		{
			fSM.PushEvent("watch.split");
			//tl.PlayFromStart();
		}
		if (GUI.Button(new Rect(50, 150, 200, 20), "stop"))
		{
			fSM.PushEvent("watch.stop");
			//tl.Stop();
		}
		if (GUI.Button(new Rect(50, 200, 200, 20), "Reverse"))
		{
			fSM.PushEvent("watch.unsplit");
			get = true;
			//tl.Reverse();
		}
		if (GUI.Button(new Rect(50, 250, 200, 20), "ReverseFromEnd"))
		{
			fSM.PushEvent("watch.reset");
			//tl.ReverseFromEnd();
		}

		if(GUI.Button(new Rect(50, 300, 200, 20), "set new time"))
		{
			fSM.PushEvent("A");

			//tl.SetNewTime(1);
		}

		if(GUI.Button(new Rect(50, 350, 200, 20), "B"))
		{
			fSM.PushEvent("B");
		}

		if(GUI.Button(new Rect(50, 400, 200, 20), "C"))
		{
			fSM.PushEvent("C");
		}

		if(GUI.Button(new Rect(50, 450, 200, 20), "D"))
		{
			fSM.PushEvent("D");
			//f.Enter();
			tl.Play();
		}

		//标签/  
		//图片按钮,点击后显示图片/  
		if (GUI.Button(new Rect(280, 250, 200, 200), "asdfasf"))
		{
			//info = "您点击了图片按钮";
		}
		//标签/  
		GUI.Label(new Rect(500, 10, 200, 20), "持续按下的时间：" + frameTime);
		//连续按钮，点击后显示按下的时间/  
		if (GUI.RepeatButton(new Rect(500, 250, 200, 20), "持续按下"))
		{
			frameTime++;
			//info = "您按下了连续按钮";
		}
		//每当鼠标按下时将frameTime重置，一遍进行下次记录/  
		if (Input.GetMouseButtonDown(0))
		{
			frameTime = 0;
		}
	}
}

