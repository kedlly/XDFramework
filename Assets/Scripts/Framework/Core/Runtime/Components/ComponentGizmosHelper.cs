using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Framework.Core.Runtime
{
#if UNITY_EDITOR
	public static class VolumeGizmosHelper
	{

		class CylinderVolume
		{

			struct VertexFormat
			{
				public Vector3 position;
				public Vector3 normal;
				public Color color;
			}

			static VertexFormat[] vertexData = new VertexFormat[]
			{
				new VertexFormat() { position = VertexInCircle(30 * 0, 0.5f, 0.5f ) , color = Color.red,  normal = VertexInCircle(30 * 0, 1f, 0	) }, //0
				new VertexFormat() { position = VertexInCircle(30 * 0, 0.5f, -0.5f) , color = Color.red,  normal = VertexInCircle(30 * 0, 1f, 0	) }, //1
				new VertexFormat() { position = VertexInCircle(30 * 1, 0.5f, 0.5f ) , color = Color.red,  normal = VertexInCircle(30 * 1, 1f, 0	) }, //2
				new VertexFormat() { position = VertexInCircle(30 * 1, 0.5f, -0.5f) , color = Color.red,  normal = VertexInCircle(30 * 1, 1f, 0	) }, //3
				new VertexFormat() { position = VertexInCircle(30 * 2, 0.5f, 0.5f ) , color = Color.red,  normal = VertexInCircle(30 * 2, 1f, 0	) }, //4
				new VertexFormat() { position = VertexInCircle(30 * 2, 0.5f, -0.5f) , color = Color.red,  normal = VertexInCircle(30 * 2, 1f, 0	) }, //5
				new VertexFormat() { position = VertexInCircle(30 * 3, 0.5f, 0.5f ) , color = Color.red,  normal = VertexInCircle(30 * 3, 1f, 0	) }, //6
				new VertexFormat() { position = VertexInCircle(30 * 3, 0.5f, -0.5f) , color = Color.red,  normal = VertexInCircle(30 * 3, 1f, 0	) }, //7
				new VertexFormat() { position = VertexInCircle(30 * 4, 0.5f, 0.5f ) , color = Color.red,  normal = VertexInCircle(30 * 4, 1f, 0	) }, //8
				new VertexFormat() { position = VertexInCircle(30 * 4, 0.5f, -0.5f) , color = Color.red,  normal = VertexInCircle(30 * 4, 1f, 0	) }, //9
				new VertexFormat() { position = VertexInCircle(30 * 5, 0.5f, 0.5f ) , color = Color.red,  normal = VertexInCircle(30 * 5, 1f, 0	) }, //10
				new VertexFormat() { position = VertexInCircle(30 * 5, 0.5f, -0.5f) , color = Color.red,  normal = VertexInCircle(30 * 5, 1f, 0	) }, //11
				new VertexFormat() { position = VertexInCircle(30 * 6, 0.5f, 0.5f ) , color = Color.red,  normal = VertexInCircle(30 * 6, 1f, 0	) }, //12
				new VertexFormat() { position = VertexInCircle(30 * 6, 0.5f, -0.5f) , color = Color.red,  normal = VertexInCircle(30 * 6, 1f, 0	) }, //13
				new VertexFormat() { position = VertexInCircle(30 * 7, 0.5f, 0.5f ) , color = Color.red,  normal = VertexInCircle(30 * 7, 1f, 0	) }, //14
				new VertexFormat() { position = VertexInCircle(30 * 7, 0.5f, -0.5f) , color = Color.red,  normal = VertexInCircle(30 * 7, 1f, 0	) }, //15
				new VertexFormat() { position = VertexInCircle(30 * 8, 0.5f, 0.5f ) , color = Color.red,  normal = VertexInCircle(30 * 8, 1f, 0	) }, //16
				new VertexFormat() { position = VertexInCircle(30 * 8, 0.5f, -0.5f) , color = Color.red,  normal = VertexInCircle(30 * 8, 1f, 0	) }, //17
				new VertexFormat() { position = VertexInCircle(30 * 9, 0.5f, 0.5f ) , color = Color.red,  normal = VertexInCircle(30 * 9, 1f, 0	) }, //18
				new VertexFormat() { position = VertexInCircle(30 * 9, 0.5f, -0.5f) , color = Color.red,  normal = VertexInCircle(30 * 9, 1f, 0	) }, //19
				new VertexFormat() { position = VertexInCircle(30 * 10, 0.5f, 0.5f ), color = Color.red,  normal = VertexInCircle(30 * 10,1f, 0 )},	 //20
				new VertexFormat() { position = VertexInCircle(30 * 10, 0.5f, -0.5f), color = Color.red,  normal = VertexInCircle(30 * 10,1f, 0 )},	 //21
				new VertexFormat() { position = VertexInCircle(30 * 11, 0.5f, 0.5f ), color = Color.red,  normal = VertexInCircle(30 * 11,1f, 0 )},	 //22
				new VertexFormat() { position = VertexInCircle(30 * 11, 0.5f, -0.5f), color = Color.red,  normal = VertexInCircle(30 * 11,1f, 0 )},	 //23

				new VertexFormat() { position = VertexInCircle(30 * 0, 0, 0.5f), color = Color.red,  normal = Vector3.up},  //24 center top
				new VertexFormat() { position = VertexInCircle(30 * 0, 0, -0.5f), color = Color.red,  normal = Vector3.down},  //25 center bottom
			};																															 			  

			static int[] indexList = new int[]
			{
				 0     ,  2	   , 1
				,2     ,  3	   , 1  
				,2     ,  4	   , 3  
				,4     ,  5	   , 3  
				,4     ,  6	   , 5  
				,6     ,  7	   , 5  
				,6     ,  8	   , 7  
				,8     ,  9	   , 7  
				,8     ,  10   , 9  
				,10    ,  11   , 9  
				,10    ,  12   , 11 
				,12    ,  13   , 11 
				,12    ,  14   , 13 
				,14    ,  15   , 13 
				,14    ,  16   , 15 
				,16    ,  17   , 15 
				,16    ,  18   , 17 
				,18    ,  19   , 17 
				,18    ,  20   , 19 
				,20    ,  21   , 19 
				,20    ,  22   , 21 
				,22    ,  23   , 21 
				,22    ,  0	   , 23 
				, 0    ,  1	   , 23 

				, 0    , 24    , 2
				, 2    , 24	   , 4	
				, 4    , 24	   , 6	
				, 6    , 24	   , 8	
				, 8    , 24	   , 10	
				, 10   , 24	   , 12	
				, 12   , 24	   , 14	
				, 14   , 24	   , 16	
				, 16   , 24	   , 18	
				, 18   , 24	   , 20	
				, 20   , 24	   , 22	
				, 22   , 24	   , 0	

				, 1     , 3    , 25
				, 3 	, 5	   , 25
				, 5 	, 7	   , 25
				, 7 	, 9	   , 25
				, 9 	, 11   , 25
				, 11	, 13   , 25
				, 13	, 15   , 25
				, 15	, 17   , 25
				, 17	, 19   , 25
				, 19	, 21   , 25
				, 21	, 23   , 25
				, 23	, 1	   , 25

			};

			public static Vector3 VertexInCircle(int degree, float radius, float height = 0)
			{
				float radians = Mathf.PI / 180 * degree;
				return new Vector3(Mathf.Sin(radians) * radius, Mathf.Cos(radians) * radius, height);
			}

			public CylinderVolume()
			{
				Mesh = new Mesh();
				//mesh.SetVertices(vertexList);
				Mesh.vertices = vertexData.Select(it => it.position).ToArray();
				Mesh.triangles = indexList;
				Mesh.colors = vertexData.Select(it => it.color).ToArray();
				Mesh.normals = vertexData.Select(it => it.normal).ToArray();
			}

			public Mesh Mesh { get; private set; }
		}

		public static Color SphereColor = new Color(1f, 0f, 0f, 0.1f);
		public static Color CuboidColor = new Color(0f, 1f, 0, 0.1f);
		public static Color CylinderColor = new Color(1f, 1f, 0f, 0.1f);
		static CylinderVolume _CylinderVolume = new CylinderVolume();

		public static Color GetColor(Color color, int Level)
		{
			color.a *= Level;
			return color;
		}

		public static void InitGizmosMatrix(Matrix4x4 mat)
		{
			Gizmos.matrix = mat;
		}

		public static void DrawVolume(VolumeData data, int colorLevel, bool wired = false)
		{
			if (data.type == VolumeType.Sphere)
			{
				DrawSphereVolume(data, GetColor(SphereColor, colorLevel), wired);
			}
			if (data.type == VolumeType.Cuboid)
			{
				DrawCuboidVolume(data, GetColor(CuboidColor, colorLevel), wired);
			}
			if (data.type == VolumeType.Cylinder)
			{
				DrawCylinderVolume(data, GetColor(CylinderColor, colorLevel), wired);
			}
		}

		public static void DrawVolumes(VolumeData[] data, int colorLevel, bool wired = false)
		{
			foreach (var p in data)
			{
				DrawVolume(p, colorLevel, wired);
			}
		}

		static void DrawSphereVolume(VolumeData data, Color32 color, bool wired = false)
		{
			Color oldColor = Gizmos.color;
			Gizmos.color = color;
			Matrix4x4 oldMat = Gizmos.matrix;
			Gizmos.color = color;
			Gizmos.matrix = Gizmos.matrix * data.matrix;
			if (wired)
			{
				Gizmos.DrawWireSphere(Vector3.zero, 0.5f);
			}
			else
			{
				Gizmos.DrawSphere(Vector3.zero, 0.5f);
			}
			Gizmos.matrix = oldMat; 
			Gizmos.color = oldColor;

		}

		static void DrawCuboidVolume(VolumeData data, Color cuboidColor, bool wired = false)
		{
			Color oldColor = Gizmos.color;
			Gizmos.color = cuboidColor;
			Matrix4x4 oldMat = Gizmos.matrix;
			Gizmos.matrix = Gizmos.matrix * data.matrix;
			if (wired)
			{
				Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
			}
			else
			{
				Gizmos.DrawCube(Vector3.zero, Vector3.one);
			}

			Gizmos.matrix = oldMat; 
			Gizmos.color = oldColor;
		}

		static void DrawCylinderVolume(VolumeData data, Color color, bool wired = false)
		{
			Color oldColor = Gizmos.color;
			Gizmos.color = color;
			Matrix4x4 oldMat = Gizmos.matrix;
			Gizmos.color = color;
			Gizmos.matrix = Gizmos.matrix * data.matrix;
			if (wired)
			{
				//Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
				Gizmos.DrawWireMesh(_CylinderVolume.Mesh);
			}
			else
			{
				//Gizmos.DrawCube(Vector3.zero, Vector3.one);
				Gizmos.DrawMesh(_CylinderVolume.Mesh);
			}
			/*
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(Vector3.zero, Vector3.forward);
			Gizmos.DrawWireCube(Vector3.forward, Vector3.one * 0.05f);
			Gizmos.color = Color.green;
			Gizmos.DrawLine(Vector3.zero, Vector3.up);
			Gizmos.DrawWireCube(Vector3.up, Vector3.one * 0.05f);
			Gizmos.color = Color.red;
			Gizmos.DrawLine(Vector3.zero, Vector3.right);
			Gizmos.DrawWireCube(Vector3.right, Vector3.one * 0.05f);*/
			Gizmos.matrix = oldMat; 
			Gizmos.color = oldColor;
		}
	}
#endif
}
