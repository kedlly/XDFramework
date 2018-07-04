using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Framework.Core.Runtime;
using System.Text;

[ExecuteInEditMode]
public class TestFieldComponent : MonoBehaviour {

	public GameObject go;

	FieldComponent com = null;

	// Use this for initialization
	void Start () {
		if (go != null)
			com = go.GetComponent<FieldComponent>();
	}

	// Update is called once per frame
	List<Vector3> sdf = new List<Vector3>();
	void Update () {
		FieldCheckResult[] get = null;
		if(com != null && com.TestWorldPosition(transform.position, out get))
		{
			
			if (sdf.Count >= 1000)
			{
				sdf.RemoveAt(0);
				
			}
			sdf.Add(transform.position);

			StringBuilder sb = new StringBuilder("In Field.");
			foreach (var g in get)
			{
				sb.Append(" layer: ").Append(g.layerIndex).Append(" index:").Append( g.dataIndex);
			}
			Debug.Log(sb.ToString());
		}
		if (sdf.Count < 2)
		{
			return;
		}
		var point = sdf[0];
		for (int i = 1;i < sdf.Count; ++ i)
		{
			Debug.DrawLine(point, sdf[i], new Color(0, 0, 1, 0.2f));
			point = sdf[i];
		}
	}
}
