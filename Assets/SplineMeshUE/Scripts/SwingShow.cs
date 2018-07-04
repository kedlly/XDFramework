using UnityEngine;
using System.Collections;

public class SwingShow : MonoBehaviour
{
    public UESpline spline;
    public float totalTime = 2.0f;

    // Use this for initialization
    void Start()
    {   
    }

    // Update is called once per frame
    void Update()
    {
        float ctime = Time.time % totalTime / totalTime;
        var pos = spline.GetPositionLerp(ctime);
        transform.position = pos;
        //spline.
    }
}
