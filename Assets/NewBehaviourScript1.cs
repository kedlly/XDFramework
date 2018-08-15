using Framework.Core.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[AddComponentMenu("hello")]
public class NewBehaviourScript1 : MonoBehaviour {

	public bool enable;
	[ConditionalHideAttribute("enable", true)]
	public string description = "desc";

	[System.Serializable]
	public class rigTest
	{
		public bool enable;

		[ConditionalHideAttribute("enable")]
		public string name = "Test";
		[ConditionalHideAttribute("enable", true)]
		public float speed = 2.0f;
	};

	public rigTest blabla;

	public rigTest[] blablaArray = new rigTest[5];

	public List<rigTest> testBlabla = new List<rigTest>();

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void fun()
	{
		Debug.Log("dasfkladjsflj");
	}
}
