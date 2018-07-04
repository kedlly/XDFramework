using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Framework.Utils.Extensions;
using System.IO;

namespace Projects.DataStruct.Courseware
{
	/// <summary>
	/// 知识点
	/// </summary>
	[Serializable]
	public class Knowledge
	{
		//零件名称
		public string name;
		//关联的的网格模型名称
		public List<string> itemPartMeshPathnameList;
		//对应文本说明
		public string description;
		//对应声音文件路径
		public string audioSourcePath;
		//对应视频文件路径
		public string videoSourcePath;

		const string KnowledgesFolderName = "Knowledges";
		const string KnowledgeAudioFoldername = "Audio";
		const string KnowledgeVideoFoldername = "Video";

		public string AudioPathRoot
		{
			get
			{
				return Owner == null || name.IsNullOrEmpty() ? "" : 
					string.Join(Path.DirectorySeparatorChar.ToString()
								, new string[] 
									{
										Owner.Root
										, KnowledgesFolderName
										, name
										, KnowledgeAudioFoldername
									}
								);
			}
		}
		public string VideoPathRoot
		{
			get
			{
				return Owner == null || name.IsNullOrEmpty() ? "" : 
					string.Join(Path.DirectorySeparatorChar.ToString()
								, new string[] 
									{
										Owner.Root
										, KnowledgesFolderName
										, name
										, KnowledgeVideoFoldername
									}
								);
			}
		}

		public Courseware Owner { get; set; }
	}

	[Serializable]
	public class Courseware
	{
		public List<Knowledge> Knowledges;
		//资源包路径
		public string ResourceFilePath;
		//设备资源名
		public string ResourceName;
		 //课件标题
		public string Title;

		public bool AddKnowledge(Knowledge knowledge)
		{
			if(Knowledges == null)
			{
				Knowledges = new List<Knowledge>();
			}
			if(Knowledges.Any(it => it.name == knowledge.name))
			{
				return false ;
			}
			Knowledges.Add(knowledge);
			knowledge.Owner = this;
			return true;
		}

		public static Courseware CreateFromJson(string json)
		{
			return JsonUtility.FromJson<Courseware>(json);
		}

		public string ToJson()
		{
			return JsonUtility.ToJson(this);
		}

		
		const string CoursewareModelFolderName = "Assets";


		public string Root
		{
			get { return Title; }
		}

		public string ModelPathRoot
		{
			get
			{
				return string.Join(Path.DirectorySeparatorChar.ToString(), new string[] { Title, CoursewareModelFolderName } );
			}
		}
	}

}
