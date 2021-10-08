using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCircle : MonoBehaviour
{
    void Start()
    {
        GameObject circle = new GameObject { name = "FloorCircle" };
        circle.transform.parent = transform;
        circle.DrawCircle(0.3f, 0.005f);

        circle.transform.localPosition = new Vector3(0.0f, -1.6f, 0.0f);
    }

    void Update()
    {
        
    }
}
