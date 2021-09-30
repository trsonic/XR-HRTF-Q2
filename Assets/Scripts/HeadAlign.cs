using System;
using UnityEngine;
using UnityEngine.UI;
using OscJack;

public class HeadAlign : MonoBehaviour
{
    public string _IPAddress = "";
    public int _oscPortOut = 9000;
    public int _oscPortIn = 6000;
    private OscServer _server;
    private OscClient _sender;

    public GameObject _mainCamera;
    public GameObject _speakerAnchor;
    public GameObject _textDisplay;

    public GameObject _headPositionTarget;
    public GameObject _headOrientationTarget;
    public GameObject _virtualSpeaker;

    private GameObject _arrow;

    private bool targetVis = false;
    private bool orientationLocked = false;
    private float speakerAz = 0.0f;
    private float speakerEl = 0.0f;
    private float speakerDist = 0.6f;
    private double _timeAtStartup;

    void Start()
    {
        _sender = new OscClient(_IPAddress, _oscPortOut);
        _server = new OscServer(_oscPortIn);

        _server.MessageDispatcher.AddCallback(
               "/targetVis",
               (string address, OscDataHandle data) =>
               {
                   if (data.GetElementAsInt(0) == 1) targetVis = true;
                   else targetVis = false;
               }
           );

        _server.MessageDispatcher.AddCallback(
               "/orientationLocked",
               (string address, OscDataHandle data) =>
               {
                   if (data.GetElementAsInt(0) == 1) orientationLocked = true;
                   else orientationLocked = false;
               }
           );

        _server.MessageDispatcher.AddCallback(
               "/speaker",
               (string address, OscDataHandle data) =>
               {
                   speakerAz = data.GetElementAsFloat(0);
                   speakerEl = data.GetElementAsFloat(1);
                   speakerDist = data.GetElementAsFloat(2);
               }
           );

        _timeAtStartup = Time.realtimeSinceStartup;

        // DRAW THE VIEWFINDER
        GameObject vfHorUp, vfHorDown, vrVertLeft, vfVertRight;
        float radius = 0.6f;
        float lineWidth = 0.001f;
        float vfMargin = 0.01f;

        vfHorDown = new GameObject { name = "vfHorDown" };
        vfHorDown.DrawCircle(radius, lineWidth);
        vfHorDown.transform.parent = _mainCamera.transform;
        vfHorDown.transform.Translate(0.0f, -vfMargin, 0.0f);

        vfHorUp = new GameObject { name = "vfHorUp" };
        vfHorUp.DrawCircle(radius, lineWidth);
        vfHorUp.transform.parent = _mainCamera.transform;
        vfHorUp.transform.Translate(0.0f, vfMargin, 0.0f);

        vrVertLeft = new GameObject { name = "vrVertLeft" };
        vrVertLeft.DrawCircle(radius, lineWidth);
        vrVertLeft.transform.parent = _mainCamera.transform;
        vrVertLeft.transform.Translate(-vfMargin, 0.0f, 0.0f);
        vrVertLeft.transform.Rotate(transform.forward, 90.0f);

        vfVertRight = new GameObject { name = "vfVertRight" };
        vfVertRight.DrawCircle(radius, lineWidth);
        vfVertRight.transform.parent = _mainCamera.transform;
        vfVertRight.transform.Translate(vfMargin, 0.0f, 0.0f);
        vfVertRight.transform.Rotate(transform.forward, 90.0f);

        // ADD LINE / ARROW
        _arrow = new GameObject { name = "Arrow" };
        _arrow.AddComponent<LineRenderer>();
        _arrow.GetComponent<LineRenderer>().startWidth = 0.05f;
        _arrow.GetComponent<LineRenderer>().endWidth = 0.001f;
        _arrow.GetComponent<LineRenderer>().positionCount = 2;
        _arrow.GetComponent<LineRenderer>().generateLightingData = true;
        _arrow.GetComponent<LineRenderer>().useWorldSpace = false;
    }

    void Update()
    {
        String text = "";

        // display the desired speaker azimuth, elevation and distance
        text += "desired speaker azi: " + speakerAz.ToString("F1") + ", ele: " + speakerEl.ToString("F1") + ", dist: " + speakerDist.ToString("F2") + "\n";

        // obtain current azimuth and elevation angles and distance
        Vector3 hsVec = Vector3.Normalize(_speakerAnchor.transform.position - _mainCamera.transform.position);
        Vector3 projectedVec = Vector3.ProjectOnPlane(hsVec, _mainCamera.transform.up);
        float azimuthAngle = Vector3.SignedAngle(_mainCamera.transform.forward, projectedVec, _mainCamera.transform.up);
        float elevationAngle = Vector3.SignedAngle(_mainCamera.transform.up, hsVec, Vector3.Cross(_mainCamera.transform.up, hsVec));
        elevationAngle = (elevationAngle - 90.0f) * -1.0f;
        float currDist = Vector3.Distance(_mainCamera.transform.position, _speakerAnchor.transform.position);
        text += "current speaker azi: " + azimuthAngle.ToString("F1") + ", ele: " + elevationAngle.ToString("F1") + ", dist: " + currDist.ToString("F2") + "\n";

        // VIRTUAL SPEAKER
        float vsDist = currDist;
        _virtualSpeaker.transform.position = _mainCamera.transform.position;
        _virtualSpeaker.transform.rotation = _mainCamera.transform.rotation;
        _virtualSpeaker.transform.Rotate(-speakerEl, speakerAz, 0.0f);
        _virtualSpeaker.transform.position = _mainCamera.transform.position + _virtualSpeaker.transform.forward * vsDist;

        // HEAD ORIENTATION TARGET
        float orTargetDist = 0.6f;
        _headOrientationTarget.transform.position = _mainCamera.transform.position;
        _headOrientationTarget.transform.rotation = _mainCamera.transform.rotation;
        _headOrientationTarget.transform.Rotate(-(speakerEl - elevationAngle), speakerAz - azimuthAngle, 0.0f);
        _headOrientationTarget.transform.position = _mainCamera.transform.position + _headOrientationTarget.transform.forward * orTargetDist;

        Vector3 mcCenter = _mainCamera.transform.position;
        Vector3 vfCenter = _mainCamera.transform.position + _mainCamera.transform.forward * 0.6f;
        Vector3 vsVec = _virtualSpeaker.transform.position - _mainCamera.transform.position;
        Vector3 lsVec = _speakerAnchor.transform.position - _mainCamera.transform.position;
        Vector3 crossVec = Vector3.Cross(vfCenter - mcCenter, _headOrientationTarget.transform.position - mcCenter);
        Vector3 direction = Vector3.Cross(vfCenter - mcCenter, crossVec);

        float headSpAngDev = Vector3.Angle(vsVec, lsVec);
        text += "Angular distance: " + headSpAngDev.ToString("F2") + "\n";

        // ARROW
        _arrow.GetComponent<LineRenderer>().SetPosition(0, vfCenter);
        if(speakerAz <= 90 && speakerAz >= -90)
        {
            Vector3 arrowEndPosition = vfCenter + direction.normalized * Mathf.Sin(headSpAngDev * Mathf.PI / 360);
            _arrow.GetComponent<LineRenderer>().SetPosition(1, arrowEndPosition);
            _headOrientationTarget.transform.position = arrowEndPosition;
        }
        else
        {
            Vector3 dir2;
            dir2 = Vector3.Reflect(direction.normalized, _mainCamera.transform.up);
            Vector3 arrowEndPosition = vfCenter + dir2 * Mathf.Sin(headSpAngDev * Mathf.PI / 360);
            _arrow.GetComponent<LineRenderer>().SetPosition(1, arrowEndPosition);
            _headOrientationTarget.transform.position = arrowEndPosition;
        }

        if (orientationLocked)
        {
            _arrow.GetComponent<LineRenderer>().material.color = Color.green;
            _headOrientationTarget.GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            _arrow.GetComponent<LineRenderer>().material.color = Color.yellow;
            _headOrientationTarget.GetComponent<Renderer>().material.color = Color.blue;
        }

        // HEAD POSITION TARGET
        _headPositionTarget.transform.position = _speakerAnchor.transform.position;
        _headPositionTarget.transform.rotation = _speakerAnchor.transform.rotation;
        _headPositionTarget.transform.Translate(0.0f, 0.0f, -speakerDist);

        float headTargetDistance = Vector3.Distance(_mainCamera.transform.position, _headPositionTarget.transform.position);
        text += "Distance from the position target: " + headTargetDistance.ToString("F2") + "\n";

        // obtain distance from the target (front back)
        float backfrontDistance = Vector3.Dot(_speakerAnchor.transform.TransformDirection(Vector3.forward), _mainCamera.transform.position - _headPositionTarget.transform.position);
        text += "back / front: " + backfrontDistance.ToString("F2") + "\n";

        // obtain distance from the target (left right)
        float leftrightDistance = Vector3.Dot(_speakerAnchor.transform.TransformDirection(Vector3.right), _mainCamera.transform.position - _headPositionTarget.transform.position);
        text += "left / right: " + leftrightDistance.ToString("F2") + "\n";

        // obtain distance from the target (down up)
        float downupDistance = Vector3.Dot(_speakerAnchor.transform.TransformDirection(Vector3.up), _mainCamera.transform.position - _headPositionTarget.transform.position);
        text += "down / up: " + downupDistance.ToString("F2") + "\n";

        float spHeadAngDev = Vector3.Angle(_headPositionTarget.transform.position - _speakerAnchor.transform.position, _mainCamera.transform.position - _speakerAnchor.transform.position);
        text += "Angular deviation: " + spHeadAngDev.ToString("F2") + "\n";

        // update text display
        _textDisplay.GetComponent<TextMesh>().text = text;

        // OSC output
        double currentTime = Time.realtimeSinceStartup - _timeAtStartup;
        currentTime *= 1000;
        String msg = (int)currentTime + "," +
                     azimuthAngle.ToString("F2") + "," +
                     elevationAngle.ToString("F2") + "," +
                     currDist.ToString("F2") + "," +
                     headSpAngDev.ToString("F2") + "," +
                     spHeadAngDev.ToString("F2") + "," +
                     headTargetDistance.ToString("F2");


        _sender.Send("/headOrientation", msg);
    }
}
