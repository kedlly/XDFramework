using Projects.DataStruct.Courseware;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using Framework.Core.FlowControl;

namespace Projects.Course
{
	public class MainLogic : MonoBehaviour
	{
		CourseProxy proxy = null;
		private void Awake()
		{
			
		}
		private void Start()
		{
			doOnceNew.OnExit += () =>
			{
				proxy = CourseProxy.New("set");
			};
			doOnceOpen.OnExit += () =>
			{
				proxy = CourseProxy.Open("set");
			};
			doOnceSave.OnExit += () =>
			{
				proxy.Save();
			};
		}

		DoOnce doOnceOpen = new DoOnce();
		DoOnce doOnceNew = new DoOnce();
		DoOnce doOnceSave = new DoOnce();

		private void Update()
		{
			if (Input.GetKey(KeyCode.N))
			{
				doOnceNew.Execute();
			}
			if(Input.GetKey(KeyCode.O))
			{
				doOnceOpen.Execute();	
			}
			if (Input.GetKey(KeyCode.D))
			{
				proxy.AddKnowledge("test2", null);
			}
			if (Input.GetKey(KeyCode.M))
			{
				SystemDialog.SetCoursewareModel(proxy.CoursewareData);
			}
			if(Input.GetKey(KeyCode.V))
			{
				SystemDialog.SetKnowledgeVideo(proxy.CoursewareData.Knowledges[0]);
			}
			if(Input.GetKey(KeyCode.A))
			{
				SystemDialog.SetKnowledgeAudio(proxy.CoursewareData.Knowledges[0]);
			}
			if(Input.GetKey(KeyCode.S))
			{
				doOnceSave.Execute();
			}

		}
	}
}
