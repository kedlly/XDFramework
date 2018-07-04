using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Framework.Core.FlowControl;

namespace Assets.Project
{

	public class CableNode
	{
		public CableNode Last;
		public CableNode Next;
		//前帧/本帧的位置  
		public Vector3 prePos, curPos;
	}

	public class cable : MonoBehaviour
	{
		//本身的碰撞体大小
		public float radius = 0.05f;
		public float card = 0.05f;
		public float time = 0.5f;
		//拖拉力
		public Vector3 dragForce = Physics.gravity;
		//自己的刚性，如0.001布 10石头，由boneAxis方向
		//检查碰撞
		//private List<CableNode> nodes = new List<CableNode>();

		private CableNode nodes = null;
		void Start()
		{
			var tran = this.transform;
			nodes = new CableNode();
			nodes.Last = null;
			nodes.curPos = nodes.prePos = tran.position;
			CableNode next = nodes;
			while(tran.childCount > 0)
			{
				var child = tran.GetChild(0);
				var newNode = new CableNode();
				newNode.curPos = newNode.prePos = child.transform.position;
				newNode.Last = next;
				newNode.Next = null;
				next.Next = newNode;
				tran = child;
				next = next.Next;
			}
			d = new Delay(time);
			d.OnExit += () =>
			{
				OnLateUpdate();
				Debug.Log("delay ....");
			};
			
		}

		Delay d = null;


		// 更新用户自己设置的力
		void Update()
		{

		}

		public void LateUpdate()
		{
			d.Execute();
		}


		public void OnLateUpdate()
		{

			//foreach (var node in nodes)
			var node = nodes;
			while (node != null)
			{ 

				#region 参考下面项目与链接 Verlet数值积分
				//参考项目unitychan-crs-master
				//http://www.cnblogs.com/miloyip/archive/2011/06/14/alice_madness_returns_hair.html
				//https://www.zhihu.com/question/22531466            
				//简单阻尼效果的Verlet数值积分 x(t+T)=x(t)+d*(x(t)-x(t-T))+a(t)*T^2 
				//T-时间增量 x(t)-t时间位置 a(t)-t时间加速度(force/mass)
				//新的现在位置 与 旧的现在位置 与旧的上一桢位置 关系
				//temp 旧的现在位置
				Vector3 temp = node.curPos;
				//verlet计算结果 新的现在位置 x(t+T)=x(t)+d*(x(t)-x(t-T))+a(t)*T^2 
				node.curPos = (node.curPos - node.prePos) + node.curPos + (dragForce * Time.deltaTime * Time.deltaTime);
				//新位置保持与父节点的长度一样 
				if (node.Last != null)
				{
					var last = node.Last;
					var distance = (node.curPos - last.curPos);
					if (distance.magnitude > card)
					{
						node.curPos  = distance.normalized * card + last.curPos;
					}
				}
				else
				{
					node.curPos = temp;
				}
				
				node.prePos = temp;

				#endregion
				node = node.Next;
			}
		}

		private void OnDrawGizmos()
		{
			if(true)
			{
				Gizmos.color = Color.yellow;
				var node = nodes;
				while(node != null)
				{
					Gizmos.DrawWireSphere(node.curPos, radius);
					node = node.Next;
				}

			}
		}
	}
}


