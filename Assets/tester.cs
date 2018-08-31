using Framework.Utils.Extensions;
using System.Collections.Generic;
using UnityEngine;
using System;
using Protocol;

public class tester : MonoBehaviour 
{
	private void OnCollisionEnter(Collision collision)
	{
		Debug.Log("OnCollisionEnter");
	}
	private void Start()
	{
		var p = new Protocol.Request.Request_LoginAuth();
		p.username = "zhaoxionghui";
		p.password = "helloworld";
		var m = p.Pack();
		Protocol.Request.Request_LoginAuth u = m.Unpack() as Protocol.Request.Request_LoginAuth;
		print(u.username +"/"+ u.password);
	}
}

