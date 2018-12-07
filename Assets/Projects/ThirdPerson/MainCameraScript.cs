using Framework.Core;
using Protocol;
using System.Collections.Generic;
using UnityEngine;
using Framework.Library.ObjectPool;
using Framework.Library.StateMachine;
using FCD = Framework.Core.DataStruct;
using System.Linq;
using Game.Core;
using Framework.Library.ThreadPool;
using Framework.Core.DataStruct;

namespace Projects.ThirdPerson
{
	public class Factory<T> : Framework.Library.ObjectPool.IObjectFactory<T> 
		where T : class, new()
	{
		public void Copy(T target, T template)
		{
			
		}

		public T Create()
		{
			return new T();
		}

		void IObjectFactory<T>.Release(T obj)
		{
			throw new System.NotImplementedException();
		}
	}

	class PostObject : IPoolable
	{
		void IPoolable.OnAllocated()
		{
			Debug.Log("OnAllocated.....");
		}

		void IPoolable.OnRecycled()
		{
			Debug.Log("OnAllocated_____");
		}
	}

	public class Factory2 : IObjectFactory<GameObject>, IObjectFactory<object>
	{
		public void Copy(object target, object template)
		{
			throw new System.NotImplementedException();
		}

		public void Copy(GameObject target, GameObject template)
		{
			throw new System.NotImplementedException();
		}

		public GameObject Create()
		{
			throw new System.NotImplementedException();
		}

		GameObject IObjectFactory<GameObject>.Create()
		{
			return new GameObject("Normal");
		}

		object IObjectFactory<object>.Create()
		{
			throw new System.NotImplementedException();
		}


		void IObjectFactory<GameObject>.Release(GameObject obj)
		{
			throw new System.NotImplementedException();
		}

		void IObjectFactory<object>.Release(object obj)
		{
			throw new System.NotImplementedException();
		}
	}

	public class Factory3 : Factory2
	{

	}

	public class MainCameraScript : MonoBehaviour
	{
		int index = 0;
		byte[] buffer = new byte[1024];
		Queue<ProtoBuf.IExtensible> networkData = new Queue<ProtoBuf.IExtensible>();
		private void Start()
		{
			/**
			GameConsole.Instance.OnCommand += cmd =>
			{
				switch (cmd)
				{
					case "1":
						cmd = "connect 127.0.0.1 8192";
						break;
					case "2":
						cmd = "login kedlly zhaoxionghui";
						break;
					case "3":
						cmd = "login uav 123456";
						break;
					case "4":
						cmd = "login robot 123456";
						break;
					default:
						break;
				}

				var cmds = cmd.Split(' ', '\t');
				
				if (cmds[0] == "connect")
				{
					var ip = cmds[1];
					var port = int.Parse(cmds[2]);
					Debug.Log("try to connect ip: " + ip + " port :" + port);
					NetworkManager.Instance.Connect(ip, port);
				}
				if (cmds[0] == "login")
				{
					var name = cmds[1];
					var password = cmds[2];
					var dataObject = new Request_LoginAuth();
					dataObject.username = name;
					dataObject.password = password;
					
					System.ArraySegment<byte> p = new System.ArraySegment<byte>(buffer);
					dataObject.Pack().Serialize(p).Send();

					//dataObject.Pack().Serialize().Send();
				}
				if (cmds[0] == "quality")
				{
					QualitySettings.SetQualityLevel(int.Parse(cmds[1]));
				}
				if (cmds[0] == "print")
				{
					if (cmds[1] == "frameTime")
					{
						Debug.Log("deltaTime :"+Time.deltaTime + " fixedDeltaTime "+ Time.fixedDeltaTime);
					}
				}
				if (cmds[0] == "f")
				{
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "0123456789abcdef"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "1OOOOOOOOOOOOOOO"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "2123456789abcdef"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "3KKKKKKKKKKKKKKK"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "4123456789abcdef"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "5TTTTTTTTTTTTTTT"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "6123456789abcdef"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "7RRRRRRRRRRRRRRR"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "8123456789abcdef"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "9MMMMMMMMMMMMMMM"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "a123456789abcdef"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "bPPPPPPPPPPPPPPP"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "c123456789abcdef"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "d123456789abcdef"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "e123456789abcdef"))).Send();
					(new System.ArraySegment<byte>(System.Text.Encoding.Default.GetBytes(index + "f123456789abcdef"))).Send();
					index++;
				}
			};
			*/
			Framework.FrameworkUtils.Application.Console.OnText += delegate (string s)
			{
				Debug.Log(s);
			};

			var pbf2 = new Factory<PostObject>().CreatePool();
 			pbf2.Allocate();
 			pbf2.Allocate();

// 			var pbf = new Factory2();
// 			pbf.Allocate();
// 			pbf.Allocate();

			Framework.FrameworkUtils.Application.Console.Register('!', "fire", delegate (string[] s)
			{
				Debug.Log(string.Join("/", s));
				if (s.Length == 0)
				{
					Debug.Log(f.CurrentStateName);
				}
				foreach (var cmd in s.Skip(1))
				{
					f.PushEvent(cmd);
					Debug.Log(f.CurrentStateName);
				}
				
			});

			ProtocolProcessor.Register<NetworkCommunication>();
			NetworkManager.Instance.OnDataReceived += data =>
			{

				var obj = data.Deserialize().Unpack();
				if (obj == null)
				{
					return;
				}
				
				ProtocolProcessor.MessagePump(obj);
			};

			byte[] bufferLast = new byte[512];
			byte[] bufferLast2 = new byte[512];

            NetworkManager.Instance.OnAsDataReceived += data => {
                Debug.Log("thread id:" + System.Threading.Thread.CurrentThread.ManagedThreadId + "type:" + "Count :" + data.Count);
                try {
                    var obj = data.Deserialize().Unpack();
                    if (obj == null) {
                        return;
                    }
                    if (bufferLast != null) {
                        byte[] q = bufferLast;
                        System.Array.Copy(data.Array, data.Offset, q, 0, data.Count);
                    }
                    this.networkData.Enqueue(obj);
                } catch (System.Exception ex) {
                    byte[] q = bufferLast2;
                    byte[] p = bufferLast;
                    System.Array.Copy(data.Array, data.Offset, q, 0, data.Count);

                    print(ex.Message);
                    throw;
                }
            };

            NetworkManager.Instance.OnConnected += (System.Net.Sockets.SocketAsyncEventArgs saea) =>
			{
				if (saea.SocketError == System.Net.Sockets.SocketError.Success)
				{
					Debug.Log("connection build succeed to Endpoint:" + saea.RemoteEndPoint);
				}
				else
				{
					Debug.LogErrorFormat("connection build failed to Endpoint: {0} Reason: {1}", saea.RemoteEndPoint , saea.SocketError);
				}
			};
			var p99 = Framework.Library.StateMachine.Parser.FSMXMLParser.LoadFromXML(obj.text);
			var fsm = new FSMExecutor<MainCameraScript>(p99);
			f = Utils.GetStateMachineExecutor(this, obj);
			f = Utils.GetStateMachineExecutor(this, obj);
			var t1 = FCD.Tuple.Create(p99, fsm);
			var t2 = FCD.Tuple.Create(p99, fsm);
			
			Debug.Log("------------>>>>>" + (t1 .Equals( t2)));

			// 			var threeMi = typeof(MainCameraScript).GetMethod("threeParam", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
			// 			//var df = (Func<MainCameraScript, string, int, bool>)Delegate.CreateDelegate(typeof(Func<MainCameraScript, string, int, bool>), null, threeMi);
			// 			var df = (Func< MainCameraScript, bool>)Delegate.CreateDelegate(typeof(Func<MainCameraScript, bool>), threeMi);
			// 			Debug.Log("--|||||---->>>>>" + df.Invoke(this));
			// 			bool dfValue = (bool)df.DynamicInvoke(cob);
			// 			Debug.Log("------------>>>>>" + dfValue);
			GameEventManager.Instance.AddListener(GameEventType.kInput_Keyboard, (o, e) => 
			{
				Debug.Log("-----------------1");
				Debug.Log(o);
				Debug.Log(e);
				Debug.Log("-----------------2");
			});
			GameEventManager.Instance.AddListener(GameEventType.kInput_Keyboard, (o, e) =>
			{
				Debug.Log("-----------------3");
				Debug.Log(o);
				Debug.Log(e);
				Debug.Log("-----------------4");
				GameEventManager.Instance.RemoveTypeAllListener(this.GetType());
			});
			GameEventManager.Instance.AddListener(GameEventType.kInput_Keyboard, (o, e) =>
			{
				Debug.Log("-----------------5");
				Debug.Log(o);
				Debug.Log(e);
				Debug.Log("-----------------6");
			});
			Debug.Log(GameEventType.kInput_Keyboard);
			Debug.Log(GameEventType.kGameEvent_Flag);
			Debug.Log(GameEventType.kInput_Mouse);
			Debug.Log(GameEventType.kOther);
			Debug.Log("~~~~~~~~~~~~~~~~~~");
			for (int i = 0; i < GameEventType.CreationIndex; ++i)
			{
				var it = GameEventType.GetEventTypeByIndex(i);
				Debug.Log(it);
			}
			Debug.Log(SystemEvent.CreationIndex);
			FLThreadPool.Instance.AddTask(() =>
			{
				Debug.Log("~/" + System.Threading.Thread.CurrentThread.ManagedThreadId);
			});// 
		}

		public bool threeParam(params string[] a)
		{
			//string a = "";
				int p = 100;
			Debug.Log("~~~~~~~~~~~~~~~" + a[0] + '/' + a[1]);
			return a != null && p > 0;
		}

		void onEntry()
		{
			Debug.Log("~~~~~~~~~~~~~~~" + "OnEntry");
		}

		void onExit()
		{
			Debug.Log("~~~~~~~~~~~~~~~" + "onExit");
		}

		public void threeParam2()
		{
			Debug.Log("SSSSSSSSSSS");
		}

		private bool getfunc(FCD.Tuple<string, int> t)
		{
			return t.Item2 > 0 && t.Item1 != null;
		}

		private bool getfunc3(string t, int num, bool k)
		{
			Debug.Log("~~~~~~+++~~~~~~~" + t + '/' +num + '|' + k);
			return k && t != null && num > 10;
		}

		private bool getfunc2(FCD.Tuple<string, int> t, int a, object sd = null, int ok = 0)
		{
			return t.Item2 > 0 && t.Item1 != null;
		}

		FSMExecutor<MainCameraScript> f;
		public TextAsset obj;
		SpringValue sv = new SpringValue { Gravity = 0.3f, Sensitivity = 0.3f, Dead = 0.001f };
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Q))
			{
				//var t = Input.GetAccelerationEvent(0);
				Framework.FrameworkUtils.Application.Exit();
			}
			if (networkData.Count > 0)
			{
				var obj = networkData.Dequeue();
				ProtocolProcessor.MessagePump(obj);
			}
			sv.Update();
			if (Input.GetKeyDown(KeyCode.X))
			{
				target = true;
			}
			if (Input.GetKeyUp(KeyCode.X)) target = false;
			if (Input.GetKeyDown(KeyCode.K)) sv.Negative = true;
			if (Input.GetKeyUp(KeyCode.K)) sv.Negative = false;
		}

		bool target = false;
		private void OnGUI()
		{
			GUI.Label(new Rect(50, 10, 200, 20), target.ToString());
			//GUI.Label(new Rect(50, 30, 200, 20), test1 + '/' + test2 + "|||" + test3 + '/' + test4 );
			if (GUI.Button(new Rect(50, 50, 200, 20), "watch.start"))
			{
				f.PushEvent("watch.start");
				//tl.Play();
				//d.Execute();
				GameEventManager.Instance.SendEvent(GameEventType.kInput_Keyboard, this, null);

				Screen.SetResolution(1920, 1080, true);
			}
			if (GUI.Button(new Rect(50, 100, 200, 20), "watch.split"))
			{
				FLThreadPool.Instance.DropTasks();
				//f.PushEvent("watch.split");
				//tl.PlayFromStart();
			}
			if (GUI.Button(new Rect(50, 150, 200, 20), "watch.stop"))
			{
				f.PushEvent("watch.stop");
				for (int i = 0; i < 40000; ++i)
				{
					var p = i;
					FLThreadPool.Instance.AddTask(() => {
						Debug.Log(p + "/" + System.Threading.Thread.CurrentThread.ManagedThreadId);
					});// 
					
				}
				//tl.Stop();
			}
			if (GUI.Button(new Rect(50, 200, 200, 20), "watch.unsplit"))
			{
				f.PushEvent("watch.unsplit");
				//get = true;
				//tl.Reverse();
			}
			if (GUI.Button(new Rect(50, 250, 200, 20), "watch.reset"))
			{
				f.PushEvent("watch.reset");
				//tl.ReverseFromEnd();
			}

			if(GUI.Button(new Rect(50, 300, 200, 20), "set new time"))
			{
				f.PushEvent("A");

				//tl.SetNewTime(1);
			}

			if(GUI.Button(new Rect(50, 350, 200, 20), "B"))
			{
				f.PushEvent("B");
			}

			if(GUI.Button(new Rect(50, 400, 200, 20), "C"))
			{
				f.PushEvent("C");
			}

			if(GUI.Button(new Rect(50, 450, 200, 20), "D"))
			{
				f.PushEvent("D");
				//f.Enter();
				//tl.Play();
			}
		}
	}
}


