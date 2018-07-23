using UnityEngine;
using System.Collections;

public class ExampleClass : MonoBehaviour
{

	public Gradient g;
	public GradientColorKey[] gck;
	public GradientAlphaKey[] gak;
	void Start()
	{

		g = new Gradient();
		gck = new GradientColorKey[2];
		gck[0].color = Color.red;
		gck[0].time = 0.0F;
		gck[1].color = Color.blue;
		gck[1].time = 1.0F;
		gak = new GradientAlphaKey[2];
		gak[0].alpha = 1.0F;
		gak[0].time = 0.0F;
		gak[1].alpha = 0.0F;
		gak[1].time = 1.0F;
		g.SetKeys(gck, gak);
		Debug.Log(g.Evaluate(0.25F));
	}
}