using UnityEngine;
using Framework.Services;
namespace Framework.Services.Positioning
{

	static class PositioningServiceInitializer
	{
		const string POSITIONING_OBJECT_NAME = "PositioningServiceObject";
		public static void Setup()
		{
			GameObject positioningObject = new GameObject(POSITIONING_OBJECT_NAME);
			positioningObject.AddComponent<PositioningService>();
			GameObject.DontDestroyOnLoad(positioningObject);
			//Debug.Log(Framework.Client.LevelInfoLoader.Instance.gameObject);
			//Debug.Log(Client.ServiceAgents.PositioningAgent.Instance.gameObject);
		}
	}

	public class PositioningService : MonoBehaviour
	{

		void Awake()
		{

		}

		// Use this for initialization
		void Start()
		{
			var text = @"
{
	""id"":100,
	""coordinate"":
	{
		""x"":1
		,""y"":2
		,""z"":3
	}
}
";
			ReceivePosition(text);
		}

		// Update is called once per frame
		void Update()
		{

		}

		void LateUpdate()
		{

		}

		void FixedUpdate()
		{

		}

		[ExternServiceAPI]
		void ReceivePosition(string jsonText)
		{
			//json text format as
			//{
			//		"id": (int)
			//		, "coordinate": 
			//			{
			//				"x": (int)
			//				, "y": (int)
			//				, "z": (int)
			//			}
			//}
			PositioningInfo info = JsonUtility.FromJson<PositioningInfo>(jsonText);
			Debug.Log(info);
		}
	}
}