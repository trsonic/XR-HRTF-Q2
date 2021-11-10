using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCircle : MonoBehaviour
{
    GameObject _floorCircle;
    public float circleLevel = -1.6f;
    public Color circleColor = Color.blue;

    public GameObject _floorArrow;
    //public GameObject _line2floor;
    public bool blockOtherGuides;
    public float arrowAzimuth = 0.0f;
    public Color arrowColor = Color.blue;

    private LineRenderer lineRenderer;

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

        lineRenderer = gameObject.AddComponent<LineRenderer>();
    }

    void Update()
    {

    }
    public void RotateArrow(float arrowAzimuth)
    {
        _floorArrow.transform.rotation = new Quaternion();
        _floorArrow.transform.Rotate(Vector3.up, arrowAzimuth);
    }
    public void DrawQuadraticBezierCurve(Vector3 point0, Vector3 point1, Vector3 point2)
    {
        lineRenderer.enabled = true;
        lineRenderer.positionCount = 200;
        lineRenderer.startWidth = 0.25f;
        lineRenderer.endWidth = 0.0001f;
        lineRenderer.material.color = Color.green;

        float t = 0f;
        Vector3 B = new Vector3(0, 0, 0);
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            B = (1 - t) * (1 - t) * point0 + 2 * (1 - t) * t * point1 + t * t * point2;
            lineRenderer.SetPosition(i, B);
            t += (1 / (float)lineRenderer.positionCount);
        }
    }
    public void HideCurve()
    {
        lineRenderer.enabled = false;
    }
}
