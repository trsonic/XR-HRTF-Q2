using System;
using UnityEngine;
using UnityEngine.UI;
using OscJack;

using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;


public class HeadAlign : MonoBehaviour
{
    public string _IPAddress = "";
    public int _oscPortOut = 9000;
    public int _oscPortIn = 6000;
    private string localIp;
    private OscServer _server;
    private OscClient _sender;

    public GameObject _mainCamera;
    public GameObject _speakerAnchor;

    public GameObject _headPositionTarget;
    public GameObject _virtualSpeaker;
    public GameObject _headOrientationTarget;
    public GameObject _headOrientationCross;

    private GameObject _arrow;

    private bool orientationLocked = false;
    private float speakerAz = 0.0f;
    private float speakerEl = 0.0f;
    private float speakerDist = 1.0f;
    private double _timeAtStartup;

#if GUIDING_DEBUG
    //float[] test_azis = { 0.0f, 45.0f, 45.0f, 135.0f, 135.0f, 225.0f, 225.0f, 315.0f, 315.0f };
    //float[] test_eles = { 0.0f, -30.0f, 30.0f, -30.0f, 30.0f, -30.0f, 30.0f, -30.0f, 30.0f };
    float[] test_azis = { 0.00f, 0.00f, 0.00f, 0.00f, 18.43f, 18.43f, 45.00f, 45.00f, 45.00f, 45.00f, 45.00f, 71.57f, 71.57f, 90.00f, 90.00f, 90.00f, 108.43f, 108.43f, 135.00f, 135.00f, 135.00f, 135.00f, 135.00f, 161.57f, 161.57f, 180.00f, 180.00f, 180.00f, 180.00f, -161.57f, -161.57f, -135.00f, -135.00f, -135.00f, -135.00f, -135.00f, -108.43f, -108.43f, -90.00f, -90.00f, -90.00f, -71.57f, -71.57f, -45.00f, -45.00f, -45.00f, -45.00f, -45.00f, -18.43f, -18.43f };
    float[] test_eles = { -45.00f, 0.00f, 45.00f, 90.00f, -17.55f, 17.55f, -64.76f, -35.26f, 0.00f, 35.26f, 64.76f, -17.55f, 17.55f, -45.00f, 0.00f, 45.00f, -17.55f, 17.55f, -64.76f, -35.26f, 0.00f, 35.26f, 64.76f, -17.55f, 17.55f, -90.00f, -45.00f, 0.00f, 45.00f, -17.55f, 17.55f, -64.76f, -35.26f, 0.00f, 35.26f, 64.76f, -17.55f, 17.55f, -45.00f, 0.00f, 45.00f, -17.55f, 17.55f, -64.76f, -35.26f, 0.00f, 35.26f, 64.76f, -17.55f, 17.55f };
    int testLocIndex = 0;
#endif

    void Start()
    {
        _timeAtStartup = Time.realtimeSinceStartup;

        // DRAW THE VIEWFINDER
        GameObject vfHorUp, vfHorDown, vrVertLeft, vfVertRight;
        float radius = 0.6f;
        float lineWidth = 0.001f;
        float vfMargin = 0.01f;

        vfHorDown = new GameObject { name = "vfHorDown" };
        vfHorDown.DrawCircle(radius, lineWidth);
        vfHorDown.GetComponent<LineRenderer>().material.color = Color.red;
        vfHorDown.transform.parent = _mainCamera.transform;
        vfHorDown.transform.Translate(0.0f, -vfMargin, 0.0f);

        vfHorUp = new GameObject { name = "vfHorUp" };
        vfHorUp.DrawCircle(radius, lineWidth);
        vfHorUp.GetComponent<LineRenderer>().material.color = Color.red;
        vfHorUp.transform.parent = _mainCamera.transform;
        vfHorUp.transform.Translate(0.0f, vfMargin, 0.0f);

        vrVertLeft = new GameObject { name = "vrVertLeft" };
        vrVertLeft.DrawCircle(radius, lineWidth);
        vrVertLeft.GetComponent<LineRenderer>().material.color = Color.green;
        vrVertLeft.transform.parent = _mainCamera.transform;
        vrVertLeft.transform.Translate(-vfMargin, 0.0f, 0.0f);
        vrVertLeft.transform.Rotate(transform.forward, 90.0f);

        vfVertRight = new GameObject { name = "vfVertRight" };
        vfVertRight.DrawCircle(radius, lineWidth);
        vfVertRight.GetComponent<LineRenderer>().material.color = Color.green;
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

        localIp = LocalIPAddress();
        setupOsc();
    }
    private void OnDestroy()
    {
        _server.Dispose();
    }
    void Update()
    {
#if GUIDING_DEBUG
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        {
            testLocIndex = (testLocIndex + 1) % test_azis.Length;
            speakerAz = test_azis[testLocIndex];
            speakerEl = test_eles[testLocIndex];
        }
#endif

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
        _virtualSpeaker.transform.position = _mainCamera.transform.position;
        _virtualSpeaker.transform.rotation = _mainCamera.transform.rotation;
        _virtualSpeaker.transform.Rotate(-speakerEl, speakerAz, 0.0f);
        _virtualSpeaker.transform.position = _mainCamera.transform.position + _virtualSpeaker.transform.forward * currDist;

        // HEAD ORIENTATION TARGET
        float orTargetDist = 0.6f;
        _headOrientationTarget.transform.position = _mainCamera.transform.position;
        _headOrientationTarget.transform.rotation = _mainCamera.transform.rotation;
        _headOrientationTarget.transform.Rotate(-(speakerEl - elevationAngle), speakerAz - azimuthAngle, 0.0f);
        _headOrientationTarget.transform.position = _mainCamera.transform.position + _headOrientationTarget.transform.forward * orTargetDist;

        Vector3 mcCenter = _mainCamera.transform.position; // reference camera/head position
        Vector3 vfCenter = _mainCamera.transform.position + _mainCamera.transform.forward * orTargetDist; // a point in front of the camera
        Vector3 vsVec = _virtualSpeaker.transform.position - _mainCamera.transform.position;
        Vector3 lsVec = _speakerAnchor.transform.position - _mainCamera.transform.position;
        Vector3 crossVec = Vector3.Cross(vfCenter - mcCenter, _headOrientationTarget.transform.position - mcCenter);
        Vector3 direction = Vector3.Cross(vfCenter - mcCenter, crossVec);

        float headSpAngDev = Vector3.Angle(vsVec, lsVec);
        text += "Angular distance: " + headSpAngDev.ToString("F2") + "\n";

        // FLOOR CIRCLE ARROW AZIMUTH
        _headPositionTarget.GetComponent<FloorCircle>().RotateArrow(-speakerAz);

        // ARROW
        float az = speakerAz;
        while (az < 0) az += 360.0f;
        _arrow.GetComponent<LineRenderer>().SetPosition(0, vfCenter);
        if(az < 90 || az >= 270)
        {
            Vector3 arrowEndPosition = vfCenter + direction.normalized * Mathf.Sin(headSpAngDev * Mathf.PI / 360.0f);
            _arrow.GetComponent<LineRenderer>().SetPosition(1, arrowEndPosition);
            _headOrientationTarget.transform.position = arrowEndPosition;
        }
        else
        {
            Vector3 dir2;
            dir2 = Vector3.Reflect(direction.normalized, _mainCamera.transform.up);
            Vector3 arrowEndPosition = vfCenter + dir2 * Mathf.Sin(headSpAngDev * Mathf.PI / 360.0f);
            _arrow.GetComponent<LineRenderer>().SetPosition(1, arrowEndPosition);
            _headOrientationTarget.transform.position = arrowEndPosition;
        }

        // HEAD ORIENTATION CROSS
        _headOrientationCross.transform.position = _mainCamera.transform.position;
        _headOrientationCross.transform.rotation = _mainCamera.transform.rotation;

        float pitchSign = 1.0f, rollSign = -1.0f;
        while (azimuthAngle < 0.0f) azimuthAngle += 360.0f;
        float crRotYaw = azimuthAngle - az;

        if (az < 90 || az >= 270) pitchSign = 1.0f;
        else pitchSign = -1.0f;

        float crRotPitch = pitchSign * Mathf.Sin(azimuthAngle * Mathf.PI / 360.0f) * (speakerEl - elevationAngle);
        float crRotRoll = rollSign * Mathf.Cos(azimuthAngle * Mathf.PI / 360.0f) * (speakerEl - elevationAngle);

        crRotYaw = wrapAngle(crRotYaw);
        crRotPitch = wrapAngle(crRotPitch);
        crRotRoll = wrapAngle(crRotRoll);

        _headOrientationCross.transform.Rotate(crRotPitch, crRotYaw, crRotRoll);

        // LOCKED ORIENTATION INDICATOR
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

        // angle between speaker axis and speaker-head vector
        float spHeadAngDev = Vector3.Angle(_headPositionTarget.transform.position - _speakerAnchor.transform.position, _mainCamera.transform.position - _speakerAnchor.transform.position);
        text += "Angular deviation: " + spHeadAngDev.ToString("F2") + " deg\n";

        // head height
        float headHeight = _mainCamera.transform.position.y;
        text += "head height: " + headHeight.ToString("F2") + " m\n";

        // Quest IP
        text += "IP: " + localIp + "\n";

        // update text display
        TextDisplay.Instance.PrintMessage(text);

        // hide some guides and block measurement
        if (headTargetDistance > 0.4f)
        {
            _arrow.GetComponent<LineRenderer>().enabled = false;
            _headOrientationTarget.GetComponent<Renderer>().enabled = false;
            ShowHeadOrientationCross(false);

            _headPositionTarget.GetComponent<FloorCircle>().DrawQuadraticBezierCurve(
                                _mainCamera.transform.position - _mainCamera.transform.up * 0.6f,
                                _mainCamera.transform.position + _mainCamera.transform.forward * 1.2f - _mainCamera.transform.up * 1.2f,
                                _headPositionTarget.transform.position - _headPositionTarget.transform.up * 1.6f
                                );

            // block measurement
        }
        else
        {
            _arrow.GetComponent<LineRenderer>().enabled = true;
            _headOrientationTarget.GetComponent<Renderer>().enabled = true;
            if (az == 0.0f || az == 180.0f) ShowHeadOrientationCross(false);
            else ShowHeadOrientationCross(true);
            _headPositionTarget.GetComponent<FloorCircle>().HideCurve();
        }

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
    void setupOsc()
    {
        _sender = new OscClient(_IPAddress, _oscPortOut);
        _server = new OscServer(_oscPortIn);

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

        _server.MessageDispatcher.AddCallback(
               "/rendererIp",
               (string address, OscDataHandle data) =>
               {
                   if (data.GetElementAsString(0) != null)
                   {
                       if(_IPAddress != data.GetElementAsString(0))
                       {
                           _IPAddress = data.GetElementAsString(0);
                           _sender.Dispose();
                           _sender = new OscClient(_IPAddress, _oscPortOut);
                       }
                   }
               }
           );
    }

    void ShowHeadOrientationCross(bool show)
    {
        int childCount = _headOrientationCross.transform.childCount;
        for (int i = 0; i < childCount; ++i)
        {
            _headOrientationCross.transform.GetChild(i).GetComponent<Renderer>().enabled = show;
        }
    }
    private float wrapAngle(float angle)
    {
        while (angle <= -180.0f) angle += 360.0f;
        while (angle > 180.0f) angle -= 360.0f;
        return angle;
    }
    private static string LocalIPAddress()
    {
        IPHostEntry host;
        string localIP = "0.0.0.0";
        host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
    }
}
