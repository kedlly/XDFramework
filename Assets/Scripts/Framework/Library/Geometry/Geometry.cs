using UnityEngine;

namespace Framework.Library.Geometry
{
	public static class GeometryUtils
	{

		/// <summary>
		/// 计算出长方体顶点数据
		/// </summary>
		/// <param name="worldPosition"></param>
		/// <param name="quat"></param>
		/// <param name="scale"></param>
		/// <returns></returns>
		public static Vector3[][] GetCuboidVertexData(Vector3 worldPosition, Quaternion quat, Vector3 scale, float baseSize = 1)
		{
			Vector3[][] ret = new Vector3[6][];

			// forward x axis
			Vector3[] verts = new Vector3[]
			{
				 new Vector3(1, 1, 1)		//TopLeft				0
				, new Vector3(1, 1, -1)		//TopRight				1
				, new Vector3(1, -1, -1)	//BottomRight			2
				, new Vector3(1, -1, 1)		//BottomLeft			3
				
				, new Vector3(-1, 1, 1)		//TopLeftBack			4
				, new Vector3(-1, 1, -1)	//TopRightBack			5
				, new Vector3(-1, -1, -1)	//BottomRightBack		6
				, new Vector3(-1, -1, 1)	//BottomLeftBack		7
			};

			// transform verts


			for(int i = 0; i < verts.Length; ++i)
			{
				verts[i] = worldPosition + quat * Vector3.Scale(scale, verts[i] * baseSize / 2);
			}

			//front
			ret[0] = new Vector3[]
						{
							verts[0]	//TopLeft
							, verts[1]	//TopRight
							, verts[2]	//BottomRight
							, verts[3]	//BottomLeft
						};

			//back
			ret[1] = new Vector3[]
						{
							verts[4]	//TopLeftBack
							, verts[5]	//TopRightBack
							, verts[6]	//BottomRightBack
							, verts[7]	//BottomLeftBack
						};

			//top
			ret[2] = new Vector3[]
						{
							verts[0]	//TopLeft
							, verts[4]	//TopLeftBack
							, verts[5]	//TopRightBack
							, verts[1]	//TopRight
						};

			//bottom
			ret[3] = new Vector3[]
						{
							verts[3]	//BottomLeft			3
							, verts[7]	//BottomLeftBack		7
							, verts[6]	//BottomRightBack		6
							, verts[2]	//BottomRight			2
						};

			// left
			ret[4] = new Vector3[]
						{
							verts[0]
							, verts[3]
							, verts[7]
							, verts[4]
						};

			// right
			ret[5] = new Vector3[]
						{
							verts[1]
							, verts[5]
							, verts[6]
							, verts[2]
						};

			return ret;
		}
	}
}
