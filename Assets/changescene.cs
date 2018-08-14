using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class changescene : MonoBehaviour
{

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if(Input.GetKeyDown(KeyCode.C))
		{
			if(SceneManager.GetActiveScene().name == "level")
			{
				SceneManager.LoadScene("level1");
			}
			if(SceneManager.GetActiveScene().name == "level1")
			{
				SceneManager.LoadScene("level");
			}
		}
		if(Input.GetKeyDown(KeyCode.D))
		{
			Destroy(GameObject.Find("A") );
		}
	}
}