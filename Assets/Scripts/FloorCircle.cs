using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCircle : MonoBehaviour
{
    GameObject _floorCircle;
    public float circleLevel = -1.6f;
    public Color circleColor = Color.blue;

    public GameObject _floorArrow;
    public float arrowAzimuth = 0.0f;
    public Color arrowColor = Color.blue;

    void Start()
    {
        _floorCircle = new GameObject { name = "FloorCircle" };
        _floorCircle.transform.parent = transform;
        _floorCircle.DrawCircle(0.3f, 0.02f);
        _floorCircle.GetComponent<LineRenderer>().material.color = circleColor;
        _floorCircle.transform.localPosition = new Vector3(0.0f, circleLevel, 0.0f);

        _floorArrow.transform.localPosition = new Vector3(0.0f, circleLevel, 0.0f);;
        _floorArrow.GetComponentsInChildren<Renderer>()[0].material.color = arrowColor;
        _floorArrow.GetComponentsInChildren<Renderer>()[1].material.color = arrowColor;
    }

    void Update()
    {
        _floorArrow.transform.rotation = new Quaternion();
        _floorArrow.transform.Rotate(Vector3.up, arrowAzimuth);

        //arrowAzimuth += 1.0f;
    }
}
