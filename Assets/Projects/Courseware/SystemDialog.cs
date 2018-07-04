using Common;
using System.IO;
using Framework.Utils.Extensions;
using WF = System.Windows.Forms;
using Projects.DataStruct.Courseware;

namespace Projects.Course
{
	public static class SystemDialog
	{
		public static string OpenFileDialog(string filter, string path)
		{
			return 文件打开保存对话框.显示打开对话框(filter, path);
		}

		public static void SetKnowledgeAudio(Knowledge knowledge)
		{
			var targetPath = Path.Combine(CourseLoader.ResourceDirectory, knowledge.AudioPathRoot);
			string audioSourceFile = OpenFileDialog("OGG音频(*.ogg)\0*.ogg", "");
			if (audioSourceFile.IsNotNullAndEmpty())
			{
				if (!Directory.Exists(targetPath))
				{
					Directory.CreateDirectory(targetPath);
				}
				string audioFilePath = Path.Combine(targetPath, Path.GetFileName(audioSourceFile));
				if(File.Exists(audioFilePath))
				{
					if(WF.MessageBox.Show("已存在同名文件是否覆盖？", "是否覆盖", WF.MessageBoxButtons.OKCancel) == WF.DialogResult.OK)
					{
						File.Copy(audioSourceFile, audioFilePath, true);
						knowledge.audioSourcePath = Path.GetFileName(audioSourceFile);
					}
				}
				else
				{
					File.Copy(audioSourceFile, audioFilePath, true);
					knowledge.audioSourcePath = Path.GetFileName(audioSourceFile);
				}
				
			}
		}

		public static void SetKnowledgeVideo(Knowledge knowledge)
		{
			var targetPath = Path.Combine(CourseLoader.ResourceDirectory, knowledge.VideoPathRoot);
			string videoSourceFile = OpenFileDialog("MP4视频(*.mp4)\0*.mp4", "");

			if(videoSourceFile.IsNotNullAndEmpty())
			{
				if(!Directory.Exists(targetPath))
				{
					Directory.CreateDirectory(targetPath);
				}
				string videoFilePath = Path.Combine(targetPath, Path.GetFileName(videoSourceFile));
				if(File.Exists(videoFilePath))
				{
					if(WF.MessageBox.Show("已存在同名文件是否覆盖？", "是否覆盖", WF.MessageBoxButtons.OKCancel) == WF.DialogResult.OK)
					{
						File.Copy(videoSourceFile, videoFilePath, true);
						knowledge.videoSourcePath = Path.GetFileName(videoSourceFile);
					}
				}
				else
				{
					File.Copy(videoSourceFile, videoFilePath, true);
					knowledge.videoSourcePath = Path.GetFileName(videoSourceFile);
				}
			}
		}

		public static void SetCoursewareModel(Courseware CoursewareData)
		{
			if(CoursewareData == null || CoursewareData.Title.IsNullOrEmpty())
			{
				WF.MessageBox.Show("没有打开任何课件, 因此不能选择模型");
				return;
			}
			var targetPath = Path.Combine(CourseLoader.ResourceDirectory, CoursewareData.ModelPathRoot);

			if(WF.MessageBox.Show("重新选择模型将会清空全部信息, 是否确定？", "选择模型", WF.MessageBoxButtons.OKCancel) == WF.DialogResult.OK)
			{
				string assetbundlePath = OpenFileDialog("模型(*.ab)\0*.ab", "");

				if(assetbundlePath.IsNotNullAndEmpty())
				{
					if(!Directory.Exists(targetPath))
					{
						Directory.CreateDirectory(targetPath);
					}
					string modelPath = Path.Combine(targetPath, Path.GetFileName(assetbundlePath));
					if(File.Exists(modelPath))
					{
						if(WF.MessageBox.Show("已存在同名文件是否覆盖？", "是否覆盖", WF.MessageBoxButtons.OKCancel) == WF.DialogResult.OK)
						{
							File.Copy(assetbundlePath, modelPath, true);
							CoursewareData.ResourceFilePath = Path.GetFileName(assetbundlePath);
						}
					}
					else
					{
						File.Copy(assetbundlePath, modelPath, true);
						CoursewareData.ResourceFilePath = Path.GetFileName(assetbundlePath);
					}
				}
			}
		}
	}
}
