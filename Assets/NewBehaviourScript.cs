using Framework.Core.Runtime.ECS;
using ProtoBuf;
using System;
using System.IO;
using UnityEngine;
using ProjectProtocal;
using Framework.Core;
using Protocals;
using System.Collections.Generic;

[Serializable]
class RotateEntity : IEntity
{
	[SerializeField]
	[Header("name")]
	public int a;
	//public int b;
}


[Serializable]
[ProtoContract]
public class Person
{
	[ProtoMember(1)]
	public string name;
	[ProtoMember(2)]
	public int age;
}

[Serializable]
[ProtoContract]
public class Group
{
	[ProtoMember(1)]
	public string groupName;
	[ProtoMember(2)]
	public Person[] persons;
}

public class NewBehaviourScript : MonoBehaviour
{
	public Group myGroup;
	void Start()
	{
		tutorial.Person p = new tutorial.Person();
		p.name = "";
		// 		if (!ProtoBuf.Meta.RuntimeTypeModel.Default.IsDefined(typeof(Vector3)))
		// 		{
		// 			ProtoBuf.Meta.RuntimeTypeModel.Default.Add(typeof(Vector3), false).Add("x", "y", "z");
		// 		}

		
	}



	

	void Update()
	{

// 		if (Input.GetKeyDown(KeyCode.T))
// 		{
// 			using (var fs = new FileStream("c:\\1.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite))
// 			{
// 				Serializer.Serialize(fs, myGroup);
// 				fs.Flush();
// 			}
// 		}
// 
// 		if (Input.GetKeyDown(KeyCode.Y))
// 		{
// 			using (var fs = new FileStream("c:\\1.txt", FileMode.Open, FileAccess.ReadWrite))
// 			{
// 				myGroup = Serializer.Deserialize<Group>(fs);
// 				fs.Flush();
// 			}
// 		}
	}
	
}


public partial class PPPoE
{
	public PPPoE()
	{
		OnConstructor();
	}

    partial void OnConstructor();
}

