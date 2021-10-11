using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpeakerAnchorPositioning : MonoBehaviour
{
    #region Public Variables
    public GameObject _speakerAnchor;
    public GameObject _textDisplay;
    #endregion

    #region Private Variables

    OVRInput.Controller controller = OVRInput.Controller.RTouch;
    #endregion

    void Start()
    {

    }
    void OnDestroy()
    {

    }
    void Update()
    {

        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, controller))
        {
            Vector3 speakerOrientation = new Vector3(0.0f, OVRInput.GetLocalControllerRotation(controller).eulerAngles.y, 0.0f);
            _speakerAnchor.transform.eulerAngles = speakerOrientation;
            _speakerAnchor.transform.position = OVRInput.GetLocalControllerPosition(controller);
            _speakerAnchor.transform.Translate(0.0f, 0.0f, 0.1f);

            _textDisplay.transform.rotation = _speakerAnchor.transform.rotation;
            _textDisplay.transform.position = _speakerAnchor.transform.position;
            _textDisplay.transform.Translate(0.0f, 0.6f, 0.0f);
        }

        //if (_homeBtnPressed)
        //{
        //    _showDisplay = !_showDisplay;
        //    _homeBtnPressed = false;
        //}


        //if (_showDisplay)
        //{
        //    _textDisplay.GetComponent<Text>().enabled = true;
        //}
        //else
        //{
        //    _textDisplay.GetComponent<Text>().enabled = false;
        //}
    }
}
